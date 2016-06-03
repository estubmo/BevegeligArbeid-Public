// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES: The application only supports test servers, this is where you can add additional servers
//======================================================
using System;
using System.Collections.Generic;

using TheWallClient.PCL.Service;
using TheWallClient.PCL.Persistence;
using BevegeligArbeid.Connectivity;
namespace BevegeligArbeid.Services
{
    public class Authenticator
    {
        private readonly Application application;
        private readonly UserClient userClient;
        private IJwtDao jwtDao;
        private ICryptoUtil cryptoUtil;
        private IJwtUtil jwtUtil;
        private ITheWallClient theWallClient;
        private Dictionary<TargetServer, IAccessTokenProvider> tokenProviders;
        public SecurityContext securityContext;
        public ServiceProducer serviceProducer;

        public IConnectivity connectivity { get; set; }
        public Dictionary<string, string> config { get; set; } 
        public string username { get; set; }
        public string password { get; set; }


        public Authenticator (Application application, UserClient userClient, IJwtDao jwtDao, ICryptoUtil cryptoUtil, IConnectivity connectivity)
		{
			this.application = application;
			this.userClient = userClient;
            this.jwtDao = jwtDao;
            this.cryptoUtil = cryptoUtil;
            this.connectivity = connectivity;

			this.config = new Dictionary<string, string> ();
            this.config.Add("arbtest", "https://arb-test.triona.no/api");
            this.config.Add("wallutv", "https://wallutv.triona.no/api/v3");
            this.config.Add("intratest", "http://arb-utv.intratriona.se/api");
            //this.config.Add("arbtest", "https://arb-test.triona.no/api");
            //this.config.Add("walltest", "https://wall-test.triona.no/api/v3");

            this.username = String.Empty;
            this.password = String.Empty;


            const string Key = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCh1PKnc9M2ag+UusDcFqS50nLu0V8fksHdEww6oPdMf04Dx1KXr9oEUNWP012W8/6LcQDFI945CTpBzXST9xKw8OC54UxsemWai2bssoUlOuEZicg/OjfZjrv8xmz88lbZy63wYafSj5ko80FqC0exrtUJb/+QXbQ/Id+1ZBGdhQIDAQAB";

		    this.jwtUtil = new JwtUtil(Key, "https://wall.triona.no", "https://arbeidsvarsling.triona.no");

            this.theWallClient = ServiceFactory.Create(
                this.config["wallutv"] + "/login",
                //this.config["walltest"] + "/login",
                jwtUtil, this.application, this.userClient
            );            
				
            this.tokenProviders = new Dictionary<TargetServer, IAccessTokenProvider>();
            this.securityContext = new SecurityContext(theWallClient, tokenProviders);
            tokenProviders[TargetServer.ArbeidsvarslingService] = ServiceFactory.CreatePersistentTokenProvider(ServiceFactory.Create(this.config["intratest"] + "/auth"), jwtUtil, jwtDao, cryptoUtil);
            //tokenProviders[TargetServer.ArbeidsvarslingService] = ServiceFactory.CreatePersistentTokenProvider(ServiceFactory.Create(this.config["arbtest"] + "/auth"), jwtUtil, jwtDao, cryptoUtil);
            this.serviceProducer = new ServiceProducer(securityContext, connectivity);
        }

        public string AttemptLoginAndGetAccessToken(string username, string password)
        {
            this.username = username;
            this.password = password;
            string accessToken = securityContext.Login(username, password);
            return accessToken;
        }

        public void Logout()
        {
            securityContext.Logout();
        }
    }
}

