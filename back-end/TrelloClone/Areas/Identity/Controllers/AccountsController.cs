using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Utilities;

namespace TrelloClone.Areas.Identity.Controllers;

[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly ICardService _cardService;

    public AccountsController(
        IAuthService authService,
        IUserService userService,
        INotificationService notificationService,
        ICardService cardService)
    {
        _authService = authService;
        _userService = userService;
        _notificationService = notificationService;
        _cardService = cardService;
    }

    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

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
        await _authService.LogoutAsync(UserId);
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

    [HttpGet("api/users/me")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var profile = await _userService.GetProfileAsync(UserId);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("api/users/me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        try
        {
            var profile = await _userService.UpdateProfileAsync(UserId, request);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("api/users/me/avatar")]
    [Authorize]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarRequest request)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        try
        {
            var profile = await _userService.UpdateAvatarAsync(UserId, request);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("api/users/me")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        try
        {
            await _userService.DeleteAccountAsync(UserId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("api/users/me/notifications")]
    [Authorize]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _notificationService.GetByUserIdAsync(UserId);
        return Ok(notifications);
    }

    [HttpPatch("api/users/me/notifications/{id:int}/read")]
    [Authorize]
    public async Task<IActionResult> MarkNotificationRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpPatch("api/users/me/notifications/read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllNotificationsRead()
    {
        await _notificationService.MarkAllAsReadAsync(UserId);
        return NoContent();
    }

    [HttpGet("api/users/me/cards")]
    [Authorize]
    public async Task<IActionResult> GetMyCards()
    {
        var cards = await _cardService.GetByMemberIdAsync(UserId);
        return Ok(cards);
    }

    [HttpGet("api/users/search")]
    [Authorize]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return UnprocessableEntity(new { message = "Query parameter 'q' is required" });

        var results = await _userService.SearchAsync(q, UserId);
        return Ok(results);
    }
}
