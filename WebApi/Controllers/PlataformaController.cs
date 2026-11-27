using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;
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

    [AllowAnonymous]
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

    [AllowAnonymous]
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

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<IActionResult> Inserir(InserirPlataformaRequestApiModel request)
    {
        var criada = await _service.InserirAsync(request.ParaModel());
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id }, RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>.Sucesso(criada.ParaResponse()));
    }
    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(int id, AlterarPlataformaRequestApiModel request)
    {
        var alterada = await _service.AlterarAsync(request.ParaModel(id));
        return Ok(RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>.Sucesso(alterada.ParaResponse()));
    }
    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }

}
