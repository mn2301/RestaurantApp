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
        public string sError;
        private string supabaseUrl = "https://qkhoblgvgddsgseybnge.supabase.co";
        private string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFraG9ibGd2Z2Rkc2dzZXlibmdlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQyNzU3NjgsImV4cCI6MjA4OTg1MTc2OH0.mgl09qDBWC5ovWxWQ0iSojLl8hM6-hOUAzaoSl04RBM";
        public static Supabase.Client _client;

        public async Task Connect()
        {
            try
            {
                if (_client == null)
                {
                    var options = new Supabase.SupabaseOptions { Schema = "MakosSushi" };
                    _client = new Supabase.Client(supabaseUrl, supabaseKey, options);
                    await _client.InitializeAsync();
                }

                Console.WriteLine("Conexión exitosa a Supabase.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a Supabase: {ex.Message}");
            }
        }

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
    }
}
