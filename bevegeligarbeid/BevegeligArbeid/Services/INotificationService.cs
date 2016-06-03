// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
namespace BevegeligArbeid.Services
{
    public interface INotificationService
    {
        void Notify(int notificationId, string title, string text);
    }
}
