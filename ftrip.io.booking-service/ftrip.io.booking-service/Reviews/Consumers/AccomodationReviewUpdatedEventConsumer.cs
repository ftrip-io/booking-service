using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Calculators;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Consumers
{
    public class AccomodationReviewUpdatedEventConsumer : IConsumer<AccomodationReviewUpdatedEvent>
    {
        private readonly IAccomodationReviewsSummaryCalculator _accomodationReviewsSummaryCalculator;

        public AccomodationReviewUpdatedEventConsumer(IAccomodationReviewsSummaryCalculator accomodationReviewsSummaryCalculator)
        {
            _accomodationReviewsSummaryCalculator = accomodationReviewsSummaryCalculator;
        }

        public async Task Consume(ConsumeContext<AccomodationReviewUpdatedEvent> context)
        {
            var accomodationReviewUpdated = context.Message;

            await _accomodationReviewsSummaryCalculator.Recalculate(accomodationReviewUpdated.AccomodationId, CancellationToken.None);
        }
    }
}