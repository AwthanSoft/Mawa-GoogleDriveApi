namespace Mawa.GoogleDriveApi.Configs
{
    public interface IGoogleDriveApiConfiguration
    {
        string ApplicationName { get; }
        string[] Scopes { get; }
        string GoogleApiApplicationCredintial_FullPath { get; }
        string GoogleApiToken_SaveFolderPath { get; }

    }
}
