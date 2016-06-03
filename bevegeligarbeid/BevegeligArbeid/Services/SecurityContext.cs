// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using System.Collections.Generic;
using TheWallClient.PCL.Service;

namespace BevegeligArbeid.Services
{
    public class SecurityContext : ISecurityContext
    {
        private readonly ITheWallClient wallClient;

        public IDictionary<TargetServer, IAccessTokenProvider> TokenProviders { get; private set; }


        public Jwt Jwt
        {
            get
            {
                return this.TokenProviders[TargetServer.ArbeidsvarslingService].Jwt;
            }
        }

        public string Subject
        {
            get
            {
                return this.Jwt.Subject;
            }
        }

        public string Company
        {
            get
            {
                return this.GetJwtCustomClaim("urn:user:company");
            }
        }

        public string Contract
        {
            get
            {
                return this.GetJwtCustomClaim("urn:user:contract");
            }
        }

        public string Name
        {
            get
            {
                return this.GetJwtCustomClaim("urn:user:name");
            }
        }

        public string Phone
        {
            get
            {
                return this.GetJwtCustomClaim("urn:user:phone");
            }
        }

        private string GetJwtCustomClaim(string fieldName)
        {
            if (!this.IsLoggedIn())
            {
                return null;
            }
            if (this.Jwt.CustomClaims == null || !this.Jwt.CustomClaims.ContainsKey(fieldName))
            {
                return null;
            }
            return this.Jwt.CustomClaims[fieldName] as string;
        }

        public SecurityContext(ITheWallClient wallClient, IDictionary<TargetServer, IAccessTokenProvider> tokenProvider)
        {
            this.wallClient = wallClient;
            this.TokenProviders = tokenProvider;
        }

        public string Login(string username, string password)
        {
            Jwt jwt = this.wallClient.Authenticate(username, password);
            ((PersistentAccessTokenProvider)this.TokenProviders[TargetServer.ArbeidsvarslingService]).Jwt = jwt;
            System.Diagnostics.Debug.WriteLine("User {0} logged in successfully to BevegeligArbeidApp.", username);
            return this.TokenProviders[TargetServer.ArbeidsvarslingService].AccessToken;
        }


        public bool IsLoggedIn()
        {
            return this.Jwt != null;
        }

    
        public void Logout()
        {
            string u = this.Subject;
            ((PersistentAccessTokenProvider)this.TokenProviders[TargetServer.ArbeidsvarslingService]).ClearJwt();

            System.Diagnostics.Debug.WriteLine("User {0} logged out from BevegeligArbeidApp.", u);
        }
    }
}
