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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cart : ContentView
    {
        CartViewModel s;
        public EventHandler closePage;
        private Command loadCatalogCommand;
        public Cart()
        {
            InitializeComponent();
            cartList.RefreshCommand = LoadCatalogCommand;
            cartList.IsGroupingEnabled = true;
            cartList.ItemTemplate = new DataTemplate(typeof(CartItemTemplate));
            CreateList(App.AppCart);
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
            cartList.IsRefreshing = true;
            try
            {
                CreateList(App.AppCart);
            }
            catch
            {

            }
            cartList.IsRefreshing = false;
        }

        public void CompletePurchase(object sender,EventArgs args)
        {
            ((HomeLayout)App.Current.MainPage).CreateCatalogPurchase(this, null);
        }

        public async void CreateList(IList<ServerCatalogGroup> catalogList)
        {
            PurchaseButton.IsVisible = App.AppCart.Count != 0;
            if (catalogList.Count > 0)
            {
                cartList.IsVisible = true;
                emptyList.IsVisible = false;
            }
            else
            {
                cartList.IsVisible = false;
                emptyList.IsVisible = true;
            }
            s = new CartViewModel(App.AppCart, SetupList(App.AppCart));

            cartList.ItemsSource = s.group;
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

        public class CartViewModel
        {
            public IList<ServerCatalogGroup> list { get; set; }
            public ObservableCollection<Grouping<string, ServerCatalogGroup>> group { get; set; }

            public CartViewModel(IList<ServerCatalogGroup> _list, ObservableCollection<Grouping<string, ServerCatalogGroup>> _group)
            {
                list = _list;
                group = new ObservableCollection<Grouping<string, ServerCatalogGroup>>(_group.OrderBy(a => a.Key));
            }

        }

    }
}