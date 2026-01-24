using Infra.NoSql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace API.HealthChecks
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly AuditDbContext _context;

        public MongoDbHealthCheck(AuditDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Tenta fazer um ping no MongoDB
                var filter = Builders<Domain.Entities.AuditLog>.Filter.Empty;
                await _context.AuditLogs.Find(filter).Limit(1).FirstOrDefaultAsync(cancellationToken);

                return HealthCheckResult.Healthy("MongoDB está funcionando.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Erro ao verificar saúde do MongoDB.",
                    ex);
            }
        }
    }
}
