using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantApp
{
    public class AdminController
    {
        ConnectAdmin connectAdmin = new ConnectAdmin();
        public async Task<bool> saveMenu(MenuData menu)
        {
            return await connectAdmin.SaveMenu(menu);
        }

        public async Task<string> saveImage(FileResult file)
        {
            return await connectAdmin.UploadImage(file);
        }

        public async Task<bool> updateMenu(MenuData menu, string ogName)
        {
            return await connectAdmin.UpdateMenu(menu, ogName);
        }
    }
}
