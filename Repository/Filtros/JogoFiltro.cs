namespace RetroGames.Repository.Filtros;

public class JogoFiltro
{
    public string? Nome { get; set; }

    public int? IdPlataforma { get; set; }

    public List<int>? IdsJogos { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanhoPagina { get; set; } = 10;

    public bool IncluirInativos { get; set; } = false;
}
