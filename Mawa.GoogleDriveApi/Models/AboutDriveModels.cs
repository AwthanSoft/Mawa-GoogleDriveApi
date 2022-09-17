using System.Collections.Generic;

namespace Mawa.GoogleDriveApi.Models
{
    public interface IDriveUser
    {
        string DisplayName { get; }
        string EmailAddress { get; }
        string PermissionId { get; }
        string ETag { get; }
        string Kind { get; }
        bool Me { get; }
        string PhotoLink { get; }
        string Str { get; }
    }
    public class DriveUser : IDriveUser
    {
        public string DisplayName { set; get; }
        public string EmailAddress { set; get; }
        public string PermissionId { set; get; }
        public string ETag { set; get; }
        public string Kind { set; get; }
        public bool Me { set; get; }
        public string PhotoLink { set; get; }
        public string Str { set; get; }
    }


    public interface IDriveStorageQuota
    {
        long Limit { get; }
        long Usage { get; }
        long UsageInDrive { get; }
        long UsageInDriveTrash { get; }
        string Str { get; }
    }
    public class DriveStorageQuota : IDriveStorageQuota
    {
        public long Limit { set; get; }
        public long Usage { set; get; }
        public long UsageInDrive { set; get; }
        public long UsageInDriveTrash { set; get; }
        public string Str { set; get; }
    }


    public interface IDriveAbout : Drives.About.IDriveAboutCore
    {
        bool AppInstalled { get; }
        bool CanCreateDrives { get; }
        bool CanCreateTeamDrives { get; }
        string ETag { get; }
        string Kind { get; }
        IReadOnlyDictionary<string, long> MaxImportSizes { get; }
        long MaxUploadSize { get; }
        IDriveStorageQuota StorageQuota { get; }
        IDriveUser User { get; }
        string Str { get; }
    }
    public class DriveAbout : IDriveAbout
    {
        public bool AppInstalled { set; get; }
        public bool CanCreateDrives { set; get; }
        public bool CanCreateTeamDrives { set; get; }
        public string ETag { set; get; }
        public string Kind { set; get; }
        public IReadOnlyDictionary<string, long> MaxImportSizes { set; get; }
        public long MaxUploadSize { set; get; }
        public DriveStorageQuota StorageQuota { set; get; }
        public DriveUser User { set; get; }
        public string Str { set; get; }

        IDriveStorageQuota IDriveAbout.StorageQuota => this.StorageQuota;
        IDriveUser IDriveAbout.User => this.User;
    }


}
