// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Location
{
	using System;

	public class LocationUpdateError
	{
		public LocationUpdateError(DateTime timeStamp, string message)
		{
			TimeStamp = timeStamp;
			Message = message;
		}

		public DateTime TimeStamp { get; private set; }

		public string Message { get; private set; }
	}
}
