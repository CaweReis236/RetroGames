using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static ListarJogoResponseApiModel ParaResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new()
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Ano = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos.Select(g => g.Nome).ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new()
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            Ano = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            IdPlataforma = jogo.IdPlataforma,
            IdCondicaoJogo = jogo.IdCondicaoJogo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos.Select(g =>
                new GeneroJogoResponseApiModel
                {
                    Id = g.Id,
                    Nome = g.Nome
                }).ToList()
        };
    }
}
