// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;
using BevegeligArbeid.Domain;
using BevegeligArbeid.Persistence;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;

namespace BevegeligArbeid.Droid.Persistence
{
    class LogEntryDao : ILogEntryDao
    {
        public bool AttemptUpload(string planPath, string pointPath, string filePath)
        {
            List<string> names = getNamesFromPath(filePath);

            string planId = names[0];
            string pointId = names[1];
            string logEntryId = names[2];
            try
            {
                System.Diagnostics.Debug.WriteLine("Try Attemptupload()" + filePath);
                System.Diagnostics.Debug.WriteLine("/plan/" + planId + "/point/" + pointId + "/log");
                string json= File.ReadAllText(filePath);
                HttpResponseMessage response = App.rc.Send("/plan/" + planId + "/point/" + pointId + "/log", HttpMethod.Put, json);

                 if (response.IsSuccessStatusCode)
                 {
                //App._notificationService.Notify(0, "Try Attemptupload() isSuccessStatusCode:", "" + response.IsSuccessStatusCode);
                     System.Diagnostics.Debug.WriteLine("Deleting file " + filePath);
                     File.Delete(filePath);
                     if (Directory.Exists(pointPath)) Directory.Delete(pointPath);
                     if (Directory.Exists(planPath)) Directory.Delete(planPath);
                     return true;
                 }
                System.Diagnostics.Debug.WriteLine("Deleting folder " + pointPath);
                if (Directory.Exists(pointPath)) Directory.Delete(pointPath);
                System.Diagnostics.Debug.WriteLine("Deleting folder " + planPath);
                if (Directory.Exists(planPath)) Directory.Delete(planPath);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception in AttemptUpload(): " + e.Message);
            }
            return false;
        }

        public void CheckForUnuploadedLogEntries()
        {
            try
            {
            string folderPath = App._fileDao.GetFolderPath();
            string[] plansPath =  Directory.GetDirectories(folderPath);
            foreach (string planPath in plansPath)
            {
                string[] pointsPath = Directory.GetDirectories(planPath);
                if (pointsPath.Length == 0) if (Directory.Exists(planPath)) Directory.Delete(planPath);

                foreach (string pointPath in pointsPath)
                {
                    string[] logEntriesPath = Directory.GetFiles(pointPath);
                    foreach(string logEntryPath in logEntriesPath)
                    {
                            System.Diagnostics.Debug.WriteLine("AttemptUpload() " + planPath + " " + pointPath + " " + logEntryPath);
                            AttemptUpload(planPath, pointPath, logEntryPath);
                    }
                }
            }
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception in CheckForUnuploadedLogEntries(): " + e.Message);
            }
        }

        public List<string> getNamesFromPath(string path)
        {
            string[] paths = path.Split('/');
            List<string> names = new List<string>();
            names.Add(paths[paths.Length - 3]);
            names.Add(paths[paths.Length - 2]);
            names.Add(paths[paths.Length - 1]);
            return names;
        }

        public void SaveLogEntryToCache(string planId, string pointId, LogEntry logEntry)
        {

           string json = JsonConvert.SerializeObject(logEntry);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tmpTime = (DateTime.Now.ToLocalTime() - epoch).TotalMilliseconds.ToString();
            if (tmpTime.Length > 13) tmpTime = tmpTime.Substring(0, 13);
            App._fileDao.WriteFile(planId + "/" + pointId + "/" + tmpTime + ".json", json);
            string filePath = App._fileDao.GetFilePath(planId + "/" + pointId + "/" + tmpTime + ".json");
            //App._notificationService.Notify(0, "File saved", filePath);
        }
    }
}