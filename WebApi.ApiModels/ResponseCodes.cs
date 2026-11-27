namespace RetroGames.WebApi.ApiModels;

public static class ResponseCodes
{
    public static readonly (int Code, string DefaultMessage) SUCESSO =
        (0, "Sucesso.");

    public static readonly (int Code, string DefaultMessage) ERRO =
        (1, "Ocorreu um erro interno.");

    public static readonly (int Code, string DefaultMessage) MODEL_NULL = (2, "O modelo veio nulo.");
    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO = (3, "Há um erro com o modelo enviado.");

}
