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
    public interface IHostReviewsSummaryRepository : IRepository<HostReviewsSummary, Guid>
    {
        Task<HostReviewsSummary> ReadByHostId(Guid hostId, CancellationToken cancellationToken);

        Task<HostReviewsSummary> ComputeByHostId(Guid hostId, CancellationToken cancellationToken);
    }

    public class HostReviewsSummaryRepository : Repository<HostReviewsSummary, Guid>, IHostReviewsSummaryRepository
    {
        public HostReviewsSummaryRepository(DbContext context) :
            base(context)
        {
        }

        public async Task<HostReviewsSummary> ReadByHostId(Guid hostId, CancellationToken cancellationToken)
        {
            return await _entities.FirstOrDefaultAsync(r => r.HostId == hostId, cancellationToken);
        }

        public async Task<HostReviewsSummary> ComputeByHostId(Guid hostId, CancellationToken cancellationToken)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();

            var accomodationParameter = new MySqlParameter("@hostId", hostId);
            command.CommandText =
                "Select AVG(CommunicationGrade) as CommunicationGrade, AVG(OverallGrade) as OverallGrade, COUNT(*) as ReviewsCount " +
                "from HostReviews " +
                "where HostId = @hostId and Active = 1";
            command.CommandType = CommandType.Text;
            command.Parameters.Add(accomodationParameter);

            await _context.Database.OpenConnectionAsync();
            using var result = await command.ExecuteReaderAsync();
            while (await result.ReadAsync())
            {
                return new HostReviewsSummary()
                {
                    HostId = hostId,
                    Grades = new HostGradesSummary()
                    {
                        Communication = result.GetDecimal(0),
                        Overall = result.GetDecimal(1),
                    },
                    ReviewsCount = result.GetInt32(2)
                };
            }

            return null;
        }
    }
}