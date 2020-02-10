using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using Android.Provider;
using Android.Graphics;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using Android.Util;
//using Firebase.Messaging;
//using Firebase.Iid;
using System.IO;
using Android;
using Plugin.DeviceInfo;
using RoundedBoxView.Forms.Plugin.Droid;
using Android.Views;
using Java.Security;
using Javax.Crypto;
using Android.Security.Keystore;
using Java.Util;
using Android.Runtime;
using MEI;
using Xamarin;
using FFImageLoading.Forms.Platform;

namespace MEI.Droid
{


    [Activity(ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop, Theme = "@style/noactionBar", WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.AdjustUnspecified)]//WindowSoftInputMode = SoftInput.AdjustPan|SoftInput.AdjustUnspecified
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {

        const int calenderRequestCode = 5;
        const int galleryRequestCode = 1;


        private KeyStore keyStore;
        private Cipher cipher;
        private string KEY_NAME = "MEI_FS";
        readonly string[] permisons = new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            base.OnCreate(savedInstanceState);
            try
            {
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                Forms.Init(this, savedInstanceState);
                FormsMaps.Init(this, savedInstanceState);                
                Forms.SetTitleBarVisibility(this,AndroidTitleBarVisibility.Never);

                App ap = new MEI.App();
                LoadApplication(ap);
                //this.ActionBar.Hide();
                CachedImageRenderer.Init(enableFastRenderer: true);
                RoundedBoxViewRenderer.Init();
                RequestPermissions(permisons, 0);
                App.ScreenWidth = (int)Resources.DisplayMetrics.WidthPixels;
                App.ScreenHeight = (int)Resources.DisplayMetrics.HeightPixels;
                App.CopyToClipBoard = CopyText;
                App.createContact = CreateContact;
                App.localPushNotification = LocalNotification;
                App.SaveUser = SaveUserID;
                App.GetUserEvent = SetGetUser;
                App.AppVersion = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName;
                App.cropImage = CropImage;
                App.ResetUser = ResetUserID;
                App.closeApp = CloseApp;
                App.SetDomainData = SetDomainData;
                App.SetNotificaiton = SetNotification;
                App.resetRoundView = ResetRoundView;
                App.SetNotificationSounds = SetNotificationSound;
                App.SetUserData = SetUserData;
                App.registerPhoneToServer = AddUserPhoneTOGCM;
                App.AddEventReminder = SetCurrentEventReminder;
                ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
                bool hasDirectory = pref.Contains("DomainDirectory");
                if (!hasDirectory)
                {
                    ISharedPreferencesEditor editor = pref.Edit();
                    editor.Clear();
                    editor.Apply();
                    editor.PutBoolean("DomainDirectory", App.NotificationSounds);
                    editor.Apply();
                }
                GetDomainData();
                GetNotification();
                GetUserData();
                GetNotificationSound();
                ap.StartApp();
                /* ThreadPool.QueueUserWorkItem(o =>
                 {
                     try
                     {
                         GoogleCloudMessaging gcm = GoogleCloudMessaging.GetInstance(this);
                         string id = gcm.Register("1040238333834");
                         App.phoneID = id;
                     }
                     catch(Exception e)
                     {
                         Console.WriteLine(e.ToString());
                     }
                 });*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Log.Debug("Creation Error", "Error in creating Application..");
            }
        }

        public void ResetRoundView(object sender, EventArgs e)
        {
            RoundedBoxViewRenderer.Init();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case calenderRequestCode:
                    SetCurrentEventReminder(this, null);
                    break;

            }
        }

        public async void SetCurrentEventReminder(object sender, EventArgs e)
        {
            if (CheckSelfPermission(Manifest.Permission.WriteCalendar.ToString()) == Android.Content.PM.Permission.Granted)
            {
                try
                {
                    if (!isEventInCal(this, App.reminderEvent.eventName))
                    {
                        ContentValues eventValues = new ContentValues();
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, 1);
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, App.reminderEvent.eventName);
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, App.reminderEvent.eventDescription);
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(BaseFunctions.GetDateTimeFull(App.reminderEvent.eventStartDate + " " + App.reminderEvent.eventStartTime)));
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(BaseFunctions.GetDateTimeFull(App.reminderEvent.eventEndDate + " " + App.reminderEvent.eventEndTime)));
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.AllDay, "0");
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.EventLocation, App.reminderEvent.eventAddress);
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.HasAlarm, "1");
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
                        eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
                        var eventUri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
                        long eventID = long.Parse(eventUri.LastPathSegment);
                        ContentValues reminderValues = new ContentValues();
                        reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 30);
                        reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
                        reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)RemindersMethod.Alert);
                        var reminderUri = ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
                        await App.Current.MainPage.DisplayAlert("Alert", "This event is added to your calender", "Ok");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "This event is already added to your calender", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            else
            {
                RequestPermissions(new string[] { Manifest.Permission.WriteCalendar.ToString() }, calenderRequestCode);
            }
        }

        public bool isEventInCal(Context context, String cal_meeting_id)
        {

            var cursor = ContentResolver.Query(
            Android.Net.Uri.Parse("content://com.android.calendar/events"),
              new String[] { "title" }, "title = ? ",
              new String[] { cal_meeting_id }, null);

            if (cursor.MoveToFirst())
            {
                return true;
            }
            return false;
        }

        long GetDateTimeMS(DateTime dateTime)
        {
            int yr = dateTime.Year;
            int month = dateTime.Month - 1;
            int day = dateTime.Day;
            int hr = dateTime.Hour;
            int min = dateTime.Minute;
            Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

            c.Set(CalendarField.DayOfMonth, day);
            c.Set(CalendarField.HourOfDay, hr);
            c.Set(CalendarField.Minute, min);
            c.Set(CalendarField.Month, month);
            c.Set(CalendarField.Year, yr);

            return c.TimeInMillis;
        }

        private bool CipherInit()
        {
            try
            {
                cipher = Cipher.GetInstance(KeyProperties.KeyAlgorithmAes
                    + "/"
                    + KeyProperties.BlockModeCbc
                    + "/"
                    + KeyProperties.EncryptionPaddingPkcs7);
                keyStore.Load(null);
                IKey key = (IKey)keyStore.GetKey(KEY_NAME, null);
                cipher.Init(CipherMode.EncryptMode, key);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void GenKey()
        {
            keyStore = KeyStore.GetInstance("AndroidKeyStore");
            KeyGenerator keyGenerator = null;
            keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, "AndroidKeyStore");
            keyStore.Load(null);
            keyGenerator.Init(new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes(KeyProperties.BlockModeCbc)
                .SetUserAuthenticationRequired(true)
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
                .Build());
            keyGenerator.GenerateKey();
        }

        public void AddUserPhoneTOGCM(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o =>
                    {

                        Uri address = new Uri("http://www.myeventit.com/PHP/AddPhone.php");
                        NameValueCollection nameValueCollection = new NameValueCollection();
                        nameValueCollection["deviceid"] = CrossDeviceInfo.Current.Id;
                        nameValueCollection["phonegcmid"] = App.phoneID;
                        nameValueCollection["userid"] = App.userID;
                        var webClient = new WebClient();
                        webClient.UploadValuesAsync(address, "POST", nameValueCollection);
                    });
        }

        public void SetNotification(object sender, EventArgs e)
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutBoolean("MEI_UserNotificaiton", App.NotificaitonEnabled);
            editor.Apply();
        }

        public void SetNotificationSound(object sender, EventArgs e)
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutBoolean("MEI_UserNotificaitonSounds", App.NotificationSounds);
            editor.Apply();
        }

        public void SetDomainData(object sender, EventArgs e)
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("MEI_AllDomainData", App.allDomainEvents);
            editor.Apply();
        }

        public void GetDomainData()
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            if (pref.Contains("MEI_AllDomainData"))
            {
                App.allDomainEvents = pref.GetString("MEI_AllDomainData", String.Empty);
            }
            else
            {
                App.allDomainEvents = String.Empty;
            }
        }

        public void SetUserData(object sender, EventArgs e)
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("MEI_UserData", App.userProfileData);
            editor.Apply();
        }

        public void GetUserData()
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            if (pref.Contains("MEI_UserData"))
            {
                App.userProfileData = pref.GetString("MEI_UserData", String.Empty);
            }
            else
            {
                App.userProfileData = String.Empty;
            }
        }

        public void GetNotification()
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            if (pref.Contains("MEI_UserNotificaiton"))
            {
                App.NotificaitonEnabled = pref.GetBoolean("MEI_UserNotificaiton", true);
            }
            else
            {
                SetNotification(this, null);
            }
        }

        public void GetNotificationSound()
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            if (pref.Contains("MEI_UserNotificaitonSounds"))
            {
                App.NotificationSounds = pref.GetBoolean("MEI_UserNotificaitonSounds", true);
            }
            else
            {
                SetNotificationSound(this, null);
            }
        }

        public void SetGetUser(object sender, EventArgs e)
        {
            App.GetUser = isLogged(this, null);
        }

        public bool CheckInternetConnection()
        {
            string CheckUrl = "http://google.com";

            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                iNetRequest.Timeout = 5000;

                WebResponse iNetResponse = iNetRequest.GetResponse();

                iNetResponse.Close();

                return true;

            }
            catch (WebException ex)
            {
                return false;
            }
        }


        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        public void CopyText(object s, EventArgs e)
        {
            CopyClipBoard(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventFaq.wifiDetails.password);
        }

        public void CropImage(object s, EventArgs e)
        {
            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage.ToString()) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage.ToString() }, galleryRequestCode);
            }
            else
            {
                Intent imageDownload = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                imageDownload.PutExtra("crop", "true");
                imageDownload.PutExtra("aspectX", 1);
                imageDownload.PutExtra("aspectY", 1);
                imageDownload.PutExtra("outputX", 300);
                imageDownload.PutExtra("outputY", 300);
                imageDownload.PutExtra("return-data", true);
                StartActivityForResult(imageDownload, 2);
            }
        }


        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private Bitmap decodeUriAsBitmap(Android.Net.Uri uri)
        {
            Bitmap bitmap = null;
            try
            {
                bitmap = BitmapFactory.DecodeStream(ContentResolver.OpenInputStream(uri));
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
            return bitmap;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 2 && resultCode == Result.Ok && data != null)
            {
                Bitmap image;
                if (data.Extras != null)
                {
                    Bundle extras = data.Extras;
                    image = extras.GetParcelable("data") as Bitmap;
                }
                else
                {
                    image = decodeUriAsBitmap(data.Data);
                }


                MemoryStream ms = new MemoryStream();
                image.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                ms.Seek(0L, SeekOrigin.Begin);
                App.uploadImageStream.Source = ImageSource.FromStream(() => { return ms; });
                App.uploadStream = ms.ToArray();
            }
        }

        public void CopyClipBoard(string text)
        {
            var clipboard = (ClipboardManager)Forms.Context.GetSystemService(Context.ClipboardService);
            var clip = ClipData.NewPlainText("Wifi Password", text);

            clipboard.PrimaryClip = clip;

        }
        public override void OnBackPressed()
        {
            if (App.Current.MainPage.GetType() == typeof(HomeLayout))
            {
                ((HomeLayout)App.Current.MainPage).OnBackPressed();
            }
            //base.OnBackPressed();
        }

        public void CloseApp(object sender, EventArgs e)
        {
            Finish();
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public void CreateContact(object sender, EventArgs e)
        {
            if (CheckSelfPermission(Manifest.Permission.WriteContacts.ToString()) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.WriteContacts.ToString() }, 1);
            }
            else
            {
                var intent = new Intent(Intent.ActionInsert);
                intent.SetType(ContactsContract.Contacts.ContentType);
                intent.PutExtra(ContactsContract.Intents.Insert.Name, App.contactuser.FirstName + " " + App.contactuser.LastName);
                intent.PutExtra(ContactsContract.Intents.Insert.Company, App.contactuser.company);
                intent.PutExtra(ContactsContract.Intents.Insert.Phone, App.contactuser.phoneNumber);
                intent.PutExtra(ContactsContract.Intents.Insert.Email, App.contactuser.email);
                StartActivity(intent);
            }
        }


        public void LocalNotification(object sender, EventArgs e)
        {
            // Instantiate the builder and set notification elements:
            string channelID = "General";
            const int notificationID = 0;
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            Notification.Builder notificationBuilder = new Notification.Builder(this, channelID)
                .SetContentTitle(App.lpushItem.title)
                .SetContentText(App.lpushItem.message)
                .SetAutoCancel(true)
                .SetSmallIcon(Resource.Drawable.mei_notification)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.mei_appicon_g))
                .SetContentIntent(pendingIntent);
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            NotificationChannel mChannel = new NotificationChannel(channelID, "name", NotificationImportance.High);
            Notification notification = notificationBuilder.Build();
            notification.Sound = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
            notification.Defaults = App.NotificationSounds ? NotificationDefaults.Sound : NotificationDefaults.Lights;

            notificationManager.CreateNotificationChannel(mChannel);
            notificationManager.Notify(notificationID, notification);
        }

        public void SaveUserID(object sender, EventArgs e)
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("MEI_UserID", App.userID.Trim());
            editor.Apply();
        }

        public void ResetUserID(object sender, EventArgs e)
        {
            App.GetUser = String.Empty;
            App.userID = String.Empty;
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("MEI_UserID", String.Empty);
            editor.PutString("MEI_AllDomainData", String.Empty);
            editor.PutString("MEI_UserData", String.Empty);
            editor.Clear();
            editor.Apply();
            pref.Dispose();
        }


        public string isLogged(object sender, EventArgs e)
        {
            ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
            if (pref.Contains("MEI_UserID"))
            {
                return pref.GetString("MEI_UserID", String.Empty);
            }
            return String.Empty;
        }
    }
}
    /*
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {       
        public override void OnMessageReceived(string from, Bundle data)
        {
            try
            {
                ISharedPreferences pref = GetSharedPreferences("MEI_UserPreferences", FileCreationMode.Private);
                if (pref.Contains("MEI_UserNotificaiton"))
                {
                    if(pref.GetBoolean("MEI_UserNotificaiton", true)&& !string.IsNullOrEmpty(pref.GetString("MEI_UserData", String.Empty)))
                    {
                        var message = App.HtmlToPlainText(data.GetString("message"));
                        var header = data.GetString("header");
                        var imageURL = data.GetString("image");
                        if (imageURL == ""||imageURL.ToLower().Contains("pdf"))
                            SendNotification(message, header);
                        else
                            SendNotification(message, header, imageURL);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void SendNotification(string message, string header)
        {
            try
            {

                
                string channelID = "General";
                const int notificationID = 0;
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
                ShortcutBadger.ApplyCount(BaseContext, 1);
                
                var notificationBuilder = new Notification.Builder(this, channelID)
                    .SetSmallIcon(Resource.Drawable.mei_notification)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.mei_appicon_g))
                    .SetContentTitle(header)
                    .SetContentText(message)
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);
                var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                NotificationChannel mChannel = new NotificationChannel(channelID, "name", NotificationImportance.High);
                Notification notification = notificationBuilder.Build();
                notification.Sound = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
                notification.Defaults = App.NotificationSounds ? NotificationDefaults.Sound : NotificationDefaults.Lights;
                
                notificationManager.CreateNotificationChannel(mChannel);
                notificationManager.Notify(notificationID, notification);
                    
                
                try
                {
                    if (App.Current.MainPage != null)
                    {
                        if (App.Current.MainPage.GetType() == typeof(HomeLayout))
                        {
                            ((HomeLayout)App.Current.MainPage).ResetRegisteredDomainList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void SendNotification(string message,string header,string image)
        {
            try
            {
                string channelID = "General";
                const int notificationID = 0;
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
                ShortcutBadger.ApplyCount(BaseContext, 1);
                var notificationBuilder = new Notification.Builder(this,channelID)
                    .SetSmallIcon(Resource.Drawable.mei_notification)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.mei_appicon_g))
                    .SetContentTitle(header)
                    .SetContentText(message)                    
                    .SetAutoCancel(true)
                    .SetStyle(new Notification.BigPictureStyle()
                    .BigPicture(GetImageBitmapFromUrl(image)).SetSummaryText(message))
                    .SetContentIntent(pendingIntent);
                var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                NotificationChannel mChannel = new NotificationChannel(channelID, "name", NotificationImportance.High);
                Notification notification = notificationBuilder.Build();
                notification.Sound = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
                notification.Defaults = App.NotificationSounds ? NotificationDefaults.Sound : NotificationDefaults.Lights;

                notificationManager.CreateNotificationChannel(mChannel);
                notificationManager.Notify(notificationID, notification);
                try
                {
                    if (App.Current.MainPage != null)
                    {
                        if (App.Current.MainPage.GetType() == typeof(HomeLayout))
                        {
                            ((HomeLayout)App.Current.MainPage).ResetRegisteredDomainList();
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

    }



    //[Service]
    //[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    //public class MyFirebaseIIDService : FirebaseInstanceIdService
    //{
    //    const string TAG = "MyFirebaseIIDService";
    //    public override void OnTokenRefresh()
    //    {
    //        var refreshedToken = FirebaseInstanceId.Instance.Token;
    //        Log.Debug(TAG, "Refreshed token: " + refreshedToken);
    //        SendRegistrationToServer(refreshedToken);
    //    }

    //    void SendRegistrationToServer(string token)
    //    {
    //        App.phoneID = token;
    //    }
    //}
}
*/

