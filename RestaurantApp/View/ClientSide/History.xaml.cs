using RestaurantApp.Controller;
using RestaurantApp.Services;
using System.Collections.ObjectModel;

namespace RestaurantApp;

public partial class History : ContentPage
{
    List<OrderData> orders = new List<OrderData>();
    ObservableCollection<FullOrder> observableOrders = new ObservableCollection<FullOrder>();

    public History()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        cvOrders.ItemsSource = observableOrders;
        await LoadUserOrders();
    }

    // Load the users' orders
    private async Task LoadUserOrders()
    {
        try
        {
            observableOrders.Clear();
            UserController userController = new UserController();

            orders = await userController.getOrders(AppSession.CurrentUser);

            if (orders != null)
            {
                var orderDetails = orders.Select(async order =>
                {
                    var details = await userController.getOrderDetails(order);
                    return new FullOrder { order = order, details = details };
                });

                var results = await Task.WhenAll(orderDetails);

                foreach (var res in results) 
                {
                    observableOrders.Add(res);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No pudimos cargar tus pedidos", "OK");
        }
    }

    // Cancel an order
    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Get the button that was clicked
            var button = (Button)sender;

            // Obtain the order associated with the button using the BindingContext
            var selectedItem = (FullOrder)button.BindingContext;

            if (selectedItem != null && selectedItem.order.status != "Cancelado")
            {
                GlobalController globalController = new GlobalController();
                string oldStatus = selectedItem.order.status;
                selectedItem.order.status = "Cancelado";

                if (await globalController.updateStatus(selectedItem.order)) // Update the order status in the database
                {
                    // Confirm cancellation to the user and refresh the order list
                    await DisplayAlertAsync("Éxito", "Pedido cancelado correctamente", "OK");
                    selectedItem.RefreshStatus();

                    // Send a notification to the admin
                    var notify = new NotificationService();
                    await notify.SendNotification("admin", "Pedido Cancelado", $"El usuario ha cancelado el pedido #{selectedItem.order.id}");
                }
                else
                {
                    selectedItem.order.status = oldStatus;
                    await DisplayAlertAsync("Error", "No pudimos cancelar tu pedido", "OK");
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}