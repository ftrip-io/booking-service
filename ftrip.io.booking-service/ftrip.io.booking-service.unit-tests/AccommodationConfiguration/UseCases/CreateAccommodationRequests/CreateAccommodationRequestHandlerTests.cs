using FluentAssertions;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.AccommodationConfiguration.UseCases.CreateAccommodationRequests;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.AccommodationConfiguration.UseCases.CreateAccommodationRequests
{
    public class CreateAccommodationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IAccommodationRepository> _accommodationRepositoryMock = new Mock<IAccommodationRepository>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly CreateAccommodationRequestHandler _handler;

        public CreateAccommodationRequestHandlerTests()
        {
            _handler = new CreateAccommodationRequestHandler(
                _unitOfWorkMock.Object,
                _accommodationRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_Successful_ReturnsCreatedAccommodation()
        {
            // Arrange
            var request = GetCreateAccommodationRequest();

            _accommodationRepositoryMock
                .Setup(r => r.Create(It.IsAny<Accommodation>(), It.IsAny<CancellationToken>()))
                .Returns((Accommodation a, CancellationToken _) =>
                {
                    a.Id = Guid.NewGuid();
                    return Task.FromResult(a);
                });

            // Act
            var createdAccommodation = await _handler.Handle(request, CancellationToken.None);

            // Assert
            createdAccommodation.Should().NotBeNull();
            createdAccommodation.IsManualAccept.Should().BeTrue();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        private CreateAccommodationRequest GetCreateAccommodationRequest()
        {
            return new CreateAccommodationRequest()
            {
                AccommodationId = Guid.NewGuid(),
            };
        }
    }
}