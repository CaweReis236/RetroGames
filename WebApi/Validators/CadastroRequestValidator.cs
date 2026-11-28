using FluentValidation;
using RetroGames.WebApi.ApiModels;
namespace RetroGames.WebApi.Validators;
public class CadastroRequestValidator : AbstractValidator<CadastroRequestApiModel>
{
    public CadastroRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(8).MaximumLength(128)
            .WithMessage("A senha deve ter de 8 a 128 caracteres.");
    }
}
