// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using BevegeligArbeid.iOS.Connectivity;

namespace BevegeligArbeid.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] a)
		{
			try
			{
				Reachability.ReachabilityChanged += (sender, e) => {
					//MessageBox.Show("Changed");
					Reachability.RemoteHostStatus();
				};

				UIApplication.Main (a, null, "AppDelegate");
			}
			catch(Exception ex)
			{
			}
		}
	}
}
