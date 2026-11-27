using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var (jogos, total) = await _service.ListarAsync(
            request.Pagina,
            request.TamanhoPagina,
            request.Busca,
            request.IdPlataforma,
            request.IdGenero
        );

        var dados = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = jogos.Select(j => j.ParaResponse()).ToList(),
            Pagina = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = request.TamanhoPagina > 0
                ? (int)Math.Ceiling((double)total / request.TamanhoPagina)
                : 0
        };

        return Ok(
            RetroGamesResponseApiModel<
                PaginaResponseApiModel<ListarJogoResponseApiModel>
            >.Sucesso(dados)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _service.BuscarPorIdAsync(id);

        return Ok(
            RetroGamesResponseApiModel<
                ListarPorIdJogoResponseApiModel
            >.Sucesso(jogo.ParaDetalheResponse())
        );
    }

    [HttpPost]
    public async Task<IActionResult> Inserir(InserirJogoRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaModel(), request.IdsGeneros);
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id }, RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>.Sucesso(criado.ParaDetalheResponse()));
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(int id, AlterarJogoRequestApiModel request)
    {
        var alterado = await _service.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        return Ok(RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>.Sucesso(alterado.ParaDetalheResponse()));
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.DesativarAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }

}
