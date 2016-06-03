// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using System;

namespace BevegeligArbeid.Connectivity
{

    public abstract class AbstractConnectivity : IConnectivity
    {
        private NetworkState networkState = NetworkState.Unknown;

        public bool IsConnected()
        {
            return this.networkState == NetworkState.ConnectedData || this.networkState == NetworkState.ConnectedWifi;
        }

        public bool HasWiFi()
        {
            return this.networkState == NetworkState.ConnectedWifi;
        }

        public event EventHandler NetworkStatusChanged;

        public ConnectivityStatus GetStatus()
        {
            if (!this.IsConnected())
            {
                return ConnectivityStatus.NoNet;
            }

            return ConnectivityStatus.OnLine;
        }

        public abstract void Start();

        public abstract void Stop();

        protected abstract void UpdateNetworkStatus();

        protected void OnNetworkStatusChanged(object sender, EventArgs e)
        {
            var currentStatus = this.networkState;

            this.UpdateNetworkStatus();

            if (currentStatus != this.networkState)
            {
                this.UpdateStatus();
            }
        }


        private void UpdateStatus()
        {
            if (this.NetworkStatusChanged != null)
            {
                this.NetworkStatusChanged(this, EventArgs.Empty);
            }
           //App._notificationService.Notify(0, "NetworkStatusChanged", "IsConnected = " + IsConnected());
            //System.Diagnostics.Debug.WriteLine("NetworkStatusChanged - Isconnected = " + IsConnected());
        }

        protected void SetState(NetworkState state)
        {
            this.networkState = state;
        }

    }
}
