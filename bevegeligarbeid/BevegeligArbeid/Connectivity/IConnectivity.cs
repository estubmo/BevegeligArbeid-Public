// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Connectivity
{
    using System;

    public interface IConnectivity
    {
        event EventHandler NetworkStatusChanged;

       // ConnectivityStatus GetStatus();

        bool IsConnected();

        bool HasWiFi();

        void Start();

        void Stop();

    }

    public enum NetworkState
    {
        Unknown,

        ConnectedWifi,

        ConnectedData,

        Disconnected
    }
}