// =====================================================
// AUTHOR: Triona AS
// NOTES: Modified by Jørgen Nyborg
//======================================================
namespace Arbeidsvarsling.iOS.Foglight
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Threading;

	using BevegeligArbeid.Foglight;
	using BevegeligArbeid.Location;

	using global::Foglight;
	using global::Foglight.Domain;
	using global::Foglight.Filtering;
	using global::Foglight.MapMatching;
	using global::Foglight.Xamarin.iOS;

	using GeoTools;

	using GpsCoordinates = BevegeligArbeid.Location.GpsCoordinates;
	using LocationUpdatedEventArgs = BevegeligArbeid.Foglight.LocationUpdatedEventArgs;
	using BevegeligArbeid.Collections;
	public class FoglightService : IFoglightService, IDisposable
	{
		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
		public event EventHandler<LocationUpdateErrorEventArgs> LocationError = delegate { };

		private Manager manager;
		private IReverseMapper revMapper;
		private IRoadRefMapper roadMapper;
		private ITracker tracker;
		private CarAndPedestrianRoadFilter filter;
		private readonly string dbPath;
		private readonly IConfigurationManager configHandler;
		private ILocationManager locationManager;

		private MappedRoadRef currentRoadRef { get; set; }
        private Position currentPosition { get; set; }
        private GpsCoordinates currentGpsCoordinates { get; set; }

		private Road preferredRoad;
		private bool isFoglightReady;

		public FoglightService(string dbPath, IConfigurationManager configManager, ILocationManager locationManager)
		{
			this.dbPath = dbPath;
			this.configHandler = configManager;
			this.locationManager = locationManager;
		}

		private void InitFoglight()
		{
			if (this.isFoglightReady) return;

			try
			{
				System.Diagnostics.Debug.WriteLine("Initializing foglight");
				// Configure and set up Foglight
				Config config = Config.CreateDefault(this.dbPath);
				this.manager = Manager.Create(config);

				this.filter = new CarAndPedestrianRoadFilter();
				this.configHandler.Sync(this.filter);
				this.configHandler.PropertyChanged += this.ConfigPropertyChanged;

				this.tracker = TrackerFactory.Create(this.manager);
				var offlineTracker = this.tracker as OfflineLighthouseTracker;
				if (offlineTracker != null)
				{
					offlineTracker.Filter = this.filter;
				}
				var lighthouseTracker = this.tracker as LighthouseTracker;
				if (lighthouseTracker != null)
				{
					lighthouseTracker.Filter = this.filter;
				}

				this.revMapper = ReverseMapperFactory.Create(this.manager);
				this.roadMapper = LookupMapperFactory.Create(this.manager);

				this.tracker.SetSearchRange(this.configHandler.GetInt(ConfigKey.PREFERENCE_SEARCH_RANGE));
				System.Diagnostics.Debug.WriteLine("Search range: {0}", this.tracker.GetSearchRange());

				System.Diagnostics.Debug.WriteLine("Foglight ready!");
				this.isFoglightReady = true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error while setting up Foglight", ex, null);
			}
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			try
			{
				if (ConfigKey.PREFERENCE_SEARCH_RANGE.Equals(args.PropertyName))
				{
					this.tracker.SetSearchRange(this.configHandler.GetInt(ConfigKey.PREFERENCE_SEARCH_RANGE));
					System.Diagnostics.Debug.WriteLine("Search range: {0}", this.tracker.GetSearchRange());
				}
				else
				{
					this.configHandler.Sync(this.filter);
					var offlineTracker = this.tracker as OfflineLighthouseTracker;
					if (offlineTracker != null)
					{
						offlineTracker.Filter = this.filter;
					}
					var lighthouseTracker = this.tracker as LighthouseTracker;
					if (lighthouseTracker != null)
					{
						lighthouseTracker.Filter = this.filter;
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error while handling config update", ex, null);
			}
		}


		private void TearDownFoglight()
		{
			if (!this.isFoglightReady) return;

			try
			{
				this.manager.Dispose();
				this.tracker = null;
				this.roadMapper = null;
				this.revMapper = null;
				this.manager = null;
				this.configHandler.PropertyChanged -= this.ConfigPropertyChanged;

				System.Diagnostics.Debug.WriteLine("Foglight torn down!");
				this.isFoglightReady = false;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error while tearing down Foglight", ex, null);
			}
		}

		private bool running;
		private Thread foglightThread;

		private BoundedBlockingQueue<Tuple<object, BevegeligArbeid.Location.LocationUpdatedEventArgs>> queue;

		public bool Start()
		{
			if (!this.isFoglightReady)
			{
				this.InitFoglight();
			}

			this.foglightThread = new Thread(this.Run);
			this.foglightThread.Name = "FogLight";
			this.queue = new BoundedBlockingQueue<Tuple<object, BevegeligArbeid.Location.LocationUpdatedEventArgs>>(5);
			this.running = true;
			this.foglightThread.Start();
			System.Diagnostics.Debug.WriteLine("Service started");
			return this.StartLocationUpdates();
		}

		private void Run()
		{
			System.Diagnostics.Debug.WriteLine(String.Format("Starting {0} thread.", Thread.CurrentThread.Name));
			while (this.running)
			{
				try{
					Tuple<object, BevegeligArbeid.Location.LocationUpdatedEventArgs> t;

				bool success = this.queue.TryDequeue(out t);  // Queue blocks until event or closed
				if (success)
				{
					System.Diagnostics.Debug.WriteLine("Got foglight event");
					try
					{
							
						IFoglightEventArgs args = this.Process(t);
						// Notify listeners
						switch (args.GetEventType())
						{
						case FoglightEventType.ERROR:
							System.Diagnostics.Debug.WriteLine("ERROR");
							this.LocationError(this, (LocationUpdateErrorEventArgs)args);
							break;
						case FoglightEventType.UPDATE:
							System.Diagnostics.Debug.WriteLine("UPDATE");
							this.LocationUpdated(this, (LocationUpdatedEventArgs)args);
							break;
						}
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine("Exception processing queue element", ex);
					}
				}

				}catch(Exception e){
				}
			}
			System.Diagnostics.Debug.WriteLine("{0} thread terminated.", Thread.CurrentThread.Name);
		}

		private IFoglightEventArgs Process(Tuple<object, BevegeligArbeid.Location.LocationUpdatedEventArgs> t)
		{
			var args = t.Item2;
			if (args.IsError())
			{
				return new LocationUpdateErrorEventArgs(args.TimeStamp, args.Error.Message);
			}
			double lat = args.Location.Latitude;
			double @long = args.Location.Longitude;
			System.Diagnostics.Debug.WriteLine("Foglight processing {0}", args.Location);

			GpsCoordinates coords = new GpsCoordinates(lat, @long, args.Location.HorizontalAccuracy,
				args.Location.VerticalAccuracy);
			UtmCoordinates utm = Converter.toUtm(lat, @long);
			Position pos = new Position(utm.easting, utm.northing);

			System.Diagnostics.Debug.WriteLine("UTM: Easting/Northing {0}/{1}.", pos.Easting, pos.Northing);

			MappedRoadRef roadref = null;
			if (this.tracker != null)
			{
				roadref = this.tracker.Map(pos);
			}

			if (roadref != null)
			{
				// use position from roadref if successful map, it has calculated speed/course
				pos = roadref.Position;
				// if preferredroad is set, check candidates for a match
				IList<MappedRoadRef> candidates = this.tracker.GetLastCandidates();
				if (this.preferredRoad != null && candidates != null && candidates.Count > 1)
				{
					foreach (MappedRoadRef candidate in candidates)
					{
						if (candidate.Road.Equals(this.preferredRoad))
						{
							if (!candidate.Equals(roadref))
							{
								System.Diagnostics.Debug.WriteLine("Changing mapped roadref from {0} to {1} due to preference for {2}", roadref, candidate, this.preferredRoad);
								roadref = candidate;
							}
							break;
						}
					}
				}
			}

			// update variables
			this.currentGpsCoordinates = coords;
			this.currentRoadRef = roadref;
			this.currentPosition = pos;

			if (this.currentTripCoordinates == null)
			{
				this.currentTripCoordinates = this.currentGpsCoordinates;
			}

			return new LocationUpdatedEventArgs(args.TimeStamp, pos, coords, roadref);
		}

		private double GetTrip()
		{
			var trip = 0.0;

			if (this.currentGpsCoordinates != null && this.currentTripCoordinates != null)
			{
				var lat1 = this.currentGpsCoordinates.Latitude;
				var lat2 = this.currentTripCoordinates.Latitude;

				var lon1 = this.currentGpsCoordinates.Longitude;
				var lon2 = this.currentTripCoordinates.Longitude;

				trip = this.DistFrom(lat1, lon1, lat2, lon2);
			}

			return trip;
		}

		private double DistFrom(double lat1, double lon1, double lat2, double lon2)
		{
			var earthRadius = 6371000; //meters
			var distanceLat = this.ConvertToRadians(lat2 - lat1);
			var distanceLon = this.ConvertToRadians(lon2 - lon1);
			var a = Math.Sin(distanceLat / 2) * Math.Sin(distanceLat / 2) +
				Math.Cos(this.ConvertToRadians(lat1)) * Math.Cos(this.ConvertToRadians(lat2)) *
				Math.Sin(distanceLon / 2) * Math.Sin(distanceLon / 2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			var dist = earthRadius * c;

			return Math.Round(dist, 0);
		}

		private double ConvertToRadians(double angle)
		{
			return (Math.PI / 180) * angle;
		}

		public bool IsRunning()
		{
			return this.running;
		}

		public bool Stop()
		{
			if (!this.running) return true;

			this.running = false;
			this.queue.Close();
			this.foglightThread.Join();
			this.foglightThread = null;
			this.queue = null;
			this.StopLocationUpdates();

			System.Diagnostics.Debug.WriteLine("Service stopped.");
			return true;
		}

		private bool StartLocationUpdates()
		{
			try
			{
				this.locationManager.LocationUpdated += this.UpdateLocation;
				bool @out = this.locationManager.StartLocationUpdates();
				if (@out)
				{
					System.Diagnostics.Debug.WriteLine("Location updates started.");
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Exception starting location updates", ex, null);
			}
			return false;
		}

		private void StopLocationUpdates()
		{
			try
			{
				this.locationManager.LocationUpdated -= this.UpdateLocation;
				bool @out = this.locationManager.StopLocationUpdates();
				if (@out)
				{
					System.Diagnostics.Debug.WriteLine("Location updates stopped.");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Exception starting location updates", ex, null);
			}
		}

		private void UpdateLocation(object sender, BevegeligArbeid.Location.LocationUpdatedEventArgs args)
		{
            // queue for listeners
            System.Diagnostics.Debug.WriteLine("Enqueuing location event.");
			try
			{
				this.queue.Enqueue(Tuple.Create((object)this, args));
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Exception queueing location updates", ex, null);
			}
		}

		public string GetLocationString()
		{
			var retval = "Ingen vegreferanse";

			if (this.currentRoadRef != null)
			{
				var trip = this.GetTrip();
				var tripString = "....";

				if (trip <= 9999)
				{
					tripString = trip + "m";
				}

				retval = this.currentRoadRef.GetCategory().ToString() + this.currentRoadRef.GetStatus().ToString() + this.currentRoadRef.GetRoadNumber().ToString() + " hp" + this.currentRoadRef.GetSection() + " m" + this.currentRoadRef.Meter + "     T: " + tripString;
			}
			return retval;
		}


		public RoadRef GetCurrentRoadRef()
		{
			return this.currentRoadRef;
		}

		public void SetPreferredRoad(Road road)
		{
			if (road != null)
			{
				this.preferredRoad = road;
			}
		}

        public Position GetCurrentPosition()
        {
            return this.currentPosition;
        }

        public GpsCoordinates GetCurrentGpsCoordinates()
		{
			return this.currentGpsCoordinates;
		}

		//public bool HasData()
		//{
		//    return tracker.HasMapData();
		//}

		//public DateTime? LastRefreshed()
		//{
		//    return tracker.MapLastRefreshed();
		//}

		//public DateTime? LastModified()
		//{
		//    return tracker.MapLastModified();
		//}

		//public int GetItemsInQueue()
		//{
		//    return tracker.GetItemsInQueue();
		//}

		//public void DownloadCounty(int county)
		//{
		//    ((Tracker)tracker).DownloadCounty(county);
		//}

		public Position GetPositionForRoadRef(RoadRef roadRef)
		{
			return this.revMapper.Map(roadRef);
		}

		public GpsCoordinates GetGpsCoordinatesForRoadRef(RoadRef roadRef)
		{
			try
			{
				var pos = this.revMapper.Map(roadRef);
				var gpsPos = Converter.toGps(pos.Easting, pos.Northing);
				return new GpsCoordinates(gpsPos.latitude, gpsPos.longitude, 100, 100);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Exception getting coordinates for roadref {0}", ex, roadRef);
				throw ex;
			}
		}

		public IList<RoadRef> GetLastCandidates()
		{
			HashSet<MappedRoadRef> o = new HashSet<MappedRoadRef>();
			try
			{
				IList<MappedRoadRef> list = this.tracker.GetLastCandidates();
				if (list != null)
				{
					foreach (MappedRoadRef rr in list)
					{
						o.Add(rr);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Exception getting last candidates", ex, null);
			}
			return new List<RoadRef>(o);

		}

		public void Dispose()
		{
			this.StopLocationUpdates();

			this.TearDownFoglight();
			this.configHandler.Dispose();
			System.Diagnostics.Debug.WriteLine("Service disposed.");
		}


		private GpsCoordinates currentTripCoordinates;

		public void SetTrip(bool auto)
		{
			if (!auto || (auto && !this.configHandler.GetBool(ConfigKey.Disable_AUTO_TRIP)))
			{
				this.currentTripCoordinates = this.currentGpsCoordinates;

				this.LocationUpdated(
					this,
					new LocationUpdatedEventArgs(
						DateTime.Now,
						this.currentPosition,
						this.currentGpsCoordinates,
						this.currentRoadRef));
			}
		}
	}
}