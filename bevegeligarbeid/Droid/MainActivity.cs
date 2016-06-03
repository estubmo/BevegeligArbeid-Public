// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using Android.App;
using Android.Content.PM;
using Android.OS;
using Authenticator = BevegeligArbeid.Services.Authenticator;
using Application = TheWallClient.PCL.Service.Application;
using TheWallClient.Android.Service;
using TheWallClient.Shared.Persistence;
using TheWallClient.Shared.Service;
using BevegeligArbeid.Droid.Persistence;
using System.IO;
using BevegeligArbeid.Droid.Services;
using Arbeidsvarsling.Droid.Foglight;
using BevegeligArbeid.Droid.Location;
using Android.Views;

namespace BevegeligArbeid.Droid
{
    [Activity(Label = "BevegeligArbeid.Droid", Icon="@drawable/TrionaLogo", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {

            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            base.OnCreate(bundle);
            var application = new Application
            {
                Name = this.PackageName,
                Build = "PreRelease",
                Version = "0.0"
            };

            var ucs = new UserClientService(this.ApplicationContext).Build();
            Connectivity.Connectivity connectivity = new Connectivity.Connectivity();
            FileDao fileDao = new FileDao(GetLogEntryCacheFolder());
            FileDao settingsDao = new FileDao(GetSettingsCacheFolder());
            Authenticator auth = new Authenticator(
                 application,
                 ucs,
                 new JwtDao(),
                 new CryptoUtil(ucs.Guid),
                 connectivity
           );
            connectivity.Start();

            NotificationService notificationService = new NotificationService(this);
            LogEntryDao logEntryDao = new LogEntryDao();
            var locationManager = new LocationManager();
            var foglightConfigurationManager = new FoglightConfigurationManager(new FileDao(GetSettingsCacheFolder()), Path.Combine(GetSettingsCacheFolder(), "Foglight.config"));
            FoglightService foglightService = new FoglightService(Path.Combine(GetSettingsCacheFolder(), "Foglight.db"), foglightConfigurationManager, locationManager);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(auth, fileDao, logEntryDao, settingsDao, notificationService, foglightService, locationManager));
        }

        private static string GetLogEntryCacheFolder()
        {
    #if (DEBUG)
            const string LogEntriesFolder = "/sdcard/BevegeligArbeid/Cache/LogEntries";
    #else
            string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            string LogEntriesFolder = Path.Combine(personalFolder, "LogEntries/"); // Logentries folder
    #endif
            if (!Directory.Exists(LogEntriesFolder))
            {
                Directory.CreateDirectory(LogEntriesFolder);
            }
            return LogEntriesFolder;
        }

		private static string GetSettingsCacheFolder()
		{
	#if (DEBUG)
			const string SettingsFolder = "/sdcard/BevegeligArbeid/Cache/Settings";
	#else
			string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
			string SettingsFolder = Path.Combine(personalFolder, "Settings/"); // Logentries folder
	#endif
			if (!Directory.Exists(SettingsFolder))
			{
				Directory.CreateDirectory(SettingsFolder);
			}
			return SettingsFolder;
		}
    }
}

