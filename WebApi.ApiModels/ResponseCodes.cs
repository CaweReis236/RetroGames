namespace RetroGames.WebApi.ApiModels;

public static class ResponseCodes
{
    public static readonly (int Code, string DefaultMessage) SUCESSO =
        (0, "Sucesso.");

    public static readonly (int Code, string DefaultMessage) ERRO =
        (1, "Ocorreu um erro interno.");

    public static readonly (int Code, string DefaultMessage) MODEL_NULL = (2, "O modelo veio nulo.");
    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO = (3, "Há um erro com o modelo enviado.");

    public static readonly (int Code, string DefaultMessage) NAO_AUTENTICADO = (4, "Faça login para continuar.");
    public static readonly (int Code, string DefaultMessage) ACESSO_NEGADO = (5, "Você não tem permissão para esta operação.");
    public static readonly (int Code, string DefaultMessage) CREDENCIAIS_INVALIDAS = (6, "E-mail ou senha inválidos.");
}
