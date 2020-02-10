using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class EventItem : StackLayout
    {
        public string eventid;
        public int eventIndex;
        public bool isLocked = true;
        public bool requested = false;
        public TapGestureRecognizer clickEvent = new TapGestureRecognizer();
        public enum attendingStatus
        {
            yes,
            no,
            maybe,
            noinfo
        };

        public EventItem()
        {
            InitializeComponent();
        }

        public async Task<bool> CheckLock(DomainEvent dEvent)
        {
            if (dEvent.s_event.eventType == "Public" || dEvent.s_event.eventType == "Private")
            {
                if (dEvent.s_event.eventType == "Public" || await App.serverData.IsRegisteredForEvent(dEvent.s_event.eventID))
                {
                    isLocked = false;
                }
                else
                {
                    if (dEvent.s_event.eventType == "Private")
                    {
                        requested = await App.serverData.IsRequestedForEvent(dEvent.s_event.eventID);
                    }
                }
            }
            else
            {
                if (App.serverData.mei_user.userSubscriptionList.Find(x => x.planID == dEvent.s_event.eventSubscriptionPlanID) == null)
                    isLocked = false;

            }
            eventLock.IsVisible = isLocked;
            return true;
        }

        public void SetLoading(bool vis)
        {
            this.InputTransparent = vis;
            loading.IsVisible = vis;
        }

        public async void setEventITem(DomainEvent dEvent, EventHandler _clickEvent, int _eventIndex)
        {
            loading.IsVisible = true;
            await CheckLock(dEvent);
            if (!string.IsNullOrEmpty(dEvent.s_event.eventName))
                eventName.Text = dEvent.s_event.eventName;
            else
                eventName.Text = "";

            if (dEvent.s_event.eventStartDate.Equals(dEvent.s_event.eventEndDate))
                date.Text = BaseFunctions.getMonthDay(dEvent.s_event.eventStartDate) + ", " + BaseFunctions.getYear(dEvent.s_event.eventEndDate);
            else
            {
                if (BaseFunctions.GetMonth(dEvent.s_event.eventStartDate) == BaseFunctions.GetMonth(dEvent.s_event.eventEndDate))
                    date.Text = BaseFunctions.getMonthDay(dEvent.s_event.eventStartDate) + " - " + BaseFunctions.getDayYear(dEvent.s_event.eventEndDate);
                else
                    date.Text = BaseFunctions.getMonthDay(dEvent.s_event.eventStartDate) + " - " + BaseFunctions.getMonthDay(dEvent.s_event.eventEndDate) + ", " + BaseFunctions.getYear(dEvent.s_event.eventEndDate);
            }



            //if (!string.IsNullOrEmpty(DomainEvent.venueName))
            //    centerName.Text = DomainEvent.venueName;
            //else
            //    centerName.Text = "";
            if (!string.IsNullOrEmpty(dEvent.s_event.eventVenueMap.venueAddress))
                place.Text = dEvent.s_event.eventVenueMap.venueAddress;
            else
                place.Text = "";
            if (!string.IsNullOrEmpty(dEvent.s_event.eventLogo))
            {
                eventImage.Source = dEvent.s_event.eventLogo;                
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(dEvent.s_event.eventName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                eventImage.Source = "";                
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(dEvent.s_event.eventName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            eventid = dEvent.s_event.eventID;
            eventIndex = _eventIndex;
            clickEvent.Tapped += _clickEvent;
            this.GestureRecognizers.Add(clickEvent);

            //switch (DomainEvent.eventStatus)
            //{
            //    case EventStatus.Current:
            //        eventActiveStatus.Text = "Current";
            //        eventActiveStatus.TextColor = Color.FromHex("00a651");
            //        break;
            //    case EventStatus.Expired:
            //        eventActiveStatus.Text = "Expired";
            //        eventActiveStatus.TextColor = Color.FromHex("ef4300");
            //        break;
            //    case EventStatus.UpComing:
            //        eventActiveStatus.Text = "Upcoming";
            //        eventActiveStatus.TextColor = Color.FromHex("ffac4e");
            //        break;
            //}
            loading.IsVisible = false;
        }

        public void setButton()
        {
           // selectedIcon.IsVisible = true;
            this.BackgroundColor = Color.FromHex("#ebeff2");
        }

        public void resetButton()
        {

            //selectedIcon.IsVisible = false;
            SetLoading(false);
            this.BackgroundColor = Color.White;
        }
    }
}
