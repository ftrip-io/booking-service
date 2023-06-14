using AutoMapper;
using FluentAssertions;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.ReservationRequests;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases.CreateReservationRequests;
using ftrip.io.booking_service.Reservations;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.ReservationRequests.UseCases.CreateReservationRequests
{
    public class CreateReservationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IReservationRequestRepository> _reservationRequestRepositoryMock = new Mock<IReservationRequestRepository>();
        private readonly Mock<IReservationRepository> _reservationRepositoryMock = new Mock<IReservationRepository>();
        private readonly Mock<IAccommodationQueryHelper> _accommodationQueryHelperMock = new Mock<IAccommodationQueryHelper>();
        private readonly Mock<IMessagePublisher> _messagePublisherMock = new Mock<IMessagePublisher>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();
        private readonly Mock<ICatalogServiceClient> _catalogServiceClientMock = new Mock<ICatalogServiceClient>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly CreateReservationRequestHandler _handler;

        public CreateReservationRequestHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateReservationRequest, ReservationRequest>();
                cfg.CreateMap<CreateDatePeriodRequest, DatePeriod>();
            }).CreateMapper();

            _handler = new CreateReservationRequestHandler(
                _unitOfWorkMock.Object,
                _reservationRequestRepositoryMock.Object,
                _reservationRepositoryMock.Object,
                _accommodationQueryHelperMock.Object,
                mapper,
                _messagePublisherMock.Object,
                _stringManagerMock.Object,
                _catalogServiceClientMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void Handle_AccommodationDoesNotExists_ThrowsMissingEntityException()
        {
            // Arrange
            var request = GetCreateReservationRequest();

            _accommodationQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is<Guid>(id => id == request.AccomodationId), It.IsAny<CancellationToken>()))
                .Throws(new MissingEntityException());

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void Handle_ReservationDateIsAlreadyTaken_ThrowsBadLogicException()
        {
            // Arrange
            var request = GetCreateReservationRequest();

            _reservationRepositoryMock
                .Setup(r => r.HasAnyByAccomodationAndDatePeriod(It.IsAny<Guid>(), It.IsAny<DatePeriod>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

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
            var request = GetCreateReservationRequest();
            _catalogServiceClientMock
                .Setup(c => c.GetPriceInfo(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new PriceInfo() 
                {
                    Problems = new System.Collections.Generic.List<string>()
                }));

            _reservationRepositoryMock
                .Setup(r => r.HasAnyByAccomodationAndDatePeriod(It.IsAny<Guid>(), It.IsAny<DatePeriod>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            _reservationRequestRepositoryMock
                .Setup(r => r.Create(It.IsAny<ReservationRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ReservationRequest r, CancellationToken _) =>
                {
                    r.Id = Guid.NewGuid();
                    return Task.FromResult(r);
                });

            _accommodationQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Accommodation()
                {
                    HostId = Guid.NewGuid()
                }));

            // Act
            var createdReservationRequest = await _handler.Handle(request, CancellationToken.None);

            // Assert
            createdReservationRequest.Should().NotBeNull();
            createdReservationRequest.Status.Should().Be(ReservationRequestStatus.Waiting);
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisherMock.Verify(mp => mp.Send<ReservationRequestCreatedEvent, string>(It.IsAny<ReservationRequestCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private CreateReservationRequest GetCreateReservationRequest()
        {
            return new CreateReservationRequest()
            {
                GuestId = Guid.NewGuid(),
                AccomodationId = Guid.NewGuid(),
                DatePeriod = new CreateDatePeriodRequest()
                {
                    DateFrom = DateTime.Now.AddDays(-3),
                    DateTo = DateTime.Now
                },
                GuestNumber = 3
            };
        }
    }
}