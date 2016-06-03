// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using BevegeligArbeid.Domain;

namespace BevegeligArbeid.Persistence
{
    public interface ILogEntryDao
    {

        void CheckForUnuploadedLogEntries();

        bool AttemptUpload(string planPath, string pointPath, string filePath);

        void SaveLogEntryToCache(string planId, string pointId, LogEntry logEntry);

    }
}
