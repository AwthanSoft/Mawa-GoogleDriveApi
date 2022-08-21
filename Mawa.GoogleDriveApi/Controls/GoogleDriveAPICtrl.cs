using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AppMe.Internet;
using Mawa.GoogleDriveApi.Configs;
using Mawa.Lock;
using Mawa.GoogleDriveApi.Helpers;

namespace Mawa.GoogleDriveApi.Controls
{
    public interface IGoogleDriveAPICtrl
    {

    }

    public class GoogleDriveAPICtrl : AppMe.IDisposableMe.DisposableMeCore, IGoogleDriveAPICtrl
    {
        #region Initial

        readonly IGoogleDriveApiConfiguration Config;

        public string[] Scopes => Config.Scopes;
        public string ApplicationName => Config.ApplicationName;

        private Lazy<DriveService> _service;
        private DriveService Service
        {
            get
            {
                if (_service == null)
                {
                    _service = new Lazy<DriveService>(GetGoogleAPI);
                    return null;
                }
                return _service.Value;
            }
        }

        private readonly ObjectLock objectLock;
        public GoogleDriveAPICtrl(IGoogleDriveApiConfiguration Config)
        {
            this.Config = Config;
            //service = GetGoogleAPI();
            objectLock = new ObjectLock();
            _service = new Lazy<DriveService>(GetGoogleAPI);
        }

        #endregion

        #region Credential

        /// <summary>
        /// Выполняет авторизацию пользователя
        /// </summary>
        /// <returns></returns>
        /// 
        public UserCredential GoogleAuthorize()
        {
            lock(this.objectLock.opening_Lock)
            {
                return _GoogleAuthorize();
            }
        }

        UserCredential _GoogleAuthorize()
        {
            UserCredential credential;
            using (var stream = new FileStream(Config.GoogleApiCredintial_FullPath, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    //GoogleClientSecrets.Load(stream).Secrets,
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    //Scopes,
                    Config.Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(Path.GetDirectoryName(Config.GoogleApiToken_SavedFullPath), true)).Result;

                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }

        public Task<UserCredential> GoogleAuthorizeAsync(CancellationToken taskCancellationToken)
        {
            lock (this.objectLock.opening_Lock)
            {
                return _GoogleAuthorizeAsync(taskCancellationToken);
            }
        }
        Task<UserCredential> _GoogleAuthorizeAsync(CancellationToken taskCancellationToken)
        {
            using (var stream = new FileStream(Config.GoogleApiCredintial_FullPath, FileMode.Open, FileAccess.Read))
            {
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Config.Scopes,
                    "user",
                    (taskCancellationToken == null)? CancellationToken.None: taskCancellationToken,
                    new FileDataStore(Path.GetDirectoryName(Config.GoogleApiToken_SavedFullPath), true));
            }
        }


        /// <summary>
        /// Создает подключение к Google Drive используя API
        /// </summary>
        /// <returns>Возвращает инициализированный объект сервиса</returns>
        private DriveService GetGoogleAPI()
        {
            //try
            //{
            //    // Create Drive API service.
            //    return new DriveService(new BaseClientService.Initializer()
            //    {
            //        HttpClientInitializer = GoogleAuthorize(),
            //        ApplicationName = ApplicationName,
            //    });
            //}
            //catch (Exception ex)
            //{
            //    GoogleDriveApi_Helper.MssgShow_Error_LoadingSettings(ex.ToString());
            //}

            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleAuthorize(),
                ApplicationName = ApplicationName,
            });
        }
        private async Task<DriveService> GetGoogleAPIAsync(CancellationToken taskCancellationToken)
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = await _GoogleAuthorizeAsync(taskCancellationToken),
                ApplicationName = ApplicationName,
            });
        }

        #endregion

        #region Accessing

        public async Task<bool> AccessServiceAsync()
        {
            bool resultt = false;
            if (NetConnectionHelper.IsNetConnected())
            {
                if (Service != null)
                {
                    var ApiKey =  Service.ApiKey;
                    try
                    {
                        if (await GetServiceAboutAsync() != null)
                        {
                            resultt = true;
                        }
                        else
                        {

                        }
                    }
                    catch(Exception ex)
                    {
                        _service = new Lazy<DriveService>(GetGoogleAPI);
                        throw ex;
                    }
                    
                }
                else
                {
                    ExceptionsHelper.Exception_ServicesIsNull();
                }
            }
            else
            {
                NetConnectionHelper.Exception_NetNoConnected();
            }
            return resultt;
        }
        public async Task<bool> AccessServiceAsync(CancellationToken cancellationToken)
        {
            bool resultt = false;
            if (NetConnectionHelper.IsNetConnected())
            {
                if (Service != null)
                {
                    var ApiKey = Service.ApiKey;
                    try
                    {
                        if (await GetServiceAboutAsync(cancellationToken) != null)
                        {
                            resultt = true;
                        }
                        else
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        _service = new Lazy<DriveService>(GetGoogleAPI);
                        throw ex;
                    }

                }
                else
                {
                    ExceptionsHelper.Exception_ServicesIsNull();
                }
            }
            else
            {
                NetConnectionHelper.Exception_NetNoConnected();
            }
            return resultt;
        }

        #endregion


        #region Service Operations Me

        public async Task<Google.Apis.Drive.v3.Data.About> GetServiceAboutAsync(CancellationToken cancellationToken)
        {
            var aboutRequest = Service.About.Get();
            aboutRequest.Fields = "*";//will be heavy for App
            //aboutRequest.Fields = "kind";

            var requestResultt = await aboutRequest.ExecuteAsync(cancellationToken);
            if (requestResultt != null)
            {

            }
            else
            {

            }
            return requestResultt;
        }
        public Task<Google.Apis.Drive.v3.Data.About> GetServiceAboutAsync()
        {
            return GetServiceAboutAsync(CancellationToken.None);
        }


        #endregion


        #region Files Operations Me

        //
        public async Task<Google.Apis.Drive.v3.Data.File[]> Get_Files_InParentAsync(string parentsId, CancellationToken cancellationToken)
        {
            FilesResource.ListRequest listRequest = Service.Files.List();

            listRequest.Q = $"'{parentsId}' in parents and mimeType !=  '{GoogleDriveConsts.Folder_MimeType}' and trashed = false";
            //listRequest.Fields = "nextPageToken, files(id, name, mimeType, trashed)";
            listRequest.Fields = "nextPageToken, files(*)";
            //listRequest.PageSize = 10;

            IList<Google.Apis.Drive.v3.Data.File> folders = (await listRequest.ExecuteAsync(cancellationToken)).Files;
            return folders.ToArray();
        }
        public Task<Google.Apis.Drive.v3.Data.File[]> Get_Files_InParentAsync(string parentsId = "root")
        {
            return Get_Files_InParentAsync(parentsId, CancellationToken.None);
        }


        //
        public async Task<Google.Apis.Drive.v3.Data.File[]> Get_AllFilesAsync(CancellationToken cancellationToken)
        {
            FilesResource.ListRequest listRequest = Service.Files.List();

            listRequest.Q = $"mimeType !=  '{GoogleDriveConsts.Folder_MimeType}' and trashed = false";
            listRequest.Fields = "nextPageToken, files(id, size, name, mimeType, md5Checksum, parents)";
            //listRequest.PageSize = 10;

            IList<Google.Apis.Drive.v3.Data.File> files = (await listRequest.ExecuteAsync(cancellationToken)).Files;
            return files.ToArray();
        }
        public Task<Google.Apis.Drive.v3.Data.File[]> Get_AllFilesAsync()
        {
            return Get_AllFilesAsync(CancellationToken.None);
        }



        /// <summary>
        /// Copy an existing file.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <param name="originFileId">ID of the origin file to copy.</param>
        /// <param name="copyTitle">Title of the copy.</param>
        /// <returns>The copied file, null is returned if an API error occurred</returns>
        public Task<Google.Apis.Drive.v3.Data.File> CopyFileAsync(String originFileId, String copyTitle, string toFolderId, CancellationToken cancellationToken)
        {
            var copiedFile = new Google.Apis.Drive.v3.Data.File();
            copiedFile.Name = copyTitle;
            copiedFile.Parents = new List<string>()
            {
                toFolderId
            };
            return Service.Files.Copy(copiedFile, originFileId).ExecuteAsync(cancellationToken);
        }
        public Task<Google.Apis.Drive.v3.Data.File> CopyFileAsync(String originFileId, String copyTitle, string toFolderId)
        {
            return CopyFileAsync(originFileId, copyTitle, toFolderId, CancellationToken.None);
        }


        //
        public async Task<Google.Apis.Drive.v3.Data.File> UploadLargeFileToDriveAsync(
            string filePact, string mimeType, string parent,
            Func<bool> IsResumeng_Predicate, Action<Google.Apis.Upload.IUploadProgress> ProgressChanged_Action,
            CancellationToken cancellationToken)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                //Name = filePact.Split('\\').Last(),
                Name = Path.GetFileName(filePact),
                MimeType = mimeType,
                Parents = new List<string>() { parent }
            };

            FilesResource.CreateMediaUpload request;

            bool isResumModel = false;
            using (var stream = new System.IO.FileStream(filePact, System.IO.FileMode.Open))
            {
                request = Service.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id, name, size";
                request.ProgressChanged += ProgressChanged_Action;

                //request.ChunkSize = 1024;

                while (true)
                {
                    try
                    {
                        if (isResumModel)
                        {
                            await request.ResumeAsync(cancellationToken);
                            break;
                        }
                        else
                        {
                            isResumModel = true;
                            await request.UploadAsync(cancellationToken);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (IsResumeng_Predicate())
                        {
                            continue;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }

                request.ProgressChanged -= ProgressChanged_Action;
                stream.Close();
            }

            return request.ResponseBody;
        }








        public Google.Apis.Drive.v3.Data.File UploadFileToDrive(
            string filePact, string mimeType, string parent)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = filePact.Split('\\').Last(),
                MimeType = mimeType,
                Parents = new List<string>() { parent }
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.FileStream(filePact, System.IO.FileMode.Open))
            {
                request = Service.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id, name, size";
                request.Upload();
                stream.Close();
            }

            return request.ResponseBody;

            //try
            //{
            //    Console.WriteLine($"\tФайл загружен: {file.Name} \tразмер файла {file.Size / 1024} КБ");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }

        public async Task<Google.Apis.Drive.v3.Data.File> UploadFileToDriveAsync(
           string filePact, string mimeType, string parent)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = filePact.Split('\\').Last(),
                MimeType = mimeType,
                Parents = new List<string>() { parent }
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.FileStream(filePact, System.IO.FileMode.Open))
            {
                request = Service.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id, name, size";
                await request.UploadAsync();
                stream.Close();
            }

            return request.ResponseBody;
        }







        #endregion


        #region Folder operations Me

        /// <summary>
        /// to check is folder ixist in specify folder.
        ///For Quota 
        ///https://developers.google.com/drive/api/v3/search-files
        ///https://medium.com/@SandeepDinesh/copying-google-drive-folders-with-the-google-drive-api-3473f09d4f3d
        ///For Fields
        ///https://developers.google.com/drive/api/v3/reference/files
        /// </summary>
        /// <param name="name">Folder Name that need to check</param>
        /// <param name="parentsId">Parent Folder Id that will searched in</param>
        /// <returns>Return Folder Id if exist or null</returns>
        public async Task<string> IsExist_Folder_InParentAsync(string name, string parentsId, CancellationToken cancellationToken)
        {
            string resultt = null;
            FilesResource.ListRequest listRequest = Service.Files.List();

            //For Quota 
            //https://developers.google.com/drive/api/v3/search-files
            //https://medium.com/@SandeepDinesh/copying-google-drive-folders-with-the-google-drive-api-3473f09d4f3d
            //For Fields
            //https://developers.google.com/drive/api/v3/reference/files
            //`'${from.id}' in parents and mimeType =  'application/vnd.google-apps.folder' and trashed = false`
            //listRequest.Q = $"'${parentsId}' in parents and mimeType =  'application/vnd.google-apps.folder' and trashed = false";
            listRequest.Q = $"'{parentsId}' in parents and mimeType = '{GoogleDriveConsts.Folder_MimeType}' and trashed = false";
            listRequest.Fields = "nextPageToken, files(id, name)";
            //listRequest.PageSize = 10;
            //try
            {
                IList<Google.Apis.Drive.v3.Data.File> folders = (await listRequest.ExecuteAsync(cancellationToken)).Files;
                var folder = folders.Where(b => b.Name.Equals(name)).FirstOrDefault();
                if (folder != null)
                {
                    resultt = folder.Id;
                }
            }

            return resultt;
        }
        public Task<string> IsExist_Folder_InParentAsync(string name, string parentsId = "root")
        {
            return IsExist_Folder_InParentAsync(name, parentsId, CancellationToken.None);
        }


        /// <summary>
        /// Create Folder in specify directory.
        /// </summary>
        /// <param name="name">new folder name</param>
        /// <param name="parentsId">parent folder id</param>
        /// <returns>new folder id</returns>
        public string CreateFolder(string FolderName, string parentsId = "root")
        {
            var fileMet = new Google.Apis.Drive.v3.Data.File()
            {
                Name = FolderName,
                MimeType = GoogleDriveConsts.Folder_MimeType, //"application/vnd.google-apps.folder",
                Parents = new List<string>() { parentsId },
            };

            var request = Service.Files.Create(fileMet);
            request.Fields = "id, name";
            var file = request.Execute();
            return file.Id;
        }
        /// <summary>
        /// Create Folder in specify directory.
        /// </summary>
        /// <param name="FolderName">new folder name</param>
        /// <param name="parentsId">parent folder id</param>
        /// <returns>new folder id</returns>
        public async Task<string> CreateFolderAsync(string FolderName, string parentsId, CancellationToken cancellationToken)
        {
            var fileMet = new Google.Apis.Drive.v3.Data.File()
            {
                Name = FolderName,
                MimeType = GoogleDriveConsts.Folder_MimeType, //"application/vnd.google-apps.folder",
                Parents = new List<string>() { parentsId },
            };

            var request = Service.Files.Create(fileMet);
            request.Fields = "id, name";
            var file = await request.ExecuteAsync(cancellationToken);
            return file.Id;
        }


        /// <summary>
        /// Get all folder in specify directory.
        /// </summary>
        /// <param name="parentsId">Parent FolderId that its folder will be returned.</param>
        /// <returns>List of GoogleDriveFilesInfo</returns>
        public async Task<Google.Apis.Drive.v3.Data.File[]> Get_Folders_InParentAsync(string parentsId, CancellationToken cancellationToken)
        {
            FilesResource.ListRequest listRequest = Service.Files.List();

            //`'${from.id}' in parents and mimeType =  'application/vnd.google-apps.folder' and trashed = false`
            listRequest.Q = $"'{parentsId}' in parents and mimeType = '{GoogleDriveConsts.Folder_MimeType}' and trashed = false";
            //listRequest.Fields = "nextPageToken, files(id, name, mimeType, trashed)";
            listRequest.Fields = "nextPageToken, files(*)";
            //listRequest.PageSize = 10;

            IList<Google.Apis.Drive.v3.Data.File> folders = (await listRequest.ExecuteAsync(cancellationToken)).Files;
            return folders.ToArray();
        }
        public Task<Google.Apis.Drive.v3.Data.File[]> Get_Folders_InParentAsync(string parentsId = "root")
        {
            return Get_Folders_InParentAsync(parentsId, CancellationToken.None);
        }


        public Task<Google.Apis.Drive.v3.Data.File> GetFolder(string FolderId, CancellationToken cancellationToken)
        {
            FilesResource.GetRequest request = Service.Files.Get(FolderId);
            request.Fields = "*";
            return request.ExecuteAsync(cancellationToken);
        }





        //public async Task<Google.Apis.Drive.v3.Data.File[]> Update_FolderAsync(string parentsId = "root")
        //{
        //    var listRequest = Service.Files.Update();

        //    //`'${from.id}' in parents and mimeType =  'application/vnd.google-apps.folder' and trashed = false`
        //    listRequest.Q = $"'{parentsId}' in parents and mimeType =  '{Folder_MimeType}' and trashed = false";
        //    //listRequest.Fields = "nextPageToken, files(id, name, mimeType, trashed)";
        //    listRequest.Fields = "nextPageToken, files(*)";
        //    //listRequest.PageSize = 10;

        //    IList<Google.Apis.Drive.v3.Data.File> folders = (await listRequest.ExecuteAsync()).Files;
        //    return folders.ToArray();
        //}



        #endregion


        #region Oprignal Operations

        /// <summary>
        /// Получает список имен файлов загруженных на Google Drive
        /// </summary>
        public void GetFilseGoogleDrive()
        {
            FilesResource.ListRequest listRequest = Service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";
            //listRequest.PageSize = 10;

            //Список файлов полученных после запроса на сервер.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;

            Console.WriteLine("Files:");

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
            }
            else
                Console.WriteLine("No files found.");
        }

        /// <summary>
        /// Создает папку на Google Drive
        /// </summary>
        /// <param name="name">Название папки</param>
        /// <param name="parentsId">
        /// Id родительской папки.
        /// По умолчанию значение равно корневой папке на Google Drive 
        /// </param>
        /// <returns>Возвращает Id созданной папки</returns>
        public string CreateDirectoryDrive(string name, string parentsId = "root")
        {
            var fileMet = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { parentsId },


            };

            var request = Service.Files.Create(fileMet);
            request.Fields = "id, name";

            var file = request.Execute();

            //Console.WriteLine($"Папка: {file.Name}");

            return file.Id;
        }

        /// <summary>
        /// Асинхронная версия метода CreateDirectoryDrive
        /// </summary>
        public async Task<string> CreateDirectoryDriveAsync(string name, string parentsId = "root")
        {
            return await System.Threading.Tasks.Task.Run(() => CreateDirectoryDrive(name, parentsId));
        }

        /// <summary>
        /// Загружает файл на Google Drive
        /// </summary>
        /// <param name="filePact">Путь к файлу</param>
        /// <param name="mimeType">Тип файла</param>
        /// <param name="parent">Родительская папка</param>
        public void UploadFileGoogleDrive(string filePact, string mimeType, string parent)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = filePact.Split('\\').Last(),
                MimeType = mimeType,
                Parents = new List<string>() { parent }
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.FileStream(filePact, System.IO.FileMode.Open))
            {
                request = Service.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id, name, size";
                request.Upload();
            }

            try
            {
                var file = request.ResponseBody;
                Console.WriteLine($"\tФайл загружен: {file.Name} \tразмер файла {file.Size / 1024} КБ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Асинхронная версия метода UploadFileGoogleDrive
        /// </summary>
        public async System.Threading.Tasks.Task UploadFileGoogleDriveAsync(string filePact, string mimeType, string parent)
        {
            await System.Threading.Tasks.Task.Run(() => UploadFileGoogleDrive(filePact, mimeType, parent));
        }

        /// <summary>
        /// Загружает папку на Google Drive 
        /// </summary>
        /// <param name="pahtDir">Путь к папке</param>
        /// <param name="parent">Id родительской папки</param>
        public async System.Threading.Tasks.Task UploadDirectoryDriveAsync(string pahtDir, string parent = "root")
        {
            string nameDir = pahtDir.Split('\\').Last();

            string idDir = await this.CreateDirectoryDriveAsync(nameDir, parent); ///! await

            FileInfo[] files = null;
            DirectoryInfo[] directories = null;

            try
            {
                files = new DirectoryInfo(pahtDir).GetFiles();
                directories = new DirectoryInfo(pahtDir).GetDirectories();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (files != null)
            {
                foreach (var item in files) ///! await
                    //await this.UploadFileGoogleDriveAsync(item.FullName, MimeMapping.GetMimeMapping(item.FullName), idDir);
                    await this.UploadFileGoogleDriveAsync(item.FullName, AppMe.MimeTypes.MimeTypeMap.GetMimeType(item.FullName), idDir);

                foreach (var item in directories)
                    await UploadDirectoryDriveAsync(item.FullName, idDir);
            }
        }


        #endregion

        #region Dispose

        protected override void Dispose_OnFreeOtherManaged()
        {

        }

        protected override void Dispose_OnFreeUnManaged()
        {
            //throw new NotImplementedException();
            if(_service.IsValueCreated)
                Service.Dispose();
            else
            {
                _service = null;
            }
        }

        ~GoogleDriveAPICtrl()
        {
            Dispose(false);
        }

        #endregion
    }

}
