using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Authentication service for local user management.
/// </summary>
public interface IAuthService
{
    Task<(bool Success, string Message)> RegisterAsync(string name, string email, string password, CancellationToken ct = default);
    Task<(User? User, string Message)> LoginAsync(string email, string password, CancellationToken ct = default);
    void Logout();
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
}
