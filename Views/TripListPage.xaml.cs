using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class TripListPage : ContentPage
{
    public TripListPage() : this(ServiceHelper.GetRequiredService<TripListViewModel>()) { }

    public TripListPage(TripListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TripListViewModel vm)
            await vm.LoadTripsCommand.ExecuteAsync(null);
    }
}
