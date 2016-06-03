// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.Http;

using Xamarin.Forms;

using BevegeligArbeid.Views;
using BevegeligArbeid.Domain;

using Newtonsoft.Json;
using BevegeligArbeid.Services;

namespace BevegeligArbeid.ViewModels
{
    public class PlanListViewModel : INotifyPropertyChanged
	{

		public ObservableCollection<PlanListItem> list { get; set; }
        public PlanListingList PlanListingList { get; set; }

        public ListView planList { get; set;}

		public string Title { get; set;}
        public string IsActiveLogText { get; set; }


        public PlanListViewModel ()
		{
			planList = new ListView ();
			Title = "Oppdrag";
            IsActiveLogText = "";
			list = new ObservableCollection<PlanListItem> ();
			LoadLogListItems ();
        }

        private async void LoadLogListItems()
        {
            PlanListingList = null;
            try
            {

                HttpResponseMessage response = App.rc.Send("/plans", HttpMethod.Get, null);
                if (response.IsSuccessStatusCode)
                {

                    HttpContent content = response.Content;

                    string json = await content.ReadAsStringAsync();

                    PlanListingList = new PlanListingList(JsonConvert.DeserializeObject<ICollection<PlanListing>>(json));
                    foreach (PlanListing planList in PlanListingList.Plans)
                    {

                        try
                        {

                            PlanListItem planListItem = new PlanListItem();
                            planListItem.PlanNr = planList.Id;
                            planListItem.Description = "Status: " + (planList.Status == 2?"aktiv":"inaktiv");
                            list.Add(planListItem);
                            this.planList.ItemsSource = list;

                            response = null;
                            content = null;
                            json = null;

                            response = App.rc.Send("/plan/" + planList.Id, HttpMethod.Get, null);
                            if (response.IsSuccessStatusCode)
                            {
                                content = response.Content;
                                json = await content.ReadAsStringAsync();
                                Plan plan = JsonConvert.DeserializeObject<Plan>(json);
                                if(plan.location.roadnr != null) planListItem.RoadName = "Vei: " + plan.location.roadnr;
                                if(plan.location.stretch != null) planListItem.Stretch = "Strekning: " + plan.location.stretch;

                                DateTime epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
                                if (plan.workPeriod != null)
                                {
                                    DateTime startDate = epochDate.AddMilliseconds(plan.workPeriod.start).ToLocalTime();
                                    DateTime stopDate = epochDate.AddMilliseconds(plan.workPeriod.stop).ToLocalTime();
                                    string dateString = String.Format("Periode: {0} - {1} ", startDate.ToString("yyyy-MM-dd"), stopDate.ToString("yyyy-MM-dd"));
                                    planListItem.Date = dateString;
                                }

                            } else
                            {
                            App._notificationService.Notify(0, "Error", "IsSuccessStatusCode = false - Plan Id: " + planList.Id);
                            }
						} catch(Exception e){
							System.Diagnostics.Debug.WriteLine ("PlanList from Rest Call is missing a value" + e.Message);
						}
                    }
                
                } else
                {
                    System.Diagnostics.Debug.WriteLine("IsSuccessStatusCode = false - During plan fetching");
                    //App._notificationService.Notify(0, "Error", "IsSuccessStatusCode = false - During plan fetching");
                }

            } catch (Exception e)
                {
					System.Diagnostics.Debug.WriteLine ("Exception in LoadLogListItems()" + e.Message);
                   //App._notificationService.Notify(0, "Exception in LoadLogListItems()", e.Message);
                }
}

		public void OnPropertyChanged (string propertyName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}