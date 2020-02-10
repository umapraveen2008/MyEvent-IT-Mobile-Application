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
    public partial class PurchaseItemInformation : ContentView
    {
        ServerCatalogGroup item;
        public PurchaseItemInformation()
        {
            InitializeComponent();
        }

        public PurchaseItemInformation(ServerCatalogGroup catalogGroup)
        {
            InitializeComponent();
            SetPurchaseItemInformation(catalogGroup);
        }
        
        public void SetPurchaseItemInformation(ServerCatalogGroup catalogGroup)
        {
            item = catalogGroup;
            if (!string.IsNullOrEmpty(item.iItem.itemImage))
            {
                productImage.Source = item.iItem.itemImage;                
            }
            else
            {
                productImage.Source = "";                
            }
            SetImageText();
            ItemName.Text = "Name: "+item.iItem.itemName;
            ItemQuantity.Text = "Quantity: " + item.cItem.itemCurrentQuantity;
            ItemPrice.Text = "Price: $" + item.cItem.itemPrice;
            ItemTotalPrice.Text = "Total Price: $" + (int.Parse(item.cItem.itemCurrentQuantity) * double.Parse(item.cItem.itemPrice));
            ItemCategory.Text = "Type: " + item.iItem.itemType;
            purchaseShippingType.Text = item.iItem.itemShippingType;
            purchaseRefund.Text = item.iItem.itemRefund;
        }

        private void SetImageText()
        {
            Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
            string init = initials.Replace(item.iItem.itemName, "$1");
            if (init.Length > 3)
                init = init.Substring(0, 3);
            logoText.Text = init.ToUpper();
        }
    }

}