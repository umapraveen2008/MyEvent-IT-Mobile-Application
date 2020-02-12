using FFImageLoading.Forms;
using MEI.Pages;
using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MEI
{
    public class MapEventArgs : EventArgs
    {
        public Position position;
        public string address;
    }

    public partial class App : Application
    {
        public static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public static string AppVersion = "1.0.0";
        public static INavigation Navigation { get; set; }
        public static bool inHome = false;
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static bool NotificationSounds = true;
        public static bool NotificaitonEnabled = true;
        public static EventHandler<MapEventArgs> iosMapClick;
        public static EventHandler SetNotificaiton;
        public static EventHandler SetNotificationSounds;
        public static EventHandler SetDomainData;
        public static EventHandler SetUserData;
        public static string phoneID;
        public static EventHandler CopyToClipBoard;
        public static EventHandler createContact;
        public static VcardContact contactuser { get; set; }
        public static LocalPushNotificationItem lpushItem;
        public static EventHandler SaveUser;
        public static EventHandler GetUserEvent;
        public static EventHandler ResetUser;
        public static string GetUser;
        public static bool eventCreation = false;
        public static bool checkForUpdates = true;
        public static bool gettingUpdates = false;
        public static EventHandler localPushNotification;
        public static EventHandler closeApp;
        public static string registerPhoneID;
        public static string allDomainEvents = String.Empty;
        public static string userProfileData = String.Empty;
        public static GetServerData serverData = new GetServerData();
        public static EventHandler<FirebasePushNotificationDataEventArgs> PushNotification;
        public static bool AppHaveInternet = true;
        public static EventHandler cropImage;
        public static string userID;
        public static ServerEvent reminderEvent;
        public static EventHandler AddEventReminder;
        public static EventHandler resetRoundView;
        public static Image uploadImageStream;
        public static bool AppResume = true;
        public static bool AppSleep = false;
        public static string firmID = "";
        public static byte[] uploadStream;
        public static AppCartList AppCart = new AppCartList();
        public static bool FirstTime;

        public struct LocalPushNotificationItem
        {
            public string title;
            public string message;
        }

        public static VcardContact SetContactUser
        {
            get { return contactuser; }
            set { contactuser = value; }
        }
        public App()
        {
            // The root page of your application      
            InitializeComponent();
            //MainPage = new LoginPage();

        }

        protected override void OnStart()
        {

            //ResetUser(this, null);
            //if (!string.IsNullOrEmpty(GetUser))
            //GetUserEvent(this, null);
            // MainPage = new LoginPage();
            // Handle when your app starts            
            
        }

        public void StartApp()
        {
            GetUserEvent(this, null);
            CrossFirebasePushNotification.Current.RegisterForPushNotifications();
            CrossFirebasePushNotification.Current.Subscribe("MEI");
            CrossFirebasePushNotification.Current.NotificationHandler = null;
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                App.phoneID = p.Token;
                System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");                
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    PushNotification(s, p);
                }
                catch (Exception ex)
                {

                }

            };
            MainPage = new LoginPage();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            AppSleep = true;
            AppResume = false;
        }

        protected override void OnResume()
        {
            AppSleep = false;
            if (App.Current.MainPage.GetType() == typeof(HomeLayout))
                AppResume = true;
            // Handle when your app resumes
        }
    }
}
