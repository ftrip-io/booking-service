using ftrip.io.booking_service.Persistance;
using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.Contracts;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Calculators
{
    public interface IAccomodationReviewsSummaryCalculator
    {
        Task<AccomodationReviewsSummary> Recalculate(Guid accomodationId, CancellationToken cancellationToken);
    }

    public class AccomodationReviewsSummaryCalculator : IAccomodationReviewsSummaryCalculator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccomodationReviewsSummaryRepository _accomodationReviewsSummaryRepository;
        private readonly ILogger _logger;

        public AccomodationReviewsSummaryCalculator(
            IUnitOfWork unitOfWork,
            IAccomodationReviewsSummaryRepository accomodationReviewsSummaryRepository,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accomodationReviewsSummaryRepository = accomodationReviewsSummaryRepository;
            _logger = logger;
        }

        public async Task<AccomodationReviewsSummary> Recalculate(Guid accomodationId, CancellationToken cancellationToken)
        {
            var summary = await _accomodationReviewsSummaryRepository.ComputeByAccomodationId(accomodationId, cancellationToken);
            if (summary == null)
            {
                _logger.Error("Could not compute summary for accommodation - AccommodationId[{AccommodationId}]", accomodationId);
                return null;
            }

            await _unitOfWork.Begin(cancellationToken);

            var savedSummary = await Save(summary, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            _logger.Information(
                "Computed summary - AccommodationId[{AccommodationId}], ReviewsCount[{ReviewsCount}], Average[{Average}]",
                accomodationId, savedSummary.ReviewsCount, savedSummary.Grades.Average
            );

            return savedSummary;
        }

        private async Task<AccomodationReviewsSummary> Save(AccomodationReviewsSummary summary, CancellationToken cancellationToken)
        {
            var existingSummary = await _accomodationReviewsSummaryRepository.ReadByAccomodationId(summary.AccomodationId, cancellationToken);
            if (existingSummary == null)
            {
                return await _accomodationReviewsSummaryRepository.Create(summary, cancellationToken);
            }

            existingSummary.Grades.Accomodation = summary.Grades.Accomodation;
            existingSummary.Grades.Location = summary.Grades.Location;
            existingSummary.Grades.ValueForMoney = summary.Grades.ValueForMoney;
            existingSummary.ReviewsCount = summary.ReviewsCount;

            return await _accomodationReviewsSummaryRepository.Update(existingSummary, cancellationToken);
        }
    }
}