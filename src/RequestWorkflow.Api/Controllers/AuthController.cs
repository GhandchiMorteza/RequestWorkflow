using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestWorkflow.Api.Contracts.Authentication;
using RequestWorkflow.Application.Authentication;

namespace RequestWorkflow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthController(
        AuthenticationService authenticationService)
    {
        ArgumentNullException.ThrowIfNull(authenticationService);

        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        RegisterRequest request)
    {
        var result = await _authenticationService.RegisterAsync(
            request.Email,
            request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors
            });
        }

        return Ok(result.Response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        LoginRequest request)
    {
        var response = await _authenticationService.LoginAsync(
            request.Email,
            request.Password);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(response);
    }
}
