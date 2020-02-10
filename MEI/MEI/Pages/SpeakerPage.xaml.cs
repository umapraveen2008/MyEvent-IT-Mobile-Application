using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class  SpeakerPage : ContentView 
    {
        SpeakerViewModel s = null;
        public bool Bookmarked =false;
        private Command loadSpeakerCommand;
        List<ServerSpeaker> speakerList = new List<ServerSpeaker>();
        bool canSearch = false;
        public SpeakerPage()
        {
            InitializeComponent();
            //speakerParent.ItemTemplate = new DataTemplate(typeof(SpeakerTemplate));
            peopleSearch.TextChanged += OnSearchTextChange;
            speakerParent.RefreshCommand = LoadSpeakerCommand;
            speakerParent.ItemTemplate = new DataTemplate(typeof(SpeakerTemplate));
            speakerParent.RowHeight = 80;
            speakerParent.IsGroupingEnabled = true;
        }

        public async void OnSearchTextChange(object sender, EventArgs e)
        {
            if (canSearch)
                CreateList(speakerList);
        }

        public Command LoadSpeakerCommand
        {
            get
            {
                return loadSpeakerCommand ?? (loadSpeakerCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            speakerParent.IsRefreshing = true;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading speakers...");
            try
            {
                if (Bookmarked)
                {
                    speakerParent.IsPullToRefreshEnabled = false;
                    peopleSearch.IsVisible = false;
                    speakerParent.IsGroupingEnabled = false;
                    speakerParent.ItemsSource = App.serverData.mei_user.b_speakerList;
                    await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                    // CreateList(App.serverData.b_speakerList.ToList());
                }
                else
                {
                    speakerList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventSpeakers(true);
                    CreateList(speakerList);
                }
            }
            catch
            {

            }
            speakerParent.IsRefreshing = false;
        }


        public async void CreateSpeakers(bool isBookmarked)
        {
            //groups.Clear();
            //speakerParent.Children.Clear();
            canSearch = false;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading speakers...");
            Bookmarked = isBookmarked;
            peopleSearch.Text = "";
            if (isBookmarked)
            {
                emptyText.Text = "You haven't bookmarked any speaker yet";
                speakerParent.IsPullToRefreshEnabled = false;
                peopleSearch.IsVisible = false;
                speakerParent.IsGroupingEnabled = false;
                speakerParent.ItemsSource = App.serverData.mei_user.b_speakerList;
                if (App.serverData.mei_user.b_speakerList.Count > 0)
                {
                    emptyList.IsVisible = false;
                    speakerParent.IsVisible = true;
                }
                else
                {
                    speakerParent.IsVisible = false;
                    emptyList.IsVisible = true;
                }
                speakerParent.ChildRemoved += (s, e) => {
                    if (App.serverData.mei_user.b_speakerList.Count > 0)
                    {
                        speakerParent.IsVisible = true;
                        emptyList.IsVisible = false;
                        //ListParent.IsVisible = true;
                    }
                    else
                    {
                        emptyList.IsVisible = true;
                        speakerParent.IsVisible = false;
                        //ListParent.IsVisible = false;
                    }
                };
                await Task.Delay(1000);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                //CreateList(App.serverData.b_speakerList.ToList());
            }
            else
            {
                speakerList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventSpeakers(false);
                CreateList(speakerList);
            }
        }


        //SpeakerGroupTemplate GroupExists(string groupTag)
        // {

        //     SpeakerGroupTemplate s = null;
        //         if (groups.Count > 0)
        //         {
        //             for (int i = 0; i < groups.Count; i++)
        //             {
        //                 if (groups[i].tagName == groupTag)
        //                     s = groups[i];
        //             }
        //         }

        //         if (s == null)
        //         {
        //             s = new SpeakerGroupTemplate();
        //             s.SetSpeakerGroup(groupTag);
        //             groups.Add(s);
        //             speakerParent.Children.Add(s);
        //         }
        //     return s;
        // }


        public async void CreateList(IList<ServerSpeaker> speakers)
        {
            if (speakers.Count > 0)
            {
                emptyList.IsVisible = false;
                speakerParent.IsVisible = true;
            }
            else
            {
                speakerParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            List<ServerSpeaker> filterPeople = new List<ServerSpeaker>();
            if (!string.IsNullOrEmpty(peopleSearch.Text))
            {
                for (int i = 0; i < speakers.Count; i++)
                {
                    if (speakers[i].speakerFirstName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) || speakers[i].speakerLastName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || speakers[i].speakerCompany.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) || speakers[i].speakerPosition.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase)||
                        speakers[i].speakerEmail.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        filterPeople.Add(speakers[i]);
                    }
                }
            }
            else
            {
                filterPeople = speakers as List<ServerSpeaker>;
            }
            filterPeople.RemoveAll(x => x == null);
            s = new SpeakerViewModel(filterPeople, SetupList(filterPeople));            
            speakerParent.ItemsSource = s.speakersGroup;
            await Task.Delay(500);
            canSearch = true;
            await((HomeLayout)App.Current.MainPage).SetLoading(false, "");
        }

        ObservableCollection<Grouping<string, ServerSpeaker>> SetupList(IList<ServerSpeaker> speakers)
        {
            
            var sorted = from speaker in speakers
                         orderby speaker.speakerFirstName
                         group speaker by speaker.speakerCompany into speakerGroup
                         select new Grouping<string, ServerSpeaker>(speakerGroup.Key, speakerGroup);

            var speakersGrouped = new ObservableCollection<Grouping<string, ServerSpeaker>>(sorted);

            return speakersGrouped;
        }

        public class SpeakerViewModel
        {
            public IList<ServerSpeaker> speakers { get; set; }
            public ObservableCollection<Grouping<string, ServerSpeaker>> speakersGroup { get; set; }

            public SpeakerViewModel(IList<ServerSpeaker> _speakers, ObservableCollection<Grouping<string, ServerSpeaker>> _speakersGroup)
            {
                speakers = _speakers;
                speakersGroup = new ObservableCollection<Grouping<string, ServerSpeaker>>(_speakersGroup.OrderBy(a=>a.Key));
            }            
        }
    }

   
}
