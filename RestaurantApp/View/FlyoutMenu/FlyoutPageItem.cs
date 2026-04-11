using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantApp
{
    public class FlyoutPageItem
    {
        // Properties for each menu item
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}
