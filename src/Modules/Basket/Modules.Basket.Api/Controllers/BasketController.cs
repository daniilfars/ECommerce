using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Basket.Api.Models;
using Modules.Basket.Application.Commands.AddItemToBasket;
using Modules.Basket.Application.Commands.CheckoutBasket;
using Modules.Basket.Application.Commands.RemoveItemFromBasket;
using Modules.Basket.Application.Commands.UpdateBasketItemQuantity;
using Modules.Basket.Application.Queries.GetBasket;
using System.Security.Claims;

namespace Modules.Basket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IMediator _mediator;
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public BasketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/basket
    [HttpGet]
    public async Task<ActionResult<GetBasketResponse>> GetBasket()
    {
        var result = await _mediator.Send(new GetBasketQuery(UserId));
        if(result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    // POST: api/basket
    [HttpPost]
    public async Task<ActionResult<GetBasketResponse>> AddItemToBasket(AddItemToBasketRequest request)
    {
        var result = await _mediator.Send(new AddItemToBasketCommand(UserId, request.ProductId, request.Quantity));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }

    // DELETE: api/basket/{productId}
    [HttpDelete("{productId}")]
    public async Task<ActionResult<GetBasketResponse>> RemoveItemFromBasket(int productId)
    {
        var result = await _mediator.Send(new RemoveItemFromBasketCommand(UserId, productId));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }

    // PATCH: api/basket/{productId}
    [HttpPatch("{productId}")]
    public async Task<ActionResult<GetBasketResponse>> UpdateQuantity(int productId, UpdateQuantityRequest request)
    {
        var result = await _mediator.Send(new UpdateBasketItemQuantityCommand(UserId, productId, request.Quantity));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }

    // DELETE : api/basket/checkout
    [HttpPost("checkout")]
    public async Task<IActionResult> CheckoutBasket(CheckoutRequest request)
    {
        var result = await _mediator.Send(new CheckoutBasketCommand(UserId, request.ShippingAddress));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return NoContent();
    }
}