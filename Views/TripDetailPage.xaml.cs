using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class TripDetailPage : ContentPage, IQueryAttributable
{
    public TripDetailPage() : this(ServiceHelper.GetRequiredService<TripDetailViewModel>()) { }

    public TripDetailPage(TripDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is TripDetailViewModel vm && query.TryGetValue("tripId", out var v))
        {
            var s = v?.ToString();
            if (int.TryParse(s, out var id))
                vm.TripId = id;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TripDetailViewModel vm && vm.TripId > 0)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
