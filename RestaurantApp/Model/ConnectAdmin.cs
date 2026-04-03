using System;
using System.Collections.Generic;
using System.Text;
using static Android.Graphics.ColorSpace;

namespace RestaurantApp
{
    public class ConnectAdmin : ConnectSupabase
    {
        public async Task<bool> SaveMenu(MenuData menu)
        {
            try
            {
                await Connect();

                await _client.From<MenuData>().Insert(menu);
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
            return true;
        }

        public async Task<string> UploadImage(FileResult file)
        {
            try
            {
                await Connect();

                // Convertir FileResult a bytes
                var stream = await file.OpenReadAsync();
                byte[] imageData;
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }

                // Nombre único para evitar colisiones
                string fileName = $"{Guid.NewGuid()}_{file.FileName}";

                // Subir al bucket 'menu-images'
                await _client.Storage.From("menu_images").Upload(imageData, fileName);

                // Obtener la URL pública final
                string publicUrl = _client.Storage.From("menu_images").GetPublicUrl(fileName);

                return publicUrl;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return null;
            }
        }

        public async Task<bool> UpdateMenu(MenuData menu, string ogName)
        {
            try
            {
                await Connect();

                var result = await _client.From<MenuData>().Where(x=>x.Name == ogName).Get();
                var newID = result.Models.FirstOrDefault();

                if(newID != null)
                {
                    menu.id = newID.id;
                }

                await _client.From<MenuData>().Where(x => x.id == menu.id).Update(menu);
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
            return true;
        }
    }
}
