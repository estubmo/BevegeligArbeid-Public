// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
namespace BevegeligArbeid.ViewModels
{
    public class AboutViewModel
	{
        public string Title { get; set; }

        public string CompanyName { get; set; }
		public string AppName { get; set; }
		public string Version { get; set; }

		public string LicencedTo { get; set; }
		public string LicenceHolder { get; set; }



		public AboutViewModel ()
		{
            Title = "Om";
			CompanyName = "Triona AS";
			AppName = "Loggbok for bevegelig arbeid";
			Version = "Versjon: " + "1.0"; //Actual version number to be added here
			LicencedTo = "Lisensiert til:";
            LicenceHolder = App._auth.username;
		}
	}
}

