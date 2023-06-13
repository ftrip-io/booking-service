using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Calculators;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Consumers
{
    public class AccomodationReviewDeletedEventConsumer : IConsumer<AccomodationReviewDeletedEvent>
    {
        private readonly IAccomodationReviewsSummaryCalculator _accomodationReviewsSummaryCalculator;

        public AccomodationReviewDeletedEventConsumer(IAccomodationReviewsSummaryCalculator accomodationReviewsSummaryCalculator)
        {
            _accomodationReviewsSummaryCalculator = accomodationReviewsSummaryCalculator;
        }

        public async Task Consume(ConsumeContext<AccomodationReviewDeletedEvent> context)
        {
            var accomodationReviewDeleted = context.Message;

            await _accomodationReviewsSummaryCalculator.Recalculate(accomodationReviewDeleted.AccomodationId, CancellationToken.None);
        }
    }
}