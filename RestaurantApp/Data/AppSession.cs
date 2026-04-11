using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RestaurantApp
{
    public static class AppSession
    {
        // Static class to save the current user and cart items across the app
        public static UserData CurrentUser { get; set; }
        public static ObservableCollection<CartItems> cartItems { get; set; } = new ObservableCollection<CartItems>();
    }
}
