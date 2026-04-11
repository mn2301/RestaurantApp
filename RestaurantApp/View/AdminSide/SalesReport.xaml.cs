using Syncfusion.Maui.DataGrid;

namespace RestaurantApp;

public partial class SalesReport : ContentPage
{ 
    List<OrderData> orders = new List<OrderData>();
    List<FullOrder> fullOrders = new List<FullOrder>();
    int iOrders = 0;
    float fSales = 0f;

    public SalesReport()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrders("Day");
        pkTimePeriod.SelectedIndex = 0;
    }

    // Load all orders for the selected time period
    private async Task LoadOrders(string time)
    {
        try
        {
            fullOrders.Clear();
            iOrders = 0;
            fSales = 0f;
            orderGrid.ItemsSource = null;
            AdminController adminController = new AdminController();
            DateTime dtToday = DateTime.Now;

            if(time == "Day")
                orders = await adminController.getOrdersDay(dtToday);
            else if (time == "Week")
                orders = await adminController.getOrdersWeek(dtToday);
            else if (time == "Month")
                orders = await adminController.getOrdersMonth(dtToday);

            if (orders != null)
            {
                foreach (var order in orders)
                {
                    var orderDetails = await adminController.getOrderDetails(order);
                    var clientDetails = await adminController.getClientDetails(order);
                    
                    fullOrders.Add(new FullOrder
                    {
                        order = order,
                        details = orderDetails,
                        users = clientDetails
                    });

                    fSales += order.totalprice;
                    iOrders++;
                }

                // Show orders, amount of orders and total sales
                orderGrid.ItemsSource = fullOrders;
                lblOrder.Text = iOrders.ToString();
                lblSales.Text = "$" + fSales.ToString("0.00");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No pudimos cargar los pedidos", "OK");
        }
    }

    // Load orders when the time period is changed
    private async void pkTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
    {
        pkTimePeriod.TextColor = Color.FromArgb("#E5E2E1");
        if (pkTimePeriod.SelectedIndex == 0)
            await LoadOrders("Day");
        else if(pkTimePeriod.SelectedIndex == 1)
            await LoadOrders("Week");
        else if (pkTimePeriod.SelectedIndex == 2)
            await LoadOrders("Month");
    }

    // Load the order details when the image is clicked
    private async void imgLink_Clicked(object sender, EventArgs e)
    {
        try
        {
            var image = (Image)sender;

            var item = (FullOrder)image.BindingContext;

            FullInfo fullInfo = new FullInfo(item);

            if (Application.Current.MainPage is FlyoutPage flyout)
            {
                await flyout.Detail.Navigation.PushAsync(fullInfo);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo abrir la pantalla", "OK");
        }
    }
}