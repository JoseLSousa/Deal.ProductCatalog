using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infra.NoSql
{
    public class AuditDbContext
    {
        public IMongoCollection<AuditLog> AuditLogs { get; }

        public AuditDbContext(IConfiguration config)
        {
            BsonSerializer.RegisterSerializer(
                new GuidSerializer(GuidRepresentation.Standard));
            var client = new MongoClient(
                config["MongoAuditSettings:ConnectionString"]);
            var database = client.GetDatabase(
                config["MongoAuditSettings:DatabaseName"]);
            AuditLogs = database.GetCollection<AuditLog>(
                config["MongoAuditSettings:CollectionName"]);
        }
    }
}
