using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.GoogleDriveApi.Configs
{
    public interface IGoogleDriveApiConfiguration
    {
        string ApplicationName { get; }
        string[] Scopes { get; }
        string GoogleApiCredintial_FullPath { get; }
        string GoogleApiToken_SavedFullPath { get; }

    }
    //public class GoogleDriveApiConfiguration : IGoogleDriveApiConfiguration
    //{
    //    //
    //    readonly string _ApplicationName;
    //    public string ApplicationName => _ApplicationName;

    //    //
    //   readonly string[] _Scopes = { 
    //        //DriveService.Scope.Drive, 
    //        //DriveService.Scope.DriveReadonly,
    //        DriveService.Scope.DriveFile,};
    //    public string[] Scopes => _Scopes;

    //    //
    //    readonly string _GoogleApiCredintial_FullPath;
    //    public string GoogleApiCredintial_FullPath => _GoogleApiCredintial_FullPath;

    //    //
    //    readonly string _GoogleApiToken_SavedFullPath;
    //    public string GoogleApiToken_SavedFullPath => _GoogleApiToken_SavedFullPath;
        

    //}
}
