using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RestaurantApp
{
    public class FullOrder : INotifyPropertyChanged
    {
        // Class that contains all the information of an order, including the order data, the details of the order and the user data
        private OrderData _order;

        // This class is used to bind the order data to the UI, and to update the UI when the order data changes
        public OrderData order
        {
            get => _order;
            set { _order = value; OnPropertyChanged(); }
        }
        public List<OrderDetailsData> details { get; set; }
        public UserData users { get; set; }

        // Properties that are used to bind the order status to the UI, and to update the UI when the order status changes
        public bool IsNotCancelled => order.status != "Cancelado" && order.status != "Entregado";
        public bool IsInProcess => order.status == "Ordenado";
        public bool IsSent => order.status == "En preparación" && order.eatlocation == "Domicilio";
        public bool IsDelivered => order.status == "Enviado" || (order.status == "En preparación" && order.eatlocation != "Domicilio");
        public bool IsFinalized => !(order.status == "Entregado" || order.status == "Cancelado");
        public bool IsDelivery => order.eatlocation == "Domicilio";

        // Calculate the height of the grid that contains the order details
        public int GridHeight => (details.Count * 80)+50;

        // INotifyPropertyChanged implementation to update the UI when the order data changes
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Method to refresh the status of the order, and update the UI when the order status changes
        public void RefreshStatus()
        {
            OnPropertyChanged(nameof(order));
            OnPropertyChanged(nameof(IsInProcess));
            OnPropertyChanged(nameof(IsSent));
            OnPropertyChanged(nameof(IsDelivered));
            OnPropertyChanged(nameof(IsNotCancelled));
            OnPropertyChanged(nameof(IsFinalized));
        }
    }
}
