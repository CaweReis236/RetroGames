using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;
using System.Text.Json;

namespace RetroGames.WebApi.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null) continue;
            var tipo = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (context.HttpContext.RequestServices.GetService(tipo) is not IValidator validator) continue;
            var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento), context.HttpContext.RequestAborted);
            if (resultado.IsValid) continue;
            var erros = resultado.Errors.GroupBy(e => JsonNamingPolicy.CamelCase.ConvertName(e.PropertyName))
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            context.Result = ModeloInvalido(erros);
            return;
        }
        await next();
    }

    public static BadRequestObjectResult ModeloInvalido(Dictionary<string, string[]> erros) =>
        new(new RetroGamesResponseApiModel<Dictionary<string, string[]>>
        {
            ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
            Data = erros
        });
}
