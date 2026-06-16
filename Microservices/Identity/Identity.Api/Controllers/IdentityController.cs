using MediatR;
using Microsoft.AspNetCore.Mvc;
using Identity.Application.Commands.Login;
using Identity.Application.Commands.Logout;
using Identity.Application.Commands.Refresh;
using Identity.Application.Commands.Register;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly IMediator mediator;

    public IdentityController(IMediator _mediator)
    {
        mediator = _mediator;
    }

    // POST: api/identity/register
    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponse>> Register(RegisterUserCommand register)
    {
        var res = await mediator.Send(register);

        if (res.IsFailure)
            return BadRequest(res.Error);

        return CreatedAtAction(nameof(Login), null, res.Value);
    }

    // POST: api/identity/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginUserResponse>> Login(LoginUserCommand login)
    {
        var res = await mediator.Send(login);

        if (res.IsFailure)
            return Unauthorized(res.Error);

        return Ok(res.Value);
    }

    // POST: api/identity/refresh
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshUserResponse>> Refresh()
    {
        var res = await mediator.Send(new RefreshUserCommand());

        if (res.IsFailure)
            return Unauthorized(res.Error);

        return Ok(res.Value);
    }

    // POST: api/identity/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutUserCommand logout)
    {
        var res = await mediator.Send(logout);

        if (res.IsFailure)
            return BadRequest(res.Error);

        return Ok();
    }
}