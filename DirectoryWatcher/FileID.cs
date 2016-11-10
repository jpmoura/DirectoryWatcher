using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

class FileID
{
    struct BY_HANDLE_FILE_INFORMATION
    {
        public uint FileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

    public static string getFileUniqueSystemID(string fileName)
    {
        bool fileRead = false;
        string fileId = "-1";

        while (!fileRead)
        {
            try
            {
                FileStream strem = File.Open(fileName, FileMode.Open);
                BY_HANDLE_FILE_INFORMATION hInfo = new BY_HANDLE_FILE_INFORMATION();
                GetFileInformationByHandle(strem.SafeFileHandle, out hInfo);
                fileId = hInfo.FileIndexHigh.ToString() + hInfo.FileIndexLow.ToString() + "|" + hInfo.VolumeSerialNumber.ToString();
                strem.Close();
                fileRead = true;
            }
            catch (IOException e)
            {
                if (e is FileNotFoundException || e is PathTooLongException) fileRead = true;
                else Thread.Sleep(100);
            }
            catch (System.UnauthorizedAccessException)
            {
                fileRead = true;
            }
        }

        return fileId;
    }
}