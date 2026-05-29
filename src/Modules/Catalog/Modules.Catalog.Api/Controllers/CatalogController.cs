using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Catalog.Application.Commands.CreateProduct;
using Modules.Catalog.Application.Commands.DeleteProduct;
using Modules.Catalog.Application.Commands.UpdateProduct;
using Modules.Catalog.Application.Queries.GetProductById;
using Modules.Catalog.Application.Queries.GetProducts;

namespace Modules.Catalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("debug")]
    public IActionResult Debug()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var isAuthenticated = User.Identity?.IsAuthenticated;
        var identityType = User.Identity?.GetType().Name;

        return Ok(new
        {
            authHeaderPresent = !string.IsNullOrEmpty(authHeader),
            authHeaderPreview = authHeader.Length > 20 ? authHeader[..20] + "..." : authHeader,
            isAuthenticated,
            identityType,
            claimsCount = claims.Count,
            claims
        });
    }

    // POST: api/catalog
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> Create(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    // GET: api/catalog
    [HttpGet]
    public async Task<ActionResult<GetProductsResponse>> GetAll([FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // GET: api/catalog/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductByIdResponse>> GetById(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    // PUT: api/catalog/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateProductResponse>> Update(int id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id в URL и теле запроса не совпадают");

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // DELETE: api/catalog/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return NotFound(result.Error);

        return NoContent();
    }
}