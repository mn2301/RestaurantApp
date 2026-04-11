using RestaurantApp.Controller;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RestaurantApp;

public partial class FullMenu : ContentPage
{
    private List<MenuData> fullMenu;

    public FullMenu()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateCartNum();
        LoadMenu();
    }

    // Load full menu
    private async void LoadMenu()
    {
        try
        {
            GlobalController globalController = new GlobalController();
            fullMenu = await globalController.getMenuItems();

            cvSushiMenu.ItemsSource = fullMenu;
        } catch(Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudo cargar el menú: {ex.Message}", "OK");
        }
    }

    private void btnAllMenu_Clicked(object sender, EventArgs e) => FilterMenu("All", (Button)sender);
    private void btnSushiMenu_Clicked(object sender, EventArgs e) => FilterMenu("Sushi", (Button)sender);
    private void btnDrinkMenu_Clicked(object sender, EventArgs e) => FilterMenu("Drink", (Button)sender);

    // Filter menu by type
    private void FilterMenu(string type, Button selectedButton)
    {
        // Filtrar la lista
        if (type == "All")
            cvSushiMenu.ItemsSource = fullMenu;
        else
            cvSushiMenu.ItemsSource = fullMenu.Where(x => x.Type == type).ToList();

        // Resetear colores de botones
        var normalBg = Color.FromArgb("#2A2A2A");
        var normalText = Color.FromArgb("#E5E2E1");

        btnAllMenu.BackgroundColor = btnSushiMenu.BackgroundColor = btnDrinkMenu.BackgroundColor = normalBg;
        btnAllMenu.TextColor = btnSushiMenu.TextColor = btnDrinkMenu.TextColor = normalText;

        // Modificar color de botón seleccionado
        selectedButton.BackgroundColor = Color.FromArgb("#74A173");
        selectedButton.TextColor = Color.FromArgb("#131313");
    }

    // Open a details page when selecting a menu item
    private void cvSushiMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ((CollectionView)sender).SelectedItem = null; // Get rid of selection highlight
        var selectedItem = e.CurrentSelection.FirstOrDefault() as MenuData; // Obtain the selected item

        if (selectedItem != null)
        {
            NavigateToMenuDetails(selectedItem, sender); // Open the details page for the selected menu item
        }
    }

    // Navigate to the details page for the selected menu item
    private async void NavigateToMenuDetails(MenuData menuItem, object sender)
    {
        MenuDetails menuDetails = new MenuDetails(menuItem);

        if (Application.Current.MainPage is FlyoutPage flyout)
        {
            await flyout.Detail.Navigation.PushAsync(menuDetails); 
        }
    }

    // Open the cart page when tapping the cart icon
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Orders cart = new Orders();
        if (Application.Current.MainPage is FlyoutPage flyout)
        {
            flyout.Detail.Navigation.PushAsync(cart);
        }
    }

    // Add the selected menu item to the cart
    private void btnAddToCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Obtain the button that was clicked
            var button = (Button)sender;

            // Obtain the menu item associated with that button
            var selectedItem = (MenuData)button.BindingContext;

            if (selectedItem != null && selectedItem.Availability == "Disponible")
            {
                string currentComment = null;
                var existingItem = AppSession.cartItems.FirstOrDefault(x => x.menuID == selectedItem.id && x.comments == currentComment);

                if (existingItem != null)
                {
                    // If the item already exists in the cart, increase its quantity
                    existingItem.quantity ++;
                }
                else
                {
                    // If the item doesn't exist in the cart, add it as a new entry
                    AppSession.cartItems.Add(new CartItems
                    {
                        menuID = selectedItem.id,
                        name = selectedItem.Name,
                        comments = null,
                        price = selectedItem.Price,
                        imageUrl = selectedItem.ImageUrl,
                        quantity = 1
                    });
                }

                UpdateCartNum();
            }

        } catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    // Update the cart item count displayed on the cart icon
    private void UpdateCartNum()
    {
        // Get total item count
        int totalItems = AppSession.cartItems.Sum(x => x.quantity);

        if (totalItems > 0)
        {
            lblCartCount.Text = totalItems.ToString();
            cartBadge.IsVisible = true;
        }
        else
        {
            cartBadge.IsVisible = false;
        }
    }
}