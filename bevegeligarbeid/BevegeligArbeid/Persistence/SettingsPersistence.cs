// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using BevegeligArbeid.Domain;
using Newtonsoft.Json;
using System;

namespace BevegeligArbeid.Persistence
{
    public class SettingsPersistence
    {
        private readonly int _maxSpeedDefaultValue = 3;
        private readonly int _timeToLogDefaultValue = 5;

        public Settings settings { get; set; }

        public SettingsPersistence()
        {
             settings = new Settings();
            try
            {
                if (App._settingsFileDao.FileExists("settings.json"))
                {
                    settings = JsonConvert.DeserializeObject<Settings>(App._settingsFileDao.ReadFile("settings.json"));
                }
                else
                {
                    settings.maxSpeed = _maxSpeedDefaultValue;
                    settings.timeToLog = _timeToLogDefaultValue;
                    string json = JsonConvert.SerializeObject(settings);
                    App._settingsFileDao.WriteFile("settings.json", json);
                }
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception in SettingsPersistence: " + e.Message);
            }
        }

        public void setMaxSpeed(int newMaxSpeed)
        {
            settings.maxSpeed = newMaxSpeed;
            string json = JsonConvert.SerializeObject(settings);
            App._settingsFileDao.WriteFile("settings.json", json);
        }


        public void setTimeToLog(int newTimeToLog)
        {
            settings.timeToLog = newTimeToLog;
            string json = JsonConvert.SerializeObject(settings);
            App._settingsFileDao.WriteFile("settings.json", json);
        }

    }
}
