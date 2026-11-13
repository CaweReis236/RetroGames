namespace RetroGames.Services.Errors;

public class BusinessRuleException : Exception
{
    // Erro definido no catálogo BusinessErrors.
    public BusinessError Erro { get; }

    public BusinessRuleException(BusinessError erro)
        : base(erro.Message)
    {
        Erro = erro;
    }
}

