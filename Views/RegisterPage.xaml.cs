using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage() : this(ServiceHelper.GetRequiredService<RegisterViewModel>()) { }

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
