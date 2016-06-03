// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Location
{
	public class GpsCoordinates
	{
		public double Latitude { get; private set; }
		public double Longitude { get; private set; }
		public double HorizontalAccuracy { get; private set; }
		public double VerticalAccuracy { get; private set; }

		public GpsCoordinates(double latitude, double longitude, double horizontalAccuracy, double verticalAccuracy)
		{
			Latitude = latitude;
			Longitude = longitude;
			HorizontalAccuracy = horizontalAccuracy;
			VerticalAccuracy = verticalAccuracy;
		}

        public override string ToString()
        {
            return "Latitude: " + Latitude + " Longitude: " + Longitude + " HorizAcc: " + HorizontalAccuracy + " VertAcc: " + VerticalAccuracy;
        }
	}
}
