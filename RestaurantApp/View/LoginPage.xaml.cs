using Android.Gms.Common.Apis;
using RestaurantApp.GoogleAuth;
using RestaurantApp.Platforms.Android;
using System.Reflection.Metadata.Ecma335;

namespace RestaurantApp;

public partial class LoginPage : ContentPage
{
	private readonly IGoogleAuthService _googleAuthService = new GoogleAuthService();
	ConnectSupabase _connectSupabase = new ConnectSupabase();

    public LoginPage()
	{
		InitializeComponent();
    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
		var loggedUser = await _googleAuthService.GetCurrentUserAsync();

        if (loggedUser == null)
		{
			loggedUser = await _googleAuthService.AuthenticateAsync();
        }

        UserData user = new UserData()
        {
            clientGoogleToken = loggedUser.TokenId,
            clientName = loggedUser.FullName,
            clientEmail = loggedUser.Email,
            clientImageURL = loggedUser.ImageURL,
            clientType = "client"
        };

        var dbUser = await _connectSupabase.UserLogin(user);

        if(dbUser!= null)
        {
            AppSession.CurrentUser = dbUser;

            // Open the new pag with the user info
            MainPage mainPage = new MainPage(dbUser);
            Application.Current.MainPage = mainPage;
        }
    }
}