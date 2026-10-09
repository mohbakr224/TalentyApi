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

        return Ok(new { token = result.value });
    }

    [AllowAnonymous]
    [HttpGet("reset-password")]
    public ContentResult ResetPasswordPage()
    {
        const string html = """
            <!doctype html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>Reset password</title>
              <style>
                body { font: 16px system-ui, sans-serif; max-width: 26rem; margin: 12vh auto; padding: 0 1rem; color: #202124; }
                form { display: grid; gap: 1rem; }
                input, button { box-sizing: border-box; width: 100%; padding: .75rem; font: inherit; }
                button { cursor: pointer; }
                #message { min-height: 1.5rem; }
              </style>
            </head>
            <body>
              <h1>Reset your password</h1>
              <form id="reset-form">
                <label for="password">New password</label>
                <input id="password" type="password" autocomplete="new-password" required minlength="8">
                <button type="submit">Update password</button>
              </form>
              <p id="message" role="status"></p>
              <script>
                const form = document.getElementById('reset-form');
                const message = document.getElementById('message');
                form.addEventListener('submit', async (event) => {
                  event.preventDefault();
                  message.textContent = 'Updating password…';
                  try {
                    const response = await fetch('/api/v1/auth/reset-password', {
                      method: 'POST',
                      headers: { 'Content-Type': 'application/json' },
                      body: JSON.stringify({ token: new URLSearchParams(location.search).get('token'), newPassword: document.getElementById('password').value })
                    });
                    const result = await response.json();
                    if (!response.ok) throw new Error(result.message || result.error || result.Error || 'This reset link is invalid or expired.');
                    form.hidden = true;
                    message.textContent = 'Your password has been updated. You can now sign in.';
                  } catch (error) {
                    message.textContent = error.message || 'Unable to reset the password. Please try again.';
                  }
                });
              </script>
            </body>
            </html>
            """;

        return Content(html, "text/html");
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
