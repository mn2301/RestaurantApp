using Android.Gms.Auth.Api.SignIn;
using RestaurantApp.GoogleAuth;
using Android.App;
using Android.Content;

namespace RestaurantApp.Platforms.Android
{
    public partial class GoogleAuthService
    {
        public static Activity _activity;
        public static GoogleSignInOptions _gso;
        public static GoogleSignInClient _googleSignInClient;

        private TaskCompletionSource<GoogleUserDTO> _taskCompletionSource = new TaskCompletionSource<GoogleUserDTO>();
        private Task<GoogleUserDTO> GoogleAuthentication
        {
            get => _taskCompletionSource.Task;
        }

        public GoogleAuthService()
        {
        }

        // Make sure to initialize the GoogleSignInClient only once and subscribe to the event only once
        private void EnsureInitialized()
        {
            // Only initialize if the client is null
            if (_googleSignInClient == null)
            {
                _activity = Platform.CurrentActivity;

                if (_activity == null)
                    throw new Exception("No se pudo obtener la Actividad actual de Android.");

                _gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                                .RequestIdToken(WebApiKey)
                                .RequestEmail()
                                .RequestId()
                                .RequestProfile()
                                .Build();

                _googleSignInClient = GoogleSignIn.GetClient(_activity, _gso);

                // Subscribe to the event only once
                MainActivity.ResultGoogleAuth -= MainActivity_ResultGoogleAuth;
                MainActivity.ResultGoogleAuth += MainActivity_ResultGoogleAuth;
            }
        }

        // Authenticate the user that signs in with Google and return a GoogleUserDTO with the user's information
        public Task<GoogleUserDTO> AuthenticateAsync()
        {
            EnsureInitialized();

            _taskCompletionSource = new TaskCompletionSource<GoogleUserDTO>();

            _activity.StartActivityForResult(_googleSignInClient.SignInIntent, 9001);

            return GoogleAuthentication;

        }

        // Handle the result of the Google sign in
        private void MainActivity_ResultGoogleAuth(object sender, (bool Success, GoogleSignInAccount Account) e)
        {
            if (e.Success)
            {
                try
                {
                    var currentAccount = e.Account;

                    _taskCompletionSource.TrySetResult(
                        new GoogleUserDTO
                        {
                            Email = currentAccount.Email,
                            FullName = currentAccount.DisplayName,
                            TokenId = currentAccount.IdToken,
                            ImageURL = currentAccount.PhotoUrl?.ToString(),
                        });
                }
                catch (Exception ex)
                {
                    _taskCompletionSource.SetException(ex);
                }
            }

        }

        // Get the current signed in user
        public async Task<GoogleUserDTO> GetCurrentUserAsync()
        {
            try
            {
                EnsureInitialized();

                var user = await _googleSignInClient.SilentSignInAsync();
                return new GoogleUserDTO
                {
                    Email = user.Email,
                    FullName = $"{user.DisplayName}",
                    TokenId = user.IdToken,
                    ImageURL = user.PhotoUrl?.ToString()
                };

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Logout
        public Task LogoutAsync() => _googleSignInClient.SignOutAsync();

    }
}
