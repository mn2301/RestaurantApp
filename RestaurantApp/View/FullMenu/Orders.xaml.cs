using RestaurantApp.Controller;
using RestaurantApp.Services;

namespace RestaurantApp;

public partial class Orders : ContentPage
{
    // Properties to bind to the UI
    public decimal Subtotal => (decimal)AppSession.cartItems.Sum(x => x.price * x.quantity);
    public decimal IVA => Subtotal * 0.16m;
    public decimal DeliveryFee { get; set; } = 0m;
    public decimal Total => Subtotal + IVA + DeliveryFee;

    public string location { get; set; }

    public Orders()
	{
		InitializeComponent();
        this.BindingContext = this;
        cartGrid.ItemsSource = AppSession.cartItems;
        location = "Sucursal";
        if (AppSession.CurrentUser.clientType == "admin")
        {
            lblLocation.IsVisible = false;
            btnsLocation.IsVisible = false;
            gdDelivFee.IsVisible = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateTotal();
    }

    // Update totals on the UI
    private void UpdateTotal()
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(IVA));
    }

    // Delete an item from the cart
    private void btnDel_Clicked(object sender, EventArgs e)
    {
        try
        {
            var image = (Image)sender;

            var item = (CartItems)image.BindingContext;

            AppSession.cartItems.Remove(item);
            UpdateTotal();

        } catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    // Increase quantity of an item
    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        var item = (CartItems)((Button)sender).CommandParameter;
        item.quantity++; 
        UpdateTotal();
    }

    // Decrease quantity of an item
    private void btnLess_Clicked(object sender, EventArgs e)
    {
        var item = (CartItems)((Button)sender).CommandParameter;
        if (item.quantity > 1)
        {
            item.quantity--;
            UpdateTotal();
        }
    }

    private void btnEatHere_Clicked(object sender, EventArgs e) => EatingLocation("Sucursal", (Button)sender);

    private void btnPickUp_Clicked(object sender, EventArgs e) => EatingLocation("Recoger", (Button)sender);

    private void btnDelivery_Clicked(object sender, EventArgs e) => EatingLocation("Domicilio", (Button)sender);

    // Handle eating location selection and update fees
    private void EatingLocation(string type, Button selectedButton)
    {
        if (type == "Domicilio")
            DeliveryFee = 50m;
        else
            DeliveryFee = 0m;

        OnPropertyChanged(nameof(DeliveryFee));
        OnPropertyChanged(nameof(IVA));
        OnPropertyChanged(nameof(Total));

        ResetButtonColor();

        // Modify selected button colors
        selectedButton.BackgroundColor = Color.FromArgb("#74A173");
        selectedButton.TextColor = Color.FromArgb("#131313");
        location = selectedButton.Text;
    }

    private void ResetButtonColor()
    {
        // Reset button colors
        var normalBg = Color.FromArgb("#2A2A2A");
        var normalText = Color.FromArgb("#E5E2E1");

        btnDelivery.BackgroundColor = btnEatHere.BackgroundColor = btnPickUp.BackgroundColor = normalBg;
        btnDelivery.TextColor = btnEatHere.TextColor = btnPickUp.TextColor = normalText;
    }

    // Save order
    private async void btnOrder_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Check if user (client) has an address if they choose delivery
            if (AppSession.CurrentUser.clientType == "client" && location == "Domicilio")
            {
                UserController userController = new UserController();
                var result = await userController.checkUserAddress(AppSession.CurrentUser);
                if (!result)
                {
                    await DisplayAlertAsync("Error", "No has agregado una dirección, por favor ve a tu perfil y agrega una para poder realizar pedidos a domicilio.", "OK");
                    return;
                }
            }

            btnOrder.IsEnabled = false;

            // Create order data
            OrderData order = new OrderData
            {
                userid = AppSession.CurrentUser.id,
                date = DateTime.Now,
                eatlocation = location,
                subtotal = (float)Subtotal,
                deliveryfee = (float)DeliveryFee,
                iva = (float)IVA,
                totalprice = (float)Total,
                status = "Ordenado"
            };

            // Create order details list
            List<OrderDetailsData> orderDetailsList = new List<OrderDetailsData>();
            foreach (var item in AppSession.cartItems)
            {
                OrderDetailsData orderDetails = new OrderDetailsData
                {
                    menuid = item.menuID,
                    quantity = item.quantity,
                    comments = item.comments,
                    name = item.name,
                    price = item.price*item.quantity
                };
                orderDetailsList.Add(orderDetails);
            }

            GlobalController globalController = new GlobalController();
            if (await globalController.SaveOrder(order, orderDetailsList)) // Save order and details to the database
            {
                await DisplayAlertAsync("Éxito", "Tu orden ha sido realizada con éxito.", "OK");

                // Clear cart and reset UI
                AppSession.cartItems.Clear();
                UpdateTotal();
                ResetButtonColor();
                btnEatHere.BackgroundColor = Color.FromArgb("#74A173");
                btnEatHere.TextColor = Color.FromArgb("#131313");
                btnOrder.IsEnabled = true;

                // Send notification to admin about new order
                var notify = new NotificationService();
                _ = notify.SendNotification("admin", "¡Pedido nuevo!", $"Orden nueva de {AppSession.CurrentUser.clientName}");
            } else
            {
                await DisplayAlertAsync("Error", "Ocurrió un error al realizar tu orden, por favor intenta de nuevo.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}