// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES:
//======================================================
using Android.App;
using Android.Content;
using BevegeligArbeid.Services;

namespace BevegeligArbeid.Droid.Services
{
    class NotificationService : INotificationService
    {
        public NotificationManager notificationManager;
        public Notification.Builder builder;
        public int notificationIdDecrementer;
        public NotificationService(MainActivity app)
        {
            notificationIdDecrementer = 0;
            builder = new Notification.Builder(app)
                .SetSmallIcon(Resource.Drawable.TrionaLogo);



            notificationManager =
                app.GetSystemService(Context.NotificationService) as NotificationManager;
        }

        public void Notify(int notificationId, string title, string text)
        {
            builder
                .SetContentTitle(title)
                .SetContentText(text);

            Notification notification = builder.Build();

            if (notificationId == 0) {
                notificationManager.Notify(notificationIdDecrementer, notification);
                notificationIdDecrementer--;
            } else {
                notificationManager.Notify(notificationId, notification);
            }
        }

    }
}