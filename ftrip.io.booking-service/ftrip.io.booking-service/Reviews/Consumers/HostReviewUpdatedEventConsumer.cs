using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Calculators;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Consumers
{
    public class HostReviewUpdatedEventConsumer : IConsumer<HostReviewUpdatedEvent>
    {
        private readonly IHostReviewsSummaryCalculator _hostReviewsSummaryCalculator;

        public HostReviewUpdatedEventConsumer(IHostReviewsSummaryCalculator hostReviewsSummaryCalculator)
        {
            _hostReviewsSummaryCalculator = hostReviewsSummaryCalculator;
        }

        public async Task Consume(ConsumeContext<HostReviewUpdatedEvent> context)
        {
            var hostReviewUpdated = context.Message;

            await _hostReviewsSummaryCalculator.Recalculate(hostReviewUpdated.HostId, CancellationToken.None);
        }
    }
}