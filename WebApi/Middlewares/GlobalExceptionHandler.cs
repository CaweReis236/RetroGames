using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services.Errors;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int status;
        int codigo;
        string mensagem;

        if (exception is BusinessRuleException regra)
        {
            status = (int)regra.Erro.HttpStatusCode;
            codigo = regra.Erro.Code;
            mensagem = regra.Message;
        }
        else
        {
            _logger.LogError(
                exception,
                "Erro inesperado ao processar a requisição."
            );

            status = StatusCodes.Status500InternalServerError;
            codigo = ResponseCodes.ERRO.Code;
            mensagem = ResponseCodes.ERRO.DefaultMessage;
        }

        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(
            new RetroGamesResponseApiModel
            {
                ResponseCode = codigo,
                ErrorMessage = mensagem
            },
            cancellationToken
        );

        return true;
    }
}
