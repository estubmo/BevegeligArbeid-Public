// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Droid.Location
{
    using System;

    using Android.Content;
    using Android.Locations;
    using Android.OS;

    using BevegeligArbeid.Location;

    using Location = BevegeligArbeid.Location.Location;
    using Object = Java.Lang.Object;
    using Xamarin.Forms;
    public class LocationManager : Object, ILocationManager, ILocationListener
    {
        private Android.Locations.LocationManager locationManager;

        private bool isRunning;

        public LocationManager()
        {
            this.isRunning = false;
        }

        public void init()
        {
            this.locationManager = Forms.Context.GetSystemService(Context.LocationService) as Android.Locations.LocationManager;

        }

        public void initFirstLocation()
        {
            var locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;
            var locationProvider = this.locationManager.GetBestProvider(locationCriteria, true);
            OnLocationChanged(this.locationManager.GetLastKnownLocation(locationProvider));
        }
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public Location LastLocation { get; private set; }

        public bool StartLocationUpdates()
        {
            try
            {
                var locationCriteria = new Criteria();
                locationCriteria.Accuracy = Accuracy.NoRequirement;
                locationCriteria.PowerRequirement = Power.NoRequirement;
                var locationProvider = this.locationManager.GetBestProvider(locationCriteria, true);
                this.locationManager.RequestLocationUpdates(locationProvider, 2000, 1, this);
               
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



        public bool IsRunning()
        {
            return this.isRunning;
        }

        public bool StopLocationUpdates()
        {
            try
            {
                this.locationManager.RemoveUpdates(this);
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

        #region ILocationListener implementation
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

        public void OnProviderDisabled(string provider)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("ProviderDisabled: {0}", provider));
        }

        public void OnProviderEnabled(string provider)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("ProviderEnabled: {0}", provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("StatusChanged: {0}", provider));
        }
        #endregion
    }

}