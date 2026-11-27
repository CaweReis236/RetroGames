using FluentValidation;
using RetroGames.WebApi.ApiModels;
namespace RetroGames.WebApi.Validators;
public class LoginRequestValidator : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Senha).NotEmpty().MaximumLength(500);
    }
}
