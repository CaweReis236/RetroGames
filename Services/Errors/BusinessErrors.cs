using System.Net;

namespace RetroGames.Services.Errors;

// Representa um erro de negócio com código, mensagem e status HTTP.
public sealed record BusinessError(
    int Code,
    string Message,
    HttpStatusCode HttpStatusCode
);

public static class BusinessErrors
{
    public static readonly BusinessError PLATAFORMA_NAO_ENCONTRADA =
        new(
            100,
            "Plataforma não encontrada.",
            HttpStatusCode.NotFound
        );

    public static readonly BusinessError PLATAFORMA_NOME_DUPLICADO =
        new(
            101,
            "Já existe uma plataforma com este nome.",
            HttpStatusCode.Conflict
        );

    public static readonly BusinessError PLATAFORMA_POSSUI_JOGOS =
        new(
            102,
            "Não é possível excluir: existem jogos desta plataforma.",
            HttpStatusCode.Conflict
        );

    public static readonly BusinessError JOGO_NAO_ENCONTRADO =
        new(
            103,
            "Jogo não encontrado.",
            HttpStatusCode.NotFound
        );

    public static readonly BusinessError CONDICAO_NAO_ENCONTRADA =
        new(
            104,
            "A condição informada não existe.",
            HttpStatusCode.NotFound
        );

    public static readonly BusinessError GENERO_NAO_ENCONTRADO =
        new(
            105,
            "Um ou mais gêneros informados não existem.",
            HttpStatusCode.NotFound
        );
    public static readonly BusinessError EMAIL_JA_CADASTRADO = new(106, "Já existe uma conta com este e-mail.", HttpStatusCode.Conflict);
    public static readonly BusinessError JOGO_INDISPONIVEL = new(110, "Um ou mais jogos não estão disponíveis para venda.", HttpStatusCode.BadRequest);
    public static readonly BusinessError ESTOQUE_INSUFICIENTE = new(111, "Estoque insuficiente.", HttpStatusCode.Conflict);
}
