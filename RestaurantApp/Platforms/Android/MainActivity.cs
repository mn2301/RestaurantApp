using Android.App;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;
using Android.Content;
using Firebase;

namespace RestaurantApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static event EventHandler<(bool Success, GoogleSignInAccount account)> ResultGoogleAuth;
        public static readonly string ChannelID = "orders_channel"; // Necesary to use a channel for Firebase notifications on Android 8.0 and up

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Initialize Firebase
            Firebase.FirebaseApp.InitializeApp(this);
            CreateNotificationChannel();
        }

        // Create a notification channel for Android 8.0 and up
        private void CreateNotificationChannel()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var channel = new NotificationChannel(ChannelID, "Pedidos", NotificationImportance.High)
                {
                    Description = "Notificaciones de pedidos del restaurante"
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        // Handle the result of the Google Sign-In intent
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if(requestCode == 9001)
            {
                try
                {
                    var currentAccount = await GoogleSignIn.GetSignedInAccountFromIntentAsync(data);
                    ResultGoogleAuth.Invoke(this, (currentAccount.Email != null, currentAccount));
                } catch(Exception ex)
                {
                    ResultGoogleAuth.Invoke(this, (false, null));
                }
            }
        }
    }
}
