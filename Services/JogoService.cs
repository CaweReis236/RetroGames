using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository.Filtros;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Errors;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    private readonly IUsuarioAtualService _usuarioAtual;

    public JogoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    // Monta os detalhes de vários jogos sem realizar
    // uma consulta ao banco para cada jogo.
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
            return new List<JogoDetalheServiceModel>();

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();

        var idsJogos = jogos
            .Select(j => j.Id)
            .Distinct()
            .ToList();

        var vinculos = await _repositorio.JogoGenero
            .ListarPorJogosAsync(idsJogos);

        var detalhes = (
            from jogo in jogos

            join plataforma in plataformas
                on jogo.IdPlataforma equals plataforma.Id

            join condicao in condicoes
                on jogo.IdCondicaoJogo equals condicao.Id

            select new JogoDetalheServiceModel
            {
                Jogo = jogo,

                NomePlataforma = plataforma.Nome,

                DescricaoCondicao = condicao.Descricao,

                Generos = (
                    from vinculo in vinculos

                    join genero in generos
                        on vinculo.IdGenero equals genero.Id

                    where vinculo.IdJogo == jogo.Id

                    select genero
                ).ToList()
            }
        ).ToList();

        return detalhes;
    }

    // Lista jogos com filtros e paginação.
    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)>
        ListarAsync(
            int pagina,
            int tamanhoPagina,
            string? nome = null,
            int? idPlataforma = null,
            int? idGenero = null, bool incluirInativos = false)
    {
        List<int>? idsJogos = null;

        // O filtro por gênero é resolvido com LINQ.
        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero
                .ListarTodosAsync();

            idsJogos = vinculos
                .Where(v => v.IdGenero == idGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        var filtro = new JogoFiltro
        {
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            Nome = nome,
            IdPlataforma = idPlataforma,
            IdsJogos = idsJogos,
            IncluirInativos = incluirInativos && _usuarioAtual.EhAdministrador

            // IncluirInativos permanece false por padrão.
        };

        var resultado = await _repositorio.Jogo.ListarAsync(filtro);

        var detalhes = await MontarDetalhesAsync(resultado.Jogos);

        return (detalhes, resultado.Total);
    }

    // Busca um jogo ativo pelo ID.
    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null || (!jogo.Ativo && !_usuarioAtual.EhAdministrador))
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO
            );
        }

        var detalhes = await MontarDetalhesAsync(
            new List<Jogo> { jogo }
        );

        return detalhes.Single();
    }

    // Verifica se plataforma, condição e gêneros existem.
    private async Task ValidarReferenciasAsync(
        Jogo jogo,
        List<int> idsGeneros)
    {
        var plataforma = await _repositorio.Plataforma
            .BuscarPorIdAsync(jogo.IdPlataforma);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );
        }

        var condicoes = await _repositorio.CondicaoJogo
            .ListarAsync();

        bool condicaoExiste = condicoes.Any(
            c => c.Id == jogo.IdCondicaoJogo
        );

        if (!condicaoExiste)
        {
            throw new BusinessRuleException(
                BusinessErrors.CONDICAO_NAO_ENCONTRADA
            );
        }

        var generos = await _repositorio.Genero.ListarAsync();

        bool generoInexistente = idsGeneros.Any(
            id => !generos.Any(g => g.Id == id)
        );

        if (generoInexistente)
        {
            throw new BusinessRuleException(
                BusinessErrors.GENERO_NAO_ENCONTRADO
            );
        }
    }

    // Insere o jogo e seus vínculos numa transação.
    public async Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        List<int> idsGeneros)
    {
        ArgumentNullException.ThrowIfNull(jogo);
        ArgumentNullException.ThrowIfNull(idsGeneros);

        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        idsGeneros = idsGeneros.Distinct().ToList();

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);

            foreach (var idGenero in idsGeneros)
            {
                await _repositorio.JogoGenero.InserirAsync(
                    new JogoGenero
                    {
                        IdJogo = jogo.Id,
                        IdGenero = idGenero
                    }
                );
            }

            transactionScope.Complete();
        }

        return (await MontarDetalhesAsync(new List<Jogo> { jogo })).Single();
    }

    // Altera um jogo e substitui seus vínculos de gêneros.
    public async Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        List<int> idsGeneros)
    {
        ArgumentNullException.ThrowIfNull(jogo);
        ArgumentNullException.ThrowIfNull(idsGeneros);

        var existente = await _repositorio.Jogo
            .BuscarPorIdAsync(jogo.Id);

        if (existente == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO
            );
        }

        jogo.Nome = jogo.Nome.Trim();

        // Preserva os campos que não devem ser redefinidos.
        jogo.DataCadastro = existente.DataCadastro;
        // O administrador pode reativar ou desativar o jogo.

        idsGeneros = idsGeneros.Distinct().ToList();

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);

            await _repositorio.JogoGenero
                .ExcluirPorJogoAsync(jogo.Id);

            foreach (var idGenero in idsGeneros)
            {
                await _repositorio.JogoGenero.InserirAsync(
                    new JogoGenero
                    {
                        IdJogo = jogo.Id,
                        IdGenero = idGenero
                    }
                );
            }

            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    // Desativa o jogo sem removê-lo do banco.
    public async Task DesativarAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null || !jogo.Ativo)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO
            );
        }

        await _repositorio.Jogo.AlterarStatusAsync(id, false);
    }
}
