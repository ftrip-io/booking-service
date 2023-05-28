using FluentAssertions;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests
{
    public class ChangeAccommodationConfigurationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IAccommodationRepository> _accommodationRepositoryMock = new Mock<IAccommodationRepository>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private ChangeAccommodationConfigurationRequestHandler _handler;

        public ChangeAccommodationConfigurationRequestHandlerTests()
        {
            _handler = new ChangeAccommodationConfigurationRequestHandler(
                _unitOfWorkMock.Object,
                _accommodationRepositoryMock.Object,
                _stringManagerMock.Object
            );
        }

        [Fact]
        public void Handle_AccommodationDoesNotExist_ThrowsMissingEntityException()
        {
            // Arrange
            var request = GetChangeAccommodationConfigurationRequest();

            _accommodationRepositoryMock
                .Setup(r => r.ReadByAccommodationId(It.Is<Guid>(id => id == request.AccommodationId), It.IsAny<CancellationToken>()))
                .Throws(new MissingEntityException());

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsConfiguredAccommodation()
        {
            // Arrange
            var request = GetChangeAccommodationConfigurationRequest();

            _accommodationRepositoryMock
                .Setup(r => r.ReadByAccommodationId(It.Is<Guid>(id => id == request.AccommodationId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(
                    new Accommodation()
                    {
                        Id = id,
                        IsManualAccept = true
                    }));

            _accommodationRepositoryMock
                .Setup(r => r.Update(It.IsAny<Accommodation>(), It.IsAny<CancellationToken>()))
                .Returns((Accommodation a, CancellationToken _) => Task.FromResult(a));

            // Act
            var configuredAccommodation = await _handler.Handle(request, CancellationToken.None);

            // Assert
            configuredAccommodation.Should().NotBeNull();
            configuredAccommodation.IsManualAccept.Should().BeFalse();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        private ChangeAccommodationConfigurationRequest GetChangeAccommodationConfigurationRequest()
        {
            return new ChangeAccommodationConfigurationRequest()
            {
                AccommodationId = Guid.NewGuid(),
                IsManualAccept = false
            };
        }
    }
}
