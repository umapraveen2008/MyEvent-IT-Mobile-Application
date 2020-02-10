using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ExhibitorsPage : ContentView
    {
        ExhibitorViewModel s = null;
        public bool Bookmarked = false;
        private Command loadExhibitorsCommand;
        List<ExhibitorGroup> exhibitorList = new List<ExhibitorGroup>();
        bool canSearch = false;

        public ExhibitorsPage()
        {
            InitializeComponent();
            peopleSearch.TextChanged += OnSearchTextChange;
            exhibitorParent.RefreshCommand = LoadExhibitorsCommand;
            exhibitorParent.ItemTemplate = new DataTemplate(typeof(ExhibitorsTemplate));
            exhibitorParent.RowHeight = 80;
            exhibitorParent.IsGroupingEnabled = true;
        }

        public async void OnSearchTextChange(object sender, EventArgs e)
        {
            if (canSearch)
                CreateList(exhibitorList);
        }

        public Command LoadExhibitorsCommand
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
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading...");
            exhibitorParent.IsRefreshing = true;
            try
            {
                if (Bookmarked)
                {
                    peopleSearch.IsVisible = false;
                    exhibitorParent.IsPullToRefreshEnabled = false;
                    exhibitorParent.IsGroupingEnabled = false;
                    exhibitorParent.ItemsSource = App.serverData.mei_user.b_exhibitorList;
                    await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                    //CreateList(App.serverData.b_exhibitorList.ToList());
                }
                else
                {
                    exhibitorList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventExhibitors(true);
                    CreateList(exhibitorList);
                }
            }
            catch
            {

            }
            exhibitorParent.IsRefreshing = false;
        }

        public async void CreateExhibitors(bool isBookMarked)
        {
            //groups.Clear();
            //exhibitorParent.Children.Clear();
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading...");
            canSearch = false;
            peopleSearch.Text = "";
            Bookmarked = isBookMarked;
            if (isBookMarked)
            {
                emptyText.Text = "You haven't bookmarked any exhibitor yet";
                peopleSearch.IsVisible = false;
                exhibitorParent.IsPullToRefreshEnabled = false;
                exhibitorParent.IsGroupingEnabled = false;
                exhibitorParent.ItemsSource = App.serverData.mei_user.b_exhibitorList;
                if (App.serverData.mei_user.b_exhibitorList.Count > 0)
                {
                    emptyList.IsVisible = false;
                    exhibitorParent.IsVisible = true;
                }
                else
                {
                    emptyList.IsVisible = true;
                    exhibitorParent.IsVisible = false;
                }
                exhibitorParent.ChildRemoved += (s, e) =>
                {
                    if (App.serverData.mei_user.b_exhibitorList.Count > 0)
                    {
                        exhibitorParent.IsVisible = true;
                        emptyList.IsVisible = false;
                        //ListParent.IsVisible = true;
                    }
                    else
                    {
                        emptyList.IsVisible = true;
                        exhibitorParent.IsVisible = false;
                        //ListParent.IsVisible = false;
                    }
                };
                await Task.Delay(1000);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                //CreateList(App.serverData.b_exhibitorList.ToList());
            }
            else
            {
                emptyText.Text = "No exhibitors for this event.";
                exhibitorList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventExhibitors(false);
                CreateList(exhibitorList);
            }
        }


        public async void CreateList(IList<ExhibitorGroup> exhibitors)
        {
            if (exhibitors.Count > 0)
            {
                emptyList.IsVisible = false;
                exhibitorParent.IsVisible = true;
            }
            else
            {
                emptyList.IsVisible = true;
                exhibitorParent.IsVisible = false;
            }

            List<ExhibitorGroup> filterPeople = new List<ExhibitorGroup>();
            if (!string.IsNullOrEmpty(peopleSearch.Text))
            {
                for (int i = 0; i < exhibitors.Count; i++)
                {
                    var exibitorCompany = exhibitors[i].company;
                    if (exibitorCompany.companyName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        filterPeople.Add(exhibitors[i]);
                    }
                }
            }
            else
            {
                filterPeople = exhibitors as List<ExhibitorGroup>;
            }
            filterPeople.RemoveAll(x => x == null);
            s = new ExhibitorViewModel(filterPeople, SetupList(filterPeople));
            exhibitorParent.ItemsSource = s.group;
            await Task.Delay(1000);
            canSearch = true;
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
        }

        ObservableCollection<Grouping<string, ExhibitorGroup>> SetupList(IList<ExhibitorGroup> list)
        {

            var sorted = from child in list
                         orderby child.company.companyName
                         group child by child.company.companyName.ToCharArray()[0].ToString() into _group
                         select new Grouping<string, ExhibitorGroup>(_group.Key, _group);

            var childGrouped = new ObservableCollection<Grouping<string, ExhibitorGroup>>(sorted);

            return childGrouped;
        }
        public class ExhibitorViewModel
        {
            public IList<ExhibitorGroup> list { get; set; }
            public ObservableCollection<Grouping<string, ExhibitorGroup>> group { get; set; }

            public ExhibitorViewModel(IList<ExhibitorGroup> _list, ObservableCollection<Grouping<string, ExhibitorGroup>> _group)
            {
                list = _list;
                group = new ObservableCollection<Grouping<string, ExhibitorGroup>>(_group.OrderBy(a => a.Key));
            }

        }
    }

}
