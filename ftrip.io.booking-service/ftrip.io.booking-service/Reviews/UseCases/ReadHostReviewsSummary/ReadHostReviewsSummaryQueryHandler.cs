using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadHostReviewsSummary
{
    public class ReadHostReviewsSummaryQueryHandler : IRequestHandler<ReadHostReviewsSummaryQuery, HostReviewsSummary>
    {
        private readonly IHostReviewsSummaryRepository _hostReviewsSummaryRepository;
        private readonly ILogger _logger;

        public ReadHostReviewsSummaryQueryHandler(
            IHostReviewsSummaryRepository hostReviewsSummaryRepository,
            ILogger logger)
        {
            _hostReviewsSummaryRepository = hostReviewsSummaryRepository;
            _logger = logger;
        }

        public async Task<HostReviewsSummary> Handle(ReadHostReviewsSummaryQuery request, CancellationToken cancellationToken)
        {
            var existingSummary = await _hostReviewsSummaryRepository.ReadByHostId(request.HostId, cancellationToken);
            if (existingSummary != null)
            {
                return existingSummary;
            }

            _logger.Warning("Returning default summary for host - HostId[{HostId}]", request.HostId);
            return GetDefaultSummary(request);
        }

        private HostReviewsSummary GetDefaultSummary(ReadHostReviewsSummaryQuery request)
        {
            return new HostReviewsSummary()
            {
                HostId = request.HostId,
                ReviewsCount = 0,
                Grades = new HostGradesSummary()
                {
                    Communication = 0,
                    Overall = 0
                }
            };
        }
    }
}