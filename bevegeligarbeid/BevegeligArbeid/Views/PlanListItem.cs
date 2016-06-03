// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using System.ComponentModel;

namespace BevegeligArbeid.Views
{
    public class PlanListItem : INotifyPropertyChanged
	{
		private string _planNr = "Plannummer";
		private string _isActiveLogText = "";

		public string PlanNr { 
			get{ return _planNr; } 
			set
			{ 
				if (_planNr != value) {
					_planNr = value;
					OnPropertyChanged ("_planNr");
				}
			} 
		}

		public string IsActiveLogText { 
			get{ return _isActiveLogText; } 
			set { 
				if (_isActiveLogText != value) {
					_isActiveLogText = value;
					OnPropertyChanged ("_isActiveLogText");
				}
			}
		}

		public string Description { get; set; }
		public string RoadName { get; set; }
		public string Stretch{ get; set; }
		public string Date { get; set;}


		#region INPC

		public void OnPropertyChanged (string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}


