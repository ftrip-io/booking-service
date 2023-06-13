using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Calculators;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.Consumers
{
    public class HostReviewDeletedEventConsumer : IConsumer<HostReviewDeletedEvent>
    {
        private readonly IHostReviewsSummaryCalculator _hostReviewsSummaryCalculator;

        public HostReviewDeletedEventConsumer(IHostReviewsSummaryCalculator hostReviewsSummaryCalculator)
        {
            _hostReviewsSummaryCalculator = hostReviewsSummaryCalculator;
        }

        public async Task Consume(ConsumeContext<HostReviewDeletedEvent> context)
        {
            var hostReviewDeleted = context.Message;

            await _hostReviewsSummaryCalculator.Recalculate(hostReviewDeleted.HostId, CancellationToken.None);
        }
    }
}