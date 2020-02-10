using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class RSVPTemplate : ContentView
    {
        public string status = String.Empty;
        public bool canEdit = false;
        public RSVPTemplate()
        {
            InitializeComponent();
        }

        public void SetRSVP(string header, string post)
        {
            rsvpHeader.Text = header;
            adminPost.Text = post;
        }

        public void ResetButtons()
        {
            yesButton.IsVisible = true;
            maybeButton.IsVisible = true;
            noButton.IsVisible = true;
            yesButton.IsEnabled = true;
            maybeButton.IsEnabled = true;
            noButton.IsEnabled = true;
            yesButton.TextColor = Color.FromHex("#00A651");
            yesButton.BackgroundColor = Color.White;

            maybeButton.TextColor = Color.FromHex("#FFAC4E");
            maybeButton.BackgroundColor = Color.White;

            noButton.TextColor = Color.FromHex("#EF4300");
            noButton.BackgroundColor = Color.White;

        }

        public async void Yes(object s, EventArgs e)
        {
            if (canEdit)
            {
                if (status != "Yes")
                {
                    var confirmation = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure?", "Yes", "No");
                    if (confirmation)
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Checking with server...");
                        if (((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventAttendanceLimit > await BaseFunctions.RSVPCount(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventID))
                        {
                            var k = await AddRSVP("Yes");
                            if (k)
                            {
                                ResetButtons();
                                ((Button)s).BackgroundColor = Color.FromHex("#00A651");
                                ((Button)s).TextColor = Color.White;
                                HomeLayout l = ((HomeLayout)App.Current.MainPage);
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Failed", "Something went wrong. Please retry.", "Ok");
                            }
                            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending RSVP...");
                            //l.IsPresented = true;

                        }
                        else
                        {
                            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending RSVP...");
                            await App.Current.MainPage.DisplayAlert("Attendance is Full", "Sorry this event is at maximum capacity...", "OK");
                        }
                        var reminder = await App.Current.MainPage.DisplayAlert("Reminder", "Thank you for registering to this event, Do you want to add this event to your calender?", "Yes", "No");
                        if (reminder)
                        {
                            App.reminderEvent = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event;
                            App.AddEventReminder(this, null);                            
                        }
                        
                    }

                }
            }
        }
        public async void MayBe(object s, EventArgs e)
        {
            if (canEdit)
            {
                if (status != "May be")
                {
                    var confirmation = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure?", "Yes", "No");
                    if (confirmation)
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Checking with server...");
                        var k = await AddRSVP("May be");
                        if (k)
                        {
                            ResetButtons();
                            ((Button)s).BackgroundColor = Color.FromHex("#FFAC4E");
                            ((Button)s).TextColor = Color.White;
                            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending RSVP...");
                            adminPost.Text = "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName;
                            //HomeLayout l = ((HomeLayout)App.Current.MainPage);
                            // l.OnSideMenuItemClicked(l.sideMenus[l.tabMenuItems.IndexOf("Sessions")], null);
                            await App.Current.MainPage.DisplayAlert("Alert", "Thank you", "OK");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Failed", "Something went wrong. Please retry.", "Ok");
                        }
                        await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending RSVP...");
                    }
                }

            }
        }
        public async void No(object s, EventArgs e)
        {
            if (canEdit)
            {
                if (status != "No")
                {
                    var confirmation = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure?", "Yes", "No");
                    if (confirmation)
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Checking with server...");
                        var k = await AddRSVP("No");
                        if (k)
                        {
                            ResetButtons();
                            ((Button)s).BackgroundColor = Color.FromHex("#EF4300");
                            ((Button)s).TextColor = Color.White;
                            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending RSVP...");
                            adminPost.Text = "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName;
                            HomeLayout l = ((HomeLayout)App.Current.MainPage);
                            await App.Current.MainPage.DisplayAlert("Alert", "Thank you", "OK");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Failed", "Something went wrong. Please retry.", "Ok");
                        }
                        await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending RSVP...");
                    }
                }
            }
        }

        public async Task<bool> CheckRSVP()
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Syncing event RSVP status...");
            status = "";
            canEdit = false;
            buttonGrid.IsVisible = true;
            DomainEvent currentEvent = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent();
            ResetButtons();
            //Debug.WriteLine(DateTime.Now);             

            //Debug.WriteLine(DateTime.Now);
            ServerRSVP rsvp = await BaseFunctions.GetRSVPStatus(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventID);

            if (rsvp != null)
            {
                status = rsvp.rsvpStatus;
                switch (rsvp.rsvpStatus)
                {
                    case "Yes":
                        yesButton.BackgroundColor = Color.FromHex("#00A651");
                        adminPost.Text = "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName;
                        yesButton.TextColor = Color.White;
                        if (BaseFunctions.GetEventStatus(currentEvent) != EventStatus.Expired)
                            canEdit = true;
                        break;
                    case "May be":
                        maybeButton.BackgroundColor = Color.FromHex("#FFAC4E");
                        adminPost.Text = "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName;
                        maybeButton.TextColor = Color.White;
                        if (BaseFunctions.GetEventStatus(currentEvent) != EventStatus.Expired)
                            canEdit = true;
                        break;
                    case "No":
                        noButton.BackgroundColor = Color.FromHex("#EF4300");
                        adminPost.Text = "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName;
                        noButton.TextColor = Color.White;
                        if (currentEvent.s_event.eventAttendanceLimit > await BaseFunctions.RSVPCount(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventID))
                        {
                            if (BaseFunctions.GetEventStatus(currentEvent) != EventStatus.Expired)
                                canEdit = true;
                        }
                        else
                        {
                            rsvpHeader.Text = "Its Full";
                            adminPost.Text = "Sorry " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName + " is at maximum capacity";
                            buttonGrid.IsVisible = false;
                            yesButton.IsVisible = false;
                            maybeButton.IsVisible = false;
                            noButton.IsVisible = false;
                            yesButton.TextColor = Color.Gray;
                            yesButton.IsEnabled = false;
                            maybeButton.TextColor = Color.Gray;
                            maybeButton.IsEnabled = false;
                            noButton.TextColor = Color.Gray;
                            noButton.IsEnabled = false;
                        }
                        break;
                }
            }
            else
            {
                int count = await BaseFunctions.RSVPCount(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventID);
                int attendance = currentEvent.s_event.eventAttendanceLimit;
                if (currentEvent.s_event.eventAttendanceLimit <= count)
                {
                    rsvpHeader.Text = "Its Full";
                    adminPost.Text = "Sorry " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName + " is at maximum capacity";
                    buttonGrid.IsVisible = false;
                    yesButton.IsVisible = false;
                    maybeButton.IsVisible = false;
                    noButton.IsVisible = false;
                    yesButton.TextColor = Color.Gray;
                    yesButton.IsEnabled = false;
                    maybeButton.TextColor = Color.Gray;
                    maybeButton.IsEnabled = false;
                    noButton.TextColor = Color.Gray;
                    noButton.IsEnabled = false;
                }
                else
                {
                    if (BaseFunctions.GetEventStatus(currentEvent) != EventStatus.Expired)
                        canEdit = true;
                }

            }
            if (BaseFunctions.GetEventStatus(currentEvent) == EventStatus.Expired)
            {
                rsvpHeader.Text = "Event Expired";
                adminPost.Text = "Sorry " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName + " has concluded";
                buttonGrid.IsVisible = false;
                yesButton.TextColor = Color.Gray;
                yesButton.IsEnabled = false;
                maybeButton.TextColor = Color.Gray;
                maybeButton.IsEnabled = false;
                noButton.TextColor = Color.Gray;
                noButton.IsEnabled = false;
                AddtoCalender.IsEnabled = false;
            }

            if (canEdit)
            {
                buttonGrid.IsVisible = true;
                SetRSVP("RSVP status", "Are you attending " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName + " ?");
                switch (status)
                {
                    case "Yes":
                        SetRSVP("RSVP status", "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName);
                        break;
                    case "May be":
                        SetRSVP("RSVP status", "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName);
                        break;
                    case "No":
                        SetRSVP("RSVP status", "Your attending status for " + ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventName);
                        break;
                }

            }
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Syncing event RSVP status...");
            return true;
        }

        public async void AddToCalender(object sender,EventArgs e)
        {
            var reminder = await App.Current.MainPage.DisplayAlert("Reminder", "Do you want to add this event to your calender?", "Yes", "No");
            if (reminder)
            {
                App.reminderEvent = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event;
                App.AddEventReminder(this, null);                
            }
        }

        public async Task<bool> AddRSVP(string rsvpStatus)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string address = "http://www.myeventit.com/PHP/AddRSVP.php/";
                    var client = App.serverData.GetHttpClient();
                    ServerRSVP user = new ServerRSVP();
                    user.userID = App.serverData.mei_user.currentUser.userID;
                    user.eventID = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventID;
                    user.firmID = App.firmID;
                    user.rsvpStatus = rsvpStatus;
                    var userString = JsonConvert.SerializeObject(user);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(address, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "1")
                    {
                        status = rsvpStatus;
                        returnObject = true;
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }        
    }
}
