// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using System.Collections.Generic;

namespace BevegeligArbeid.Persistence
{
    public interface IFileDao
    {
        string ReadFile(string fileName);

        string GetFolderPath();

        void CopyFile(string source, string dest);

        void WriteFile(string fileName, string jsonString);

        void WriteBytes(string fileName, byte[] data);

        bool Exists(string filePath);

        bool FileExists(string fileName);

        bool ResourceExists(string fileName);

        void DeleteFile(string fileName);

        void DeleteFolder(string folderName, string parentFolder = null);

        IEnumerable<string> ListFiles(string searchPattern);

        IEnumerable<string> ListFolders(string searchPattern, string subfolder = null);

        string GetFilePath(string fileName);
    }
}
