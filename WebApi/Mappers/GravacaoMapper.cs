using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class GravacaoMapper
{
    public static Plataforma ParaModel(this InserirPlataformaRequestApiModel request, int id = 0) => new()
    {
        Id = id, Nome = request.Nome, Fabricante = request.Fabricante, AnoLancamento = request.AnoLancamento
    };
    public static Jogo ParaModel(this InserirJogoRequestApiModel request, int id = 0) => new()
    {
        Id = id, Nome = request.Nome, Descricao = request.Descricao, AnoLancamento = request.AnoLancamento,
        Preco = request.Preco, Estoque = request.Estoque, IdPlataforma = request.IdPlataforma,
        IdCondicaoJogo = request.IdCondicaoJogo, Ativo = request is AlterarJogoRequestApiModel a ? a.Ativo : true
    };
}
