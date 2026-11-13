using RetroGames.Model.Model;
using RetroGames.Repository.Exceptions;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Errors;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    // Lista todas as plataformas.
    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _repositorio.Plataforma.ListarAsync();
    }

    // Busca uma plataforma pelo ID.
    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );
        }

        return plataforma;
    }

    // Insere uma plataforma, verificando duplicidade.
    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        bool existe = await _repositorio.Plataforma
            .ExisteComNomeAsync(plataforma.Nome);

        if (existe)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        try
        {
            plataforma.Id = await _repositorio.Plataforma
                .InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        return plataforma;
    }

    // Altera uma plataforma existente.
    public async Task<Plataforma> AlterarAsync(Plataforma plataforma)
    {
        // Verifica se a plataforma existe.
        await BuscarPorIdAsync(plataforma.Id);

        plataforma.Nome = plataforma.Nome.Trim();

        // Ignora a própria plataforma na verificação.
        bool existe = await _repositorio.Plataforma
            .ExisteComNomeAsync(plataforma.Nome, plataforma.Id);

        if (existe)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        try
        {
            await _repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        return plataforma;
    }

    // Exclui uma plataforma somente se ela não possuir jogos.
    public async Task ExcluirAsync(int id)
    {
        // Verifica se a plataforma existe.
        await BuscarPorIdAsync(id);

        bool possuiJogos = await _repositorio.Plataforma
            .PossuiJogosAsync(id);

        if (possuiJogos)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_POSSUI_JOGOS
            );
        }

        await _repositorio.Plataforma.ExcluirAsync(id);
    }
}
