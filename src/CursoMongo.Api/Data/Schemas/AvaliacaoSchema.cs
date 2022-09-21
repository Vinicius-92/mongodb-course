using CursoMongo.Api.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CursoMongo.Api.Data.Schemas
{
    public class AvaliacaoSchema
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string RestauranteId { get; set; }
        public ObjectId Id { get; set; }
        public int Estrelas { get; set; }
        public string Comentario { get; set; }
    }

    public static class AvaliacaoSchemaExtension
    {
        public static Avaliacao ConverterParaDomain(this AvaliacaoSchema document)
        {
            return new Avaliacao(document.Estrelas, document.Comentario);
        }
    }
}