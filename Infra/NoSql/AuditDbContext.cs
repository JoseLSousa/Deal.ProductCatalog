using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infra.NoSql
{
    public class AuditDbContext
    {
        public IMongoCollection<AuditLog> AuditLogs { get; }

        public AuditDbContext(IConfiguration config)
        {
            var client = new MongoClient(
                config["MongoAuditSettings:ConnectionString"]);
            var database = client.GetDatabase(
                config["MongoAuditSettings:DatabaseName"]);
            AuditLogs = database.GetCollection<AuditLog>(
                config["MongoAuditSettings:CollectionName"]);
        }
    }
}
