// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using Xamarin.Forms;
using System.Windows.Input;
using System.ComponentModel;
using System;

namespace BevegeligArbeid.ViewModels
{

    public class LoginViewModel : INotifyPropertyChanged
	{
		public static bool isLoggedIn;
		public string Title { get; set; }
		public string AppName { get; set; }
		public string ContactInfo { get; set; }

		public string UsernamePlaceholder { get; set; }
		public string PasswordPlaceholder { get; set; }
		public string LoginButtonText { get; set; }
		bool _isBusy;

		public bool IsBusy {
			get {
				return _isBusy;
			}
			set {
				_isBusy = value;
				OnPropertyChanged ("IsBusy");
				OnPropertyChanged ("NotIsBusy");
			}
		}
		public bool NotIsBusy { get{ return !IsBusy;} }
		public string Username { get;	set; }
		public string Password {	get; set;}

		string _status;
		public string Status {
			get {
				return _status;
			}
			set {
				_status = value;
				OnPropertyChanged ("Status");
			}
		}

		public ICommand LoginCommand { get; set; }

		public LoginViewModel ()
		{
			Title = "Innlogging";
			AppName = "Loggbok for Bevegelig Arbeid";
			UsernamePlaceholder = "Brukernavn";
			PasswordPlaceholder = "Passord";
			ContactInfo = "Kontakt salg@triona.no eller 72 90 00 30 for lisens";
			LoginButtonText = "Logg inn";
			IsBusy = false;
			Status = string.Empty;
			LoginCommand = new Command (LoginButtonPressed);
		}

		public void LoginButtonPressed(){
			IsBusy = true;
            bool loginSuccessful = ExecuteLogin(Username, Password);

            if (loginSuccessful){
				Status = "Innlogging suksessfull";
                App.OnLogin();
                
			} 

			IsBusy = false;
	}	

		public bool ExecuteLogin(string username, string password){
            try
            {
                string accessToken = null;
                accessToken = App._auth.AttemptLoginAndGetAccessToken(username, password);
                if(accessToken != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Equals("StatusCode = Unauthorized"))
                {
                    Status = "Uatorisert bruker";
                }
                else
                {
                    Status = e.Message;
                }
                System.Diagnostics.Debug.WriteLine(e.Message);
               
            }
            return false;
		}
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


