using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PurchaseHistory : ContentView
    {
        PurchaseViewModel s;
        private Command loadCatalogCommand;

        public PurchaseHistory()
        {
            InitializeComponent();
            itemSearch.TextChanged += OnSearchTextChange;
            purchaseParent.RefreshCommand = LoadCatalogCommand;
            purchaseParent.IsGroupingEnabled = true;
            purchaseParent.ItemTemplate = new DataTemplate(typeof(PurchaseTemplate));
            CreatePurchaseList();
        }

        public void OnSearchTextChange(object sender, EventArgs e)
        {
            CreateList(App.serverData.mei_user.userTransactionList);
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
            purchaseParent.IsRefreshing = true;
            try
            {
                await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading order history...");
                var k = await App.serverData.GetUserTransactions();
                CreateList(App.serverData.mei_user.userTransactionList);
            }
            catch
            {

            }
            purchaseParent.IsRefreshing = false;
        }


        public async void CreatePurchaseList()
        {
            itemSearch.Text = "";
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading order history...");
            var k = await App.serverData.GetUserTransactions();
            CreateList(App.serverData.mei_user.userTransactionList);
        }



        public async void CreateList(IList<ServerTransaction> pList)
        {
            if (pList.Count > 0)
            {
                purchaseParent.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                purchaseParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            List<ServerTransaction> filterList = new List<ServerTransaction>();
            if (!string.IsNullOrEmpty(itemSearch.Text))
            {
                for (int i = 0; i < pList.Count; i++)
                {
                    if (pList[i].transactionName.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase) || pList[i].transactionPrice.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase) || pList[i].transactionID.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        filterList.Add(pList[i]);
                    }
                }
            }
            else
            {
                filterList = pList as List<ServerTransaction>;
            }
            filterList.RemoveAll(x => x == null);
            s = new PurchaseViewModel(filterList, SetupList(filterList));

            purchaseParent.ItemsSource = s.group;
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        ObservableCollection<Grouping<string, ServerTransaction>> SetupList(IList<ServerTransaction> list)
        {

            List<ServerTransaction> l = new List<ServerTransaction>(list);
            l.OrderBy(x => BaseFunctions.GetDateTimeFull(x.transactionDate));
            list = l;
            var sorted = from child in list
                         group child by child.transactionType into _group
                         select new Grouping<string, ServerTransaction>(_group.Key, _group);

            var childGrouped = new ObservableCollection<Grouping<string, ServerTransaction>>(sorted);

            return childGrouped;
        }

        public class PurchaseViewModel
        {
            public IList<ServerTransaction> list { get; set; }
            public ObservableCollection<Grouping<string, ServerTransaction>> group { get; set; }

            public PurchaseViewModel(IList<ServerTransaction> _list, ObservableCollection<Grouping<string, ServerTransaction>> _group)
            {
                list =  _list;
                group = new ObservableCollection<Grouping<string, ServerTransaction>>(_group.OrderBy(a => a.Key));
            }

        }
    }
}
