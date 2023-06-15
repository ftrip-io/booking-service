using ftrip.io.booking_service.contracts.ReservationRequests;
using ftrip.io.framework.Correlation;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests
{
    public interface ICatalogServiceClient
    {
        Task<PriceInfo> GetPriceInfo(Guid accommodationId, DateTime checkIn, DateTime checkOut, int guests, CancellationToken cancellationToken);
    }

    public class CatalogServiceClient : ICatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly CorrelationContext _correlationContext;

        public CatalogServiceClient(
            IHttpClientFactory httpClientFactory,
            CorrelationContext correlationContext)
        {
            _httpClient = httpClientFactory.CreateClient("catalog");
            _correlationContext = correlationContext;
        }

        public async Task<PriceInfo> GetPriceInfo(Guid accommodationId, DateTime checkIn, DateTime checkOut, int guests, CancellationToken cancellationToken)
        {
            var requestUrl = $"api/Accommodations/{accommodationId}/calc-price?checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}&guests={guests}";
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add(CorrelationConstants.HeaderAttriute, _correlationContext.Id);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var responseStr = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PriceInfo>(responseStr, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
