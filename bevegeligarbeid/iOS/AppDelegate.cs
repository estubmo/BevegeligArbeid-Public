// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;

using Authenticator = BevegeligArbeid.Services.Authenticator;

using Foundation;
using UIKit;
using System.IO;
using BevegeligArbeid.iOS.Services;
using BevegeligArbeid.iOS.Persistence;
using BevegeligArbeid.iOS.Location;
using BevegeligArbeid.iOS.Foglight;
using Arbeidsvarsling.iOS.Foglight;

namespace BevegeligArbeid.iOS
{
    [Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{

			var application = new TheWallClient.PCL.Service.Application
			{
				Name = "no.triona.bevegeligarbeid",
				Build = "PreRelease",
				Version = "0.0"
			};

            UIApplication.SharedApplication.IdleTimerDisabled = true;

            Connectivity.Connectivity connectivity = new Connectivity.Connectivity ();
			var ucs = new UserClientService ().Build ();

			Authenticator auth = new Authenticator(
				application,
				ucs,
				new JwtDao(),
				new CryptoUtil(ucs.Guid),
				connectivity
			);

			connectivity.Start ();

			#region Notifications
            var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);
            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

            NotificationService notificationService = new NotificationService();
            #endregion

            global::Xamarin.Forms.Forms.Init ();

			var fileDao = new FileDao (GetCacheFolder());
            var settingsDao = new FileDao(GetSettingsFolder());
            var logEntryDao = new LogEntryDao();
			var locationManager = new LocationManager();

			var foglightConfigurationManager = new FoglightConfigurationManager(new FileDao(GetCacheFolder()), Path.Combine(GetCacheFolder(), "Foglight.config"));
			FoglightService foglightService = new FoglightService(Path.Combine(GetCacheFolder(), "Foglight.db"), foglightConfigurationManager, locationManager);




			try{
			LoadApplication (new App (auth, fileDao, logEntryDao, settingsDao, notificationService, foglightService, locationManager));
			}catch(Exception e){
				System.Diagnostics.Debug.WriteLine (e);
			}

			return base.FinishedLaunching (app, options);
		}


		private static string GetCacheFolder()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				NSUrl libraryUrl =
					NSFileManager.DefaultManager.GetUrls(
						NSSearchPathDirectory.LibraryDirectory,
						NSSearchPathDomain.User)[0];

				return libraryUrl.Path;
			}

			string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
			return Path.Combine(personalFolder, "../Cache/LogEntries"); 
		}
        private static string GetSettingsFolder()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                NSUrl libraryUrl =
                    NSFileManager.DefaultManager.GetUrls(
                        NSSearchPathDirectory.LibraryDirectory,
                        NSSearchPathDomain.User)[0];

                return libraryUrl.Path;
            }

            string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
            return Path.Combine(personalFolder, "../Cache/Settings"); 
        }
    }
}