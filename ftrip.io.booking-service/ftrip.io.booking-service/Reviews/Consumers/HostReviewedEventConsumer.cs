using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Calculators;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Consumers
{
    public class HostReviewedEventConsumer : IConsumer<HostReviewedEvent>
    {
        private readonly IHostReviewsSummaryCalculator _hostReviewsSummaryCalculator;

        public HostReviewedEventConsumer(IHostReviewsSummaryCalculator hostReviewsSummaryCalculator)
        {
            _hostReviewsSummaryCalculator = hostReviewsSummaryCalculator;
        }

        public async Task Consume(ConsumeContext<HostReviewedEvent> context)
        {
            var hostReviewed = context.Message;

            await _hostReviewsSummaryCalculator.Recalculate(hostReviewed.HostId, CancellationToken.None);
        }
    }
}