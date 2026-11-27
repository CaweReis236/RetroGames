using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirPlataformaRequestValidator : AbstractValidator<InserirPlataformaRequestApiModel>
{
    public InserirPlataformaRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome.").MaximumLength(80);
        RuleFor(x => x.Fabricante).NotEmpty().WithMessage("Informe o fabricante.").MaximumLength(80);
        RuleFor(x => x.AnoLancamento).InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido.");
    }
}
public class AlterarPlataformaRequestValidator : AbstractValidator<AlterarPlataformaRequestApiModel>
{
    public AlterarPlataformaRequestValidator() { Include(new InserirPlataformaRequestValidator()); }
}
public class InserirJogoRequestValidator : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descricao).MaximumLength(1000);
        RuleFor(x => x.AnoLancamento).InclusiveBetween(1970, 2100);
        RuleFor(x => x.Preco).GreaterThan(0).PrecisionScale(10, 2, false);
        RuleFor(x => x.Estoque).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IdPlataforma).GreaterThan(0);
        RuleFor(x => x.IdCondicaoJogo).GreaterThan(0);
        RuleFor(x => x.IdsGeneros).NotEmpty().WithMessage("Selecione pelo menos um gênero.");
        RuleForEach(x => x.IdsGeneros).GreaterThan(0);
    }
}
public class AlterarJogoRequestValidator : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator() { Include(new InserirJogoRequestValidator()); }
}
public class ListarJogoRequestValidator : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(x => x.Pagina).InclusiveBetween(1, 1000000);
        RuleFor(x => x.TamanhoPagina).InclusiveBetween(1, 100);
        RuleFor(x => x.Busca).MaximumLength(150);
    }
}
