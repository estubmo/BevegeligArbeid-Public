// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using System.Collections.Generic;
using System.Linq;
using BevegeligArbeid.Persistence;
using System.Threading;
using System.IO;

namespace BevegeligArbeid.Droid.Persistence
{
    class FileDao : IFileDao
    {
        private static readonly ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim();
        public string FolderPath { get; protected set; }

        public FileDao(string folderPath)
        {
            this.FolderPath = folderPath;
        }

        public string GetFolderPath()
        {
            return FolderPath;
        }
        public void CopyFile(string source, string dest)
        {
            var sourcePath = Path.Combine(this.FolderPath, source);
            var destPath = Path.Combine(this.FolderPath, dest);
            File.Copy(sourcePath, destPath, true);
        }

        public string ReadFile(string fileName)
        {
            var filePath = Path.Combine(this.FolderPath, fileName);
            RwLock.EnterReadLock();
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }
            finally
            {
                RwLock.ExitReadLock();
            }
            return string.Empty;
        }

        public void WriteFile(string fileName, string jsonString)
        {
            var filePath = Path.Combine(this.FolderPath, fileName);
            var dir = Path.GetDirectoryName(filePath);
            if (dir == null)
            {
                return;
            }
            RwLock.EnterWriteLock();
            try
            {
                Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, jsonString);
            }
            finally
            {
                RwLock.ExitWriteLock();
            }
        }

        public void WriteBytes(string fileName, byte[] data)
        {
            var filePath = Path.Combine(this.FolderPath, fileName);
            var dir = Path.GetDirectoryName(filePath);
            if (dir == null)
            {
                return;
            }
            RwLock.EnterWriteLock();
            try
            {
                Directory.CreateDirectory(dir);
                File.WriteAllBytes(filePath, data);
            }
            finally
            {
                RwLock.ExitWriteLock();
            }
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void DeleteFile(string fileName)
        {
            var filePath = Path.Combine(this.FolderPath, fileName);
            if (File.Exists(filePath))
            {
                RwLock.EnterWriteLock();
                try
                {
                    File.Delete(filePath);
                }
                finally
                {
                    RwLock.ExitWriteLock();
                }
            }
        }

        public void DeleteFolder(string folderName, string parentFolder = null)
        {
            var folder = (parentFolder == null) ? this.FolderPath : Path.Combine(this.FolderPath, parentFolder);
            var folderPath = Path.Combine(folder, folderName);
            if (Directory.Exists(folderPath))
            {
                RwLock.EnterWriteLock();
                try
                {
                    Directory.Delete(folderPath, true);
                }
                finally
                {
                    RwLock.ExitWriteLock();
                }
            }
        }

        public bool FileExists(string fileName)
        {
            var filePath = Path.Combine(this.FolderPath, fileName);
            return File.Exists(filePath);
        }

        public bool ResourceExists(string resource)
        {
            var drawType = typeof(Resource.Drawable);
            var members = drawType.GetMembers();

            return members.Any(o => o.Name.Equals(resource));
        }

        public IEnumerable<string> ListFiles(string searchPattern)
        {
            var list = Directory.EnumerateFiles(this.FolderPath, searchPattern);
            return list.Select(Path.GetFileName).ToList();
        }

        public IEnumerable<string> ListFolders(string searchPattern, string subfolder = null)
        {
            var folder = (subfolder == null) ? this.FolderPath : Path.Combine(this.FolderPath, subfolder);

            var list = Directory.EnumerateDirectories(folder, searchPattern);
            return list.Select(Path.GetFileName).ToList();
        }

        public string GetFilePath(string fileName)
        {
            return Path.Combine(this.FolderPath, fileName);
        }

    }
}