using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;
namespace RetroGames.WebApi.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PedidoController(IPedidoService service, IUsuarioAtualService usuario) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Finalizar(PedidoRequestApiModel request)
    {
        var pedido=await service.FinalizarAsync(usuario.IdUsuario!.Value,request.Itens.Select(i=>new CompraItemServiceModel(i.IdJogo,i.Quantidade)).ToList());
        return StatusCode(201,RetroGamesResponseApiModel<PedidoResponseApiModel>.Sucesso(Converter(pedido)));
    }
    [HttpGet("meus")]
    public async Task<IActionResult> Meus()
    {
        var pedidos=await service.ListarMeusAsync(usuario.IdUsuario!.Value);
        return Ok(RetroGamesResponseApiModel<List<PedidoResponseApiModel>>.Sucesso(pedidos.Select(Converter).ToList()));
    }
    private static PedidoResponseApiModel Converter(PedidoServiceModel pedido) => new(pedido.Id,pedido.DataPedido,pedido.Total,pedido.Situacao,pedido.Itens.Select(i=>new ItemPedidoResponseApiModel(i.IdJogo,i.NomeJogo,i.Quantidade,i.ValorUnitario)).ToList());
}
