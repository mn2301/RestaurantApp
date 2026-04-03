using System;
using System.Collections.Generic;
using System.Text;
using Android.Gms.Fido.U2F.Api.Common;
using Supabase;

namespace RestaurantApp
{
    public class ConnectUser : ConnectSupabase
    {
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

                await _client.From<UserData>().Update(model);

                AppSession.CurrentUser.clientAddress = address;
                AppSession.CurrentUser.clientPhone = phone;

            } catch (Exception ex) 
            {
                sError = ex.Message;
                return false;
            }
            return true;
        }

        public async Task<bool> CheckUserAdress(UserData client)
        {
            try
            {
                await Connect();

                var response = await _client.From<UserData>().Select(u => new object[] { u.clientAddress }).Where(u => u.id == client.id).Single();
    
                if (response.clientAddress == null)
                    return false;

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
