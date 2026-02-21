using Microsoft.Extensions.DependencyInjection;
using TripMate.Data;
using TripMate.Views;

namespace TripMate;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation (not in visual hierarchy)
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute("TripList", typeof(TripListPage));
        Routing.RegisterRoute(nameof(CreateTripPage), typeof(CreateTripPage));
        Routing.RegisterRoute("TripDetail", typeof(TripDetailPage));
        Routing.RegisterRoute(nameof(EditTripPage), typeof(EditTripPage));
        Routing.RegisterRoute(nameof(AddExpensePage), typeof(AddExpensePage));
        Routing.RegisterRoute("ExpenseList", typeof(ExpenseListPage));
        Routing.RegisterRoute("Settlement", typeof(SettlementPage));
        Routing.RegisterRoute("TripMembers", typeof(TripMembersPage));

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnLoaded;
        await SeedDatabaseAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        if (Helpers.DatabaseConfiguration.UseApiBackend) return; // No local DB when using API
        try
        {
            var sp = Handler?.MauiContext?.Services;
            if (sp == null) return;
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetService<Data.AppDbContext>();
            if (db != null)
                await Data.DataSeeder.SeedAsync(db);
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Database seed error: {ex}");
#endif
        }
    }
}
