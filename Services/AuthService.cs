using TripMate.Data;
using TripMate.Helpers;
using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Authentication service implementation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string name, string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, "Name is required.");
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email is required.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return (false, "Password must be at least 6 characters.");

        var existing = await _userRepository.GetByEmailAsync(email, ct);
        if (existing != null)
            return (false, "An account with this email already exists.");

        var user = new User
        {
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = PasswordHelper.HashPassword(password)
        };

        await _userRepository.AddAsync(user, ct);
        _currentUser = user;
        return (true, "Registration successful!");
    }

    public async Task<(User? User, string Message)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (null, "Email and password are required.");

        var user = await _userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant(), ct);
        if (user == null)
            return (null, "Invalid email or password.");

        if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
            return (null, "Invalid email or password.");

        _currentUser = user;
        return (user, "Login successful!");
    }

    public void Logout()
    {
        _currentUser = null;
    }
}
