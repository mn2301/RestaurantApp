using RestaurantApp.Controller;
using RestaurantApp.GoogleAuth;
using RestaurantApp.Platforms.Android;

namespace RestaurantApp;

public partial class FlyoutMenuPage : ContentPage
{
    private readonly IGoogleAuthService _googleAuthService = new GoogleAuthService();
    public FlyoutMenuPage()
	{
		InitializeComponent();
	}

    private void btnLogout_Clicked(object sender, EventArgs e)
    {
        logout();
    }

    private async void logout()
    {
        await _googleAuthService?.LogoutAsync();
        AppSession.cartItems.Clear();
        LoginPage mainPage = new LoginPage();
        Application.Current.MainPage = mainPage;
    }
}