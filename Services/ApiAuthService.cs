using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Auth service that uses the API backend.
/// </summary>
public class ApiAuthService : IAuthService
{
    private readonly ITripMateApiClient _api;
    private User? _currentUser;

    public ApiAuthService(ITripMateApiClient api) => _api = api;
    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public async Task<(bool Success, string Message)> RegisterAsync(string name, string email, string password, CancellationToken ct = default)
    {
        var (success, msg) = await _api.RegisterAsync(name, email, password, ct);
        if (success)
        {
            var (user, _) = await _api.LoginAsync(email, password, ct);
            _currentUser = user;
        }
        return (success, msg);
    }

    public async Task<(User? User, string Message)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var (user, msg) = await _api.LoginAsync(email, password, ct);
        if (user != null) _currentUser = user;
        return (user, msg);
    }

    public void Logout() => _currentUser = null;
}
