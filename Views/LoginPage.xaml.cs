using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage() : this(ServiceHelper.GetRequiredService<LoginViewModel>()) { }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
