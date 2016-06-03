// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Location
{
	using System;

	public interface ILocationManager
	{
		event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

		Location LastLocation { get; }

		bool StartLocationUpdates();

		bool IsRunning();

		bool StopLocationUpdates();

        void init();

        void initFirstLocation();
	}
}
