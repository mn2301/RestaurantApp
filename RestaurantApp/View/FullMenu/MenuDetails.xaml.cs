using RestaurantApp.Controller;

namespace RestaurantApp;

public partial class MenuDetails : ContentPage
{
	MenuData selectedMenuItem;

    // Add menu item details to the page and set the title to the menu item name
    public MenuDetails(MenuData menuItem)
	{
		selectedMenuItem = menuItem;
		InitializeComponent();
        this.BindingContext = selectedMenuItem;
        this.Title = selectedMenuItem.Name;
        if (AppSession.CurrentUser.clientType == "client")
            imgEdit.IsVisible = false;
        else
            imgEdit.IsVisible = true;
    }

    // Add quantity of the menu item to be added to the cart, with a minimum of 1
    private void btnLess_Clicked(object sender, EventArgs e)
    {
        if(lblAmount.Text != "1")
        {
            int curentAmount = int.Parse(lblAmount.Text);
            lblAmount.Text = (curentAmount - 1).ToString();
            if(lblAmount.Text == "1")
            {
                btnLess.IsEnabled = false;
            }
        }
    }

    // Add quantity of item to be added to the cart
    private void btnMore_Clicked(object sender, EventArgs e)
    {
        int curentAmount = int.Parse(lblAmount.Text);
        lblAmount.Text = (curentAmount + 1).ToString();
        if (lblAmount.Text != "1")
        {
            btnLess.IsEnabled = true;
        }
    }
    
    // Add menu item to the cart
    private void btnAddToCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Obtain the button that was clicked
            var button = (Button)sender;

            // Obttain the menu item associated with the button using the BindingContext
            var selectedItem = (MenuData)button.BindingContext;

            if (selectedItem != null && selectedItem.Availability == "Disponible")
            {
                string currentComment = enComment.Text;
                var existingItem = AppSession.cartItems.FirstOrDefault(x => x.menuID == selectedItem.id && x.comments == currentComment);

                if (existingItem != null)
                {
                    // If it exists, increase the quantity
                    existingItem.quantity++;
                }
                else
                {
                    // If it doesn't exist, add a new item to the cart
                    AppSession.cartItems.Add(new CartItems
                    {
                        menuID = selectedItem.id,
                        name = selectedItem.Name,
                        comments = enComment.Text,
                        price = selectedItem.Price,
                        imageUrl = selectedItem.ImageUrl,
                        quantity = Convert.ToInt16(lblAmount.Text),
                    });
                }
            }

        } catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    // Navigate to the EditMenu page, passing the selected menu item as a parameter
    private void imgEdit_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if(AppSession.CurrentUser.clientType == "admin")
            {
                EditMenu editMenu = new EditMenu(selectedMenuItem);
                if (Application.Current.MainPage is FlyoutPage flyout)
                {
                    flyout.Detail.Navigation.PushAsync(editMenu);
                }
            }

        } catch(Exception ex)
        {
                Console.WriteLine(ex.ToString());
        }
    }
}