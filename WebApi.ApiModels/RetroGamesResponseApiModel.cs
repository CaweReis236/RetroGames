namespace RetroGames.WebApi.ApiModels;

public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; }
    public string? ErrorMessage { get; set; }

    public static RetroGamesResponseApiModel Sucesso() => new()
    {
        ResponseCode = ResponseCodes.SUCESSO.Code
    };
}

public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public T? Data { get; set; }

    public static RetroGamesResponseApiModel<T> Sucesso(T data) => new()
    {
        ResponseCode = ResponseCodes.SUCESSO.Code,
        Data = data
    };
}
