using Infra.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace API.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _context;

        public DatabaseHealthCheck(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                
                if (canConnect)
                {
                    return HealthCheckResult.Healthy("PostgreSQL está funcionando.");
                }

                return HealthCheckResult.Unhealthy("Não foi possível conectar ao PostgreSQL.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Erro ao verificar saúde do PostgreSQL.", 
                    ex);
            }
        }
    }
}
