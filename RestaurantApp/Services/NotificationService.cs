using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;

namespace RestaurantApp.Services;

public class NotificationService
{
    private readonly string projectId = "restaurantnotis";
    private readonly string fcmUrl; // Url of the FCM API endpoint

    public NotificationService()
    {
        fcmUrl = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";
    }

    // Obtain an access token using the service account credentials
    private async Task<string> GetAccessTokenAsync()
    {
        // Open the service account JSON file from the app package
        using var stream = await FileSystem.OpenAppPackageFileAsync("firebase-auth.json");

        var credential = GoogleCredential.FromStream(stream).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        return token;
    }

    // Sends a notification to a specific topic (admin or user) with the given title and body
    public async Task SendNotification(string toTopic, string title, string body)
    {
        try
        {
            await Task.Delay(20 * 1000); // Add a delay

            var accessToken = await GetAccessTokenAsync();

            // Construct the message
            var message = new
            {
                message = new
                {
                    topic = toTopic,
                    notification = new { title, body }
                }
            };

            // Serialize the message to JSON
            var json = JsonSerializer.Serialize(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request to FCM
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.PostAsync(fcmUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"FCM Response: {result}"); // Log the response from FCM

            // Show a local notification using a Snackbar if the app is in foreground
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var activity = Platform.CurrentActivity;

                // Verify the app is in foreground and the topic is relevant to the current user
                if (activity != null && toTopic.Contains(AppSession.CurrentUser.clientType))
                {
                    // If the topic is for clients, check if the topic's client ID matches the current user's ID
                    if (AppSession.CurrentUser.clientType == "client")
                    {
                        string id = toTopic.Replace("client_", ""); // Extract the client ID from the topic
                        if(id != AppSession.CurrentUser.id.ToString())
                        {
                            // If the topic's client ID does not match the current user's ID, do not show the notification
                            return;
                        }
                    }

                    // Show the notification using a Snackbar
                    var view = activity.Window.DecorView.FindViewById(global::Android.Resource.Id.Content);

                    var snack = global::Google.Android.Material.Snackbar.Snackbar.Make(view, $"{title}: {body}", global::Google.Android.Material.Snackbar.Snackbar.LengthLong);

                    var snackView = snack.View;
                    var lp = new global::Android.Widget.FrameLayout.LayoutParams(snackView.LayoutParameters);

                    lp.Gravity = global::Android.Views.GravityFlags.Top;

                    lp.TopMargin = 80;

                    snackView.LayoutParameters = lp;

                    snack.SetAction("OK", v => { }).Show();
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando notificación: {ex.Message}");
        }
    }
}