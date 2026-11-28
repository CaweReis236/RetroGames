using FluentValidation;
using RetroGames.WebApi.ApiModels;
namespace RetroGames.WebApi.Validators;
public class PedidoRequestValidator : AbstractValidator<PedidoRequestApiModel>
{
    public PedidoRequestValidator()
    {
        RuleFor(x=>x.Itens).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Adicione ao menos um item.")
            .Must(itens=>itens.All(i=>i is not null) && itens.Select(i=>i.IdJogo).Distinct().Count()==itens.Count)
            .WithMessage("Cada jogo deve aparecer uma única vez.");
        RuleForEach(x=>x.Itens).NotNull().ChildRules(item=>{
            item.RuleFor(i=>i.IdJogo).GreaterThan(0);
            item.RuleFor(i=>i.Quantidade).InclusiveBetween(1,10).WithMessage("A quantidade deve ser de 1 a 10.");
        });
    }
}
