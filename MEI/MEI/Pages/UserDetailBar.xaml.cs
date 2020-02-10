using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class UserDetailBar : ContentView
    {
        public UserDetailBar()
        {
            InitializeComponent();
            //searchButton.BackgroundColor = Color.FromRgba(255, 255, 255, 80);
        }

        public void SetEventDetails(DomainEvent currentEvent)
        {
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventVenueName))
                currentCenter.Text = currentEvent.s_event.eventVenueName;
            else
                currentCenter.Text = "";
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventName))
                currentEventName.Text = currentEvent.s_event.eventName;
            else
                currentEventName.Text = "";
            if (!string.IsNullOrEmpty(currentEvent.s_event.eventVenueMap.venueAddress))
                currentCityState.Text = currentEvent.s_event.eventVenueMap.venueAddress;
            else
                currentCityState.Text = "";

            if (!string.IsNullOrEmpty(currentEvent.s_event.eventLogo))
                eventIcon.Source = currentEvent.s_event.eventLogo;
            else
                eventIcon.Source = "eventicon.png";

            if (currentEvent.s_event.eventStartDate.Equals(currentEvent.s_event.eventEndDate))
                currentSchedule.Text = BaseFunctions.getMonthDay(currentEvent.s_event.eventStartDate) + ", " + BaseFunctions.getFullYear(currentEvent.s_event.eventStartDate);
            else
            {
                if (BaseFunctions.GetMonth(currentEvent.s_event.eventStartDate) == BaseFunctions.GetMonth(currentEvent.s_event.eventEndDate))
                    currentSchedule.Text = BaseFunctions.getMonthDay(currentEvent.s_event.eventStartDate) + " - " + BaseFunctions.getDayYear(currentEvent.s_event.eventStartDate);
                else
                    currentSchedule.Text = BaseFunctions.getMonthDay(currentEvent.s_event.eventStartDate) + " - " + BaseFunctions.getMonthDay(currentEvent.s_event.eventEndDate) + ", " + BaseFunctions.getYear(currentEvent.s_event.eventEndDate);
            }

        }

    }
}
