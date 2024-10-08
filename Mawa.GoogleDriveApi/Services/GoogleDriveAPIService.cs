﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Mawa.Drives.Files;
using Mawa.GoogleDriveApi.Configs;
using Mawa.GoogleDriveApi.Controls;
//using DriveAbout = Google.Apis.Drive.v3.Data.About;
using Mawa.GoogleDriveApi.Exceptions;
using Mawa.GoogleDriveApi.Models;
using Mawa.Lock;

using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace Mawa.GoogleDriveApi.Services
{
    public interface IGoogleDriveAPIService : IDisposable, Drives.APIs.IDriveApiService<DriveAbout>
    {
        #region Files

        //
        Task<DriveFile[]> GetFiles_InParentAsync(string parentsId, CancellationToken cancellationToken);
        Task<DriveFile[]> GetFiles_InParentAsync(string parentsId);


        /// <summary>
        /// Copy an existing file.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <param name="originFileId">ID of the origin file to copy.</param>
        /// <param name="copyTitle">Title of the copy.</param>
        /// <returns>The copied file, null is returned if an API error occurred</returns>
        Task<DriveFile> CopyFileAsync(String originFileId, String copyTitle, string toFolderId, CancellationToken cancellationToken);


        Task<DriveFile> UploadLargeFileToDriveAsync(string filePact, string mimeType, string parent,
            Func<bool> IsResumeng_Predicate,
            Action<Google.Apis.Upload.IUploadProgress> ProgressChanged_Action,
            CancellationToken cancellationToken);


        #endregion

        #region Folders

        Task<DriveFile[]> GetFolders_InParentAsync(string parentsId, CancellationToken cancellationToken);
        Task<DriveFile[]> GetFolders_InParentAsync(string parentsId);

        #endregion

        #region About Drive

        //Task<IDriveAbout> GetDriveAboutAsync(CancellationToken cancellationToken);
        #endregion

    }

    class GoogleDriveAPIService : AppMe.IDisposableMe.DisposableMeCore, IGoogleDriveAPIService
    {
        #region Initial

        readonly IGoogleDriveApiConfiguration Config;

        private readonly ObjectLock objectLock;
        public GoogleDriveAPIService(IGoogleDriveApiConfiguration Config)
        {
            this.Config = Config;
            this.objectLock = new ObjectLock();

            pre_initial();
        }

        private void pre_initial()
        {
            pre_initial_DriveApiService();

        }


        #endregion

        #region DriveApiService

        private Lazy<GoogleDriveAPICtrl> __apiCtrl;
        private GoogleDriveAPICtrl _apiCtrl => __apiCtrl.Value;
        private void pre_initial_DriveApiService()
        {
            __apiCtrl = new Lazy<GoogleDriveAPICtrl>(() => new GoogleDriveAPICtrl(Config));
        }

        #endregion

        #region Folders 

        //
        public Task<string> IsFolderExist_InParentAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            //try
            //{
            //    objectLock.open_lock();
            //}
            //catch (Exception)
            //{
            //    objectLock.close_lock();
            //    throw;
            //}
            //objectLock.close_lock();
            return _IsFolderExist_InParentAsync(FolderName, parentsId, cancellationToken);
        }
        public Task<string> IsFolderExist_InParentAsync(string FolderName, string parentsId)
        {
            return _IsFolderExist_InParentAsync(FolderName, parentsId, CancellationToken.None);
        }
        Task<string> _IsFolderExist_InParentAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            return _apiCtrl.IsExist_Folder_InParentAsync(FolderName, parentsId, cancellationToken);
        }


        //
        public Task<string> CreateFolderAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            return _CreateFolderAsync(FolderName, parentsId, cancellationToken);
        }
        public Task<string> CreateFolderAsync(string FolderName, string parentsId)
        {
            return _CreateFolderAsync(FolderName, parentsId, CancellationToken.None);
        }
        Task<string> _CreateFolderAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            return _apiCtrl.CreateFolderAsync(FolderName, parentsId, cancellationToken);
        }


        //
        public Task<DriveFile[]> GetFolders_InParentAsync(string parentsId, CancellationToken cancellationToken)
        {
            return _GetFolders_InParentAsync(parentsId, cancellationToken);
        }
        public Task<DriveFile[]> GetFolders_InParentAsync(string parentsId)
        {
            return _GetFolders_InParentAsync(parentsId, CancellationToken.None);
        }
        Task<DriveFile[]> _GetFolders_InParentAsync(string parentsId, CancellationToken cancellationToken)
        {
            return _apiCtrl.Get_Folders_InParentAsync(parentsId, cancellationToken);
        }

        #endregion

        #region Files

        //
        public Task<DriveFile[]> GetFiles_InParentAsync(string parentsId, CancellationToken cancellationToken)
        {
            return _GetFiles_InParentAsync(parentsId, cancellationToken);
        }
        public Task<DriveFile[]> GetFiles_InParentAsync(string parentsId)
        {
            return _GetFiles_InParentAsync(parentsId, CancellationToken.None);
        }
        Task<DriveFile[]> _GetFiles_InParentAsync(string parentsId, CancellationToken cancellationToken)
        {
            return _apiCtrl.Get_Files_InParentAsync(parentsId, cancellationToken);
        }


        //
        public Task<DriveFile> CopyFileAsync(String originFileId, String copyTitle, string toFolderId, CancellationToken cancellationToken)
        {
            return _CopyFileAsync(originFileId, copyTitle, toFolderId, cancellationToken);
        }
        Task<DriveFile> _CopyFileAsync(String originFileId, String copyTitle, string toFolderId, CancellationToken cancellationToken)
        {
            return _apiCtrl.CopyFileAsync(originFileId, copyTitle, toFolderId, cancellationToken);
        }


        //
        public Task<DriveFile> UploadLargeFileToDriveAsync(string filePact, string mimeType, string parent,
         Func<bool> IsResumeng_Predicate,
         Action<Google.Apis.Upload.IUploadProgress> ProgressChanged_Action,
         CancellationToken cancellationToken)
        {
            return _UploadLargeFileToDriveAsync(filePact, mimeType, parent, IsResumeng_Predicate, ProgressChanged_Action, cancellationToken);
        }
        Task<DriveFile> _UploadLargeFileToDriveAsync(string filePact, string mimeType, string parent,
            Func<bool> IsResumeng_Predicate,
            Action<Google.Apis.Upload.IUploadProgress> ProgressChanged_Action,
            CancellationToken cancellationToken)
        {
            return _apiCtrl.UploadLargeFileToDriveAsync(filePact, mimeType, parent, IsResumeng_Predicate, ProgressChanged_Action, cancellationToken);

        }


        #endregion

        #region Drive (Folders & Files) Helper

        //OneFolder 
        public Task<FolderDriveStructFile[]> GetFolders_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            //try
            //{
            //    objectLock.open_lock();
            //}
            //catch (Exception)
            //{
            //    objectLock.close_lock();
            //    throw;
            //}
            //objectLock.close_lock();
            return _GetFolders_inFolderAsync(folderId, cancellationToken);
        }
        async Task<FolderDriveStructFile[]> _GetFolders_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            var temp_list = new List<FolderDriveStructFile>();
            var RootFolders = await _apiCtrl.Get_Folders_InParentAsync(folderId, cancellationToken);
            foreach (var file in RootFolders)
            {
                var sub = new FolderDriveStructFile()
                {
                    DriveFileId = file.Id,
                    ParentDriveId = folderId,
                    Name = file.Name,
                    DriveSize = (file.Size != null) ? file.Size.Value : 0,
                    MimeType = file.MimeType,
                    IsInDrive = true

                };
                temp_list.Add(sub);
            }
            return temp_list.ToArray();
        }
        //Recursion
        public Task<FolderDriveStructFile[]> GetFolders_inFolder_WithSubsAsync(string folderId, CancellationToken cancellationToken)
        {
            return _GetFolders_inFolder_WithSubsAsync(folderId, cancellationToken);
        }
        async Task<FolderDriveStructFile[]> _GetFolders_inFolder_WithSubsAsync(string folderId, CancellationToken cancellationToken)
        {
            var RootFolders = await _GetFolders_inFolderAsync(folderId, cancellationToken);
            foreach (var folder in RootFolders)
            {

                folder.Folders.AddRange(await _GetFolders_inFolder_WithSubsAsync(folder.DriveFileId, cancellationToken));
            }
            return RootFolders;
        }


        //GetFiles in Folder
        public Task<FileDriveStructFile[]> GetFiles_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            return _GetFiles_inFolderAsync(folderId, cancellationToken);
        }
        public Task<FileDriveStructFile[]> GetFiles_inFolderAsync(string folderId)
        {
            return _GetFiles_inFolderAsync(folderId, CancellationToken.None);
        }
        async Task<FileDriveStructFile[]> _GetFiles_inFolderAsync(string folderId, CancellationToken cancellationToken)
        {
            var temp_list = new List<FileDriveStructFile>();
            var dFils = await _apiCtrl.Get_Files_InParentAsync(folderId, cancellationToken);
            if (dFils.Length > 0)
            {
                temp_list.AddRange(
                    dFils.Select(b => new FileDriveStructFile()
                    {
                        DriveFileId = b.Id,
                        ParentDriveId = folderId,
                        Name = b.Name,
                        MimeType = b.MimeType,
                        DriveSize = (b.Size != null) ? b.Size.Value : 0,
                        Md5Checksum = b.Md5Checksum,
                        IsInDrive = true
                    })
                    .ToArray());
            }

            return temp_list.ToArray();
        }


        //
        public Task<FileDriveStructFile[]> GetAllExistFilesInDriveAsync()
        {
            return _GetAllExistFilesInDriveAsync(CancellationToken.None);
        }
        public Task<FileDriveStructFile[]> GetAllExistFilesInDriveAsync(CancellationToken cancellationToken)
        {
            return _GetAllExistFilesInDriveAsync(cancellationToken);
        }
        async Task<FileDriveStructFile[]> _GetAllExistFilesInDriveAsync(CancellationToken cancellationToken)
        {
            var temp_list = new List<FileDriveStructFile>();
            var driveFiles = await _apiCtrl.Get_AllFilesAsync(cancellationToken);

            if (driveFiles == null)
                throw new ArgumentException();

            foreach (var fil in driveFiles)
            {
                temp_list.Add(new FileDriveStructFile
                {
                    IsInDrive = true,
                    DriveFileId = fil.Id,
                    DriveSize = (fil.Size != null) ? fil.Size.Value : 0,
                    Name = fil.Name,
                    Md5Checksum = fil.Md5Checksum,
                    MimeType = fil.MimeType,
                    ParentDriveId = (fil.Parents.Count > 0) ? fil.Parents[0] : null
                });
            }
            return temp_list.ToArray();
        }


        //
        public Task<FolderDriveStructFile> LoadFolder_asStructAsync(string folderId, bool withSubFolder, bool withFiles, CancellationToken cancellationToken)
        {
            return _LoadFolder_asStructAsync(folderId, withSubFolder, withFiles, cancellationToken);
        }
        async Task<FolderDriveStructFile> _LoadFolder_asStructAsync(string folderId, bool withSubFolder, bool withFiles, CancellationToken cancellationToken)
        {
            var driveFile = await _apiCtrl.GetFolder(folderId, cancellationToken);
            if (driveFile == null)
                throw new GoogleDriveApiGeneralException($"Folder({folderId}) not exist in drive.")
                {
                    ExceptionType = "FolderExisting"
                };

            var rootStruct = new FolderDriveStructFile()
            {
                DriveFileId = driveFile.Id,
                ParentDriveId = driveFile.Parents.FirstOrDefault(),
                Name = driveFile.Name,
                DriveSize = (driveFile.Size != null) ? driveFile.Size.Value : 0,
                MimeType = driveFile.MimeType,
                IsInDrive = true
            };

            if (withSubFolder)
            {
                var folders = await _GetFolders_inFolderAsync(folderId, cancellationToken);
                rootStruct.Folders.AddRange(folders);
                foreach (var fldr in folders)
                {
                    var tempFldrs = await _GetFolders_inFolder_WithSubsAsync(fldr.DriveFileId, cancellationToken);
                    fldr.Folders.AddRange(tempFldrs);
                };
            }

            if (withFiles)
            {
                var folders = new FolderDriveStructFile[] { rootStruct }.AllSubFolderStructs(true);
                foreach (var fldr in folders)
                {
                    var files = await _GetFiles_inFolderAsync(fldr.DriveFileId, cancellationToken);
                    fldr.Files.AddRange(files);
                }
            }

            return rootStruct;
        }

        #endregion

        #region Accessing

        public Task<bool> AccessServiceAsync()
        {
            return _AccessServiceAsync(CancellationToken.None);
        }
        public Task<bool> AccessServiceAsync(CancellationToken cancellationToken)
        {
            return _AccessServiceAsync(cancellationToken);
        }
        Task<bool> _AccessServiceAsync(CancellationToken cancellationToken)
        {
            return _apiCtrl.AccessServiceAsync(cancellationToken);
        }

        #endregion

        #region About Drive

        public Task<DriveAbout> GetDriveAboutAsync(CancellationToken cancellationToken)
        {
            return _GetDriveAboutAsync(cancellationToken);
        }
        async Task<DriveAbout> _GetDriveAboutAsync(CancellationToken cancellationToken)
        {
            var tt = await _apiCtrl.GetServiceAboutAsync(cancellationToken);

            return new DriveAbout()
            {
                User = new DriveUser()
                {
                    DisplayName = tt.User.DisplayName,
                    EmailAddress = tt.User.EmailAddress,
                    PermissionId = tt.User.PermissionId,
                    ETag = tt.User.ETag,
                    Kind = tt.User.Kind,
                    Me = (tt.User.Me != null) ? tt.User.Me.Value : false,
                    PhotoLink = tt.User.PhotoLink,
                    Str = tt.User.ToString()
                },
                StorageQuota = new DriveStorageQuota()
                {
                    UsageInDrive = tt.StorageQuota.UsageInDrive ?? 0,
                    UsageInDriveTrash = tt.StorageQuota.UsageInDriveTrash ?? 0,
                    Limit = tt.StorageQuota.Limit ?? 0,
                    Usage = tt.StorageQuota.Usage ?? 0,
                    Str = tt.StorageQuota.ToString(),
                },
                AppInstalled = tt.AppInstalled ?? false,
                ETag = tt.ETag,
                CanCreateDrives = tt.CanCreateDrives ?? false,
                CanCreateTeamDrives = tt.CanCreateTeamDrives ?? false,
                Kind = tt.Kind,
                Str = tt.ToString(),
                MaxUploadSize = tt.MaxUploadSize ?? 0,
                MaxImportSizes = tt.MaxImportSizes.ToDictionary(b => b.Key, b => b.Value ?? 0)
            };
        }


        #endregion

        #region Dispose

        protected override void Dispose_OnFreeOtherManaged()
        {
        }

        protected override void Dispose_OnFreeUnManaged()
        {
            if (__apiCtrl != null)
            {
                if (__apiCtrl.IsValueCreated)
                {
                    _apiCtrl.Dispose();
                }
                __apiCtrl = null;
            }
        }

        ~GoogleDriveAPIService()
        {
            Dispose(false);
        }

        #endregion
    }

}
