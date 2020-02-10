using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class AboutPage : ScrollView
    {
        public ServerSubscription subscription = null;
        DomainEvent currentEvent = new DomainEvent();
        public AboutPage()
        {
            InitializeComponent();
            rsvpTemplate.SetRSVP("RSVP Here", "Are you attending this event?");            
        }

        public async void SetAbout()
        {
            eventDescription.IsVisible = true;
            emptyList.IsVisible = false;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Syncing event RSVP status...");            
            currentEvent = await ((HomeLayout)App.Current.MainPage).GetCurrentDomainEventFromServer();
            if (currentEvent.s_event.eventType != "Subscription")
            {
                cancelSubscription.IsVisible = false;
            }
            else
            {                
                subscription = App.serverData.mei_user.userSubscriptionList.Find(x => x.planID == currentEvent.s_event.eventSubscriptionPlanID);
                if (subscription != null)
                {
                    
                    await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Syncing event RSVP status...");
                    string subStatus = await App.serverData.GetSubscriptionStatus(subscription.subscriptionID, "sub_status");
                    cancelSubscription.IsVisible = true;
                    if ((subStatus.Equals("Canceled") || subStatus.Equals("Expired"))&&currentEvent.s_event.eventSubscriptionType!="Single")
                    {                     
                        cancelSubscription.IsEnabled = false;
                        if (subStatus.Equals("Canceled"))
                            cancelSubscription.Text = "Subscription Canceled";
                        else
                            cancelSubscription.Text = "Subscription Expired";
                    }
                    else if(currentEvent.s_event.eventSubscriptionType != "Single")
                    {
                        cancelSubscription.IsVisible = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventName))
                eventName.Text = currentEvent.s_event.eventName;
            else
                eventName.Text = "No event name";
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventLogo))
            {
                aboutEventLogo.Source = currentEvent.s_event.eventLogo;                
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentEvent.s_event.eventName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                aboutEventLogo.Source = "";
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentEvent.s_event.eventName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventVenueName))
                eventVenueName.Text = currentEvent.s_event.eventVenueName;
            else
                eventVenueName.Text = "";
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventAbout))
            {                
                eventDescription.Text = currentEvent.s_event.eventAbout;
            }
            else
            {
                eventDescription.IsVisible = false;
                emptyList.IsVisible = true;
                if (!string.IsNullOrEmpty(currentEvent.s_event.eventDescription))
                {
                    eventDescription.Text = currentEvent.s_event.eventDescription;
                }
                else
                {
                    eventDescription.IsVisible = false;
                    emptyList.IsVisible = true;
                }
            }
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventAddress))
                eventCityState.Text = currentEvent.s_event.eventAddress;
            else
                eventCityState.Text = "";
            if (currentEvent.s_event.eventStartDate.Equals(currentEvent.s_event.eventEndDate))
                eventDays.Text = BaseFunctions.getMonthDay(currentEvent.s_event.eventStartDate) + ", " + BaseFunctions.getYear(currentEvent.s_event.eventEndDate);
            else
            {
                if (BaseFunctions.GetMonth(currentEvent.s_event.eventStartDate) == BaseFunctions.GetMonth(currentEvent.s_event.eventEndDate))
                    eventDays.Text = BaseFunctions.getMonthDay(currentEvent.s_event.eventStartDate) + " - " + BaseFunctions.getDayYear(currentEvent.s_event.eventEndDate);
                else
                    eventDays.Text = BaseFunctions.getMonthDay(currentEvent.s_event.eventStartDate) + " - " + BaseFunctions.getMonthDay(currentEvent.s_event.eventEndDate) + ", " + BaseFunctions.getYear(currentEvent.s_event.eventEndDate);
            }
            var k = await rsvpTemplate.CheckRSVP();
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        public async void CancelSubscriptionFunction(object sender ,EventArgs e)
        {
            var k = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure to cancel this subscription", "Yes", "No");
            if (k)
            {
                var j = await App.serverData.UnSubscibeForDomain(subscription.subscriptionID);
                if (j)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Subscription is canceled successfully.", "Ok");
                    cancelSubscription.IsEnabled = false;
                    cancelSubscription.Text = "Subscription Canceled";                    
                }
            }
        }
    }
}
