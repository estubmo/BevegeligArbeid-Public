// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Foglight
{
	using System;
	using System.ComponentModel;

	using global::Foglight.Filtering;

	public interface IConfigurationManager : IDisposable, INotifyPropertyChanged
	{
		string GetString(string key);

		void SetString(string key, string value);

		bool GetBool(string key);

		void SetBool(string key, bool value);

		int GetInt(string key);

		void SetInt(string key, int value);

		double GetDouble(string key);

		void SetDouble(string key, double value);

		void Sync(CarAndPedestrianRoadFilter filter);

		void InitialiseConfigurationDefaults();

	}

	public static class ConfigKey
	{
		public const string PREFERENCE_SEARCH_RANGE = "search_range";

		public const string PREFERENCE_SHOW_E = "filter_include_e";

		public const string PREFERENCE_SHOW_R = "filter_include_r";

		public const string PREFERENCE_SHOW_F = "filter_include_f";

		public const string PREFERENCE_SHOW_K = "filter_include_k";

		public const string PREFERENCE_SHOW_P = "filter_include_p";

		public const string PREFERENCE_SHOW_PEDESTRIAN_ROADS = "filter_include_pedestrian_roads";

		public const string Disable_AUTO_TRIP = "disable_auto_trip";

		//public const string PREFERENCE_OFFLINE = "tracker_offline";

		//public const string EXTRAKEY_ROADREF = "RoadRef";
		//public const string EXTRAKEY_ROADREFS = "RoadRefs";
		//public const string SAVEDREF_D = "SAVEDREF{0}";
	}
}
