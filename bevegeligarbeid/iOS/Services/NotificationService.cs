// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using BevegeligArbeid.Services;
using UIKit;

namespace BevegeligArbeid.iOS.Services
{
    class NotificationService : INotificationService
    {
           public void Notify(int notificationId, string title, string text)
        {

			UILocalNotification notification = new UILocalNotification();
			notification.AlertAction = title;
			notification.AlertBody = text;
			UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}
