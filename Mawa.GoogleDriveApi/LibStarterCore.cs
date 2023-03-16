using Mawa.DependencyInjection.Controls;
using Mawa.GoogleDriveApi.Services;

namespace Mawa.GoogleDriveApi
{
    public static class LibStarterCore
    {
        #region Hosting
        public static void ConfigureServices(ICollectionServicesControl CollectionServicesCtrl)
        {
            CollectionServicesCtrl.AddSingleton<IGoogleDriveAPIService, GoogleDriveAPIService>();
        }
        #endregion
    }
}
