using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.Contracts;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Calculators
{
    public interface IHostReviewsSummaryCalculator
    {
        Task<HostReviewsSummary> Recalculate(Guid hostId, CancellationToken cancellationToken);
    }

    public class HostReviewsSummaryCalculator : IHostReviewsSummaryCalculator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostReviewsSummaryRepository _hostReviewsSummaryRepository;
        private readonly ILogger _logger;

        public HostReviewsSummaryCalculator(
            IUnitOfWork unitOfWork,
            IHostReviewsSummaryRepository hostReviewsSummaryRepository,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _hostReviewsSummaryRepository = hostReviewsSummaryRepository;
            _logger = logger;
        }

        public async Task<HostReviewsSummary> Recalculate(Guid hostId, CancellationToken cancellationToken)
        {
            var summary = await _hostReviewsSummaryRepository.ComputeByHostId(hostId, cancellationToken);
            if (summary == null)
            {
                _logger.Error("Could not compute summary for host - HostId[{HostId}]", hostId);
                return null;
            }

            await _unitOfWork.Begin(cancellationToken);

            var savedSummary = await Save(summary, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            _logger.Information(
                "Computed summary - HostId[{HostId}], ReviewsCount[{ReviewsCount}], Average[{Average}]",
                hostId, savedSummary.ReviewsCount, savedSummary.Grades.Average
            );

            return savedSummary;
        }

        private async Task<HostReviewsSummary> Save(HostReviewsSummary summary, CancellationToken cancellationToken)
        {
            var existingSummary = await _hostReviewsSummaryRepository.ReadByHostId(summary.HostId, cancellationToken);
            if (existingSummary == null)
            {
                return await _hostReviewsSummaryRepository.Create(summary, cancellationToken);
            }

            existingSummary.Grades.Communication = summary.Grades.Communication;
            existingSummary.Grades.Overall = summary.Grades.Overall;
            existingSummary.ReviewsCount = summary.ReviewsCount;

            return await _hostReviewsSummaryRepository.Update(existingSummary, cancellationToken);
        }
    }
}