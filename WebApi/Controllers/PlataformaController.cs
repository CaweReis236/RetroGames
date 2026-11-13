using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _service;

    public PlataformaController(IPlataformaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _service.ListarAsync();

        var dados = plataformas
            .Select(p => p.ParaResponse())
            .ToList();

        return Ok(
            RetroGamesResponseApiModel<
                List<ListarPlataformaResponseApiModel>
            >.Sucesso(dados)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _service.BuscarPorIdAsync(id);

        return Ok(
            RetroGamesResponseApiModel<
                ListarPlataformaResponseApiModel
            >.Sucesso(plataforma.ParaResponse())
        );
    }
}
