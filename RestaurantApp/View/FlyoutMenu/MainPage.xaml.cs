using Android.Gms.Tasks;
using RestaurantApp.Controller;
using RestaurantApp.GoogleAuth;
using RestaurantApp.Platforms.Android;
using RestaurantApp.Services;
using Supabase.Gotrue;

namespace RestaurantApp
{
    public partial class MainPage : FlyoutPage
    {
        public UserData loggedUser = new UserData();

        public MainPage(UserData user)
        {
            loggedUser = user;
            InitializeComponent();
            UserChange();
            flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;

#if ANDROID
            try
            {
                // Make sure to check notification permissions before trying to get the token
                System.Threading.Tasks.Task.Run(async () => await CheckNotificationPermission());

                // Get the FCM token for this device
                var instance = Firebase.Messaging.FirebaseMessaging.Instance;
                instance.GetToken().AddOnCompleteListener(new OnCompleteListener(task =>
                {
                    if (task.IsSuccessful)
                    {
                        var token = task.Result.ToString();
                        Console.WriteLine($"FCM TOKEN: {token}");

                        /*MainThread.BeginInvokeOnMainThread(async () => {
                            await DisplayAlert("Token de mi Celular", token, "Copiar");
                            await Clipboard.SetTextAsync(token);
                        });*/
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de Firebase: {ex.Message}");
            }
#endif
        }

        // Validate notification permission
        public async System.Threading.Tasks.Task CheckNotificationPermission()
        {
#if ANDROID
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }

            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Permiso de notificaciones denegado.");
            }
#endif
            // Agregar esto para que en otras plataformas no marque error
            await System.Threading.Tasks.Task.CompletedTask;
        }

#if ANDROID
        // Helper class to handle the async token retrieval callback from Firebase
        public class OnCompleteListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
        {
            private readonly Action<Android.Gms.Tasks.Task> _callback;
            public OnCompleteListener(Action<Android.Gms.Tasks.Task> callback) => _callback = callback;
            public void OnComplete(Android.Gms.Tasks.Task task) => _callback(task);
        }
#endif

        // Handle menu selection changes
        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                    IsPresented = false;
            }
        }

        // Update the menu based on the user's role
        private void UserChange()
        {
            var menuItems = new List<FlyoutPageItem>();

            // Menu items for all users
            menuItems.Add(new FlyoutPageItem { Title = "Menu", TargetType = typeof(FullMenu), IconSource = "menu.png" });

            if (loggedUser.clientType == "admin")
            {
                // Tabs for admin users
                menuItems.Add(new FlyoutPageItem { Title = "Agregar artículos", TargetType = typeof(AddMenu), IconSource = "addmenu.png" });
                menuItems.Add(new FlyoutPageItem { Title = "Pedidos", TargetType = typeof(OrderInfo), IconSource = "order.png" });
                menuItems.Add(new FlyoutPageItem { Title = "Reporte de ventas", TargetType = typeof(SalesReport), IconSource = "report.png" });
#if ANDROID
                try
                {
                    // Subscribe to the "admin" topic for admin users
                    Firebase.Messaging.FirebaseMessaging.Instance.SubscribeToTopic("admin");
                    Console.WriteLine("Suscripción exitosa al tópico: admin");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al suscribir: {ex.Message}");
                }
#endif
            }
            else
            {
                // Tabs for regular users
                menuItems.Add(new FlyoutPageItem { Title = "Mis pedidos", TargetType = typeof(History), IconSource = "history.png" });
                menuItems.Add(new FlyoutPageItem { Title = "Mi información", TargetType = typeof(MyInfo), IconSource = "client.png" });
#if ANDROID
                try
                {
                    // Subscribe to a "client" topic for this client using their user ID
                    Firebase.Messaging.FirebaseMessaging.Instance.SubscribeToTopic($"client_{AppSession.CurrentUser.id}");
                    Console.WriteLine($"Suscripción exitosa al tópico: client_{ AppSession.CurrentUser.id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al suscribir: {ex.Message}");
                }
#endif
            }

            flyoutPage.collectionView.ItemsSource = menuItems;
        }
    }
}
