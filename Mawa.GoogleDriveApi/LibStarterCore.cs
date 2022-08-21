using Mawa.GoogleDriveApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mawa.GoogleDriveApi
{
    public static class LibStarterCore
    {
        #region Hosting
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGoogleDriveAPIService, GoogleDriveAPIService>();
        }
        #endregion
    }
}
