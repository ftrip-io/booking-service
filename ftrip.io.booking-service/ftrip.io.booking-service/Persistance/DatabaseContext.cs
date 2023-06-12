using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.Persistence.Sql.Database;
using Microsoft.EntityFrameworkCore;

namespace ftrip.io.booking_service.Persistance
{
    public class DatabaseContext : DatabaseContextBase<DatabaseContext>
    {
        public DbSet<ReservationRequest> ReservationRequests { get; set; }    
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, CurrentUserContext currentUserContext) :
            base(options, currentUserContext)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReservationRequest>().OwnsOne(r => r.DatePeriod);

            modelBuilder.Entity<ReservationRequest>().Ignore("CanBeModified");

            modelBuilder.Entity<Reservation>().OwnsOne(r => r.DatePeriod);

            base.OnModelCreating(modelBuilder);
        }
    }
}
