using CursoMongo.Api.Controllers.Inputs;
using CursoMongo.Api.Controllers.Outputs;
using CursoMongo.Api.Data.Repositories;
using CursoMongo.Api.Data.Schemas;
using CursoMongo.Api.Domain.Entities;
using CursoMongo.Api.Domain.Enums;
using CursoMongo.Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CursoMongo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RestaurantesController : ControllerBase
{
    private readonly RestauranteRepository _restaurantesRepository;

    public RestaurantesController(RestauranteRepository restauranteRepository)
    {
        _restaurantesRepository = restauranteRepository;
    }

    [HttpPost]
    public ActionResult IncluirRestaurante(RestauranteInclusao restauranteInclusao)
    {
        var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteInclusao.Cozinha);

        var restaurante = new Restaurante(restauranteInclusao.Nome!, cozinha);
        var endereco = new Endereco(
            restauranteInclusao.Logradouro,
            restauranteInclusao.Numero,
            restauranteInclusao.Cidade,
            restauranteInclusao.UF,
            restauranteInclusao.Cep
        );

        restaurante.AtribuirEndereco(endereco);

        if (!restaurante.Validar())
        {
            return BadRequest(
                new
                {
                    errors = restaurante.ValidationResult!.Errors.Select(_ => _.ErrorMessage)
                }
            );
        }

        _restaurantesRepository.Inserir(restaurante);

        return Ok(
            new
            {
                data = "Restaurante inserido com sucesso."
            }
        );
    }

    [HttpGet]
    public async Task<ActionResult> ObterRestaurantes()
    {
        var restaurantes = await _restaurantesRepository.ObterTodos();

        var listagem = restaurantes.Select(x => new RestauranteListagem
        {
            Id = x.Id,
            Nome = x.Nome,
            Cozinha = (int) x.Cozinha,
            Cidade = x.Endereco.Cidade
        });

        return Ok(
            new
            {
                data = listagem
            }
        );
    }

    [HttpGet("{id}")]
    public ActionResult ObterRestaurantePorId(string id)
    {
        var restaurante = _restaurantesRepository.ObterPorId(id);

        if (restaurante is null)
        {
            return NotFound();
        }

        var exibicao = new RestauranteExibicao
        {
            Id = restaurante.Id,
            Nome = restaurante.Nome,
            Cozinha = (int) restaurante.Cozinha,
            Endereco = new EnderecoExibicao
            {
                Logradouro = restaurante.Endereco.Logradouro,
                Numero = restaurante.Endereco.Numero,
                Cidade = restaurante.Endereco.Cidade,
                Cep = restaurante.Endereco.Cep,
                UF = restaurante.Endereco.UF
            }
        };

        return Ok(
            new
            {
                data = exibicao
            }
        );
    }

    [HttpPut]
    public ActionResult AlterarRestaurante(RestauranteAlteracaoCompleta rest)
    {
        var restaurante = _restaurantesRepository.ObterPorId(rest.Id);

        if (restaurante is null)
        {
            return NotFound();
        }

        var cozinha = ECozinhaHelper.ConverterDeInteiro(rest.Cozinha);
        
        restaurante = new Restaurante(rest.Id, rest.Nome, cozinha);
        Endereco endereco = new(
            rest.Logradouro,
            rest.Numero,
            rest.Cidade,
            rest.UF,
            rest.Cep
        );

        restaurante.AtribuirEndereco(endereco);

        if (!restaurante.Validar())
        {
            return BadRequest(
                new
                {
                    erros = restaurante.ValidationResult.Errors.Select(x => x.ErrorMessage)
                }
            );
        }

        if (!_restaurantesRepository.AlterarCompleto(restaurante))
        {
            return BadRequest(
                new
                {
                    errors = "Nenhum documento foi alterado"
                }
            );
        }

        return Ok(
            new
            {
                data = "Restaurante alterado com sucesso"
            }
        );
    }

    [HttpPatch("{id}")]
    public ActionResult AlterarCozinha(string id, RestauranteAlteracaoParcial rest)
    {
        var restaurante = _restaurantesRepository.ObterPorId(id);

        if (restaurante is null)
        {
            return NotFound();
        }

        var cozinha = ECozinhaHelper.ConverterDeInteiro(rest.Cozinha);

        if (_restaurantesRepository.AlterarCozinha(id, cozinha))
        {
            return BadRequest(new
            {
                errors = "Nenhum documento foi alterado"
            });
        }

        return Ok(new
        {
            data = "Restaurante alterado com sucesso."
        });
    }

    [HttpGet("{nome}")]
    public ActionResult ObterRestaurantePorNome(string nome)
    {
        var restaurantes = _restaurantesRepository.ObterPorNome(nome);

        var listagem = restaurantes.Select(x => new RestauranteListagem
        {
            Id = x.Id,
            Nome = x.Nome,
            Cozinha = (int) x.Cozinha,
            Cidade = x.Endereco.Cidade
        });

        return Ok(
            new
            {
                data = listagem
            }
        );
    }

    [HttpPost("{id}/avaliar")]
    public ActionResult AvaliarRestaurante(string id, AvaliacaoInclusao avaliacaoInclusao)
    {
        var restaurante = _restaurantesRepository.ObterPorId(id);

        if (restaurante is null)
        {
            return NotFound();
        }

        Avaliacao avaliacao = new(avaliacaoInclusao.Estrelas, avaliacaoInclusao.Comentario);

        if (!avaliacao.Validar())
        {
            return BadRequest(
                new
                {
                    errors = avaliacao.ValidationResult.Errors.Select(x => x.ErrorMessage)
                }
            );
        }

        _restaurantesRepository.Avaliar(id, avaliacao);

        return Ok(new
        {
            data = "Restaurante avaliado com sucesso."
        });
    }

    [HttpGet("/top3")]
    public async Task<ActionResult> ObterTop3Restaurantes()
    {
        var top3 = await _restaurantesRepository.ObterTop3();

        var listagem = top3.Select(x => new RestauranteTop3
        {
            Id = x.Key.Id,
            Nome = x.Key.Nome,
            Cozinha = (int) x.Key.Cozinha,
            Cidade = x.Key.Endereco.Cidade,
            Estrelas = x.Value
        });

        return Ok(
            new
            {
                data = listagem
            }
        );
    }

    [HttpGet("/top3-lookup")]
    public async Task<ActionResult> ObterTop3RestaurantesComLookup()
    {
        var top3 = await _restaurantesRepository.ObterTop3_ComLookup();

        var listagem = top3.Select(x => new RestauranteTop3
        {
            Id = x.Key.Id,
            Nome = x.Key.Nome,
            Cozinha = (int) x.Key.Cozinha,
            Cidade = x.Key.Endereco.Cidade,
            Estrelas = x.Value
        });

        return Ok(
            new
            {
                data = listagem
            }
        );
    }

    [HttpDelete("{id}")]
    public ActionResult Remover(string id)
    {
        var restaurante = _restaurantesRepository.ObterPorId(id);

        if (restaurante is null)
        {
            return NotFound();
        }

        (var totalRestauranteRemovido, var totalAvaliacoesRemovidas) = _restaurantesRepository.Remover(id);

        return Ok(
            new
            {
                data = $"Total de exclusões: {totalRestauranteRemovido} restaurante com {totalAvaliacoesRemovidas} avaliações."
            }
        );
    }

    [HttpGet("buscaPorNome")]
    public async Task<ActionResult> ObterPorBuscaTextual([FromQuery] string texto)
    {
        var restaurantes = await _restaurantesRepository.ObterPorBuscaTextual(texto);

        var listagem = restaurantes.ToList().Select(x => new RestauranteListagem
        {
            Id = x.Id,
            Nome = x.Nome,
            Cozinha = (int) x.Cozinha,
            Cidade = x.Endereco.Cidade
        });

        return Ok(
            new
            {
                data = listagem
            }
        );
    }
}
