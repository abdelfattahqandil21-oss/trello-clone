using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrelloClone.DTOs;
using TrelloClone.Utilities;

namespace TrelloClone.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDpContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IEmailSender _emailSender;

    public AuthService(
        UserManager<AppUser> userManager,
        AppDpContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            return new AuthResponse { Success = false, Message = "Email is already registered." };

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername != null)
            return new AuthResponse { Success = false, Message = "Username is already taken." };

        var now = DateTime.UtcNow;

        var user = new AppUser
        {
            UserName = request.Username,
            Email = request.Email,
            DisplayName = request.Username,
            IsEmailVerified = false,
            IsActive = true,
            AvatarUrl = string.Empty,
            CreatedAt = now,
            UpdatedAt = now
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return new AuthResponse
            {
                Success = false,
                Message = "Registration failed.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };

        await _userManager.AddToRoleAsync(user, SD.CUSTOMER_ROLE);

        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:4200";
        var verifyLink = $"{frontendUrl}/verify-email?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(emailToken)}";

        await _emailSender.SendEmailAsync(
            request.Email,
            "Verify your email - TrelloClone",
            $"<h2>Welcome to TrelloClone!</h2><p>Please verify your email by clicking <a href='{verifyLink}'>here</a>.</p>"
        );

        _logger.LogInformation("User {Email} registered. Verification email sent.", request.Email);

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful. Please verify your email.",
            UserId = user.Id,
            Email = user.Email,
            Username = user.UserName
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return new AuthResponse { Success = false, Message = "Invalid email or password." };

        if (!user.IsActive)
            return new AuthResponse { Success = false, Message = "Account is deactivated." };

        if (!user.IsEmailVerified)
            return new AuthResponse { Success = false, Message = "Email not verified. Please verify your email first." };

        var (accessToken, expiresAt) = await GenerateAccessTokenAsync(user);
        var (refreshToken, refreshExpiresAt) = await GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful.",
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            UserId = user.Id,
            Email = user.Email,
            Username = user.UserName
        };
    }

    public async Task<AuthResponse> LogoutAsync(string userId)
    {
        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsUsed && !rt.IsRevoked)
            .ToListAsync();

        foreach (var rt in refreshTokens)
        {
            rt.IsRevoked = true;
        }

        await _context.SaveChangesAsync();

        return new AuthResponse { Success = true, Message = "Logged out successfully." };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return new AuthResponse { Success = false, Message = "Refresh token is required." };

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
            return new AuthResponse { Success = false, Message = "Invalid refresh token." };

        if (storedToken.IsUsed)
            return new AuthResponse { Success = false, Message = "Refresh token has been used." };

        if (storedToken.IsRevoked)
            return new AuthResponse { Success = false, Message = "Refresh token has been revoked." };

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            return new AuthResponse { Success = false, Message = "Refresh token has expired." };

        storedToken.IsUsed = true;
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();

        var (accessToken, expiresAt) = await GenerateAccessTokenAsync(storedToken.User);
        var (newRefreshToken, refreshExpiresAt) = await GenerateRefreshTokenAsync(storedToken.User);

        return new AuthResponse
        {
            Success = true,
            Message = "Token refreshed successfully.",
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            UserId = storedToken.User.Id,
            Email = storedToken.User.Email,
            Username = storedToken.User.UserName
        };
    }

    public async Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return new AuthResponse { Success = true, Message = "If the email exists, a reset link has been sent." };

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        _logger.LogInformation("Password reset requested for {Email}. Token: {Token}", request.Email, resetToken);

        return new AuthResponse
        {
            Success = true,
            Message = "If the email exists, a reset link has been sent."
        };
    }

    public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return new AuthResponse { Success = false, Message = "Invalid email or token." };

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
            return new AuthResponse
            {
                Success = false,
                Message = "Password reset failed.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };

        return new AuthResponse { Success = true, Message = "Password has been reset successfully." };
    }

    public async Task<AuthResponse> VerifyEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new AuthResponse { Success = false, Message = "Invalid verification request." };

        if (user.IsEmailVerified)
            return new AuthResponse { Success = false, Message = "Email is already verified." };

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return new AuthResponse { Success = false, Message = "Email verification failed. Token may be invalid or expired." };

        user.IsEmailVerified = true;
        await _userManager.UpdateAsync(user);

        return new AuthResponse { Success = true, Message = "Email verified successfully." };
    }

    public async Task<AuthResponse> ResendVerificationAsync(ResendVerificationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.IsEmailVerified)
            return new AuthResponse { Success = true, Message = "If the email exists and is not verified, a new verification link has been sent." };

        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:4200";
        var verifyLink = $"{frontendUrl}/verify-email?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(emailToken)}";

        await _emailSender.SendEmailAsync(
            request.Email,
            "Verify your email - TrelloClone",
            $"<h2>Verify your email</h2><p>Click <a href='{verifyLink}'>here</a> to verify your email.</p>"
        );

        _logger.LogInformation("Verification email resent to {Email}.", request.Email);

        return new AuthResponse
        {
            Success = true,
            Message = "If the email exists and is not verified, a new verification link has been sent."
        };
    }

    private async Task<(string token, DateTime expiresAt)> GenerateAccessTokenAsync(AppUser user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var jwtKey = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("isActive", user.IsActive.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = jwtSection["Issuer"],
            Audience = jwtSection["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expiresAt);
    }

    private async Task<(string token, DateTime expiresAt)> GenerateRefreshTokenAsync(AppUser user)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = token,
            JwtId = Guid.NewGuid().ToString(),
            IsUsed = false,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return (token, expiresAt);
    }
}
