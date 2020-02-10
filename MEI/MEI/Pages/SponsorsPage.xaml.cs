using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class SponsorsPage : ContentView
    {
        SponsorViewModel s;
        public bool Bookmarked = false;
        private Command loadSponsorsCommand;
        List<SponsorGroup> sponsorList = new List<SponsorGroup>();
        bool canSearch = false;

        public SponsorsPage()
        {
            InitializeComponent();
            peopleSearch.TextChanged += OnSearchTextChange;
            sponsorParent.RefreshCommand = LoadSponsorsCommand;            
            sponsorParent.IsGroupingEnabled = true;
            sponsorParent.RowHeight = 80;
            sponsorParent.ItemTemplate = new DataTemplate(typeof(SponsorsTemplate));            
        }

        public void OnSearchTextChange(object sender, EventArgs e)
        {
            if(canSearch)
                CreateList(sponsorList);
        }

        public Command LoadSponsorsCommand
        {
            get
            {
                return loadSponsorsCommand ?? (loadSponsorsCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            sponsorParent.IsRefreshing = true;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading sponsors...");
            try
            {
                if (Bookmarked)
                {

                    peopleSearch.IsVisible = false;
                    sponsorParent.IsPullToRefreshEnabled = false;
                    sponsorParent.IsGroupingEnabled = false;
                    sponsorParent.ItemsSource = App.serverData.mei_user.b_sponsorList;                    
                    //CreateList(App.serverData.b_sponsorList.ToList());
                }
                else
                {
                    sponsorList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventSponsors(true);
                    CreateList(sponsorList);
                }
            }
            catch
            {

            }
            sponsorParent.IsRefreshing = false;
        }


        public async void CreateSponsors(bool isBookmarked)
        {
            //groups.Clear();
            //sponsorParent.Children.Clear();
            canSearch = false;
            Bookmarked = isBookmarked;
            peopleSearch.Text = "";
            if (isBookmarked)
            {
                emptyText.Text = "You haven't bookmarked any sponsor yet";
                peopleSearch.IsVisible = false;
                sponsorParent.IsPullToRefreshEnabled = false;
                sponsorParent.IsGroupingEnabled = false;
                sponsorParent.ItemsSource = App.serverData.mei_user.b_sponsorList;
                if (App.serverData.mei_user.b_sponsorList.Count > 0)
                {
                    sponsorParent.IsVisible = true;
                    emptyList.IsVisible = false;
                }
                else
                {
                    sponsorParent.IsVisible = false;
                    emptyList.IsVisible = true;
                }
                sponsorParent.ChildRemoved += (s, e) => {
                    if (App.serverData.mei_user.b_sponsorList.Count > 0)
                    {
                        sponsorParent.IsVisible = true;
                        emptyList.IsVisible = false;
                        //ListParent.IsVisible = true;
                    }
                    else
                    {
                        emptyList.IsVisible = true;
                        sponsorParent.IsVisible = false;
                        //ListParent.IsVisible = false;
                    }
                };
                await Task.Delay(1000);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                //  CreateList(App.serverData.b_sponsorList.ToList());
            }
            else
            {
                sponsorList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventSponsors(false);
                CreateList(sponsorList);
            }
            
        }


        //SponsorsGroupTemplate GroupExists(string groupTag)
        //{
        //    SponsorsGroupTemplate s = null;
        //    if (groups.Count > 0)
        //    {
        //        for (int i = 0; i < groups.Count; i++)
        //        {
        //            if (groups[i].tagName == groupTag)
        //                s = groups[i];
        //        }
        //    }

        //    if(s==null)
        //    {
        //        s = new SponsorsGroupTemplate();
        //        s.SetSponsorGroup(groupTag);
        //        groups.Add(s);
        //        sponsorParent.Children.Add(s);
        //    }
        //    return s;
        //}

        //public void CreateList(List<Sponsor> sponsors)
        //{
        //    for(int i = 0; i<sponsors.Count;i++)
        //    {
        //        GroupExists(sponsors[i].sponsorLevel).AddSponsor(sponsors[i]);               
        //    }
        //}

        public async void CreateList(IList<SponsorGroup> exhibitors)
        {
            if (exhibitors.Count > 0)
            {
                sponsorParent.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                sponsorParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            List<SponsorGroup> filterPeople = new List<SponsorGroup>();
            if (!string.IsNullOrEmpty(peopleSearch.Text))
            {
                for (int i = 0; i < exhibitors.Count; i++)
                {
                    var company = exhibitors[i].company;
                    if(company.companyName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        filterPeople.Add(exhibitors[i]);
                    }
                }
            }
            else
            {
                filterPeople = exhibitors as List<SponsorGroup>;
            }
            filterPeople.RemoveAll(x => x == null);
            s = new SponsorViewModel(filterPeople, SetupList(filterPeople));
            
            sponsorParent.ItemsSource = s.group;
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
            canSearch = true;
        }

        ObservableCollection<Grouping<string, SponsorGroup>> SetupList(IList<SponsorGroup> list)
        {

            var sorted = from child in list
                         orderby child.company.companyName
                         group child by child.company.companyName.ToCharArray()[0].ToString() into _group
                         select new Grouping<string, SponsorGroup>(_group.Key, _group);

            var childGrouped = new ObservableCollection<Grouping<string, SponsorGroup>>(sorted);

            return childGrouped;
        }

        public class SponsorViewModel
        {
            public IList<SponsorGroup> list { get; set; }
            public ObservableCollection<Grouping<string, SponsorGroup>> group { get; set; }

            public SponsorViewModel(IList<SponsorGroup> _list, ObservableCollection<Grouping<string, SponsorGroup>> _group)
            {
                list = _list;
                group = new ObservableCollection<Grouping<string, SponsorGroup>>(_group.OrderBy(a=>a.Key));
            }

        }
    }
}
