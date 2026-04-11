using Android.App;
using Firebase.Messaging;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class MyFirebaseMessagingService : FirebaseMessagingService
{
    // Calls this method when a notification is received while the app is in the foreground
    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        var notification = message.GetNotification();
        Console.WriteLine($"Mensaje recibido: {notification.Title} - {notification.Body}");
    }

    // Calls this method when a new token is generated for the device
    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);
        Console.WriteLine($"Nuevo Token generado: {token}");
    }
}