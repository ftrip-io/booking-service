using ftrip.io.booking_service.AccommodationOccupancies.UseCases.CheckAvailability;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.booking_service.Reservations.UseCases.ReadReservation;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations
{
    public interface IReservationRepository : IRepository<Reservation, Guid>
    {
        Task<bool> HasAnyByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken);

        Task<IEnumerable<Reservation>> ReadByQuery(ReadReservationQuery query, CancellationToken cancellationToken);

        Task<IEnumerable<Reservation>> ReadForGuestInPast(Guid guestId, CancellationToken cancellationToken);

        Task<int> CountActiveForGuest(Guid guestId, CancellationToken cancellationToken);

        Task<int> CountActiveByAccommodations(IEnumerable<Guid> accomodationIds, CancellationToken cancellationToken);

        Task<bool> HasGuestReservedAccomodationInPast(Guid guestId, Guid accomodationId, CancellationToken cancellationToken);

        Task<bool> HasGuestReservedAnyOfAccomodationsInPast(Guid guestId, IEnumerable<Guid> accomodationIds, CancellationToken cancellationToken);

        Task<IEnumerable<Guid>> ReadByAccommodationsAndDatePeriod(CheckAvailabilityQuery query, CancellationToken cancellationToken);
    }

    public class ReservationRepository : Repository<Reservation, Guid>, IReservationRepository
    {
        public ReservationRepository(DbContext context) : base(context)
        {
        }

        public async Task<bool> HasAnyByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken)
        {
            return await _entities.Where(r => r.AccomodationId == accomodationId && !r.IsCancelled &&
                                        ((r.DatePeriod.DateFrom <= period.DateTo && r.DatePeriod.DateTo >= period.DateFrom) ||
                                        (r.DatePeriod.DateFrom >= period.DateFrom && r.DatePeriod.DateFrom <= period.DateTo)))
                                  .AnyAsync(cancellationToken);
        }

        public async Task<IEnumerable<Reservation>> ReadByQuery(ReadReservationQuery query, CancellationToken cancellationToken)
        {
            return await _entities
                .Where(r => !r.IsCancelled || (r.IsCancelled == query.IncludeCancelled.GetValueOrDefault()))
                .Where(r => !query.GuestId.HasValue || r.GuestId == query.GuestId)
                .Where(r => !query.AccommodationId.HasValue || r.AccomodationId == query.AccommodationId)
                .Where(r => !query.PeriodFrom.HasValue || r.DatePeriod.DateFrom >= query.PeriodFrom)
                .Where(r => !query.PeriodTo.HasValue || r.DatePeriod.DateTo <= query.PeriodTo)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Reservation>> ReadForGuestInPast(Guid guestId, CancellationToken cancellationToken)
        {
            return await _entities.Where(r =>
                !r.IsCancelled &&
                r.GuestId == guestId &&
                r.DatePeriod.DateFrom < DateTime.Now)
            .ToListAsync(cancellationToken);
        }

        public async Task<int> CountActiveForGuest(Guid guestId, CancellationToken cancellationToken)
        {
            return await _entities.CountAsync(r =>
                !r.IsCancelled &&
                r.GuestId == guestId &&
                r.DatePeriod.DateFrom >= DateTime.Now
            , cancellationToken);
        }

        public async Task<int> CountActiveByAccommodations(IEnumerable<Guid> accomodationIds, CancellationToken cancellationToken)
        {
            return await _entities.CountAsync(r =>
               !r.IsCancelled &&
               accomodationIds.Contains(r.AccomodationId) &&
               r.DatePeriod.DateFrom >= DateTime.Now
           , cancellationToken);
        }

        public async Task<bool> HasGuestReservedAccomodationInPast(Guid guestId, Guid accomodationId, CancellationToken cancellationToken)
        {
            return await _entities.AnyAsync(r =>
                !r.IsCancelled &&
                r.GuestId == guestId &&
                r.AccomodationId == accomodationId &&
                r.DatePeriod.DateFrom < DateTime.Now
            , cancellationToken);
        }

        public async Task<bool> HasGuestReservedAnyOfAccomodationsInPast(Guid guestId, IEnumerable<Guid> accomodationIds, CancellationToken cancellationToken)
        {
            return await _entities.AnyAsync(r =>
                !r.IsCancelled &&
                r.GuestId == guestId &&
                accomodationIds.Contains(r.AccomodationId) &&
                r.DatePeriod.DateFrom < DateTime.Now
            , cancellationToken);
        }

        public async Task<IEnumerable<Guid>> ReadByAccommodationsAndDatePeriod(CheckAvailabilityQuery query, CancellationToken cancellationToken) {
            return await _entities
                .Where(r => !r.IsCancelled)
                .Where(r => query.AccommodationIds.Contains(r.AccomodationId))
                 .Where(r => ((r.DatePeriod.DateFrom <= query.PeriodTo && r.DatePeriod.DateTo >= query.PeriodFrom) ||
                              (r.DatePeriod.DateFrom >= query.PeriodFrom && r.DatePeriod.DateFrom <= query.PeriodTo)))
                .Select(r => r.AccomodationId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}