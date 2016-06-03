namespace Arbeidsvarsling.Droid.Foglight
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    using BevegeligArbeid.Foglight;
    using BevegeligArbeid.Location;

    using BlackBox.PCL;


    using GpsCoordinates = BevegeligArbeid.Location.GpsCoordinates;
    using LocationUpdatedEventArgs = BevegeligArbeid.Foglight.LocationUpdatedEventArgs;

    public class FoglightService : IFoglightService, IDisposable
    {
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public GpsCoordinates GetCurrentGpsCoordinates()
        {
            throw new NotImplementedException();
        }

        public global::Foglight.Domain.Position GetCurrentPosition()
        {
            throw new NotImplementedException();
        }

        public global::Foglight.Domain.RoadRef GetCurrentRoadRef()
        {
            throw new NotImplementedException();
        }

        public IList<global::Foglight.Domain.RoadRef> GetLastCandidates()
        {
            throw new NotImplementedException();
        }

        public string GetLocationString()
        {
            throw new NotImplementedException();
        }

        public global::Foglight.Domain.Position GetPositionForRoadRef(global::Foglight.Domain.RoadRef roadRef)
        {
            throw new NotImplementedException();
        }

        public bool IsRunning()
        {
            throw new NotImplementedException();
        }

        public void SetPreferredRoad(global::Foglight.Domain.Road roadParcel)
        {
            throw new NotImplementedException();
        }

        public void SetTrip(bool auto)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}