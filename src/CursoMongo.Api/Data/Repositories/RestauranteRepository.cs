using CursoMongo.Api.Data.Schemas;
using CursoMongo.Api.Domain.Entities;
using CursoMongo.Api.Domain.Enums;
using CursoMongo.Api.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CursoMongo.Api.Data.Repositories
{
    public class RestauranteRepository
    {
        IMongoCollection<RestauranteSchema> _restaurantes;

        public RestauranteRepository(MongoDb mongoDB)
        {
            _restaurantes = mongoDB.DB.GetCollection<RestauranteSchema>("restaurante");
        }

        public void Inserir(Restaurante restaurante)
        {
            RestauranteSchema doc = new()
            {
                Nome = restaurante.Nome!,
                Cozinha = restaurante.Cozinha,
                Endereco = new EnderecoSchema
                {
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    Cidade = restaurante.Endereco.Cidade,
                    Cep = restaurante.Endereco.Cep,
                    UF = restaurante.Endereco.UF
                }
            };

            _restaurantes.InsertOne(doc);
        }

        public async Task<IEnumerable<Restaurante>> ObterTodos()
        {
            List<Restaurante> restaurantes = new();

            // // Utilizando filtros

            // var filter = Builders<RestauranteSchema>.Filter.Empty;
            //     await _restaurantes.Find(filter).ForEachAsync(d =>
            // {
            //     Restaurante r = new(d.Id.ToString(), d.Nome, d.Cozinha);
            //     Endereco e = new(d.Endereco.Logradouro, d.Endereco.Numero, d.Endereco.Cidade, d.Endereco.UF, d.Endereco.Cep);
            //     r.AtribuirEndereco(e);
            //     restaurantes.Add(r);
            // });

            // // Utilizando lambdas
            
            // await _restaurantes.Find(r => r.Nome.Length > 0).ForEachAsync(d =>
            // {
            //     Restaurante r = new(d.Id.ToString(), d.Nome, d.Cozinha);
            //     Endereco e = new(d.Endereco.Logradouro, d.Endereco.Numero, d.Endereco.Cidade, d.Endereco.UF, d.Endereco.Cep);
            //     r.AtribuirEndereco(e);
            //     restaurantes.Add(r);
            // });

            // // Utilizando querys

            await _restaurantes.AsQueryable().ForEachAsync(d => 
            {
                Restaurante r = new(d.Id.ToString(), d.Nome, d.Cozinha);
                Endereco e = new(d.Endereco.Logradouro, d.Endereco.Numero, d.Endereco.Cidade, d.Endereco.UF, d.Endereco.Cep);
                r.AtribuirEndereco(e);
                restaurantes.Add(r);
            });

            return restaurantes;
        }

        public Restaurante ObterPorId(string id)        
        {
            var document = _restaurantes.AsQueryable().FirstOrDefault(x => x.Id == id);

            if (document is null)
            {
                return null;
            }

            return document.ConverterParaDomain();
        }

        public bool AlterarCompleto(Restaurante rest)
        {
            RestauranteSchema doc = new()
            {
                Id = rest.Id,
                Nome = rest.Nome,
                Cozinha = rest.Cozinha,
                Endereco = new EnderecoSchema
                {
                    Logradouro = rest.Endereco.Logradouro,
                    Numero = rest.Endereco.Numero,
                    Cidade = rest.Endereco.Cidade,
                    Cep = rest.Endereco.Cep,
                    UF = rest.Endereco.UF
                }
            };

            var resultado = _restaurantes.ReplaceOne(x => x.Id == doc.Id, doc);

            return resultado.ModifiedCount > 0;
        }

        public bool AlterarCozinha(string id, ECozinha cozinha)
        {
            var atualizacao = Builders<RestauranteSchema>.Update.Set(x => x.Cozinha, cozinha);

            var resultado = _restaurantes.UpdateOne(x => x.Id == id, atualizacao);

            return resultado.ModifiedCount > 0;
        }

        public IEnumerable<Restaurante> ObterPorNome(string nome)
        {
            List<Restaurante> restaurantes = new();

            // Usando filters
            // var filter = new BsonDocument { { "nome", new BsonDocument { { "$regex", nome }, { "$options", "i" } } } };
            
            // _restaurantes.Find(filter)
            //     .ToList()
            //     .ForEach(d => restaurantes.Add(d.ConverterParaDomain()));

            // Usando LINQ
            _restaurantes.AsQueryable()
                .Where(x => x.Nome.ToLower().Contains(nome.ToLower()))
                .ToList()
                .ForEach(d => restaurantes.Add(d.ConverterParaDomain()));
            
            return restaurantes;
        }
    }
}