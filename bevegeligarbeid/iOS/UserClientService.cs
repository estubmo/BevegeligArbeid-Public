// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using TheWallClient.PCL.Service;
using UIKit;

namespace BevegeligArbeid.iOS
{
    public class UserClientService : IUserClientService
	{
		public UserClient Build()
		{
			return new UserClient
			{
				Type = "device",
				Guid = UIDevice.CurrentDevice.IdentifierForVendor.AsString(),
				Manufacturer = "apple",
				Model = UIDevice.CurrentDevice.Model,
				OperatingSystem = TheWallClient.PCL.Service.OperatingSystem.Ios,
				OperatingSystemVersion = UIDevice.CurrentDevice.SystemVersion
			};
		}
	}
}

