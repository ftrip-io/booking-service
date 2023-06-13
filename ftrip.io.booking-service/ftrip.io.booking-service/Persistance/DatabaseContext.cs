using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.booking_service.Reviews.Domain;
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

        public DbSet<AccomodationReview> AccomodationReviews { get; set; }
        public DbSet<AccomodationReviewsSummary> AccomodationReviewsSummaries { get; set; }
        public DbSet<HostReview> HostReviews { get; set; }
        public DbSet<HostReviewsSummary> HostReviewsSummaries { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, CurrentUserContext currentUserContext) :
            base(options, currentUserContext)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReservationRequest>().OwnsOne(r => r.DatePeriod);

            modelBuilder.Entity<ReservationRequest>().Ignore("CanBeModified");

            modelBuilder.Entity<Reservation>().OwnsOne(r => r.DatePeriod);

            ConfigureAccomodationReview(modelBuilder);
            ConfigureAccomodationReviewsSummary(modelBuilder);

            ConfigureHostReview(modelBuilder);
            ConfigureHostReviewsSummary(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureAccomodationReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccomodationReview>(reviewBuilder =>
            {
                reviewBuilder.OwnsOne(r => r.Grades, gradesBuilder =>
                {
                    gradesBuilder.Property(g => g.Accomodation).HasColumnName("AccomodationGrade");
                    gradesBuilder.Property(g => g.Location).HasColumnName("LocationGrade");
                    gradesBuilder.Property(g => g.ValueForMoney).HasColumnName("ValueForMoneyGrade");

                    gradesBuilder.Ignore("Average");
                });

                reviewBuilder.OwnsOne(r => r.Recension, recensionBuilder =>
                {
                    recensionBuilder.Property(r => r.Text).HasColumnName("RecensionText");
                });

                reviewBuilder
                    .HasIndex(p => p.AccomodationId)
                    .IncludeProperties(nameof(AccomodationReview.Grades));
            });
        }

        private void ConfigureAccomodationReviewsSummary(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccomodationReviewsSummary>(reviewBuilder =>
            {
                reviewBuilder.OwnsOne(r => r.Grades, gradesBuilder =>
                {
                    gradesBuilder.Property(g => g.Accomodation).HasColumnName("AccomodationGrade");
                    gradesBuilder.Property(g => g.Location).HasColumnName("LocationGrade");
                    gradesBuilder.Property(g => g.ValueForMoney).HasColumnName("ValueForMoneyGrade");

                    gradesBuilder.Ignore("Average");
                });

                reviewBuilder
                    .HasIndex(p => p.AccomodationId)
                    .IsUnique()
                    .IncludeProperties(nameof(AccomodationReviewsSummary.ReviewsCount), nameof(AccomodationReviewsSummary.Grades));
            });
        }

        private void ConfigureHostReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HostReview>(reviewBuilder =>
            {
                reviewBuilder.OwnsOne(r => r.Grades, gradesBuilder =>
                {
                    gradesBuilder.Property(g => g.Communication).HasColumnName("CommunicationGrade");
                    gradesBuilder.Property(g => g.Overall).HasColumnName("OverallGrade");

                    gradesBuilder.Ignore("Average");
                });

                reviewBuilder.OwnsOne(r => r.Recension, recensionBuilder =>
                {
                    recensionBuilder.Property(r => r.Text).HasColumnName("RecensionText");
                });

                reviewBuilder
                    .HasIndex(p => p.HostId)
                    .IncludeProperties(nameof(HostReview.Grades));
            });
        }

        private void ConfigureHostReviewsSummary(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HostReviewsSummary>(reviewBuilder =>
            {
                reviewBuilder.OwnsOne(r => r.Grades, gradesBuilder =>
                {
                    gradesBuilder.Property(g => g.Communication).HasColumnName("CommunicationGrade");
                    gradesBuilder.Property(g => g.Overall).HasColumnName("OverallGrade");

                    gradesBuilder.Ignore("Average");
                });

                reviewBuilder
                    .HasIndex(p => p.HostId)
                    .IsUnique()
                    .IncludeProperties(nameof(HostReviewsSummary.ReviewsCount), nameof(HostReviewsSummary.Grades));
            });
        }
    }
}