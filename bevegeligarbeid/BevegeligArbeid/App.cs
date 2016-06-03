// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;

using Xamarin.Forms;
using BevegeligArbeid.Views;
using BevegeligArbeid.Services;
using BevegeligArbeid.Persistence;
using BevegeligArbeid.Foglight;
using BevegeligArbeid.Location;

namespace BevegeligArbeid
{
	public class App : Application
	{
        public static bool isLoggedIn { get; set; }
		public static MasterDetailsPage _main { get; set;}
		public static LogEntryView _logEntry { get; set;}
        public static bool _isLoggingActive { get; set; }
        public static Authenticator _auth { get; set; }
		public static IFileDao _fileDao { get; set;}
        public static IFileDao _settingsFileDao { get; set; }
        public static SettingsPersistence _settings { get; set; }
        public static ILogEntryDao _logEntryDao { get; set; }
		public static IFoglightService _fogLightService { get; set;}
        public static INotificationService _notificationService { get; set;  }
        public static BevegeligArbeidRestClient rc { get; set; }
        public static ILocationManager _locationManager { get; set; }

		public App (Authenticator auth, IFileDao fileDao, ILogEntryDao logEntryDao, IFileDao settingsFileDao, INotificationService notificationService, IFoglightService foglightService, ILocationManager locationManager)
		{
            _isLoggingActive = false;
            _notificationService = notificationService;
            _auth = auth;
            _fileDao = fileDao;
            _settingsFileDao = settingsFileDao;
          
            _main = null;
            _logEntryDao = logEntryDao;
            /*
			if (_auth.securityContext.Jwt != null && isLoggedIn == true)
            {
                if (!_auth.securityContext.Jwt.IsExpired())
                {
                   MainPage = new MasterDetailsPage ();
                }
            } else
            {
            }*/
            MainPage = new LoginView();
            _locationManager = locationManager;
            _fogLightService = foglightService;

        }

        public static void OnLogin()
        {
            try
            {
            _locationManager.init();
                //_notificationService.Notify(0, "OnLogin()", "");
            rc = _auth.serviceProducer.CreateBevegeligArbeidRestClient();
            isLoggedIn = true;
            _logEntry = null;
            _main = new MasterDetailsPage();
            Current.MainPage = _main;
            _fogLightService.Start();
            _settings = new SettingsPersistence();
            _locationManager.initFirstLocation();
            _logEntryDao.CheckForUnuploadedLogEntries();
            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                {
                    _logEntryDao.CheckForUnuploadedLogEntries();
                        System.Diagnostics.Debug.WriteLine("_logEntryDao.CheckForUnuploadedLogEntries()");
                    return true;
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public static void OnLogout()
        {
            _notificationService.Notify(0, "OnLogout()", "");
            Current.MainPage = new LoginView();
            _main = new MasterDetailsPage();
            _logEntry = null;
            _auth.Logout();
            isLoggedIn = false;
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
      
        }
	}
}

