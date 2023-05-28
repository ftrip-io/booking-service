using AutoMapper;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CreateReservation
{
    public class CreateReservationRequestHandler : IRequestHandler<CreateReservationRequest, Reservation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;

        public CreateReservationRequestHandler(
            IUnitOfWork unitOfWork, 
            IReservationRepository reservationRepository, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
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

            return await _reservationRepository.Create(reservation, cancellationToken);
        }
    }
}
