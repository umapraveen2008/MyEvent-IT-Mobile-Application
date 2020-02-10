using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class SettingsPage : ContentView
    {
        public SettingsPage()
        {
            InitializeComponent();

            privateProfile.IsToggled = BaseFunctions.CheckBool(App.serverData.mei_user.currentUser.userPrivacy);
            SetEditProfile();
            SetFeedbackAndSuggestions();
            SetChangePassword();
            SetApplicationTerms();
            SetPrivacyPolicy();
            SetPaymentHistory();
            SetPaymentList();
            SetShippingList();
            notSetting.IsToggled = App.NotificaitonEnabled;
            SetPushNotification();
            notSounds.IsToggled = App.NotificationSounds;
            SetPushNotificationSounds();
            SetPrivateProfile();
            SetLogout();
            if (Device.OS == TargetPlatform.Android)
                versionNumber.Text = "Ver " + App.AppVersion;
        }


        public void SetPrivateProfile()
        {
            privateProfile.Toggled += async  (s, e) =>
            {
                App.serverData.mei_user.currentUser.userPrivacy = privateProfile.IsToggled.ToString();
                var k = await BaseFunctions.SaveUserToServer();
            };
            
        }



        public void SetFeedbackAndSuggestions()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).CreateReportBugPage;
            feedbackAndSuggestions.GestureRecognizers.Add(tap);
        }

        public void SetPushNotification()
        {
            
            notSetting.Toggled += (s, e) =>
             {
                 App.lpushItem = new App.LocalPushNotificationItem();
                 App.lpushItem.title = "MyEvent it Push Notifications";

                 if (notSetting.IsToggled)
                 {
                     App.NotificaitonEnabled = true;
                     App.lpushItem.message = "Push notifications have been enabled";
                 }
                 else
                 {
                     App.NotificaitonEnabled = false;
                     App.lpushItem.message = "Push notifications have been disabled";
                 }
                 App.localPushNotification(this, null);
                 App.SetNotificaiton(this, null);
             };
        }

        public void SetHowToUse(object sender,EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).OpenEducation(sender,e);
        }

        public void SetPushNotificationSounds()
        {
            notSounds.Toggled += (s, e) =>
            {
                App.lpushItem = new App.LocalPushNotificationItem();
                App.lpushItem.title = "MyEvent it Push notifications sounds";

                if (notSounds.IsToggled)
                {
                    App.NotificationSounds = true;
                    App.lpushItem.message = "Push notifications sounds have been enabled";
                }
                else
                {
                    App.NotificationSounds = false;
                    App.lpushItem.message = "Push notifications sounds have been disabled";
                }
                App.localPushNotification(this, null);
                App.SetNotificationSounds(this, null);
            };
        }

        public void SetPaymentList()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).CreatePaymentList;
            paymentInformation.GestureRecognizers.Add(tap);
        }

        public void SetShippingList()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).CreateShippingList;
            addressBook.GestureRecognizers.Add(tap);
        }

        public void SetPaymentHistory()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).CreatePurchaseHistory;
            paymentHistory.GestureRecognizers.Add(tap);
        }

        public void SetEditProfile()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).CreateEditProfile;
            editProfile.GestureRecognizers.Add(tap);
        }

        public void SetChangePassword()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).CreateChangePassword;
            changePassword.GestureRecognizers.Add(tap);
        }

        public void SetApplicationTerms()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateWebView(s, e, "http://www.myeventit.com/termsandconditions/", "Application Terms"); };
            applicationTerms.GestureRecognizers.Add(tap);
        }

        public void SetPrivacyPolicy()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s,e)=> { ((HomeLayout)App.Current.MainPage).CreateWebView(s, e, "http://www.myeventit.com/privacypolicy/", "Privacy Policy"); };
            privacyPolicy.GestureRecognizers.Add(tap);
        }

        public void SetLogout()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ((HomeLayout)App.Current.MainPage).LogoutFunction;
            logoutButton.GestureRecognizers.Add(tap);
        }

        
    }

   
}
