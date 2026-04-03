using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantApp
{
    public class UserController
    {
        public async Task<bool> saveInfo (UserData user, string address, string phone)
        {
            ConnectUser connectUser = new ConnectUser();
            return await connectUser.SaveInfo(user, address, phone);
        }

        public async Task<bool> checkUserAddress(UserData client)
        {
            ConnectUser connectUser = new ConnectUser();
            return await connectUser.CheckUserAdress(client);
        }
    }
}
