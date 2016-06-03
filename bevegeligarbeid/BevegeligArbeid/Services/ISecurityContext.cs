// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Services
{
    using System.Collections.Generic;
    using TheWallClient.PCL.Service;

    public interface ISecurityContext
    {
        IDictionary<TargetServer, IAccessTokenProvider> TokenProviders { get; }

        Jwt Jwt { get; }

        string Subject { get; }

        string Name { get; }

        string Phone { get; }

        string Company { get; }

        string Contract { get; }

        string Login(string username, string password);

        bool IsLoggedIn();

        void Logout();
    }
}