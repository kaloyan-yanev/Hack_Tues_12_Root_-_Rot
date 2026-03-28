using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RootAndRot.Server.Controllers;
using RootAndRot.Server.Data;
using RootAndRot.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace RootAndRot.Server.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _dbContext;

    public AuthenticationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ── Register ─────────────────────────────────────────────
    public async Task<bool> Register(string name, string password)
    {
        var exists = await _dbContext.Users.AnyAsync(u => u.Name == name);
        if (exists)
            return false;

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = name,
            Password = HashPassword(password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    // ── Login ────────────────────────────────────────────────
    public async Task<User?> LogIn(string name, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == name);
        if (user == null)
            return null;

        if (user.Password != HashPassword(password))
            return null;

        return user;
    }

    // ── Logout ───────────────────────────────────────────────
    public async Task<bool> LogOut(Guid userId)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == userId);
        // For JWT, logout is mostly client-side; could remove tokens if stored in DB
        return userExists;
    }

    // ── Issue JWT + Refresh Token ───────────────────────────
    public async Task<(string accessToken, string refreshToken)> IssueTokenPair(
        User user, string key, string issuer, string audience)
    {
        var accessToken = AuthenticationController.CreateAccessToken(user.Name, key, issuer, audience);
        var refreshToken = CreateRefreshToken();

        // Save refresh token in DB
        var refreshTokenEntity = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Consumed = false
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    // ── Get Refresh Token ───────────────────────────────────
    public async Task<RefreshToken?> GetRefreshToken(string token)
    {
        return await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    // ── Save / update Refresh Token ─────────────────────────
    public async Task SaveRefreshToken(RefreshToken token)
    {
        _dbContext.RefreshTokens.Update(token);
        await _dbContext.SaveChangesAsync();
    }

    // ── Helpers ────────────────────────────────────────────
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static string CreateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}