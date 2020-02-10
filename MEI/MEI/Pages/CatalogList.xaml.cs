using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class CatalogList : ContentView
    {
        CatalogViewModel s;
        private Command loadCatalogCommand;
        List<ServerCatalogGroup> catalogList = new List<ServerCatalogGroup>();
        public CatalogList()
        {
            InitializeComponent();
            itemSearch.TextChanged += OnSearchTextChange;
            catalogParent.RefreshCommand = LoadCatalogCommand;
            catalogParent.IsGroupingEnabled = true;
            catalogParent.ItemTemplate = new DataTemplate(typeof(CatalogTemplate));            
        }

        public async void OnSearchTextChange(object sender, EventArgs e)
        {            
            CreateList(catalogList);
        }

        public Command LoadCatalogCommand
        {
            get
            {
                return loadCatalogCommand ?? (loadCatalogCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading catalog list...");
            catalogParent.IsRefreshing = true;
            try
            {
                catalogList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventCatalog(true);
                CreateList(catalogList);
            }
            catch
            {

            }
            catalogParent.IsRefreshing = false;
        }


        public async void CreateCatalog()
        {
            itemSearch.Text = "";
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading catalog list...");
            catalogList = await ((HomeLayout)App.Current.MainPage).GetCurrentEventCatalog(false);
            CreateList(catalogList);
        }
                
        public async void CreateList(IList<ServerCatalogGroup> catalogList)
        {
            if (catalogList.Count > 0)
            {
                catalogParent.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                catalogParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            List<ServerCatalogGroup> filterList = new List<ServerCatalogGroup>();
            if (!string.IsNullOrEmpty(itemSearch.Text))
            {
                for (int i = 0; i < catalogList.Count; i++)
                {                    
                    if (catalogList[i].iItem.itemName.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)|| catalogList[i].cItem.itemPrice.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        filterList.Add(catalogList[i]);
                    }
                }
            }
            else
            {
                filterList = catalogList as List<ServerCatalogGroup>;
            }
            filterList.RemoveAll(x => x == null);
            s = new CatalogViewModel(filterList, SetupList(filterList));

            catalogParent.ItemsSource = s.group;
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        ObservableCollection<Grouping<string, ServerCatalogGroup>> SetupList(IList<ServerCatalogGroup> list)
        {

            var sorted = from child in list
                         orderby child.iItem.itemName
                         group child by child.iItem.itemType into _group
                         select new Grouping<string, ServerCatalogGroup>(_group.Key, _group);

            var childGrouped = new ObservableCollection<Grouping<string, ServerCatalogGroup>>(sorted);

            return childGrouped;
        }

        public class CatalogViewModel
        {
            public IList<ServerCatalogGroup> list { get; set; }
            public ObservableCollection<Grouping<string, ServerCatalogGroup>> group { get; set; }

            public CatalogViewModel(IList<ServerCatalogGroup> _list, ObservableCollection<Grouping<string, ServerCatalogGroup>> _group)
            {
                list = _list;
                group = new ObservableCollection<Grouping<string, ServerCatalogGroup>>(_group.OrderBy(a => a.Key));
            }

        }
    }
}
