using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PaymentList : ContentView
    {
        PaymentViewModel s;
        private Command loadCatalogCommand;

        public PaymentList()
        {
            InitializeComponent();
            itemSearch.TextChanged += OnSearchTextChange;
            cardParent.RefreshCommand = LoadCatalogCommand;
            cardParent.IsGroupingEnabled = true;
            cardParent.ItemTemplate = new DataTemplate(typeof(PaymentTemplate));
            CreateAddressList();
        }

        public void OnSearchTextChange(object sender, EventArgs e)
        {
            CreateList(App.serverData.mei_user.userCustomerTokenList);
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
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading cards...");
            cardParent.IsRefreshing = true;
            try
            {
                //var k = await App.serverData.CreateUserTokenList();
                CreateList(App.serverData.mei_user.userCustomerTokenList);
            }
            catch
            {

            }
            cardParent.IsRefreshing = false;           
        }

        public void CreateNewCard(object sender,EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreatePaymentInformation(App.serverData.mei_user.userCustomerTokenList.Count);
        }

        public async void CreateAddressList()
        {
            itemSearch.Text = "";
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading...");
            CreateList(App.serverData.mei_user.userCustomerTokenList);         
        }



        public async void CreateList(IList<UserCard> catalogList)
        {
            if (catalogList.Count > 0)
            {
                cardParent.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                cardParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            List<UserCard> filterList = new List<UserCard>();
            if (!string.IsNullOrEmpty(itemSearch.Text))
            {
                for (int i = 0; i < catalogList.Count; i++)
                {
                    if (catalogList[i].card.cardName.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].card.cardType.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].card.cardExpYear.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].card.cardExpMonth.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].card.card4Digits.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)                        
                        )
                    {
                        filterList.Add(catalogList[i]);
                    }
                }
            }
            else
            {
                filterList = catalogList as List<UserCard>;
            }
            filterList.RemoveAll(x => x == null);
            s = new PaymentViewModel(filterList, SetupList(filterList));

            cardParent.ItemsSource = s.group;
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        ObservableCollection<Grouping<string, UserCard>> SetupList(IList<UserCard> list)
        {

            var sorted = from child in list
                         orderby child.card.cardType
                         group child by isPrimary(list, child) into _group
                         select new Grouping<string, UserCard>(_group.Key, _group);

            var childGrouped = new ObservableCollection<Grouping<string, UserCard>>(sorted);

            return childGrouped;
        }

        public string isPrimary(IList<UserCard> list, UserCard item)
        {
            if (list.IndexOf(item) == 0)
                return "Primary";
            else
                return "Secondary";
        }

        public class PaymentViewModel
        {
            public IList<UserCard> list { get; set; }
            public ObservableCollection<Grouping<string, UserCard>> group { get; set; }

            public PaymentViewModel(IList<UserCard> _list, ObservableCollection<Grouping<string, UserCard>> _group)
            {
                list = _list;
                group = new ObservableCollection<Grouping<string, UserCard>>(_group.OrderBy(a => a.Key));
            }

        }
    }
}
