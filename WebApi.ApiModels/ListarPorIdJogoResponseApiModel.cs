namespace RetroGames.WebApi.ApiModels;

public class GeneroJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ListarPorIdJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Ano { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<GeneroJogoResponseApiModel> Generos { get; set; } = new();
}
