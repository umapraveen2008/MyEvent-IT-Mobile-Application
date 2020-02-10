using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class EventUpdates : ContentView
    {
        private Command loadExhibitorsCommand;
        string eventID = "";
        public ObservableCollection<ServerEventPost> posts = new ObservableCollection<ServerEventPost>();
        public EventUpdates()
        {
            InitializeComponent();            
            eventUpdatesList.RefreshCommand = LoadEventUpdatesCommand;
            eventUpdatesList.ItemTemplate = new DataTemplate(typeof(EventUpdateTemplate));
            eventUpdatesList.HasUnevenRows = true;
            eventUpdatesList.RowHeight = -1;
        }

        public Command LoadEventUpdatesCommand
        {
            get
            {
                return loadExhibitorsCommand ?? (loadExhibitorsCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            //if (contactsParent.IsRefreshing)
            //  return;
            eventUpdatesList.IsRefreshing = true;            
            await CreateUpdates();
            eventUpdatesList.IsRefreshing = false;
        }

        public async Task<bool> CreateUpdates()
        {
            try
            {
                App.gettingUpdates = true;
                eventID = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().s_event.eventID;
                posts = new ObservableCollection<ServerEventPost>((await ((HomeLayout)App.Current.MainPage).GetCurrentEventPosts()).Reverse<ServerEventPost>());
                await ((HomeLayout)App.Current.MainPage).SetProgressBar(.5);
                if (posts.Count > 0)
                {
                    emptyList.IsVisible = false;
                    eventUpdatesList.IsVisible = true;
                }
                else
                {
                    eventUpdatesList.IsVisible = false;
                    emptyList.IsVisible = true;
                }
                eventUpdatesList.ItemsSource = posts;
                await ((HomeLayout)App.Current.MainPage).SetProgressBar(.8);
                await Task.Delay(1000);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Syncing event update posts...");
                //PrepareRSVP();
                App.gettingUpdates = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
