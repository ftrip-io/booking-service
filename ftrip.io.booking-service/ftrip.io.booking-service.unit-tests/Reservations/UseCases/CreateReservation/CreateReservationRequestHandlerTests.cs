using AutoMapper;
using FluentAssertions;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.Reservations;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.booking_service.Reservations.UseCases.CreateReservation;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.Reservations.UseCases.CreateReservation
{
    public  class CreateReservationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IReservationRepository> _reservationRepositoryMock = new Mock<IReservationRepository>();

        private CreateReservationRequestHandler _handler;

        public CreateReservationRequestHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateReservationRequest, Reservation>();
                cfg.CreateMap<CreateDatePeriodRequest, DatePeriod>();
            }).CreateMapper();

            _handler = new CreateReservationRequestHandler(
                _unitOfWorkMock.Object,
                _reservationRepositoryMock.Object,
                mapper
            );
        }

        [Fact]
        public async Task Handle_Successful_ReturnsCreatedReservation()
        {
            // Arrange
            var request = GetCreateReservationRequest();

            _reservationRepositoryMock
                .Setup(r => r.Create(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
                .Returns((Reservation r, CancellationToken _) =>
                {
                    r.Id = Guid.NewGuid();
                    return Task.FromResult(r);
                });

            // Act
            var createdReservation = await _handler.Handle(request, CancellationToken.None);

            // Assert
            createdReservation.Should().NotBeNull();
            createdReservation.IsCancelled.Should().BeFalse();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
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
