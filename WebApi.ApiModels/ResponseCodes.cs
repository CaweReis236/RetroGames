namespace RetroGames.WebApi.ApiModels;

public static class ResponseCodes
{
    public static readonly (int Code, string DefaultMessage) SUCESSO =
        (0, "Sucesso.");

    public static readonly (int Code, string DefaultMessage) ERRO =
        (1, "Ocorreu um erro interno.");
}
