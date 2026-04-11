using Android.Service.Voice;
using RestaurantApp.Controller;
using RestaurantApp.Services;
using System.Collections.ObjectModel;
using System.Drawing;

namespace RestaurantApp;

public partial class OrderInfo : ContentPage
{
    List<OrderData> orders = new List<OrderData>();
    ObservableCollection<FullOrder> observableOrders = new ObservableCollection<FullOrder>();

    public OrderInfo()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        pkOrders.SelectedIndex = 0;

        orderGrid.ItemsSource = observableOrders;
    }

    // Load today's orders that need to be processed
    private async Task LoadUserOrders()
    {
        try
        {
            observableOrders.Clear();
            AdminController adminController = new AdminController();
            DateTime dtToday = DateTime.Now;

            orders = await adminController.getOrders(dtToday); // Get orders

            if (orders != null)
            {
                foreach (var order in orders)
                {
                    if(order.status != "Entregado" && order.status != "Cancelado")
                    {
                        var orderDetails = await adminController.getOrderDetails(order);
                        var clientDetails = await adminController.getClientDetails(order);
                        observableOrders.Add(new FullOrder
                        {
                            order = order,
                            details = orderDetails,
                            users = clientDetails
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No pudimos cargar los pedidos", "OK");
        }
    }

    // Load today's orders that are either delivered or cancelled
    private async Task LoadUserFinishedOrders()
    {
        try
        {
            observableOrders.Clear();
            AdminController adminController = new AdminController();
            DateTime dtToday = DateTime.Now;

            orders = await adminController.getOrders(dtToday);

            if (orders != null)
            {
                foreach (var order in orders)
                {
                    if (order.status == "Entregado" || order.status == "Cancelado")
                    {
                        var orderDetails = await adminController.getOrderDetails(order);
                        var clientDetails = await adminController.getClientDetails(order);
                        observableOrders.Add(new FullOrder
                        {
                            order = order,
                            details = orderDetails,
                            users = clientDetails
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No pudimos cargar los pedidos", "OK");
        }
    }

    // Load orders based on picker selection
    private async void pkOrders_SelectedIndexChanged(object sender, EventArgs e)
    {
        pkOrders.TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#E5E2E1");
        if(pkOrders.SelectedIndex == 0) 
        {
            await LoadUserOrders();
        }
        else
        {
            await LoadUserFinishedOrders();
        }
    }

    // Navigate to order details page when clicking on the image
    private async void imgLink_Clicked(object sender, EventArgs e)
    {
        try
        {
            var image = (Image)sender;

            var item = (FullOrder)image.BindingContext;

            OrderInfoDetails fullInfo = new OrderInfoDetails(item); // Create an instance of OrderInfoDetails with the selected order

            // Get the FlyoutPage and navigate within the NavigationPage of menuDetails
            if (Application.Current.MainPage is FlyoutPage flyout)
            {
                await flyout.Detail.Navigation.PushAsync(fullInfo); // Navigate within the NavigationPage that is in the Detail
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo abrir la pantalla", "OK");
        }
    }
}