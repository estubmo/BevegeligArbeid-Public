// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System.Collections.Generic;

using Xamarin.Forms;

namespace BevegeligArbeid.Views
{
    public partial class MasterPage : ContentPage
	{
		public ListView ListView { get { return listView; } }

		public MasterPage ()
		{
			InitializeComponent ();

			var menuPageItems = new List<MenuPageItem> ();
			menuPageItems.Add (new MenuPageItem {
				Title = "Oppdrag",
				IconSource = "Sign_406_0.png",
				TargetType = typeof(PlanListView)
			});
			menuPageItems.Add (new MenuPageItem {
				Title = "Innstillinger",
				IconSource = "Sign_608_0.png",
				TargetType = typeof(SettingsView)
			});
			menuPageItems.Add (new MenuPageItem {
				Title = "Om",
				IconSource = "Sign_635_0.png",
				TargetType = typeof(AboutView)
			});
			menuPageItems.Add (new MenuPageItem {
				Title = "Logg ut",
				IconSource = "Sign_570_1V.png",
				TargetType = typeof(LoginView)
			});

			listView.ItemsSource = menuPageItems;
		}
	}
}

