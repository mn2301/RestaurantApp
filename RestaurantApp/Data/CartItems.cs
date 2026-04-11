using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RestaurantApp
{
    public class CartItems : INotifyPropertyChanged
    {
        // Class to represent items in the cart, with properties for menuID, name, comments, price, imageUrl, and quantity
        public int menuID {  get; set; }
        public string name { get; set; }
        public string? comments { get; set; }
        public float price { get; set; }
        public string imageUrl { get; set; }
        private int _quantity;

        // Property for quantity with change notification, updates the UI when the quantity changes
        public int quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        // Event and method for property change notification, allowing the UI to update when properties of this class change
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
