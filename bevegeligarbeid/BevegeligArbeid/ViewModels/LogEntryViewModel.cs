// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BevegeligArbeid.Views;
using System.Net.Http;
using BevegeligArbeid.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foglight.Domain;
using GeoTools;

namespace BevegeligArbeid.ViewModels
{
    public class LogEntryViewModel : INotifyPropertyChanged
	{
        private PointListingList PointListingList { get; set; }

        private string Title { get; set; }

        private PlanListItem _planListItem;
        public PlanListItem planListItem
        {
            get
            {
                return _planListItem;
            }
            set
            {
                _planListItem = value;
                OnPropertyChanged("LogButtonText");
            }
        }
        public bool isFalse { get; set; }
        public bool pointIsSelected { get; set; }
        private PointListing _selectedPoint;
        public PointListing selectedPoint
        { 
            get
            {
                return _selectedPoint;
            } set
            {
                if (value == _selectedPoint)
                    return;
                else 
                {
                    _selectedPoint = value;
                    pointIsSelected = true;
                    OnPropertyChanged("pointIsSelected");
                    System.Diagnostics.Debug.WriteLine("_selectedPoint" + _selectedPoint.id);
                }
            }
        }


        public string LogButtonText { get; set;}
        private int _logEntryCounter { get; set; }
        public int LogEntryCounter {
            get
            {
                return _logEntryCounter;
            }
            set
            {
                _logEntryCounter = _logEntryCounter + 1;
                OnPropertyChanged("LogEntryCounter");

            }
        }
        public int _loggingCounter
        {
            get; set;
        }
        public int loggingCounter {
            get {
                return _loggingCounter;
            }
            set
            {
                _loggingCounter = value;
                TimeToNextEntry = value;
            }
        }

        public int _timeToNextEntry { get; set; }
        public int TimeToNextEntry { get
            {
                return _timeToNextEntry;
            }
            set
            {
                _timeToNextEntry = (App._settings.settings.timeToLog - value);
                OnPropertyChanged("TimeToNextEntry");
            }
        }

        public string planNr { get; set; }

		bool _isLogging { get; set; }

		public bool IsLogging {
			get {
				return _isLogging;
			}
			set {
				_isLogging = value;
				OnPropertyChanged ("IsLogging");
				OnPropertyChanged ("NotIsLogging");
			}
		}

		public bool NotIsLogging { get{ return !IsLogging;} }

		public ObservableCollection<PointListing> list {get; set;}
        public ListView pointListView { get; set; }

		public LogEntryViewModel ()
		{
            Position _pos = App._fogLightService.GetCurrentPosition();
            _logEntryCounter = -1;
            isFalse = false;
            list = new ObservableCollection<PointListing>();
            pointListView = new ListView();
            IsLogging = false;
            LogButtonText = "Start";
            _planListItem = new PlanListItem ();
			App._isLoggingActive = false;
            MessagingCenter.Subscribe<PlanListView, object> (this, "PlanObject", (s, e) => {
				PlanListItem plan = e as PlanListItem;
				if(plan != null){
                    _planListItem.PlanNr = plan.PlanNr;
                    planNr = plan.PlanNr;
                    _planListItem.Description = plan.Description;
					_planListItem.RoadName = plan.RoadName;
					_planListItem.Date = plan.Date;
					_planListItem.IsActiveLogText = plan.IsActiveLogText;
                    LoadPoints();
                }
            });
            MessagingCenter.Unsubscribe<PlanListView> (this, "No longer subscribing for Objects");

            OnPropertyChanged("LogEntryTitle");
			ToggleLogCommand = new Command (ToggleLogging);
            LogEntryCounter = -1;
        }

        public async void LoadPoints()
        {
            try
            {
               // System.Diagnostics.Debug.WriteLine("/plan/" + planNr + "/points");
                HttpResponseMessage response = App.rc.Send("/plan/" + planNr + "/points", HttpMethod.Get, null);
                if (response.IsSuccessStatusCode)
                {
                    HttpContent content = response.Content;

                    string json = await content.ReadAsStringAsync();
                    PointListingList = new PointListingList(JsonConvert.DeserializeObject<ICollection<PointListing>>(json));

                    foreach (PointListing pointList in PointListingList.PointList)
                    {
                        // TODO Somehow show list of points, and be able to select them.
                        PointListing pointListItem = new PointListing();
                        pointListItem.id = pointList.id;
                        pointListItem.status = pointList.status;
                        list.Add(pointListItem);
                        //System.Diagnostics.Debug.WriteLine(pointList.id);
                        this.pointListView.ItemsSource = list;

                        //THIS GETS ALL LOG ENTRIES FOR EACH POINT. USE THIS TO SEE POINT INFO?
                        //SAME DOMAIN CLASSES ARE NEEDED FOR MAKING LOG ENTRIES
                        //response = null;
                        //content = null;
                        //json = null;

                        //response = App.rc.Send("/plan/" + plannr + "/point/" + pointList.id + "/log", HttpMethod.Get, null);
                        //App._notificationService.Notify(0, "PointList: ", "/plan/" + plannr + "/point/" + pointList.id + "/log");
                        //LogEntryList LogEntryList;
                        //if (response.IsSuccessStatusCode)
                        //{
                        //    content = response.Content;
                        //    json = await content.ReadAsStringAsync();
                        //    LogEntryList = new LogEntryList(JsonConvert.DeserializeObject<ICollection<LogEntry>>(json));
                        //}
                        //else
                        //{
                        //    App._notificationService.Notify(0, "Error", "IsSuccessStatusCode = false - Point Id: " + pointList.id);
                        //}
                    }
                    list = new ObservableCollection<PointListing>();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("IsSuccessStatusCode = false - During point fetching");
                   //App._notificationService.Notify(0, "Error", "IsSuccessStatusCode = false - During point fetching");
                }

            }
            catch (Exception e)
            {
                App._notificationService.Notify(0, "Exception in LoadPoints()", e.Message);
            }
        }

        private async void MakeLogEntry(LogEntry logEntry)
        {
            try
            {
                //MAKE LOGENTRY FROM GPS DATA

                //MOCK
                Plan Plan = new Plan();
                PointListing Point = new PointListing();
                logEntry.description = "Bevegelig Arbeid - LogEntry";

                var rr = App._fogLightService.GetCurrentRoadRef();
                System.Diagnostics.Debug.WriteLine("*************************" + rr);

                logEntry.roadRef = new Domain.RoadRef
                {
                    county = rr.HasCounty() ? rr.GetCounty() : -1,
                    municipality = rr.HasMunicipality() ? rr.GetMunicipality() : -1,
                    roadnr = rr.GetRoadNumber(),
                    roadStatus = rr.GetStatus().ToString(),
                    roadCategory = rr.GetCategory().ToString(),
                    from_hp = rr.GetSection(),
                    to_hp = rr.GetSection(),
                    from_km = (int)rr.Meter,
                    to_km = (int)rr.Meter
                };

                logEntry.location = new LocationLongLat();
                if (App._fogLightService.GetCurrentGpsCoordinates() != null)
                {
                var loc = App._fogLightService.GetCurrentGpsCoordinates();
                  logEntry.location.longitude = loc.Longitude;
                  logEntry.location.latitude = loc.Latitude;
                }
                System.Diagnostics.Debug.WriteLine("*************************" + App._fogLightService.GetCurrentPosition());
                //
                //
                //
                Position pos = App._fogLightService.GetCurrentPosition();
                var gpsPos = Converter.toGps(pos.Easting, pos.Northing);
                var coords = new Location.GpsCoordinates(gpsPos.latitude, gpsPos.longitude, 100, 100);

                logEntry.setup = _selectedPoint.id;
                logEntry.equipment = "v5V9xKOWko1917M5w1gPMFA3cdMQn8UUP00UhFUM05c";
                logEntry.sign = "tempSign@sefsef.com";

                string json = JsonConvert.SerializeObject(logEntry);
                HttpResponseMessage response = App.rc.Send("/plan/" + planNr + "/point/" + selectedPoint.id + "/log", HttpMethod.Put, json);
                //HttpResponseMessage response = App.rc.Send("/plan/" + planNr + "/point/" + selectedPoint.id + "/log", HttpMethod.Get, null);
                if (response.IsSuccessStatusCode == false)
                {
                    App._logEntryDao.SaveLogEntryToCache(planNr, selectedPoint.id, logEntry);
                }

                string content = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                //App._notificationService.Notify(0, "Exception in LogEntryViewModel", e.Message);
                System.Diagnostics.Debug.WriteLine("Exception in LogEntryViewModel " + e.Message);

            }
        }


        private readonly double _checkSpeedInterval = 1;
        public DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        public async void ToggleLogging()
        {
            LogEntry logEntry = new LogEntry();
            if (IsLogging == true)
            {
                _planListItem.IsActiveLogText = "Aktiv";
                LogButtonText = "Start";
                IsLogging = false;
                App._isLoggingActive = IsLogging;
            }
            else
            {
                _planListItem.IsActiveLogText = string.Empty;
                LogButtonText = "Stop";
                IsLogging = true;
                App._isLoggingActive = IsLogging;
            }

            double speed = 0;
            loggingCounter = 0;
            List<Position> posList = new List<Position>();
            OnPropertyChanged("LogButtonText");
            while (IsLogging)
            {
                await Task.Delay(TimeSpan.FromSeconds(_checkSpeedInterval));
                try

                {
                    if(App._fogLightService.GetCurrentPosition().Speed != null)
                    {
                        speed = App._fogLightService.GetCurrentPosition().Speed.KilometerPerHour;
                    }else
                    {
                        speed = 0;
                    }
                    var lastPos =  App._fogLightService.GetCurrentPosition();
                   
                    posList.Add(App._fogLightService.GetCurrentPosition());

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("CurrentPosition == null" + e.Message);
                }
                finally
                {
                    if (speed < App._settings.settings.maxSpeed)
                    {
                        if (posList.Count > 1)
                        {
                            if (posList[posList.Count-1].ToString().Equals(posList[posList.Count - 2].ToString()))
                            {
                                System.Diagnostics.Debug.WriteLine("Last pos == current pos");
                                System.Diagnostics.Debug.WriteLine("*************************" + App._fogLightService.GetCurrentPosition());
                                speed = 0;
                            }
                        }

                       
                        var tmpTime = (DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds.ToString();
                        if (tmpTime.Length > 13) tmpTime = tmpTime.Substring(0, 13);
                        logEntry.start_time = tmpTime;
                        loggingCounter++;
                        if (loggingCounter >= App._settings.settings.timeToLog)
                        {
                            
                            while (speed < App._settings.settings.maxSpeed && IsLogging == true)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(_checkSpeedInterval));
                                loggingCounter++;
                                try
                                {
                                    speed = App._fogLightService.GetCurrentPosition().Speed.KilometerPerHour;
                                    System.Diagnostics.Debug.WriteLine("*************************" + App._fogLightService.GetCurrentPosition());


                                }
                                catch (Exception e)
                                {
                                    speed = 0;
                                    System.Diagnostics.Debug.WriteLine("App._fogLightService.GetCurrentPosition().Speed.KilometerPerHour is null: " + e.Message);
                                }
                                System.Diagnostics.Debug.WriteLine("INSIDE WHILE - counter = " + loggingCounter + " speed = " + speed);
                               
                            }
                            System.Diagnostics.Debug.WriteLine("Make Log Entry " + _selectedPoint.id);

                            tmpTime = (DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds.ToString();
                            if (tmpTime.Length > 13) tmpTime = tmpTime.Substring(0, 13);
                            logEntry.stop_time = tmpTime;
                            MakeLogEntry(logEntry);
                            LogEntryCounter = 0;
                            loggingCounter = 0;


                        }
                    } else
                    {
                        loggingCounter = 0;
                    }
                }
            }
        }

        public ICommand ToggleLogCommand { get; set; }

        #region INPC
        public void OnPropertyChanged (string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}


