using ftrip.io.booking_service.contracts.ReservationRequests;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests
{
    public interface ICatalogServiceClient
    {
        Task<PriceInfo> GetPriceInfo(Guid id, DateTime checkIn, DateTime checkOut, int guests);
    }
    public class CatalogServiceClient : ICatalogServiceClient
    {
        private readonly HttpClient _httpClient;

        public CatalogServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("catalog");
        }

        public async Task<PriceInfo> GetPriceInfo(Guid id, DateTime checkIn, DateTime checkOut, int guests)
        {
            var responseStr = await _httpClient.GetStringAsync(
                $"api/Accommodations/{id}/calc-price?" +
                $"checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}&guests={guests}"
            );
            Console.WriteLine(responseStr);
            return JsonSerializer.Deserialize<PriceInfo>(responseStr, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        }
    }
}
