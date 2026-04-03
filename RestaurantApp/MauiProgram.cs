using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace RestaurantApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("KleeOne-Regular.ttf", "KleeOneRegular");
                    fonts.AddFont("KleeOne-SemiBold.ttf", "KleeOneSemiBold");
                    fonts.AddFont("ZalandoSansExpanded.ttf", "ZalandoSans");
                    fonts.AddFont("ClimateCrisis-Regular.ttf", "ClimateCrisis");
                    fonts.AddFont("YujiBoku-Regular.ttf", "YujiBoku");
                    fonts.AddFont("RocknRollOne-Regular.ttf", "RocknRollOne");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            return builder.Build();
        }
    }
}
