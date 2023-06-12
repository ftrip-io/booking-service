using AutoMapper;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CreateReservation
{
    public class CreateReservationRequestHandler : IRequestHandler<CreateReservationRequest, Reservation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateReservationRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRepository reservationRepository,
            IMapper mapper,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Reservation> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var reservation = _mapper.Map<Reservation>(request);

            var createdReservation = await CreateReservation(reservation, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return createdReservation;
        }

        private async Task<Reservation> CreateReservation(Reservation reservation, CancellationToken cancellationToken)
        {
            reservation.IsCancelled = false;

            var createdReservation = await _reservationRepository.Create(reservation, cancellationToken);

            _logger.Information(
                "Reservation created - ReservationId[{ReservationId}], GuestId[{GuestId}], AccommodationId[{AccommodationId}]",
                createdReservation.Id, createdReservation.GuestId, createdReservation.AccomodationId
            );

            return createdReservation;
        }
    }
}