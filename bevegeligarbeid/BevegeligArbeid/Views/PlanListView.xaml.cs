// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using Xamarin.Forms;
using System.Net.Http;

namespace BevegeligArbeid.Views
{
    public partial class PlanListView
    {
        private bool IsLogging { get; set; }
        private string planNr { get; set; }
        public PlanListView()
        {
            InitializeComponent();
            planList.ItemSelected += PlanList_ItemSelected;
            
        }
        public async void PlanList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var plan = e.SelectedItem as PlanListItem;

                if (App._logEntry == null)
                {
                    App._logEntry = new LogEntryView();
                    App._logEntry.Title = plan.PlanNr;
                    await App._main.Detail.Navigation.PushAsync(App._logEntry);
                }
                else if (App._logEntry.Title != plan.PlanNr)
                {
                    if (App._isLoggingActive)
                    {
                        var cancel = await App.Current.MainPage.DisplayAlert(
                                         "Aktiv arbeidsplan",
                                         "Loggføring for en annen arbeidsplan er aktiv. Vil du avslutte loggføringen?",
                                         "Nei", "Ja");
                        if (!cancel)
                        {

                            App._isLoggingActive = false;
                            await App.Current.MainPage.DisplayAlert(
                                "Loggføring avsluttet",
                                "Loggføringen er avsluttet.",
                                "OK"
                            );

                            App._logEntry = new LogEntryView();
                            App._logEntry.Title = plan.PlanNr;
                            await App._main.Detail.Navigation.PushAsync(App._logEntry);
                        }
                    }
                    else
                    {
                        App._logEntry = new LogEntryView();
                        App._logEntry.Title = plan.PlanNr;
                        await App._main.Detail.Navigation.PushAsync(App._logEntry);
                    }
                }
                else
                {
                    await App._main.Detail.Navigation.PushAsync(App._logEntry);
                }

                HttpResponseMessage response = App.rc.Send("/plan/" + plan.PlanNr, HttpMethod.Get, null);
                this.planList.SelectedItem = null;
                MessagingCenter.Send<PlanListView, object>(this, "PlanObject", plan);
            }
        }
    }
}
