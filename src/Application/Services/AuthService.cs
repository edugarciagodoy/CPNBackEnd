using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null) return false;

        return VerifyPassword(password, user.PasswordHash);
    }

    public async Task<bool> RegisterUserAsync(string username, string password)
    {
        if (await _userRepository.GetUserByUsernameAsync(username) != null)
            return false;

        var passwordHash = HashPassword(password);
        var user = new User { Username = username, PasswordHash = passwordHash };

        await _userRepository.AddUserAsync(user);
        return true;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
