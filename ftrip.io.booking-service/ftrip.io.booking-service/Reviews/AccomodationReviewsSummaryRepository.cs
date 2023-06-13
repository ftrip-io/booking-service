using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    public interface IAccomodationReviewsSummaryRepository : IRepository<AccomodationReviewsSummary, Guid>
    {
        Task<AccomodationReviewsSummary> ReadByAccomodationId(Guid accomodationId, CancellationToken cancellationToken);

        Task<AccomodationReviewsSummary> ComputeByAccomodationId(Guid accomodationId, CancellationToken cancellationToken);
    }

    public class AccomodationReviewsSummaryRepository : Repository<AccomodationReviewsSummary, Guid>, IAccomodationReviewsSummaryRepository
    {
        public AccomodationReviewsSummaryRepository(DbContext context) :
            base(context)
        {
        }

        public async Task<AccomodationReviewsSummary> ReadByAccomodationId(Guid accomodationId, CancellationToken cancellationToken)
        {
            return await _entities.FirstOrDefaultAsync(r => r.AccomodationId == accomodationId, cancellationToken);
        }

        public async Task<AccomodationReviewsSummary> ComputeByAccomodationId(Guid accomodationId, CancellationToken cancellationToken)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();

            var accomodationParameter = new MySqlParameter("@accomodationId", accomodationId);
            command.CommandText =
                "Select AVG(AccomodationGrade) as AccomodationGrade, AVG(LocationGrade) as LocationGrade, AVG(ValueForMoneyGrade) as ValueForMoneyGrade, COUNT(*) as ReviewsCount " +
                "from AccomodationReviews " +
                "where AccomodationId = @accomodationId and Active = 1";
            command.CommandType = CommandType.Text;
            command.Parameters.Add(accomodationParameter);

            await _context.Database.OpenConnectionAsync();
            using var result = await command.ExecuteReaderAsync();
            while (await result.ReadAsync())
            {
                return new AccomodationReviewsSummary()
                {
                    AccomodationId = accomodationId,
                    Grades = new AccomodationGradesSummary()
                    {
                        Accomodation = result.GetDecimal(0),
                        Location = result.GetDecimal(1),
                        ValueForMoney = result.GetDecimal(2)
                    },
                    ReviewsCount = result.GetInt32(3)
                };
            }

            return null;
        }
    }
}