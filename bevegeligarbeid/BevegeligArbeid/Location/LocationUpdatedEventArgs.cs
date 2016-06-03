// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Location
{
	using System;

	public class LocationUpdatedEventArgs : EventArgs
	{
		public LocationUpdatedEventArgs(DateTime timeStamp, Location location, LocationUpdateError error)
		{
			TimeStamp = timeStamp;
			Location = location;
			Error = error;
		}

		public DateTime TimeStamp { get; private set; }

		public Location Location { get; private set; }

		public LocationUpdateError Error { get; private set; }

		public bool IsError()
		{
			return Error != null;
		}
	}
}
