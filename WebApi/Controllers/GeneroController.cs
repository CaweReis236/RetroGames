using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _service;

    public GeneroController(IGeneroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var generos = await _service.ListarAsync();

        var dados = generos.Select(g => g.ParaResponse()).ToList();

        return Ok(
            RetroGamesResponseApiModel<
                List<ListarGeneroResponseApiModel>
            >.Sucesso(dados)
        );
    }
}
