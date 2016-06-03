// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Droid.Connectivity
{
    using System;

    using Android.Content;

    [BroadcastReceiver()]
    public class NetworkStatusBroadcastReceiver : BroadcastReceiver
    {

        public event EventHandler ConnectionStatusChanged;

        public override void OnReceive(Context context, Intent intent)
        {
            this.ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}