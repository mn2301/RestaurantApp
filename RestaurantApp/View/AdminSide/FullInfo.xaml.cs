namespace RestaurantApp;

public partial class FullInfo : ContentPage
{
	// Show all the details of the order that was selected
	public FullInfo(FullOrder fullOrder)
	{
		InitializeComponent();
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
    }
}