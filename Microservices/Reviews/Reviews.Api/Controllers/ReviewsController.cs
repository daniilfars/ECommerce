using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reviews.Application.Commands.CreateReview;
using Reviews.Application.Commands.DeleteReview;
using Reviews.Application.Commands.UpdateReview;
using Reviews.Application.DTOs;
using Reviews.Application.Queries.GetReviews;
using Reviews.Application.Queries.GetReviewsByUserId;
using System.Security.Claims;

namespace Reviews.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/reviews/create
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<CreateReviewResponse>> Create(CreateReviewDto command)
    {
        var result = await _mediator.Send(new CreateReviewCommand(UserId, command.ProductId, command.Text, command.Stars));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // GET: api/reviews/getbyproductid
    [HttpGet("getbyproductid")]
    public async Task<ActionResult<GetReviewsByProductIdResponse>> GetByProductId([FromQuery] GetReviewsByProductIdQuery query)
    {
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // GET: api/reviews?page=1&pageSize=10
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<GetReviewsByUserIdResponse>> GetByUserId(int Page = 1, int PageSize = 10)
    {
        var result = await _mediator.Send(new GetReviewsByUserIdQuery(UserId, Page, PageSize));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // PUT: api/reviews
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<UpdateReviewResponse>> Update(UpdateReviewDto command)
    {
        var result = await _mediator.Send(new UpdateReviewCommand(UserId, command.Id ,command.Text, command.Stars));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // DELETE: api/reviews/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteReviewCommand(UserId, id));

        if (result.IsFailure)
            return NotFound(result.Error);

        return NoContent();
    }
}