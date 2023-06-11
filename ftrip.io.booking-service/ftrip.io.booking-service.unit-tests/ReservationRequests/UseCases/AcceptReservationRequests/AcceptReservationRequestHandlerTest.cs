using FluentAssertions;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases;
using ftrip.io.booking_service.ReservationRequests.UseCases.AcceptReservationRequests;
using ftrip.io.booking_service.Reservations.UseCases.CreateReservation;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.ReservationRequests.UseCases.AcceptReservationRequests
{
    public class AcceptReservationRequestHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IReservationRequestRepository> _reservationRequestRepositoryMock = new Mock<IReservationRequestRepository>();
        private readonly Mock<IReservationRequestQueryHelper> _reservationRequestQueryHelperMock = new Mock<IReservationRequestQueryHelper>();
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IMessagePublisher> _messagePublisherMock = new Mock<IMessagePublisher>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly AcceptReservationRequestHandler _handler;

        public AcceptReservationRequestHandlerTest()
        {
            _handler = new AcceptReservationRequestHandler(
                _unitOfWorkMock.Object,
                _reservationRequestRepositoryMock.Object,
                _reservationRequestQueryHelperMock.Object,
                _mediatorMock.Object,
                _messagePublisherMock.Object,
                _stringManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void Handle_ReservationRequestDoesNotExist_ThrowsMissingEntityException()
        {
            // Arrange
            var request = GetAcceptReservationRequest();

            _reservationRequestQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is<Guid>(id => id == request.ReservationRequestId), It.IsAny<CancellationToken>()))
                .Throws(new MissingEntityException());

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void Handle_ReservationIsAlreadyHandled_ThrowsBadLogicException()
        {
            // Arrange
            var request = GetAcceptReservationRequest();

            _reservationRequestQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is<Guid>(id => id == request.ReservationRequestId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(
                    new ReservationRequest()
                    {
                        Id = id,
                        Status = ReservationRequestStatus.Accepted
                    }));

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<BadLogicException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsReservationRequest()
        {
            // Arrange
            var request = GetAcceptReservationRequest();

            _reservationRequestQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is<Guid>(id => id == request.ReservationRequestId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(
                    new ReservationRequest()
                    {
                        Id = id,
                        Status = ReservationRequestStatus.Waiting,
                        AccomodationId = Guid.NewGuid(),
                        GuestId = Guid.NewGuid(),
                        GuestNumber = 3,
                        DatePeriod = new DatePeriod()
                        {
                            DateFrom = DateTime.Now.AddDays(-3),
                            DateTo = DateTime.Now
                        }
                    }));

            _reservationRequestRepositoryMock
                .Setup(r => r.Update(It.IsAny<ReservationRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ReservationRequest r, CancellationToken _) => Task.FromResult(r));

            // Act
            var acceptedReservationRequest = await _handler.Handle(request, CancellationToken.None);

            // Assert
            acceptedReservationRequest.Should().NotBeNull();
            acceptedReservationRequest.Status.Should().Be(ReservationRequestStatus.Accepted);
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(mp => mp.Send(It.IsAny<CreateReservationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisherMock.Verify(mp => mp.Send<ReservationRequestAcceptedEvent, string>(It.IsAny<ReservationRequestAcceptedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private AcceptReservationRequest GetAcceptReservationRequest()
        {
            return new AcceptReservationRequest()
            {
                ReservationRequestId = Guid.NewGuid()
            };
        }
    }
}