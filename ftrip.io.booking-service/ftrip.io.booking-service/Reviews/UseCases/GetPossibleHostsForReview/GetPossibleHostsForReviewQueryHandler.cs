using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.Reservations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.GetPossibleHostsForReview
{
    public class GetPossibleHostsForReviewQueryHandler : IRequestHandler<GetPossibleHostsForReviewQuery, IEnumerable<Guid>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IHostReviewRepository _hostReviewRepository;

        public GetPossibleHostsForReviewQueryHandler(
            IReservationRepository reservationRepository,
            IAccommodationRepository accommodationRepository,
            IHostReviewRepository hostReviewRepository)
        {
            _reservationRepository = reservationRepository;
            _accommodationRepository = accommodationRepository;
            _hostReviewRepository = hostReviewRepository;
        }

        public async Task<IEnumerable<Guid>> Handle(GetPossibleHostsForReviewQuery request, CancellationToken cancellationToken)
        {
            var reservedAccommodationHostIds = await GetReservedAccommodationHostIds(request.GuestId, cancellationToken);
            var reviewedHostIds = await GetReviewedHostIds(request.GuestId, cancellationToken);

            return reservedAccommodationHostIds.Except(reviewedHostIds);
        }

        private async Task<IEnumerable<Guid>> GetReservedAccommodationHostIds(Guid guestId, CancellationToken cancellationToken)
        {
            var reservations = await _reservationRepository.ReadForGuestInPast(guestId, cancellationToken);
            var reservedAccommodationIds = reservations.Select(reservation => reservation.AccomodationId).ToList();
            var reservedAccommodations = await _accommodationRepository.ReadByAccommodationIds(reservedAccommodationIds);

            return reservedAccommodations.Select(a => a.HostId).ToHashSet().ToList();
        }

        private async Task<IEnumerable<Guid>> GetReviewedHostIds(Guid guestId, CancellationToken cancellationToken)
        {
            var reviews = await _hostReviewRepository.ReadByGuestId(guestId, cancellationToken);

            return reviews.Select(review => review.HostId);
        }
    }
}