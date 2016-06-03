// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;
using System.Collections.Generic;
using System.IO;
using BevegeligArbeid.Domain;
using BevegeligArbeid.Persistence;
using Newtonsoft.Json;
using System.Net.Http;

namespace BevegeligArbeid.iOS.Persistence
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
				HttpResponseMessage response = App.rc.Send("/plan/" + planId + "/point/" + pointId + "/log", HttpMethod.Get, null);
				//PUTLOGENN
				//HttpMethod.Put
				if (response.IsSuccessStatusCode)
				{
					App._notificationService.Notify(0, "Try Attemptupload() isSuccessStatusCode:", "" + response.IsSuccessStatusCode);
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
				App._notificationService.Notify(0, "CheckForUnuploadedLogEntries()", "");
				string folderPath = App._fileDao.GetFolderPath();
				string[] plansPath = Directory.GetDirectories(folderPath);
				foreach (string planPath in plansPath)
				{
					string[] pointsPath = Directory.GetDirectories (planPath);
					if (pointsPath.Length == 0) if (Directory.Exists(planPath)) Directory.Delete(planPath);

					foreach (string pointPath in pointsPath)
					{
						string[] logEntriesPath = Directory.GetFiles(pointPath);
						foreach(string logEntryPath in logEntriesPath)
						{
							AttemptUpload(planPath, pointPath, logEntryPath);
							System.Diagnostics.Debug.WriteLine(logEntryPath);
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
			App._fileDao.WriteFile(planId + "/" + pointId + "/" + logEntry.insertion_time + ".json", json);
			string filePath = App._fileDao.GetFilePath(planId + "/" + pointId + "/" + logEntry.insertion_time + ".json");
			App._notificationService.Notify(0, "File saved", filePath);
        }
    }
}
