// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Foglight
{
	using System;
	using System.Collections.Generic;

	using BevegeligArbeid.Location;

	using global::Foglight.Domain;
	using global::Foglight.MapMatching;

	public interface IFoglightService
	{
		event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

		RoadRef GetCurrentRoadRef();

		void SetPreferredRoad(Road roadParcel);

		Position GetCurrentPosition();

		GpsCoordinates GetCurrentGpsCoordinates();

		//bool HasData();

		//DateTime? LastRefreshed();

		//DateTime? LastModified();

		//int GetItemsInQueue();

		//void DownloadCounty(int county);

		Position GetPositionForRoadRef(RoadRef roadRef);

		IList<RoadRef> GetLastCandidates();

		bool Start();

		bool IsRunning();

		bool Stop();

		void SetTrip(bool auto);

		string GetLocationString();
	}

	public interface IFoglightEventArgs
	{
		DateTime Timestamp { get; }
		FoglightEventType GetEventType();
	}

	public enum FoglightEventType
	{
		UPDATE,
		ERROR
	}

	public class LocationUpdatedEventArgs : EventArgs, IFoglightEventArgs
	{
		public DateTime Timestamp { get; private set; }
		public GpsCoordinates Coordinates { get; private set; }
		public Position Position { get; private set; }
		public MappedRoadRef RoadRef { get; private set; }

		public LocationUpdatedEventArgs(DateTime timestamp, Position pos, GpsCoordinates coordinates, MappedRoadRef mrr)
		{
			Timestamp = timestamp;
			Position = pos;
			Coordinates = coordinates;
			RoadRef = mrr;
		}

		public FoglightEventType GetEventType()
		{
			return FoglightEventType.UPDATE;
		}
	}

	public class LocationUpdateErrorEventArgs : EventArgs, IFoglightEventArgs
	{
		public DateTime Timestamp { get; private set; }
		public string Message { get; private set; }

		public LocationUpdateErrorEventArgs(DateTime timestamp, string message)
		{
			Timestamp = timestamp;
			Message = message;
		}

		public FoglightEventType GetEventType()
		{
			return FoglightEventType.ERROR;
		}
	}
}
