// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System.ComponentModel;
using BevegeligArbeid.Domain;

namespace BevegeligArbeid.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public readonly int maxMaxSpeedValue = 100;
        public readonly int minMaxSpeedValue = 0;
        public readonly int maxLogEntryTimeValue = 60;
        public readonly int minLogEntryTimeValue = 1;

        public string MaxSpeedTitle { get; set;}
		public string MaxSpeedDesc {get; set;}
		private int _maxSpeedValue { get; set;}
		public int MaxSpeedValue { 
			get
			{ 
				return _maxSpeedValue;
			} 
			set{ 
                if (value > maxMaxSpeedValue)
                {
                    value = maxMaxSpeedValue;
                    App.Current.MainPage.DisplayAlert("Feilmelding", "Maks hastighet må være mellom " + minMaxSpeedValue + " og " + maxMaxSpeedValue + ".", "Ok");
                } else if (value < minMaxSpeedValue)
                {
                    value = maxMaxSpeedValue;
                    App.Current.MainPage.DisplayAlert("Feilmelding", "Maks hastighet må være mellom " + minMaxSpeedValue + " og " + maxMaxSpeedValue + ".", "Ok");
                }
				_maxSpeedValue = value;
				OnPropertyChanged ("MaxSpeedValue");
                App._settings.setMaxSpeed(value);
			}
		}

		public string LogEntryTimeTitle { get; set;}
		public string LogEntryTimeDesc { get; set;}
        private int _logEntryTimeValue { get; set; }

        public int LogEntryTimeValue
        {
            get
            {
                return _logEntryTimeValue;
            }
            set
            {
                if (value > maxLogEntryTimeValue)
                {
                    value = maxLogEntryTimeValue;
                    App.Current.MainPage.DisplayAlert("Feilmelding", "Tid før innslag må være mellom " + minLogEntryTimeValue + " og " + maxLogEntryTimeValue + ".", "Ok");
                }
                else if (value < minLogEntryTimeValue)
                {
                    value = maxLogEntryTimeValue;
                    App.Current.MainPage.DisplayAlert("Feilmelding", "Tid før innslag må være mellom " + minLogEntryTimeValue + " og " + maxLogEntryTimeValue + ".", "Ok");
                }
                _logEntryTimeValue = value;
                OnPropertyChanged("LogEntryTimeValue");
                App._settings.setTimeToLog(value);

            }
        }


        public SettingsViewModel ()
		{
            
			Title = "Innstillinger";
			MaxSpeedTitle = "Maks hastighet";
			MaxSpeedDesc = "Hastigheten kjøretøyet må holde seg under for at logginnslag kan føres (km/t).";
			LogEntryTimeTitle = "Tid før innslag";
			LogEntryTimeDesc = "Antall sekunder kjøretøyet må holde seg under 'Maks hastighet' før innslag blir gjort.";

            Settings settings = App._settings.settings;
            MaxSpeedValue = settings.maxSpeed;
            LogEntryTimeValue = settings.timeToLog;
		}

    #region INPC

    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

}
}

