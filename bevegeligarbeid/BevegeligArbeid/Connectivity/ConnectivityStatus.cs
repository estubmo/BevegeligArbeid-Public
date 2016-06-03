// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
namespace BevegeligArbeid.Connectivity
{
    public sealed class ConnectivityStatus
    {
        private readonly int value;

        private readonly string description;

        public static readonly ConnectivityStatus NoNet = new ConnectivityStatus(1, "Nettverk ikke tilgjengelig ");

        public static readonly ConnectivityStatus OnLine = new ConnectivityStatus(3, "Har forbindelse med server");

        private ConnectivityStatus(int value, string description)
        {
            this.value = value;
            this.description = description;
        }

        public int GetIntValue()
        {
            return this.value;
        }

        public override string ToString()
        {
            return this.description;
        }
    }

}