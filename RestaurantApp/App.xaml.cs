using Android.Content.Res;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage();
        }
    }
}