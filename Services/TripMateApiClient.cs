using System.Net.Http.Json;
using System.Text.Json;
using TripMate.Models;

namespace TripMate.Services;

public class TripMateApiClient : ITripMateApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TripMateApiClient(HttpClient http) => _http = http;

    public async Task<(User? User, string Message)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("api/users/login", new { email, password }, ct);
            if (!res.IsSuccessStatusCode) return (null, "Invalid email or password.");
            var user = await res.Content.ReadFromJsonAsync<User>(JsonOptions, ct);
            return (user, "Login successful!");
        }
        catch (Exception ex) { return (null, ex.Message); }
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string name, string email, string password, CancellationToken ct = default)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("api/users/register", new { name, email, password }, ct);
            var msg = await res.Content.ReadAsStringAsync(ct);
            if (!res.IsSuccessStatusCode) return (false, msg.Contains("already") ? "Email already exists." : msg);
            return (true, "Registration successful!");
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    public async Task<IReadOnlyList<Trip>> GetMyTripsAsync(int userId, CancellationToken ct = default)
    {
        var list = await _http.GetFromJsonAsync<List<Trip>>($"api/trips/user/{userId}", ct) ?? new List<Trip>();
        return list;
    }

    public async Task<Trip?> GetTripByIdAsync(int id, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<Trip>($"api/trips/{id}", ct);

    public async Task<Trip> CreateTripAsync(string name, string destination, DateTime startDate, DateTime endDate, int userId, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/trips", new { name, destination, startDate, endDate, userId }, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<Trip>(JsonOptions, ct))!;
    }

    public async Task UpdateTripAsync(int id, string name, string destination, DateTime startDate, DateTime endDate, TripStatus status, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/trips/{id}", new { name, destination, startDate, endDate, status = (int)status }, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteTripAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/trips/{id}", ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<User>> GetTripMembersAsync(int tripId, CancellationToken ct = default)
    {
        var list = await _http.GetFromJsonAsync<List<User>>($"api/trips/{tripId}/members", ct) ?? new List<User>();
        return list;
    }

    public async Task AddTripMemberAsync(int tripId, int userId, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/trips/members", new { tripId, userId }, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task RemoveTripMemberAsync(int tripId, int userId, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/trips/{tripId}/members/{userId}", ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<Expense>> GetExpensesByTripAsync(int tripId, int? paidByUserId, CancellationToken ct = default)
    {
        var url = paidByUserId.HasValue ? $"api/expenses/trip/{tripId}?paidByUserId={paidByUserId}" : $"api/expenses/trip/{tripId}";
        var list = await _http.GetFromJsonAsync<List<Expense>>(url, ct) ?? new List<Expense>();
        return list;
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<Expense>($"api/expenses/{id}", ct);

    public async Task<Expense> CreateExpenseAsync(int tripId, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default)
    {
        var payload = new { tripId, title, amount, paidByUserId, splitType = (int)splitType, note, customSplits = customSplits != null ? (object)customSplits : null };
        var res = await _http.PostAsJsonAsync("api/expenses", payload, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<Expense>(JsonOptions, ct))!;
    }

    public async Task UpdateExpenseAsync(int id, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default)
    {
        var payload = new { title, amount, paidByUserId, splitType = (int)splitType, note, customSplits = customSplits != null ? (object)customSplits : null };
        var res = await _http.PutAsJsonAsync($"api/expenses/{id}", payload, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteExpenseAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/expenses/{id}", ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<SettlementItem>> GetSettlementsAsync(int tripId, CancellationToken ct = default)
    {
        var list = await _http.GetFromJsonAsync<List<SettlementDto>>($"api/settlement/trip/{tripId}", ct) ?? new List<SettlementDto>();
        return list.Select(d => new SettlementItem(d.PayerName, d.ReceiverName, d.Amount)).ToList();
    }

    private record SettlementDto(string PayerName, string ReceiverName, decimal Amount);
}
