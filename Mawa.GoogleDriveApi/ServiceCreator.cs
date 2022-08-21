using Mawa.GoogleDriveApi.Configs;

namespace Mawa.GoogleDriveApi
{
    public static class ServiceCreator
    {
        public static Services.IGoogleDriveAPIService CreateNewService(IGoogleDriveApiConfiguration Config)
        {
            return new Services.GoogleDriveAPIService(Config);
        }
    }
}
