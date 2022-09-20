using CursoMongo.Api.Domain.Entities;
using CursoMongo.Api.Domain.Enums;
using CursoMongo.Api.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CursoMongo.Api.Data.Schemas
{
    public class RestauranteSchema
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public EnderecoSchema Endereco { get; set; }
    }

    public static class RestauranteSchemaExtension
    {
        public static Restaurante ConverterParaDomain(this RestauranteSchema document)
        {
            Restaurante restaurante = new(document.Id.ToString(), document.Nome, document.Cozinha);
            Endereco endereco = new(document.Endereco.Logradouro, 
                                    document.Endereco.Numero, 
                                    document.Endereco.Cidade, 
                                    document.Endereco.UF, 
                                    document.Endereco.Cep);
            
            restaurante.AtribuirEndereco(endereco);

            return restaurante;
        }
    }
}