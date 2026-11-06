using MySqlConnector;
using RetroGames.Repository.Config;
using RetroGames.Repository.Exceptions;

namespace RetroGames.Repository.Base;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    // Cria e abre uma conexão com o banco.
    protected async Task<MySqlConnection> CriarConexaoAbertaAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    // Cria um comando SQL associado à conexão.
    protected MySqlCommand CriarComando(
        MySqlConnection conexao,
        string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    // Adiciona um parâmetro ao comando.
    protected void AdicionarParametro(
        MySqlCommand comando,
        string nome,
        object? valor)
    {
        if (!nome.StartsWith("@"))
        {
            nome = "@" + nome;
        }

        comando.Parameters.AddWithValue(
            nome,
            valor ?? DBNull.Value
        );
    }

    // Executa INSERT ou UPDATE.
    // Converte o erro 1062 em uma exceção própria.
    protected async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException erro) when (erro.Number == 1062)
        {
            throw new RegistroDuplicadoException(
                "Já existe um registro com esse valor."
            );
        }
    }
}
