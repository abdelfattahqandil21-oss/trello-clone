using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrelloClone.DTOs;
using TrelloClone.Services.IServices;

namespace TrelloClone.Areas.Identity.Controllers;

[Area("Identity")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = "/api/auth"
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private static AuthResponse StripRefreshToken(AuthResponse response)
    {
        response.RefreshToken = null;
        response.RefreshTokenExpiresAt = null;
        return response;
    }

    [HttpPost("api/auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        var result = await _authService.RegisterAsync(request);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("api/auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        var result = await _authService.LoginAsync(request);
        if (!result.Success) return Unauthorized(new { message = result.Message });

        if (result.RefreshToken is not null && result.RefreshTokenExpiresAt is not null)
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt.Value);

        return Ok(StripRefreshToken(result));
    }

    [HttpPost("api/auth/logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _authService.LogoutAsync(userId);
        Response.Cookies.Delete("refreshToken");
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("api/auth/refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var result = await _authService.RefreshTokenAsync(refreshToken);
        if (!result.Success) return Unauthorized(new { message = result.Message });

        if (result.RefreshToken is not null && result.RefreshTokenExpiresAt is not null)
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt.Value);

        return Ok(StripRefreshToken(result));
    }

    [HttpPost("api/auth/forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        var result = await _authService.ForgotPasswordAsync(request);
        return Ok(result);
    }

    [HttpPost("api/auth/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        var result = await _authService.ResetPasswordAsync(request);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("api/auth/verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        var result = await _authService.VerifyEmailAsync(email, token);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("api/auth/resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        var result = await _authService.ResendVerificationAsync(request);
        return Ok(result);
    }
}
