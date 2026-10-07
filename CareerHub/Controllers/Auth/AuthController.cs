using Application.DTOs.Requests;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.VerifyEmail;
using Application.Features.Auth.Commands.Login;
using MediatR;
using Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] Application.Features.Auth.Commands.ForgotPassword.ForgotPasswordCommand request)
    {
        var result = await _mediator.Send(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] Application.Features.Auth.Commands.ResetPassword.ResetPasswordRequest request)
    {
        var command = new Application.Features.Auth.Commands.ResetPassword.ResetPasswordCommand(request.Token, request.NewPassword);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var result = await _mediator.Send(
            new LoginCommand(request));

        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromQuery] string token)
    {
        var result = await _mediator.Send(
            new VerifyEmailCommand(token));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(Result.Failure("Invalid user"));

        var result = await _mediator.Send(new Application.Features.Auth.Commands.Logout.LogoutCommand(userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}