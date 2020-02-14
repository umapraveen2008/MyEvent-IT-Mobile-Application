using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Diagnostics;
using Firebase;
using Firebase.CloudMessaging;
using System.Collections.Specialized;
using System.Net;
using Contacts;
using ContactsUI;
using Xamarin.Forms;
using CoreGraphics;
using UserNotifications;
using RoundedBoxView.Forms.Plugin.iOSUnified;
using Firebase.InstanceID;
using System.IO;
using EventKit;
using Plugin.DeviceInfo;
using CoreLocation;
using Xamarin.Forms.Maps;
using Badge.Plugin;
using MEI;
using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;
using ObjCRuntime;

namespace MEI.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {


            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.FormsMaps.Init();
            App ap = new  App();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            RoundedBoxViewRenderer.Init();

            App.createContact = CreateContact;
            App.CopyToClipBoard = CopyText;
            App.localPushNotification = PushNotification;
            App.SaveUser = SaveUser;
            App.GetUserEvent = SetGetUser;
            App.ResetUser = ResetUser;
            App.cropImage = CropImage;
            App.AppVersion = (NSBundle.MainBundle.InfoDictionary["CFBundleVersion"] as NSString).ToString();
            App.SetNotificaiton = SetNotification;
            App.SetNotificationSounds = SetNotificationSound;
            App.SetDomainData = SaveDomainData;
            App.AddEventReminder = AddEventToCalender;
            App.PushNotification = IOSNotification;
            App.SetUserData = SaveUserData;
            App.iosMapClick = OpenPin;
            if(NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("DirectoryUpdate")) == null)
            {
                NSUserDefaults.StandardUserDefaults.SetBool(true, "DirectoryUpdate");
                ResetUser(this, null);
            }
            GetDomainData();
            GetUserData();
            GetNotification();
            GetNotificationSound();
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
            FirebasePushNotificationManager.Initialize(options,true);          
            CrossBadge.Current.ClearBadge();
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    Console.WriteLine(granted);
                });

                UNUserNotificationCenter.Current.Delegate = this;
                UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
                {
                    var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
                });
            }
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
            FirebasePushNotificationManager.Initialize(options, true);
            LoadApplication(ap);
            ap.StartApp();
            return base.FinishedLaunching(app, options);

        }

        public void OpenPin(object sender, MapEventArgs e)
        {
            Position p = e.position;
            string address = e.address;
            CLLocationCoordinate2D center = new CLLocationCoordinate2D(p.Latitude, p.Longitude);
            MapKit.MKPlacemarkAddress m_address = new MapKit.MKPlacemarkAddress(new Foundation.NSDictionary());

            MapKit.MKPlacemark placeMark = new MapKit.MKPlacemark(center, m_address);
            var map = new MapKit.MKMapItem(placeMark);


            MapKit.MKCoordinateSpan corSpan = new MapKit.MKCoordinateSpan();
            corSpan.LatitudeDelta = p.Latitude;
            corSpan.LongitudeDelta = p.Longitude;
            MapKit.MKLaunchOptions options = new MapKit.MKLaunchOptions();
            options.DirectionsMode = MapKit.MKDirectionsMode.Driving;
            options.MapCenter = center;
            map.Name = address;

            options.MapSpan = corSpan;
            options.MapType = MapKit.MKMapType.Standard;
            MapKit.MKMapItem current = MapKit.MKMapItem.MapItemForCurrentLocation();
            MapKit.MKMapItem[] arrayKit = new MapKit.MKMapItem[] { current, map };
            MapKit.MKMapItem.OpenMaps(arrayKit, options);
        }

        public async void AddEventToCalender(object sender, EventArgs e)
        {
            try
            {
                var store = new EKEventStore();
                if (EKEventStore.GetAuthorizationStatus(EKEntityType.Reminder) == EKAuthorizationStatus.Authorized)
                {
                    if (EKEventStore.GetAuthorizationStatus(EKEntityType.Event) == EKAuthorizationStatus.Authorized)
                    {
                        NSDate startDate = ConvertDateTimeToNSDate(BaseFunctions.GetDateTimeFull(App.reminderEvent.eventStartDate + " " + App.reminderEvent.eventStartTime));
                        NSDate endDate = ConvertDateTimeToNSDate(BaseFunctions.GetDateTimeFull(App.reminderEvent.eventEndDate + " " + App.reminderEvent.eventEndTime));
                        NSPredicate query = store.PredicateForEvents(startDate, endDate, null);
                        EKCalendarItem[] events = store.EventsMatching(query);
                        bool exists = false;
                        for (int i = 0; i < events.Length; i++)
                        {
                            if (events[i].Title == App.reminderEvent.eventName)
                                exists = true;
                        }
                        if (!exists)
                        {
                            EKEvent eEvent = EKEvent.FromStore(store);
                            eEvent.AddAlarm(EKAlarm.FromDate(ConvertDateTimeToNSDate(BaseFunctions.GetDateTimeFull(App.reminderEvent.eventStartDate + " " + App.reminderEvent.eventStartTime))));
                            eEvent.StartDate = startDate;
                            eEvent.EndDate = endDate;
                            eEvent.Title = App.reminderEvent.eventName;
                            eEvent.TimeZone = new NSTimeZone("UTC");
                            eEvent.Location = App.reminderEvent.eventAddress;
                            eEvent.Notes = App.reminderEvent.eventDescription;
                            eEvent.Calendar = store.DefaultCalendarForNewEvents;
                            NSError eventError;
                            store.SaveEvent(eEvent, EKSpan.ThisEvent, out eventError);

                            EKReminder reminder = EKReminder.Create(store);
                            reminder.Title = App.reminderEvent.eventName;
                            reminder.AddAlarm(EKAlarm.FromDate(ConvertDateTimeToNSDate(BaseFunctions.GetDateTimeFull(App.reminderEvent.eventStartDate + " " + App.reminderEvent.eventStartTime))));
                            reminder.TimeZone = new NSTimeZone("UTC");
                            reminder.Calendar = store.DefaultCalendarForNewEvents;
                            await App.Current.MainPage.DisplayAlert("Alert", "This event is added to your calender", "Ok");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "This event is already added to your calender", "Ok");
                        }
                    }
                    else
                    {
                        store.RequestAccess(EKEntityType.Event, StoreAcceptRequest);
                    }
                }
                else
                {
                    store.RequestAccess(EKEntityType.Reminder, StoreAcceptRequest);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public NSDate ConvertDateTimeToNSDate(DateTime date)
        {
            DateTime newDate = System.TimeZoneInfo.ConvertTime(
                new DateTime(2001, 1, 1, 0, 0, 0),TimeZoneInfo.Local);
            return NSDate.FromTimeIntervalSinceReferenceDate(
                (date - newDate).TotalSeconds);
        }

        private void StoreAcceptRequest(bool arg1, NSError arg2)
        {
            AddEventToCalender(this, null);
        }

        public void SetNotification(object sender, EventArgs e)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(App.NotificaitonEnabled, "UserNotificaiton");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public void SetNotificationSound(object sender, EventArgs e)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(App.NotificationSounds, "UserNotificaitonSounds");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public void GetNotification()
        {
            if (NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("UserNotificaiton")) != null)
            {
                App.NotificaitonEnabled = NSUserDefaults.StandardUserDefaults.BoolForKey("UserNotificaiton");
            }
            else

            {
                NSUserDefaults.StandardUserDefaults.SetBool(App.NotificaitonEnabled, "UserNotificaiton");
                NSUserDefaults.StandardUserDefaults.Synchronize();
            }
        }

        public void GetNotificationSound()
        {
            if (NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("UserNotificaitonSounds")) != null)
            {
                App.NotificationSounds = NSUserDefaults.StandardUserDefaults.BoolForKey("UserNotificaitonSounds");
            }
            else
            {
                NSUserDefaults.StandardUserDefaults.SetBool(App.NotificaitonEnabled, "UserNotificaitonSounds");
                NSUserDefaults.StandardUserDefaults.Synchronize();
            }
        }

        public void SetGetUser(object sender, EventArgs e)
        {
            var userid = GetUser(this, null);
            if (userid == null)
                App.GetUser = String.Empty;
            else
                App.GetUser = userid;
        }

        public void AddUserPhoneTOGCM(object sender, EventArgs e)
        {
            //App.phoneID = InstanceId.SharedInstance.Token;
            Uri address = new Uri("http://www.myeventit.com/PHP/AddPhone.php");
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection["deviceid"] = CrossDeviceInfo.Current.Id;
            nameValueCollection["phonegcmid"] = App.phoneID;
            nameValueCollection["userid"] = App.userID;
            var webClient = new WebClient();
            webClient.UploadValuesAsync(address, "POST", nameValueCollection);
        }

        public override void DidEnterBackground(UIApplication application)
        {            
            Console.WriteLine("Disconnected from FCM");
        }

        

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            System.Console.WriteLine(notification.Request.Content.UserInfo);
            var notifications = UNNotificationPresentationOptions.Alert;
            completionHandler(notifications);
        }

        public void IOSNotification(object sender, FirebasePushNotificationDataEventArgs fps)
        {
            CrossBadge.Current.SetBadge(1);
            if (NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("UserNotificaiton")) != null && NSUserDefaults.StandardUserDefaults.BoolForKey("UserNotificaiton") && !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey("MEI_UserID")))
            {
                string title = (fps.Data[new NSString("header")] as NSString).ToString();
                string message = App.HtmlToPlainText((fps.Data[new NSString("message")] as NSString).ToString());
                string imageURL = (fps.Data[new NSString("image")] as NSString).ToString();
                var notificaiton = new UNMutableNotificationContent();

                notificaiton.Title = title;
                notificaiton.Body = App.HtmlToPlainText(message);
                if (!string.IsNullOrEmpty(imageURL) && !imageURL.ToLower().Contains("pdf"))
                {
                    var url = new Uri(imageURL.ToString());
                    var webClient = new WebClient();
                    webClient.DownloadDataAsync(url);
                    webClient.DownloadDataCompleted += (s, ex) =>
                    {
                        var bytes = ex.Result; // get the downloaded data
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        string localFilename = "downloaded.png";
                        string localPath = Path.Combine(documentsPath, localFilename);
                        File.WriteAllBytes(localPath, bytes);
                        var localUrl = NSUrl.FromString("file:///" + localPath);
                        var attachmentID = "image";
                        var options = new UNNotificationAttachmentOptions();
                        NSError _error;
                        var attachment = UNNotificationAttachment.FromIdentifier(attachmentID, localUrl, options, out _error);
                        if (_error == null)
                        {
                            notificaiton.Attachments = new UNNotificationAttachment[] { attachment };
                            notificaiton.Badge = 1;
                            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);
                            var requestID = "MEI_LocalNotification";
                            var request = UNNotificationRequest.FromIdentifier(requestID, notificaiton, trigger);
                            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
                            {
                                if (error != null)
                                {

                                }
                            });
                        }
                        else
                        {
                            Debug.Write(_error.ToString());
                        }
                    };

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
                else
                {
                    notificaiton.Badge = 1;
                    var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);
                    var requestID = "MEI_LocalNotification";
                    var request = UNNotificationRequest.FromIdentifier(requestID, notificaiton, trigger);
                    UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
                    {
                        if (err != null)
                        {

                        }
                    });

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
            }

        }

        static CNContactViewController personViewController;
        static UINavigationController navController;


        public static void CopyText(object s, EventArgs e)
        {
            CopyClipBoard(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventFaq.wifiDetails.password);
        }

        public static void PushNotification(object s, EventArgs e)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var notificaiton = new UNMutableNotificationContent();
                notificaiton.Title = App.lpushItem.title;
                notificaiton.Body = App.lpushItem.message;
                notificaiton.Badge = 1;
                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
                var requestID = "MEI_LocalNotification";
                var request = UNNotificationRequest.FromIdentifier(requestID, notificaiton, trigger);
                UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
                {
                    if (err != null)
                    {

                    }
                });
            }
            else
            {
                UILocalNotification notification = new UILocalNotification();
                notification.AlertAction = App.lpushItem.title;
                notification.AlertBody = App.lpushItem.message;
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
            //UILocalNotification notification = new UILocalNotification();
            //notification.AlertAction = App.lpushItem.title;
            //notification.AlertBody = App.lpushItem.message;
            //notification.FireDate = NSDate.FromTimeIntervalSinceNow(0);
            //notification.ApplicationIconBadgeNumber = 1;
            //notification.SoundName = UILocalNotification.DefaultSoundName;
            //UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public static void CopyClipBoard(string text)
        {
            UIPasteboard clipboard = UIPasteboard.General;
            clipboard.String = text;
        }

        public static void SaveUser(object sender, EventArgs e)
        {
            NSUserDefaults.StandardUserDefaults.SetString(App.userID, "MEI_UserID");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public static void SaveDomainData(object sender, EventArgs e)
        {
            NSUserDefaults.StandardUserDefaults.SetString(App.allDomainEvents, "MEI_DomainData");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public static void SaveUserData(object sender, EventArgs e)
        {
            NSUserDefaults.StandardUserDefaults.SetString(App.userProfileData, "MEI_UserData");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public static void ResetUser(object sender, EventArgs e)
        {
            App.userID = String.Empty;
            App.GetUser = String.Empty;
            NSUserDefaults.StandardUserDefaults.SetString(App.userID, "MEI_UserID");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public string GetUser(object sender, EventArgs e)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey("MEI_UserID");
        }

        public string GetUserData()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey("MEI_UserData");
        }

        public string GetDomainData()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey("MEI_DomainData");
        }

        public void CropImage(object sender, EventArgs e)
        {
            UIViewController v = new UIViewController();
            UIImagePickerController imagePicker = new UIImagePickerController();
            imagePicker.AllowsImageEditing = true;
            imagePicker.ContentSizeForViewInPopover = new CGSize(300, 300);
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.Delegate = Self;
            imagePicker.FinishedPickingMedia += (object s, UIImagePickerMediaPickedEventArgs eimage) =>
            {
                UIImage image = eimage.Info[UIImagePickerController.EditedImage] as UIImage;
                App.uploadImageStream.Source = ImageSource.FromStream(() => { return image.AsJPEG().AsStream(); });
                App.uploadStream = image.AsJPEG().ToArray();
                imagePicker.DismissModalViewController(false);

            };
            imagePicker.Canceled += (object s, EventArgs eimage) =>
            {
                imagePicker.DismissModalViewController(false);
            };
            UIApplication.SharedApplication.KeyWindow.RootViewController.ShowViewController(imagePicker, null);
        }
        
        public static async void CreateContact(object sender, EventArgs e)
        {
            var store = new CNContactStore();
            NSError error;
            if (CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts) == CNAuthorizationStatus.Authorized)
            {
                CNContact currentUser = null;
                var predicate = CNContact.GetPredicateForContacts("Appleseed");
                var fetchKeys = new NSString[] { CNContactKey.GivenName, CNContactKey.FamilyName };
                NSError cError;
                var contacts = store.GetUnifiedContacts(predicate, fetchKeys, out cError);
                for (int i = 0; i < contacts.Count(); i++)
                {
                    if (contacts[i].GivenName == App.contactuser.FirstName && contacts[i].FamilyName == App.contactuser.LastName)
                    {
                        currentUser = contacts[i];
                    }
                }


                // Found?
                if (currentUser != null)
                {
                    bool k = await App.Current.MainPage.DisplayAlert("Exists", "Requested contact already exists...", "Edit", "OK");
                    if (k)
                    {
                        personViewController = CNContactViewController.FromContact(currentUser);
                        var done = new UIBarButtonItem(UIBarButtonSystemItem.Done);
                        done.Clicked += (s, ea) =>
                        {
                            personViewController.DismissViewController(false, null);
                        };
                        personViewController.NavigationItem.LeftBarButtonItem = done;

                        navController.PushViewController(personViewController, true);
                        UIApplication.SharedApplication.KeyWindow.RootViewController.ShowViewController(navController, null);
                    }
                }
                else
                {
                    var contact = new CNMutableContact();

                    contact.GivenName = App.contactuser.FirstName;
                    contact.FamilyName = App.contactuser.LastName;

                    // Add email addresses
                    var email = new CNLabeledValue<NSString>(CNLabelKey.Home, new NSString(App.contactuser.email));
                    contact.EmailAddresses = new CNLabeledValue<NSString>[] { email };

                    // Add phone numbers
                    var cellPhone = new CNLabeledValue<CNPhoneNumber>(CNLabelPhoneNumberKey.iPhone, new CNPhoneNumber(App.contactuser.phoneNumber));
                    contact.PhoneNumbers = new CNLabeledValue<CNPhoneNumber>[] { cellPhone };
                    // Save new contact
                    var _store = new CNContactStore();
                    var saveRequest = new CNSaveRequest();
                    saveRequest.AddContact(contact, _store.DefaultContainerIdentifier);

                    NSError cerror;
                    if (store.ExecuteSaveRequest(saveRequest, out cerror))
                    {
                        personViewController = CNContactViewController.FromContact(contact);
                        var done = new UIBarButtonItem(UIBarButtonSystemItem.Done);
                        done.Clicked += (s, ea) =>
                        {
                            personViewController.DismissViewController(false, null);
                        };
                        personViewController.NavigationItem.LeftBarButtonItem = done;
                        navController.PushViewController(personViewController, true);
                        UIApplication.SharedApplication.KeyWindow.RootViewController.ShowViewController(navController, null);
                    }
                    else
                    {
                        Console.WriteLine("Save error: {0}", cerror);
                    }

                }

            }
            else
            {
                store.RequestAccess(CNEntityType.Contacts, RequestAccepted);
            }

        }

        private static void RequestAccepted(bool granted, NSError error)
        {
            if (error != null)
            {
                CreateContact(null, null);
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }
    }

}
