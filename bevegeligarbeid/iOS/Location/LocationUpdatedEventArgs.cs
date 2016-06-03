// =====================================================
// AUTHOR: Jørgen Nyborg
// NOTES: Based on Xamarin Documentation Guide:
// iOS Application Fundamentals Backgrounding Part 4 - Walkthroughs: Backgrounding in iOS
// https://developer.xamarin.com/guides/ios/application_fundamentals/backgrounding/part_4_ios_backgrounding_walkthroughs/location_walkthrough/
//======================================================
using System;
using CoreLocation;

namespace BevegeligArbeid.iOS.Location.Event
{
	public class LocationUpdatedEventArgs : EventArgs
	{
		CLLocation location;

		public LocationUpdatedEventArgs(CLLocation location)
		{
			this.location = location;
		}

		public CLLocation Location
		{
			get { return location; }
		}
	}
}

