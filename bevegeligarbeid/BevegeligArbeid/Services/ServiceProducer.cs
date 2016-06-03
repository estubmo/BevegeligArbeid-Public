// =====================================================
// AUTHOR: Triona AS
// NOTES: Modified to work with Loggbok For Bevegelig Arbeid
//======================================================
using BevegeligArbeid.Connectivity;

namespace BevegeligArbeid.Services
{
    public class ServiceProducer
    {
        private readonly SecurityContext securityContext;
        private readonly IConnectivity connectivity;

        public ServiceProducer(SecurityContext securityContext, IConnectivity connectivity)
        {
            this.securityContext = securityContext;
            this.connectivity = connectivity;
        }

        public BevegeligArbeidRestClient CreateBevegeligArbeidRestClient()
        {
            return new BevegeligArbeidRestClient(securityContext.TokenProviders[TargetServer.ArbeidsvarslingService], "http://arb-utv.intratriona.se/api");
            //return new BevegeligArbeidRestClient(this.securityContext.TokenProviders[TargetServer.ArbeidsvarslingService], "https://arb-test.triona.no/api");
        }
    }
}