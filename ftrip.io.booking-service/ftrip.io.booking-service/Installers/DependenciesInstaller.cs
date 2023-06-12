using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.ReservationRequests;
using ftrip.io.booking_service.Reservations;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Proxies;
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
            _services.AddProxiedScoped<IReservationRequestRepository, ReservationRequestRepository>();
            _services.AddProxiedScoped<IReservationRequestPermissionHelper, ReservationRequestPermissionHelper>();

            _services.AddProxiedScoped<IReservationRepository, ReservationRepository>();
            _services.AddProxiedScoped<IReservationRequestQueryHelper, ReservationRequestQueryHelper>();
            _services.AddProxiedScoped<IReservationsPermissionHelper, ReservationsPermissionHelper>();

            _services.AddProxiedScoped<IAccommodationRepository, AccommodationRepository>();
            _services.AddProxiedScoped<IAccommodationQueryHelper, AccommodationQueryHelper>();
            _services.AddProxiedScoped<IAccommodationPermissionHelper, AccommodationPermissionHelper>();
        }
    }
}