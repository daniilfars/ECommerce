using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Commands.CreateProduct;
using Catalog.Application.Commands.DeleteProduct;
using Catalog.Application.Commands.UpdateProduct;
using Catalog.Application.Commands.UploadProductImage;
using Catalog.Application.Queries.GetProductById;
using Catalog.Application.Queries.GetProducts;

namespace Catalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
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

    // POST: api/catalog/{id}/upload-image
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/upload-image")]
    public async Task<ActionResult<UploadProductImageResponse>> UploadImage(int id, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Файл не выбран или пустой");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Размер файла не должен превышать 5 МБ");

        await using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new UploadProductImageCommand(id, stream, file.ContentType));

        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }
}