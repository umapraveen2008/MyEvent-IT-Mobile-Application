using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MEI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartItemTemplate : ViewCell
    {

        public CartItemTemplate()
        {
            View = new CartItemTemplateView();
        }

        public string GetID()
        {
            return ((CartItemTemplateView)View).id;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if(BindingContext != null)
            {
                ((CartItemTemplateView)View).SetCartItemDetails((ServerCatalogGroup)BindingContext);
            }
        }

        public ServerCatalogGroup GetCurrentCatalogItem()
        {
            return ((CartItemTemplateView)View)._cartItem;
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ((ListView)this.Parent).SelectedItem = null;
            ((HomeLayout)App.Current.MainPage).CreateCartItemDetail(this, null);
        }
    }

    public partial class CartItemTemplateView : ContentView
    {
        public ServerCatalogGroup _cartItem;
        public string id;
        public CartItemTemplateView()
        {
            InitializeComponent();
        }

        public void IncreaseQuantity(object sender,EventArgs e)
        {
            int i = int.Parse(_cartItem.cItem.itemCurrentQuantity);
            i++;
            _cartItem.cItem.itemCurrentQuantity = i.ToString();
            itemQuantity.Text = _cartItem.cItem.itemCurrentQuantity;
            itemPrice.Text = "$" + (int.Parse(_cartItem.cItem.itemCurrentQuantity) * double.Parse(_cartItem.cItem.itemPrice));
        }

        public void DecreaseQuantity(object sender,EventArgs e)
        {
            int i = int.Parse(_cartItem.cItem.itemCurrentQuantity);
            i--;
            if(i <= 0)
            {
                i = 0;                
            }
            _cartItem.cItem.itemCurrentQuantity = i.ToString();
            itemQuantity.Text = _cartItem.cItem.itemCurrentQuantity;
            itemPrice.Text = "$" + (int.Parse(_cartItem.cItem.itemCurrentQuantity) * double.Parse(_cartItem.cItem.itemPrice));
            if (i == 0)
            {
                App.AppCart.Remove(_cartItem);
                ((ListView)Parent.Parent).BeginRefresh();
            }
        }

        public void SetCartItemDetails(ServerCatalogGroup _catalogGroupItem)
        {
            _cartItem = _catalogGroupItem;
            if (!string.IsNullOrEmpty(_cartItem.iItem.itemName))
                itemName.Text = _cartItem.iItem.itemName;
            else
                itemName.Text = "";
            if (!string.IsNullOrEmpty(_cartItem.iItem.itemImage))
            {
                itemImage.Source = _cartItem.iItem.itemImage;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(_cartItem.iItem.itemName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                itemImage.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(_cartItem.iItem.itemName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            itemQuantity.Text = _cartItem.cItem.itemCurrentQuantity;
            if (!string.IsNullOrEmpty(_cartItem.cItem.itemPrice))
                itemPrice.Text = "$" + (int.Parse(_cartItem.cItem.itemCurrentQuantity) * double.Parse(_cartItem.cItem.itemPrice));
            else
                itemPrice.Text = "$0";
        }
    }
}