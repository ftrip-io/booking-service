using ftrip.io.booking_service.Reservations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.GetPossibleAccommodationsForReview
{
    public class GetPossibleAccommodationsForReviewQueryHandler : IRequestHandler<GetPossibleAccommodationsForReviewQuery, IEnumerable<Guid>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;

        public GetPossibleAccommodationsForReviewQueryHandler(
            IReservationRepository reservationRepository,
            IAccomodationReviewRepository accomodationReviewRepository)
        {
            _reservationRepository = reservationRepository;
            _accomodationReviewRepository = accomodationReviewRepository;
        }

        public async Task<IEnumerable<Guid>> Handle(GetPossibleAccommodationsForReviewQuery request, CancellationToken cancellationToken)
        {
            var reservedAccomodationIds = await GetReservedAccommodationIds(request.GuestId, cancellationToken);
            var reviewedAccomodationIds = await GetReviewedAccommodationIds(request.GuestId, cancellationToken);

            return reservedAccomodationIds.Except(reviewedAccomodationIds);
        }

        private async Task<IEnumerable<Guid>> GetReservedAccommodationIds(Guid guestId, CancellationToken cancellationToken)
        {
            var reservations = await _reservationRepository.ReadForGuestInPast(guestId, cancellationToken);

            return reservations.Select(reservation => reservation.AccomodationId);
        }

        private async Task<IEnumerable<Guid>> GetReviewedAccommodationIds(Guid guestId, CancellationToken cancellationToken)
        {
            var reviews = await _accomodationReviewRepository.ReadByGuestId(guestId, cancellationToken);

            return reviews.Select(review => review.AccomodationId);
        }
    }
}