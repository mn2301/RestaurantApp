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

    // Login
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
		var loggedUser = await _googleAuthService.GetCurrentUserAsync(); // Check if the user is already logged in

        if (loggedUser == null)
		{
			loggedUser = await _googleAuthService.AuthenticateAsync(); // If not, prompt the user to log in through Google
        }

        // If the user is successfully logged in, create a UserData object to check or save it to the database
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

            // Open the new page with the user info
            MainPage mainPage = new MainPage(dbUser);
            Application.Current.MainPage = mainPage;
        }
    }
}