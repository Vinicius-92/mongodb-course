using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CursoMongo.Api.Data.Schemas
{
    public class RestauranteAvalicaoSchema
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public double MediaEstrelas { get; set; }
        public List<RestauranteSchema> Restaurante { get; set; }
        public List<AvaliacaoSchema> Avaliacoes { get; set; }
    }
}