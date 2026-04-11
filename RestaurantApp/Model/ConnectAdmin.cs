using System;
using System.Collections.Generic;
using System.Text;
using static Android.Graphics.ColorSpace;
using static Supabase.Postgrest.Constants;

namespace RestaurantApp
{
    public class ConnectAdmin : ConnectSupabase
    {
        // Connection class for admin operations

        // Save new menu item to the database
        public async Task<bool> SaveMenu(MenuData menu)
        {
            try
            {
                await Connect();

                await _client.From<MenuData>().Insert(menu); // Insert new menu item
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
            return true;
        }

        // Upload image to Supabase Storage and return public URL
        public async Task<string> UploadImage(FileResult file)
        {
            try
            {
                await Connect();

                // Convert FileResult to bytes
                var stream = await file.OpenReadAsync();
                byte[] imageData;
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }

                // Generate a unique name to avoid collisions
                string fileName = $"{Guid.NewGuid()}_{file.FileName}";

                // Save to bucket 'menu-images'
                await _client.Storage.From("menu_images").Upload(imageData, fileName);

                // Get the final public URL
                string publicUrl = _client.Storage.From("menu_images").GetPublicUrl(fileName);

                return publicUrl;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Update an existing menu item
        public async Task<bool> UpdateMenu(MenuData menu, string ogName)
        {
            try
            {
                await Connect();

                // Get the menu item by original name to find its ID
                var result = await _client.From<MenuData>().Where(x=>x.Name == ogName).Get(); 

                var newID = result.Models.FirstOrDefault();

                if(newID != null)
                {
                    menu.id = newID.id;
                }

                await _client.From<MenuData>().Where(x => x.id == menu.id).Update(menu); // Update the menu item
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
            return true;
        }

        // Get all orders for a today's date
        // Ordered by ID in ascending order to show the oldest orders first, used for orders
        public async Task<List<OrderData>> GetOrders(DateTime dtToday)
        {
            try
            {
                await Connect();

                DateTime startOfDay = dtToday.Date.ToUniversalTime(); // Start of the day (00:00:00)
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1); // End of the day (23:59:59.9999999)

                // Get orders for today
                var result = await _client.From<OrderData>().Where(x => x.date >= startOfDay).Where(x => x.date <= endOfDay).Order(x => x.id, Ordering.Ascending).Get();

                foreach(var order in result.Models)
                {
                    DateTime local = order.date.ToLocalTime(); // Supabase saves dates as UTC, so modify the date to local time for display
                    order.date = local;
                }

                return result.Models;

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Get all orders for the current week
        public async Task<List<OrderData>> GetOrdersWeek(DateTime dtToday)
        {
            try
            {
                await Connect();

                DateTime startOfDay = dtToday.Date.ToUniversalTime(); // Start of the day (00:00:00)
                DateTime startOfWeek = startOfDay.AddDays(-(int)startOfDay.DayOfWeek); // Start of the week (Sunday)
                DateTime endOfWeek = startOfWeek.AddDays(7).AddTicks(-1); // End of the week (Saturday)

                // Get orders for the current week
                var result = await _client.From<OrderData>().Where(x => x.date >= startOfWeek).Where(x => x.date <= endOfWeek).Order(x => x.id, Ordering.Descending).Get();

                foreach(var order in result.Models)
                {
                    DateTime local = order.date.ToLocalTime();
                    order.date = local;
                }

                return result.Models;

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Get orders for today's date
        // Ordered by ID in descending order to show the most recent orders first, used for sales report
        public async Task<List<OrderData>> GetOrdersDay(DateTime dtToday)
        {
            try
            {
                await Connect();

                DateTime startOfDay = dtToday.Date.ToUniversalTime(); // Start of the day (00:00:00)
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1); // End of the day (23:59:59.9999999)

                // Get orders for today
                var result = await _client.From<OrderData>().Where(x => x.date >= startOfDay).Where(x => x.date <= endOfDay).Order(x => x.id, Ordering.Descending).Get();

                foreach(var order in result.Models)
                {
                    DateTime local = order.date.ToLocalTime();
                    order.date = local;
                }

                return result.Models;

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Get orders for the current month
        public async Task<List<OrderData>> GetOrdersMonth(DateTime dtToday)
        {
            try
            {
                await Connect();

                DateTime startOfDay = dtToday.Date.ToUniversalTime(); // Start of the day (00:00:00)
                DateTime startOfMonth = new DateTime(startOfDay.Year, startOfDay.Month, 1); // Start of the month
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1); // End of the month

                // Get orders for the current month
                var result = await _client.From<OrderData>().Where(x => x.date >= startOfMonth).Where(x => x.date <= endOfMonth).Order(x => x.id, Ordering.Descending).Get();

                foreach (var order in result.Models)
                {
                    DateTime local = order.date.ToLocalTime();
                    order.date = local;
                }

                return result.Models;

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Get order details for a specific order
        public async Task<List<OrderDetailsData>> GetOrderDetails(OrderData order)
        {
            try
            {
                await Connect();

                var result = await _client.From<OrderDetailsData>().Where(x => x.orderid == order.id).Get(); // Get details for order by id
                return result.Models;

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        // Get client details for a specific order
        public async Task<UserData> GetClientDetails(OrderData order)
        {
            try
            {
                await Connect();

                var result = await _client.From<UserData>().Where(x => x.id == order.userid).Get(); // Get client info by userid from order

                return result.Models.FirstOrDefault();

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }
    }
}
