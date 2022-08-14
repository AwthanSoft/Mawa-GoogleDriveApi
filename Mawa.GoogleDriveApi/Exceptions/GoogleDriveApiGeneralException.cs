using Mawa.ExceptionsApp.Core;
using System;
using System.Text;

namespace Mawa.GoogleDriveApi.Exceptions
{
    public class GoogleDriveApiGeneralException : AppExceptionCore
    {

        public GoogleDriveApiGeneralException()
        {

        }
        public GoogleDriveApiGeneralException(string message) : base(message)
        {

        }
        public GoogleDriveApiGeneralException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public override string AppName => "Google Drive Api";
    }
}
