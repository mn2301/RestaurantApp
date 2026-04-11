using RestaurantApp.GoogleAuth;

namespace RestaurantApp.Platforms.Android
{
    public partial class GoogleAuthService : IGoogleAuthService
    {
        private const string WebApiKey = Secrets.WebApiKey; // Obtain my Web API Key from Secrets
    }
}
