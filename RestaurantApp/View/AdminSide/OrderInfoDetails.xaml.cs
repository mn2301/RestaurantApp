using Android.Renderscripts;
using RestaurantApp.Controller;
using RestaurantApp.Services;

namespace RestaurantApp;

public partial class OrderInfoDetails : ContentPage
{
    FullOrder forder = new FullOrder();

    // Show order details
    public OrderInfoDetails(FullOrder fullOrder)
	{
		InitializeComponent();
        forder = fullOrder;
        lblName.Text = fullOrder.users.clientName;
        lblLocation.Text = fullOrder.order.eatlocation + " • " + fullOrder.order.status;
        lblDate.Text = fullOrder.order.date.ToString("dd/MM/yyyy HH:mm");
        lblSubtotal.Text = "Subtotal: $" + fullOrder.order.subtotal.ToString("0.00");
        if (fullOrder.order.deliveryfee != null)
            lblDelivery.Text = "Delivery fee: $" + fullOrder.order.deliveryfee.Value.ToString("0.00");
        else
            lblDelivery.Text = "Delivery fee: N/A";
        lblIVA.Text = "IVA: $" + fullOrder.order.iva.ToString("0.00");
        lblTotal.Text = "Total: $" + fullOrder.order.totalprice.ToString("0.00");

        gdOrderInfo.ItemsSource = fullOrder.details;
        vslButtons.BindingContext = fullOrder;
    }

    // Change order status
    private async void btnStatusChange(string newStatus, object sender, EventArgs e)
    {
        try
        {
            // Get button that was clicked
            var button = (Button)sender;

            // Obtain the FullOrder object from the button's BindingContext
            var selectedItem = (FullOrder)button.BindingContext;

            if (selectedItem != null && selectedItem.order.status != newStatus)
            {
                GlobalController globalController = new GlobalController();
                string oldStatus = selectedItem.order.status;
                selectedItem.order.status = newStatus;

                if (await globalController.updateStatus(selectedItem.order)) // Update status
                {
                    // Update UI
                    lblLocation.Text = forder.order.eatlocation + " • " + selectedItem.order.status;
                    selectedItem.RefreshStatus();

                    // Send a notification to the user about the status change
                    var notify = new NotificationService();
                    string topic = "";
                    if (selectedItem.users.clientType == "client")
                        topic = $"client_{selectedItem.order.userid}";
                    else
                        topic = "admin";
                    await notify.SendNotification(topic, "¡Actualización de tu pedido!", $"Tu pedido ahora está: {newStatus}");
                }
                else
                {
                    selectedItem.order.status = oldStatus;
                    await DisplayAlertAsync("Error", "No se pudo actualizar en el servidor", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No pudimos cambiar el estado del pedido", "OK");
        }
    }

    private void btnInProcess_Clicked(object sender, EventArgs e) => btnStatusChange("En preparación", sender, e);

    private void btnSent_Clicked(object sender, EventArgs e) => btnStatusChange("Enviado", sender, e);

    private void btnDelivered_Clicked(object sender, EventArgs e) => btnStatusChange("Entregado", sender, e);
}