using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RestaurantApp
{
    public static class AppSession
    {
        public static UserData CurrentUser { get; set; }
        public static ObservableCollection<CartItems> cartItems { get; set; } = new ObservableCollection<CartItems>();
    }
}
