using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviewsSummary
{
    public class ReadAccomodationReviewsSummaryQueryHandler : IRequestHandler<ReadAccomodationReviewsSummaryQuery, AccomodationReviewsSummary>
    {
        private readonly IAccomodationReviewsSummaryRepository _accomodationReviewsSummaryRepository;
        private readonly ILogger _logger;

        public ReadAccomodationReviewsSummaryQueryHandler(
            IAccomodationReviewsSummaryRepository accomodationReviewsSummaryRepository,
            ILogger logger)
        {
            _accomodationReviewsSummaryRepository = accomodationReviewsSummaryRepository;
            _logger = logger;
        }

        public async Task<AccomodationReviewsSummary> Handle(ReadAccomodationReviewsSummaryQuery request, CancellationToken cancellationToken)
        {
            var existingSummary = await _accomodationReviewsSummaryRepository.ReadByAccomodationId(request.AccomodationId, cancellationToken);
            if (existingSummary != null)
            {
                return existingSummary;
            }

            _logger.Warning("Returning default summary for accommodation - AccommodationId[{AccommodationId}]", request.AccomodationId);
            return GetDefaultSummary(request);
        }

        private AccomodationReviewsSummary GetDefaultSummary(ReadAccomodationReviewsSummaryQuery request)
        {
            return new AccomodationReviewsSummary()
            {
                AccomodationId = request.AccomodationId,
                ReviewsCount = 0,
                Grades = new AccomodationGradesSummary()
                {
                    Accomodation = 0,
                    Location = 0,
                    ValueForMoney = 0
                }
            };
        }
    }
}