// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES: Made just to have something to put into the App constructor in AppDelegate
//======================================================
using TheWallClient.PCL.Persistence;

namespace BevegeligArbeid.iOS
{
    public class JwtDao : IJwtDao
	{
		public JwtDao (){
		}

		public void Delete(){
		}

		public string Load(){
			return "Load";
		}

		public void Save(string jwt){
		}
	}
}

