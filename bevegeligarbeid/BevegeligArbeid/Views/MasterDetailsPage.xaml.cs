// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;
using Xamarin.Forms;

namespace BevegeligArbeid.Views
{
    public partial class MasterDetailsPage : MasterDetailPage
	{
		public MasterDetailsPage ()
		{
			InitializeComponent ();
			masterPage.ListView.ItemSelected += OnMenuItemSelected;
            this.IsGestureEnabled = false;
        }

        async void OnMenuItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			if (Detail.Navigation.NavigationStack.Count.GetType () == typeof(LogEntryView)) {
                await Detail.Navigation.PopAsync();
			} else {

				var item = e.SelectedItem as MenuPageItem;
				if (item != null) {
					if (item.TargetType != typeof(LoginView)) {
						Detail = new NavigationPage ((Page)Activator.CreateInstance (item.TargetType));
						App._main = this;
					} else {
						if (App._isLoggingActive) {
							var cancel = await App.Current.MainPage.DisplayAlert (
								            "Logg ut", 
								            "Loggføring for en annen arbeidsplan er aktiv. Vil du logge ut og avslutte loggføringen?", 
								            "Nei", "Ja");
							if (!cancel) {
								App._isLoggingActive = false;
								await App.Current.MainPage.DisplayAlert (
									"Loggføring avsluttet",
									"Loggføringen er avsluttet. Logger ut.",
									"OK"
								);   
                                App.OnLogout();
                            }
						} else {
                            App.OnLogout();
                        }
                    }
					masterPage.ListView.SelectedItem = null;
					IsPresented = false;
				}
			}
		}
    }
}