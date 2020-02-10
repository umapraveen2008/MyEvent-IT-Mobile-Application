using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class DomainNotifications : ContentView
    {
        private Command loadExhibitorsCommand;
        int currentDomainID = 0;
        ObservableCollection<ServerEventPost> posts = new ObservableCollection<ServerEventPost>();

        public DomainNotifications()
        {
            InitializeComponent();
            domainNotificationList.RefreshCommand = LoadEventUpdatesCommand;
            domainNotificationList.ItemTemplate = new DataTemplate(typeof(EventUpdateTemplate));
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
            domainNotificationList.IsRefreshing = true;
            await CreateUpdates(currentDomainID);
            domainNotificationList.IsRefreshing = false;
        }

        public async Task<bool> CreateUpdates(int id)
        {
            try
            {
                await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Syncing notifications...");
                currentDomainID = id;
                App.gettingUpdates = true;
                App.serverData.allDomainEvents[currentDomainID].userNotifications = await ((HomeLayout)App.Current.MainPage).GetCurrentDomainUserPosts(currentDomainID);
                if (App.serverData.allDomainEvents[currentDomainID].userNotifications.Count > 0)
                        App.serverData.SaveDomainDBToLocal();                
                posts = new ObservableCollection<ServerEventPost>(App.serverData.allDomainEvents[currentDomainID].userNotifications);
                posts = new ObservableCollection<ServerEventPost>(posts.Reverse());
                await ((HomeLayout)App.Current.MainPage).SetProgressBar(.5);
                if (posts.Count > 0)
                {
                    emptyList.IsVisible = false;
                    domainNotificationList.IsVisible = true;
                }
                else
                {
                    domainNotificationList.IsVisible = false;
                    emptyList.IsVisible = true;
                }                
                domainNotificationList.ItemTemplate = new DataTemplate(typeof(EventUpdateTemplate));
                domainNotificationList.HasUnevenRows = true;
                domainNotificationList.ItemsSource = posts;
                ((HomeLayout)App.Current.MainPage).ResetRegisteredDomainList();
                await ((HomeLayout)App.Current.MainPage).SetProgressBar(.8);
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
