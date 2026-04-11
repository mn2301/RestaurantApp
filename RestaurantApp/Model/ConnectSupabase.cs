using Android.Gms.Common.Apis;
using Android.Graphics;
using Java.Net;
using Supabase;
using System;
using System.Collections.Generic;
using System.Text;
using static Android.Graphics.ColorSpace;
using static Supabase.Postgrest.Constants;

namespace RestaurantApp
{
    public class ConnectSupabase
    {
        // Main class for connecting to Supabase
        public string sError;
        public static Supabase.Client _client;

        // Method to connect to Supabase
        public async Task Connect()
        {
            try
            {
                if (_client == null)
                {
                    // Initialize the Supabase client with the provided URL and key
                    var options = new Supabase.SupabaseOptions { Schema = "MakosSushi" };
                    _client = new Supabase.Client(Secrets.supabaseUrl, Secrets.supabaseKey, options);
                    await _client.InitializeAsync();
                }

                Console.WriteLine("Conexión exitosa a Supabase.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a Supabase: {ex.Message}");
            }
        }

        // Method for user login, checks if the user exists and returns the user data, otherwise inserts a new user
        public async Task<UserData> UserLogin(UserData user)
        {
            try
            {
                await Connect();

                var result = await _client.From<UserData>().Filter("email", Supabase.Postgrest.Constants.Operator.Equals, user.clientEmail).Get();
                var existingUser = result.Models.FirstOrDefault();

                if (existingUser != null)
                {
                    return existingUser;
                }
                else
                {
                    var insertResponse = await _client.From<UserData>().Insert(user);
                    return insertResponse.Models.First();
                }
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Method to get menu items, returns a list of menu items ordered by name
        public async Task<List<MenuData>> GetMenuItems()
        {
            try
            {
                await Connect();
                var result = await _client.From<MenuData>().Order(x => x.Name, Ordering.Ascending).Get();
                return result.Models;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // I don't use this but it says it has a reference
        public async Task<List<MenuData>> GetMenuItems(MenuData menuItem, string ogName)
        {
            try
            {
                await Connect();
                var result = await _client.From<MenuData>().Where(x => x.Name == ogName).Get();
                var newID = result.Models.FirstOrDefault();

                if (newID != null)
                {
                    menuItem.id = newID.id;
                }

                var result2 = await _client.From<MenuData>().Where(x=>x.id == menuItem.id).Get();
                return result2.Models;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Method to save an order, inserts the order data and the order details data into the database
        public async Task<bool> SaveOrder(OrderData order, List<OrderDetailsData> orderData)
        {
            try
            {
                await Connect();

                var response = await _client.From<OrderData>().Insert(order);
                var newOrder = response.Models.FirstOrDefault();

                if (newOrder != null)
                {
                    foreach (var item in orderData)
                    {
                        item.orderid = newOrder.id;
                    }

                    await _client.From<OrderDetailsData>().Insert(orderData);
                }

                return true;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }

        // Method to update the status of an order, updates the order data in the database
        public async Task<bool> UpdateStatus(OrderData order)
        {
            try
            {
                await Connect();

                await _client.From<OrderData>().Where(x => x.id == order.id).Update(order);

                return true;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }
    }
}
