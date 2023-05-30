using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.ReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases;
using ftrip.io.booking_service.Reservations;
using ftrip.io.framework.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace ftrip.io.booking_service.Installers
{
    public class DependenciesInstaller : IInstaller
    {
        private readonly IServiceCollection _services;

        public DependenciesInstaller(IServiceCollection services)
        {
            _services = services;
        }

        public void Install() 
        {
            _services.AddScoped<IReservationRequestRepository, ReservationRequestRepository>();
            _services.AddScoped<IReservationRepository, ReservationRepository>();
            _services.AddScoped<IReservationRequestQueryHelper, ReservationRequestQueryHelper>();
            _services.AddScoped<IAccommodationRepository, AccommodationRepository>();
            _services.AddScoped<IAccommodationQueryHelper, AccommodationQueryHelper>();
        }
    }
}
