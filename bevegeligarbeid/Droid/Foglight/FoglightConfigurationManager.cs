// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace Arbeidsvarsling.Droid.Foglight
{
    using System;
    using System.Collections.Generic;

    using BevegeligArbeid.Foglight;
    using BevegeligArbeid.Persistence;

    using global::Foglight.Domain;
    using global::Foglight.Filtering;

    using Newtonsoft.Json;
    using BevegeligArbeid.Helpers;
    public class FoglightConfigurationManager : NotificationObject, IConfigurationManager
    {

        private Dictionary<String, String> data;

        private readonly IFileDao fileDao;

        private readonly string configPath;

        public FoglightConfigurationManager(IFileDao fileDao, string configPath)
        {
            this.fileDao = fileDao;
            this.configPath = configPath;
            this.LoadConfig();
        }

        public void Dispose()
        {
            if (this.data != null)
            {
                this.data.Clear();
            }
            this.data = null;
        }

        public string GetString(string key)
        {
            if (this.data.ContainsKey(key))
            {
                return this.data[key];
            }
            return null;
        }

        public void SetString(string key, string value)
        {
            if (this.data.ContainsKey(key))
            {
                this.data[key] = value;
            }
            else
            {
                this.data.Add(key, value);
            }
            this.SaveConfig();
            this.RaisePropertyChanged(() => key);
        }


        public bool GetBool(string key)
        {
            bool result = false;
            if (this.data.ContainsKey(key))
            {
                Boolean.TryParse(this.data[key], out result);
            }
            return result;
        }

        public void SetBool(string key, bool value)
        {
            this.SetString(key, value.ToString());
        }


        public int GetInt(string key)
        {
            int result = -1;
            if (this.data.ContainsKey(key))
            {
                Int32.TryParse(this.data[key], out result);
            }
            return result;
        }

        public void SetInt(string key, int value)
        {
            this.SetString(key, value.ToString());
        }

        public double GetDouble(string key)
        {
            double result = 0;
            if (this.data.ContainsKey(key))
            {
                Double.TryParse(this.data[key], out result);
            }
            return result;
        }

        public void SetDouble(string key, double value)
        {
            this.SetString(key, value.ToString());
        }

        public void Sync(CarAndPedestrianRoadFilter filter)
        {
            filter.IncludeCategory(RoadCategory.E, this.GetBool(ConfigKey.PREFERENCE_SHOW_E));
            filter.IncludeCategory(RoadCategory.R, this.GetBool(ConfigKey.PREFERENCE_SHOW_R));
            filter.IncludeCategory(RoadCategory.F, this.GetBool(ConfigKey.PREFERENCE_SHOW_F));
            filter.IncludeCategory(RoadCategory.K, this.GetBool(ConfigKey.PREFERENCE_SHOW_K));
            filter.IncludeCategory(RoadCategory.P, this.GetBool(ConfigKey.PREFERENCE_SHOW_P));
            filter.IncludePedestrianRoads = this.GetBool(ConfigKey.PREFERENCE_SHOW_PEDESTRIAN_ROADS);
        }

        private void SaveConfig()
        {
            string jsonString = JsonConvert.SerializeObject(this.data);
            //write string to file
            this.fileDao.WriteFile(this.configPath, jsonString);
        }

        private void LoadConfig()
        {
            string jsonString = this.fileDao.ReadFile(this.configPath);
            if (jsonString != null && jsonString.Length > 1)
            {
                data = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonString);
            }
            this.InitialiseConfigurationDefaults();
        }

        public void InitialiseConfigurationDefaults()
        {
            if (this.data == null)
            {
                this.data = new Dictionary<string, string>();
                this.SetInt(ConfigKey.PREFERENCE_SEARCH_RANGE, 20);
                this.SetBool(ConfigKey.PREFERENCE_SHOW_E, true);
                this.SetBool(ConfigKey.PREFERENCE_SHOW_R, true);
                this.SetBool(ConfigKey.PREFERENCE_SHOW_F, true);
                this.SetBool(ConfigKey.PREFERENCE_SHOW_K, true);
                this.SetBool(ConfigKey.PREFERENCE_SHOW_P, false);
                this.SetBool(ConfigKey.PREFERENCE_SHOW_PEDESTRIAN_ROADS, false);
            }
        }
    }
}
