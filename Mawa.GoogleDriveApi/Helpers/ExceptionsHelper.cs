using Mawa.GoogleDriveApi.Exceptions;

namespace Mawa.GoogleDriveApi.Helpers
{
    static class ExceptionsHelper
    {
        #region Exceptions
        public static void Exception_Error_General(string mess = "")
        {
            throw new GoogleDriveApiGeneralException($"Error GoogleDrive :\n\n{mess}");
        }

        public static void Exception_ServicesIsNull()
        {
            throw new GoogleDriveApiGeneralException("There is no GoogleService");
        }
        public static void Exception_Error_EnsureCreate(string mess = "")
        {
            throw new GoogleDriveApiGeneralException($"Error GoogleDrive in EnsureCreate :\n\n{mess}");
        }

        //backup refreshing
        public static void Exception_Error_RefreshingBackup(string mess = "")
        {
            throw new GoogleDriveApiGeneralException($"Error GoogleDrive in Refresh Backup :\n" +
                $"Can't refrsh backup." +
                $"\n\n{mess}");
        }

        #endregion


    }
}
