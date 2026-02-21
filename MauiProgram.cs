using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripMate.Data;
using TripMate.Helpers;
using TripMate.Services;
using TripMate.ViewModels;
using TripMate.Views;

namespace TripMate;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Database - SQL Server
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = Helpers.DatabaseConfiguration.GetConnectionString();
            options.UseSqlServer(connectionString);
        });

        // Repositories
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<ITripRepository, TripRepository>();
        builder.Services.AddTransient<IExpenseRepository, ExpenseRepository>();

        // Services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddTransient<ITripService, TripService>();
        builder.Services.AddTransient<IExpenseService, ExpenseService>();
        builder.Services.AddTransient<IExpenseCalculationService, ExpenseCalculationService>();
        builder.Services.AddTransient<ISettlementService, SettlementService>();
        builder.Services.AddTransient<IDashboardService, DashboardService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<TripListViewModel>();
        builder.Services.AddTransient<CreateTripViewModel>();
        builder.Services.AddTransient<TripDetailViewModel>();
        builder.Services.AddTransient<EditTripViewModel>();
        builder.Services.AddTransient<AddExpenseViewModel>();
        builder.Services.AddTransient<ExpenseListViewModel>();
        builder.Services.AddTransient<SettlementViewModel>();
        builder.Services.AddTransient<TripMembersViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<TripListPage>();
        builder.Services.AddTransient<CreateTripPage>();
        builder.Services.AddTransient<TripDetailPage>();
        builder.Services.AddTransient<EditTripPage>();
        builder.Services.AddTransient<AddExpensePage>();
        builder.Services.AddTransient<ExpenseListPage>();
        builder.Services.AddTransient<SettlementPage>();
        builder.Services.AddTransient<TripMembersPage>();

        var app = builder.Build();
        ServiceHelper.Services = app.Services;
        return app;
    }
}
