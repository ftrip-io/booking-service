using FluentAssertions;
using ftrip.io.booking_service.ReservationRequests;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequests;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.booking_service.unit_tests.ReservationRequests.UseCases.DeleteReservationRequests
{
    public class DeleteReservationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IReservationRequestRepository> _reservationRequestRepositoryMock = new Mock<IReservationRequestRepository>();
        private readonly Mock<IReservationRequestQueryHelper> _reservationRequestQueryHelperMock = new Mock<IReservationRequestQueryHelper>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private readonly DeleteReservationRequestHandler _handler;

        public DeleteReservationRequestHandlerTests()
        {
            _handler = new DeleteReservationRequestHandler(
                _unitOfWorkMock.Object,
                _reservationRequestRepositoryMock.Object,
                _reservationRequestQueryHelperMock.Object,
                _stringManagerMock.Object
            );
        }

        [Fact]
        public void Handle_ReservationRequestDoesNotExist_ThrowsMissingEntityException()
        {
            // Arrange
            var request = GetDeleteReservationRequest();

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
            var request = GetDeleteReservationRequest();

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
        public async Task Handle_Successful_ReturnsDeletedReservationRequest()
        {
            // Arrange
            var request = GetDeleteReservationRequest();

            _reservationRequestQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is<Guid>(id => id == request.ReservationRequestId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(
                    new ReservationRequest()
                    {
                        Id = id,
                        Status = ReservationRequestStatus.Waiting,
                    }));

            _reservationRequestRepositoryMock
                .Setup(r => r.Delete(It.Is<Guid>(id => id == request.ReservationRequestId), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) =>
                    Task.FromResult(
                        new ReservationRequest()
                        {
                            Id = id,
                            Active = false
                        }
                    ));

            // Act
            var deletedReservationRequest = await _handler.Handle(request, CancellationToken.None);

            // Assert
            deletedReservationRequest.Should().NotBeNull();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        private DeleteReservationRequest GetDeleteReservationRequest()
        {
            return new DeleteReservationRequest()
            {
                ReservationRequestId = Guid.NewGuid()
            };
        }
    }
}
