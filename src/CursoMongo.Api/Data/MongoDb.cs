using CursoMongo.Api.Data.Schemas;
using CursoMongo.Api.Domain.Entities;
using CursoMongo.Api.Domain.Enums;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CursoMongo.Api.Data.Repositories
{
    public class MongoDb
    {
        public IMongoDatabase DB { get; }

        public MongoDb(IConfiguration configuration)
        {
            try
            {
                var settings = MongoClientSettings.FromUrl(new MongoUrl(configuration["ConnectionString"]));
                var client = new MongoClient(settings);
                DB = client.GetDatabase(configuration["NomeBanco"]);
                MapClasses();
            }
            catch (Exception ex)
            {
                throw new MongoException("Não foi possível se conectar ao MongoDB", ex);
            }
        }

        private void MapClasses()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(RestauranteSchema)))
            {
                BsonClassMap.RegisterClassMap<RestauranteSchema>(i => 
                {
                    i.AutoMap();
                    i.MapIdMember(c => c.Id);
                    i.MapMember(c => c.Cozinha).SetSerializer(new EnumSerializer<ECozinha>(MongoDB.Bson.BsonType.Int32));
                    i.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}