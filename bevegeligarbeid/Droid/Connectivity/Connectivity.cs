// =====================================================
// AUTHOR: Triona AS
// NOTES: 
//======================================================
using BevegeligArbeid.Connectivity;

using Android.Net;
using Android.App;
using Android.Content;

namespace BevegeligArbeid.Droid.Connectivity
{
    public class Connectivity : AbstractConnectivity
{

    private NetworkStatusBroadcastReceiver broadcastReceiver;

    protected override void UpdateNetworkStatus()
    {
        this.SetState(NetworkState.Unknown);

        // Retrieve the connectivity manager service
        var connectivityManager =
            (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);

        // Check if the network is connected or connecting.
        // This means that it will be available,
        // or become available in a few seconds.
        var activeNetworkInfo = connectivityManager.ActiveNetworkInfo;

        NetworkState state;
        if (activeNetworkInfo != null && activeNetworkInfo.IsConnectedOrConnecting)
        {
            // Now that we know it's connected, determine if we're on WiFi or something else.
            state = activeNetworkInfo.Type == ConnectivityType.Wifi
                ? NetworkState.ConnectedWifi
                : NetworkState.ConnectedData;
        }
        else
        {
            state = NetworkState.Disconnected;
        }
        this.SetState(state);
    }

    public override void Start()
    {
        if (this.broadcastReceiver != null)
        {
           System.Diagnostics.Debug.WriteLine("Network status monitoring already active.");
            return;
        }

        // Create the broadcast receiver and bind the event handler
        // so that the app gets updates of the network connectivity status
        this.broadcastReceiver = new NetworkStatusBroadcastReceiver();
        this.broadcastReceiver.ConnectionStatusChanged += this.OnNetworkStatusChanged;

        // Register the broadcast receiver
        Application.Context.RegisterReceiver(this.broadcastReceiver,
            new IntentFilter(ConnectivityManager.ConnectivityAction));
    }

    public override void Stop()
    {
        if (this.broadcastReceiver == null)
        {
                System.Diagnostics.Debug.WriteLine("Network status monitoring not active.");
            return;

        }

        // Unregister the receiver so we no longer get updates.
        Application.Context.UnregisterReceiver(this.broadcastReceiver);

        // Set the variable to nil, so that we know the receiver is no longer used.
        this.broadcastReceiver.ConnectionStatusChanged -= this.OnNetworkStatusChanged;
        this.broadcastReceiver = null;
    }
}
}