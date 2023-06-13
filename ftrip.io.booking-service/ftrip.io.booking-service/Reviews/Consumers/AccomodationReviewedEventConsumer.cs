using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Calculators;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Consumers
{
    public class AccomodationReviewedEventConsumer : IConsumer<AccomodationReviewedEvent>
    {
        private readonly IAccomodationReviewsSummaryCalculator _accomodationReviewsSummaryCalculator;

        public AccomodationReviewedEventConsumer(IAccomodationReviewsSummaryCalculator accomodationReviewsSummaryCalculator)
        {
            _accomodationReviewsSummaryCalculator = accomodationReviewsSummaryCalculator;
        }

        public async Task Consume(ConsumeContext<AccomodationReviewedEvent> context)
        {
            var accomodationReviewed = context.Message;

            await _accomodationReviewsSummaryCalculator.Recalculate(accomodationReviewed.AccomodationId, CancellationToken.None);
        }
    }
}