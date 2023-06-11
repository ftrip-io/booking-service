using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.contracts.Reservations.Events;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CancelReservation
{
    public class CancelReservationRequestHandler : IRequestHandler<CancelReservationRequest, Reservation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public CancelReservationRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRepository reservationRepository,
            IAccommodationQueryHelper accommodationQueryHelper,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _reservationRepository = reservationRepository;
            _accommodationQueryHelper = accommodationQueryHelper;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<Reservation> Handle(CancelReservationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingReservation = await ReadOrThrow(request.ReservationId, cancellationToken);
            Validate(existingReservation);

            await CancelReservation(existingReservation, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishReservationCanceledEvent(existingReservation, cancellationToken);

            return existingReservation;
        }

        private async Task<Reservation> ReadOrThrow(Guid reservationId, CancellationToken cancellationToken)
        {
            var existingReservation = await _reservationRepository.Read(reservationId, cancellationToken);
            if (existingReservation == null)
            {
                _logger.Error("Reservation not found - ReservationId[{ReservationId}]", reservationId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", reservationId));
            }

            return existingReservation;
        }

        public void Validate(Reservation reservation)
        {
            var lessThenDayBeforeReservation = (reservation.DatePeriod.DateFrom - DateTime.UtcNow).Days < 1;
            if (lessThenDayBeforeReservation)
            {
                _logger.Error(
                    "Reservation cannot be cancelled because there is less then a day - ReservationId[{ReservationId}], Date[{Date}]",
                    reservation.Id, reservation.DatePeriod.DateFrom
                );
                throw new BadLogicException(_stringManager.Format("Reservation_Validation_TooLate", reservation.Id));
            }
        }

        private async Task<Reservation> CancelReservation(Reservation reservation, CancellationToken cancellationToken)
        {
            reservation.IsCancelled = true;

            var cancelledReservation = await _reservationRepository.Update(reservation, cancellationToken);

            _logger.Information("Reservation cancelled - ReservationId[{ReservationId}]", reservation.Id);

            return cancelledReservation;
        }

        private async Task PublishReservationCanceledEvent(Reservation reservation, CancellationToken cancellationToken)
        {
            var accomodation = await _accommodationQueryHelper.ReadOrThrow(reservation.AccomodationId, cancellationToken);

            var reservationCanceled = new ReservationCanceledEvent()
            {
                ReservationId = reservation.Id,
                AccomodationId = reservation.AccomodationId,
                GuestId = reservation.GuestId,
                HostId = accomodation.HostId,
                From = reservation.DatePeriod.DateFrom,
                To = reservation.DatePeriod.DateTo
            };

            await _messagePublisher.Send<ReservationCanceledEvent, string>(reservationCanceled, cancellationToken);
        }
    }
}