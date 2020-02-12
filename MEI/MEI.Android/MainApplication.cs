using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;

namespace MEI.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();            
            FirebaseApp.InitializeApp(this);
            CrossCurrentActivity.Current.Init(this);
            //Set the default notification channel for your app when running Android Oreo            


            //If debug you should reset the token each time.
#if DEBUG            
            FirebasePushNotificationManager.Initialize(this,false);
#else
              FirebasePushNotificationManager.Initialize(this,false);
#endif
            

        }
    }
}