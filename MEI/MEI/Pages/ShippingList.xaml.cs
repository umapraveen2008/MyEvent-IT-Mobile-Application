using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ShippingList : ContentView
    {
        ShippingViewModel s;
        private Command loadCatalogCommand;

        public ShippingList()
        {
            InitializeComponent();
            itemSearch.TextChanged += OnSearchTextChange;
            shippingParent.RefreshCommand = LoadCatalogCommand;
            shippingParent.IsGroupingEnabled = true;
            shippingParent.ItemTemplate = new DataTemplate(typeof(ShippingTemplate));
            CreateAddressList();
        }

        public void OnSearchTextChange(object sender, EventArgs e)
        {
            CreateList(App.serverData.mei_user.userAddressList);
        }

        public Command LoadCatalogCommand
        {
            get
            {
                return loadCatalogCommand ?? (loadCatalogCommand = new Command(PullToRefresh));
            }
        }

        public void CreateNewAddress(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateShippingInformation(App.serverData.mei_user.userAddressList.Count);
        }

        public async void PullToRefresh()
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading address book...");
            shippingParent.IsRefreshing = true;
            try
            {
                CreateList(App.serverData.mei_user.userAddressList);
            }
            catch
            {

            }
            shippingParent.IsRefreshing = false;           
        }


        public async void CreateAddressList()
        {
            itemSearch.Text = "";
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading address book...");            
            CreateList(App.serverData.mei_user.userAddressList);           
        }



        public async void CreateList(IList<BillingInformation> catalogList)
        {
            if (catalogList.Count > 0)
            {
                shippingParent.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                shippingParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            List<BillingInformation> filterList = new List<BillingInformation>();
            if (!string.IsNullOrEmpty(itemSearch.Text))
            {
                for (int i = 0; i < catalogList.Count; i++)
                {
                    if (catalogList[i].addressLine1.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase) 
                        || catalogList[i].addressLine2.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].state.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].city.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].postalCode.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].email.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        || catalogList[i].phone.Contains(itemSearch.Text, StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        filterList.Add(catalogList[i]);
                    }
                }
            }
            else
            {
                filterList = catalogList as List<BillingInformation>;
            }
            filterList.RemoveAll(x => x == null);
            s = new ShippingViewModel(filterList, SetupList(filterList));

            shippingParent.ItemsSource = s.group;
            await Task.Delay(1000);
            await((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        ObservableCollection<Grouping<string, BillingInformation>> SetupList(IList<BillingInformation> list)
        {

            var sorted = from child in list
                         orderby child.addressLine1
                         group child by isPrimary(list,child) into _group
                         select new Grouping<string, BillingInformation>(_group.Key, _group);

            var childGrouped = new ObservableCollection<Grouping<string, BillingInformation>>(sorted);

            return childGrouped;
        }

        public string isPrimary(IList<BillingInformation> list,BillingInformation item)
        {
            if (list.IndexOf(item) == 0)
                return "Primary";
            else
                return "Secondary";
        }

        public class ShippingViewModel
        {
            public IList<BillingInformation> list { get; set; }
            public ObservableCollection<Grouping<string, BillingInformation>> group { get; set; }

            public ShippingViewModel(IList<BillingInformation> _list, ObservableCollection<Grouping<string, BillingInformation>> _group)
            {
                list = _list;
                group = new ObservableCollection<Grouping<string, BillingInformation>>(_group.OrderBy(a => a.Key));
            }

        }
    }
}
