using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MEI.Pages
{    
    public partial class DomainUserDirectory : ContentView
    {
        private Command loadPeopleCommand;
        ObservableCollection<ServerUser> attendeeList = new ObservableCollection<ServerUser>();
        bool canSearch = false;

        public DomainUserDirectory()
        {
            InitializeComponent();
            peopleSearch.TextChanged += OnSearchTextChange;
            contactsParent.RowHeight = 80;
            contactsParent.RefreshCommand = LoadPeopleCommand;
            contactsParent.ItemTemplate = new DataTemplate(typeof(PeopleSpeakerTemplate));
        }



        public void OnSearchTextChange(object sender, EventArgs e)
        {
            if (canSearch)
                SetContactDetails(attendeeList);
        }

        public Command LoadPeopleCommand
        {
            get
            {
                return loadPeopleCommand ?? (loadPeopleCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            //if (contactsParent.IsRefreshing)
            //  return;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading domain directory...");
            contactsParent.IsRefreshing = true;

            attendeeList = new ObservableCollection<ServerUser>(await ((HomeLayout)App.Current.MainPage).GetCurrentDomainUsers(true));            
            attendeeList = new ObservableCollection<ServerUser>(attendeeList.Where(x => x.userPrivacy == "False").ToList());
            SetContactDetails(attendeeList);

            contactsParent.IsRefreshing = false;
        }

        public async void CreatePeople()
        {
            canSearch = false;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading domain directory...");

            peopleSearch.Text = "";

            attendeeList = new ObservableCollection<ServerUser>(await ((HomeLayout)App.Current.MainPage).GetCurrentDomainUsers(true));
            attendeeList = new ObservableCollection<ServerUser>(attendeeList.Where(x => x.userPrivacy == "False").ToList());
            SetContactDetails(attendeeList);

        }

        public async void SetContactDetails(ObservableCollection<ServerUser> people)
        {
            //for (int i = 0; i < people.Count; i++)
            //{
            //    PeopleSpeakerTemplate dummyP1 = new PeopleSpeakerTemplate();
            //    dummyP1.SetDetails(people[i]);
            //    dummyP1.SetPeopleClick(((HomeLayout)App.Current.MainPage).CreatePeopleDetail);
            //    peopleList.Add(dummyP1);
            //}
            if (people.Count > 0)
            {
                contactsParent.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                contactsParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            ObservableCollection<ServerUser> filterPeople = new ObservableCollection<ServerUser>();
            if (!string.IsNullOrEmpty(peopleSearch.Text))
            {
                for (int i = 0; i < people.Count; i++)
                {
                    if (!BaseFunctions.CheckBool(people[i].userPrivacy))
                    {
                        if (people[i].userFirstName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) || people[i].userLastName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase)
                            || people[i].userCompany.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) || people[i].userPosition.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            filterPeople.Add(people[i]);
                        }
                    }
                }
            }
            else
            {
                filterPeople = people as ObservableCollection<ServerUser>;
            }
            if (filterPeople.Count > 0)
                filterPeople = new ObservableCollection<ServerUser>(filterPeople.OrderBy(a => GetSort(a)));

            contactsParent.ItemsSource = filterPeople;
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Syncing event RSVP status...");
            canSearch = true;
        }

        public string GetSort(ServerUser s)
        {
            if (s != null)
            {
                if (s.userLastName != null)
                    return s.userLastName;
                else
                    return s.userFirstName;
            }
            return string.Empty;
        }


    }
}