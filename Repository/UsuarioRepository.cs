using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;
namespace RetroGames.Repository;
public class UsuarioRepository(RepositoryConfig config) : RepositoryBase(config), IUsuarioRepository
{
    private async Task<Usuario?> BuscarAsync(string sql, object valor)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@valor", valor);
        await using var leitor = await comando.ExecuteReaderAsync();
        if (!await leitor.ReadAsync()) return null;
        return new Usuario { Id = leitor.GetInt32("co_seq_usuario"), Nome = leitor.GetString("no_usuario"),
            Email = leitor.GetString("ds_email"), SenhaHash = leitor.GetString("ds_senha_hash"),
            Ativo = leitor.GetBoolean("st_ativo"), DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPerfil = leitor.GetInt32("co_perfil") };
    }
    public Task<Usuario?> BuscarPorEmailAsync(string email) => BuscarAsync("SELECT * FROM tb_usuario WHERE ds_email=@valor", email);
    public Task<Usuario?> BuscarPorIdAsync(int id) => BuscarAsync("SELECT * FROM tb_usuario WHERE co_seq_usuario=@valor", id);
    public async Task<bool> ExisteAdministradorAsync()
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "SELECT COUNT(*) FROM tb_usuario WHERE co_perfil=@perfil");
        AdicionarParametro(comando, "@perfil", PerfilIds.Administrador);
        return Convert.ToInt32(await comando.ExecuteScalarAsync()) > 0;
    }
    public async Task<int> InserirAsync(Usuario usuario)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "INSERT INTO tb_usuario (no_usuario,ds_email,ds_senha_hash,st_ativo,dt_cadastro,co_perfil) VALUES (@nome,@email,@hash,@ativo,@data,@perfil)");
        AdicionarParametro(comando,"@nome",usuario.Nome);
        AdicionarParametro(comando,"@email",usuario.Email);
        AdicionarParametro(comando,"@hash",usuario.SenhaHash);
        AdicionarParametro(comando,"@ativo",usuario.Ativo);
        AdicionarParametro(comando,"@data",usuario.DataCadastro);
        AdicionarParametro(comando,"@perfil",usuario.IdPerfil);
        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }
}
