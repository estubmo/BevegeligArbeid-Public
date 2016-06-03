// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES: Based on Xamarin Documentation Guide:
// iOS Application Fundamentals Backgrounding Part 4 - Walkthroughs: Backgrounding in iOS
// https://developer.xamarin.com/guides/ios/application_fundamentals/backgrounding/part_4_ios_backgrounding_walkthroughs/location_walkthrough/
//======================================================
using CoreLocation;
using UIKit;

namespace BevegeligArbeid.iOS.Location
{
	using System;

	using BevegeligArbeid.Location;

	using Location = BevegeligArbeid.Location.Location;
	public class LocationManager : ILocationManager
	{
		protected CLLocationManager locationManager;

		private bool isRunning;

		public LocationManager()
		{
			this.isRunning = false;

		}

		public void init()
		{
			
			this.locationManager = new CLLocationManager ();

			// iOS 8 has additional permissions requirements
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				locationManager.RequestAlwaysAuthorization (); // works in background
				//locMgr.RequestWhenInUseAuthorization (); // only in foreground
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				locationManager.AllowsBackgroundLocationUpdates = true;
			}

		//	this.locationManager.PausesLocationUpdatesAutomatically = false;
			System.Diagnostics.Debug.WriteLine("locManager for iOS: " + locationManager.GetType ());
		}

		// event for the location changing
		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

		public Location LastLocation { get; private set; }

        public void initFirstLocation()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {

                //set the desired accuracy, in meters
                locationManager.DesiredAccuracy = 1;
                locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {


                    //LocationUpdated += OnLocationUpdated;

                    Location loc = new Location
                    {
                        Latitude = e.Locations[e.Locations.Length - 1].Coordinate.Latitude,
                        Longitude = e.Locations[e.Locations.Length - 1].Coordinate.Longitude,
                        HorizontalAccuracy = e.Locations[e.Locations.Length - 1].HorizontalAccuracy,
                        VerticalAccuracy = e.Locations[e.Locations.Length - 1].VerticalAccuracy,
                        Altitude = e.Locations[e.Locations.Length - 1].Altitude,
                        Course = e.Locations[e.Locations.Length - 1].Course,
                        Speed = e.Locations[e.Locations.Length - 1].Speed
                    };


                    // fire our custom Location Updated event
                    LocationUpdated(this, new LocationUpdatedEventArgs(DateTime.Now, loc, null));
                    System.Diagnostics.Debug.WriteLine("LAT:" + e.Locations[e.Locations.Length - 1].Coordinate.Latitude);
                    //var update = new LocationUpdatedEventArgs ()
                };
                locationManager.StartUpdatingLocation();
            }
        }

        public bool StartLocationUpdates()
		{
			try
			{
				
				if (CLLocationManager.LocationServicesEnabled) {

					//set the desired accuracy, in meters
					locationManager.DesiredAccuracy = 1;
					locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
					{


						//LocationUpdated += OnLocationUpdated;

						Location loc = new Location {
							Latitude = e.Locations[e.Locations.Length-1].Coordinate.Latitude,
							Longitude = e.Locations[e.Locations.Length-1].Coordinate.Longitude,
							HorizontalAccuracy = e.Locations[e.Locations.Length-1].HorizontalAccuracy,
							VerticalAccuracy = e.Locations[e.Locations.Length-1].VerticalAccuracy,
							Altitude = e.Locations[e.Locations.Length-1].Altitude,
							Course = e.Locations[e.Locations.Length-1].Course,
							Speed = e.Locations[e.Locations.Length-1].Speed
						};


						// fire our custom Location Updated event
						LocationUpdated (this, new LocationUpdatedEventArgs (DateTime.Now, loc, null));
						System.Diagnostics.Debug.WriteLine ("LAT:" + e.Locations[e.Locations.Length-1].Coordinate.Latitude);
						//var update = new LocationUpdatedEventArgs ()
					};
					locationManager.StartUpdatingLocation();
				}


			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("Requesting location updates failed.", ex, null));
				return false;
			}

			this.isRunning = true;
			System.Diagnostics.Debug.WriteLine(String.Format("Requested location updates."));

			return true;
		}

		public void OnLocationUpdated(object sender, CLLocationUpdatedEventArgs e){
			CLLocation c = e.NewLocation;

			Location loc = new Location {
				Latitude = c.Coordinate.Latitude,
				Longitude = c.Coordinate.Longitude,
				HorizontalAccuracy = c.HorizontalAccuracy,
				VerticalAccuracy = c.VerticalAccuracy,
				Altitude = c.Altitude,
				Course = c.Course,
				Speed = c.Speed
			};

			//var loc = new BevegeligArbeid.iOS.Location.Event.LocationUpdatedEventArgs (e.l

			System.Diagnostics.Debug.WriteLine ("lat:" + loc.Latitude);
		}

		public bool IsRunning()
		{
			return this.isRunning;
		}

		public bool StopLocationUpdates()
		{
			try
			{
			//	locationManager.StopUpdatingLocation ();
				//this.locationManager.RemoveUpdates(this);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("Stopping location updates failed.", ex, null));
				return false;
			}

			this.isRunning = false;
			System.Diagnostics.Debug.WriteLine(String.Format("Stopped requesting location updates."));
			return true;
		}

		/*
		public void OnLocationChanged(Android.Locations.Location location)
		{
			System.Diagnostics.Debug.WriteLine(String.Format("Location changed {0}", location));
			this.LastLocation = new Location
			{
				Altitude = location.Altitude,
				Course = location.Bearing,
				HorizontalAccuracy = location.Accuracy,
				Latitude = location.Latitude,
				Longitude = location.Longitude,
				Speed = location.Speed,
				VerticalAccuracy = location.Accuracy
			};
			System.Diagnostics.Debug.WriteLine(String.Format("Location changed {0}", this.LastLocation));


			if (this.LocationUpdated != null)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("Firing location updated event. New location: {0}", this.LastLocation));
				this.LocationUpdated(this, new LocationUpdatedEventArgs(DateTime.Now, this.LastLocation, null));
			}
		}
		*/

		public void Add(){
			
		}

		public void Remove(){
			
		}

   
    }
}