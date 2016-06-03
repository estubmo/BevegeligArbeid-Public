// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Location
{
	public class Location
	{
		public double Latitude { get; set; }

		public double HorizontalAccuracy { get; set; }

		public double VerticalAccuracy { get; set; }

		public double Longitude { get; set; }

		public double Altitude { get; set; }

		public double Course { get; set; }

		public double Speed { get; set; }

		public override string ToString()
		{
			return string.Format(
				"Lat: {0}, Long: {1}, Alt: {2}, Course: {3}, Speed: {4}, HorAcc: {5}, VerAcc: {6}",
				Latitude,
				Longitude,
				Altitude,
				Course,
				Speed,
				HorizontalAccuracy,
				VerticalAccuracy);
		}
	}
}
