namespace RetroGames.Repository.Exceptions;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException()
        : base("Já existe um registro com esse valor.")
    {
    }

    public RegistroDuplicadoException(string mensagem)
        : base(mensagem)
    {
    }
}
