using Android.Gms.Fido.U2F.Api.Common;
using Supabase;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Text;
using static Supabase.Postgrest.Constants;

namespace RestaurantApp
{
    public class ConnectUser : ConnectSupabase
    {
        // Class to manage user actions

        // Save user information (address and phone number)
        public async Task<bool> SaveInfo(UserData user, string address, string phone)
        {
            try
            {
                await Connect();

                var model = new UserData { 
                    id = user.id,
                    clientGoogleToken = user.clientGoogleToken,
                    clientName = user.clientName,
                    clientEmail = user.clientEmail,
                    clientImageURL = user.clientImageURL,
                    clientAddress = address, 
                    clientPhone = phone,
                    clientType = user.clientType
                };

                await _client.From<UserData>().Update(model); // Update the user information in the database

                // Update the current user information in the session
                AppSession.CurrentUser.clientAddress = address;
                AppSession.CurrentUser.clientPhone = phone;

            } catch (Exception ex) 
            {
                sError = ex.Message;
                return false;
            }
            return true;
        }

        // Check if the user has an address saved in the database
        public async Task<bool> CheckUserAddress(UserData client)
        {
            try
            {
                await Connect();

                var response = await _client.From<UserData>().Select("address").Where(u => u.id == client.id).Get();

                var address = response.Models.FirstOrDefault();

                // Check if the address is not null or empty
                return address != null && !string.IsNullOrWhiteSpace(address.clientAddress); 
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }

        // Get the list of orders for a specific user
        public async Task<List<OrderData>> GetOrders(UserData client)
        {
            try
            {
                await Connect();

                // Get a client id through their email
                var response = await _client.From<UserData>().Where(u => u.clientEmail == client.clientEmail).Get();

                var clientID = response.Models.FirstOrDefault();

                if (clientID == null)
                    return null;

                // Get orders for the client id, ordered by date in descending order
                var result = await _client.From<OrderData>().Where(x => x.userid == clientID.id).Order(x => x.id, Ordering.Descending).Get();

                foreach (var order in result.Models)
                {
                    DateTime local = order.date.ToLocalTime(); // Supabase saves dates in UTC, so convert them to local time for display
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

                var result = await _client.From<OrderDetailsData>().Where(x => x.orderid == order.id).Get();
                return result.Models;

            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }
    }
}
