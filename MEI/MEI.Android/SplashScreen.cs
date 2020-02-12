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
using System.Threading.Tasks;
using Java.Lang;
using Android.Content.PM;
using MEI.Droid;
using Android;
using Firebase;
using Plugin.FirebasePushNotification;

namespace MEI.Droid
{
    [Activity(MainLauncher = true, NoHistory = true,LaunchMode = LaunchMode.SingleTop, Theme = "@style/MyTheme.Splash", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.splash);
        }
     

        protected override void OnResume()
        {
            base.OnResume();
            Load();
        }
        public async void Load()
        {
            await Task.Delay(1000);
            await Task.Factory.StartNew(() => {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            });          
        }
    }
}