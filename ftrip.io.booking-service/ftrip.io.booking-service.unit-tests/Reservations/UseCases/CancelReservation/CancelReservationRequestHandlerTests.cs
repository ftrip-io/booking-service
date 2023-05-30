using FluentAssertions;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.Reservations.Events;
using ftrip.io.booking_service.Reservations;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.booking_service.Reservations.UseCases.CancelReservation;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.Reservations.UseCases.CancelReservation
{
    public class CancelReservationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IReservationRepository> _reservationRepositoryMock = new Mock<IReservationRepository>();
        private readonly Mock<IAccommodationQueryHelper> _accommodationQueryHelperMock = new Mock<IAccommodationQueryHelper>();
        private readonly Mock<IMessagePublisher> _messagePublisherMock = new Mock<IMessagePublisher>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private CancelReservationRequestHandler _handler;

        public CancelReservationRequestHandlerTests()
        {
            _handler = new CancelReservationRequestHandler(
                _unitOfWorkMock.Object,
                _reservationRepositoryMock.Object,
                _accommodationQueryHelperMock.Object,
                _messagePublisherMock.Object,
                _stringManagerMock.Object
            );
        }

        [Fact]
        public void Handle_ReservationDoesNotExist_ThrowsMissingEntityException()
        {
            // Arrange
            var request = GetCancelReservationRequest();

            _reservationRepositoryMock
                .Setup(r => r.Read(It.Is<Guid>(id => id == request.ReservationId), It.IsAny<CancellationToken>()))
                .Throws(new MissingEntityException());

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void Handle_CannotCancelInLessThenADayBeforeReservationDoesNotExist_ThrowsBadLogicException()
        {
            // Arrange
            var request = GetCancelReservationRequest();

            _reservationRepositoryMock
                .Setup(r => r.Read(It.Is<Guid>(id => id == request.ReservationId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(
                    new Reservation()
                    {
                        Id = id,
                        DatePeriod = new DatePeriod()
                        {
                            DateFrom = DateTime.Now.AddHours(12)
                        }
                    }));

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<BadLogicException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsCanceledReservation()
        {
            // Arrange
            var request = GetCancelReservationRequest();

            _reservationRepositoryMock
                .Setup(r => r.Read(It.Is<Guid>(id => id == request.ReservationId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(
                    new Reservation()
                    {
                        Id = id,
                        DatePeriod = new DatePeriod()
                        {
                            DateFrom = DateTime.Now.AddDays(2)
                        }
                    }));

            _reservationRepositoryMock
                .Setup(r => r.Update(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
                .Returns((Reservation r, CancellationToken _) => Task.FromResult(r));

            _accommodationQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Accommodation()
                {
                    HostId = Guid.NewGuid()
                }));

            // Act
            var canceledReservation = await _handler.Handle(request, CancellationToken.None);

            // Assert
            canceledReservation.Should().NotBeNull();
            canceledReservation.IsCancelled.Should().BeTrue();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisherMock.Verify(mp => mp.Send<ReservationCanceledEvent, string>(It.IsAny<ReservationCanceledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private CancelReservationRequest GetCancelReservationRequest()
        {
            return new CancelReservationRequest()
            {
                ReservationId = Guid.NewGuid()
            };
        }
    }
}
