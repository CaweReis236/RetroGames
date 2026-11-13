using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _service;

    public CondicaoJogoController(ICondicaoJogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _service.ListarAsync();

        var dados = condicoes
            .Select(c => c.ParaResponse())
            .ToList();

        return Ok(
            RetroGamesResponseApiModel<
                List<ListarCondicaoJogoResponseApiModel>
            >.Sucesso(dados)
        );
    }
}
