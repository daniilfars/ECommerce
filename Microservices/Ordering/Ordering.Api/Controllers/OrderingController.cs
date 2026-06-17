using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands.CancelOrder;
using Ordering.Application.Commands.CreateOrder;
using Ordering.Application.Commands.DeliverOrder;
using Ordering.Application.Commands.PayOrder;
using Ordering.Application.Commands.ShipOrder;
using Ordering.Application.Queries.GetOrderById;
using Ordering.Application.Queries.GetOrders;
using System.Security.Claims;

namespace Ordering.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderingController : ControllerBase
{
    private readonly IMediator _mediator;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public OrderingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: /api/ordering/create
    [HttpPost("create")]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }

    // GET: /api/ordering?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<GetOrdersResponse>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetOrdersQuery(UserId, page, pageSize));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetOrdersResponse>> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetOrdersQuery(UserId, page, pageSize, All: true));
        if (result.IsFailure)
            return BadRequest(result.Error!);
        return Ok(result.Value);
    }

    // GET: api/ordering/{orderId}
    [HttpGet("{orderId}")]
    public async Task<ActionResult<GetOrderByIdResponse>> GetOrderById(int orderId)
    {
        var isAdmin = User.IsInRole("Admin");
        var result = await _mediator.Send(new GetOrderByIdQuery(orderId, isAdmin ? Guid.Empty : UserId, isAdmin));

        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value!);
    }

    // POST: /api/ordering/{orderId}/cancel
    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var isAdmin = User.IsInRole("Admin");
        var result = await _mediator.Send(new CancelOrderCommand(orderId, isAdmin ? Guid.Empty : UserId, isAdmin));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return NoContent();
    }

    // POST: /api/ordering/{orderId}/pay
    [HttpPost("{orderId}/pay")]
    public async Task<IActionResult> PayOrder(int orderId)
    {
        var isAdmin = User.IsInRole("Admin");
        var result = await _mediator.Send(new PayOrderCommand(orderId, isAdmin ? Guid.Empty : UserId, isAdmin));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return NoContent();
    }

    // POST: /api/ordering/{orderId}/ship
    [HttpPost("{orderId}/ship")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ShipOrder(int orderId)
    {
        var result = await _mediator.Send(new ShipOrderCommand(orderId));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return NoContent();
    }

    // POST: /api/ordering/{orderId}/deliver
    [HttpPost("{orderId}/deliver")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeliverOrder(int orderId)
    {
        var result = await _mediator.Send(new DeliverOrderCommand(orderId));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return NoContent();
    }
}