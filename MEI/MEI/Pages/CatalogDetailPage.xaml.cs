using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class CatalogDetailPage : ContentView
    {
        public string id;
        public ServerCatalogGroup currentItem = new ServerCatalogGroup();
        public EventHandler closePage;

        public CatalogDetailPage()
        {
            InitializeComponent();
        }

        public void CatalogDetails(ServerCatalogGroup item)
        {
            currentItem = item;

            if (currentItem != null)
            {
                if (!string.IsNullOrEmpty(currentItem.iItem.itemImage))
                {
                    itemImage.Source = currentItem.iItem.itemImage;
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentItem.iItem.itemName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    itemImage.Source = "";
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentItem.iItem.itemName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                if (!string.IsNullOrEmpty(currentItem.iItem.itemName))
                    itemName.Text = currentItem.iItem.itemName;
                if (!string.IsNullOrEmpty(currentItem.iItem.itemDescription))
                    description.Text = currentItem.iItem.itemDescription;
                else
                    description.Text = "Description unavailable";

                if (!string.IsNullOrEmpty(currentItem.iItem.itemType))
                    itemType.Text = "Category : " + currentItem.iItem.itemType;

                if (!string.IsNullOrEmpty(currentItem.cItem.itemPrice))
                {
                    itemPrice.Text = "Price : $" + currentItem.cItem.itemPrice;
                }
                else
                {
                    itemPrice.Text = "Price : Free";
                    purchaseButton.IsVisible = false;
                }
                if (!string.IsNullOrEmpty(currentItem.cItem.itemCurrentQuantity))
                {
                    if (currentItem.cItem.itemMaxQuantity == "-1")
                        itemAvailable.Text = "Quantity Available : No Limit";
                    else
                    {
                        int avai = (int.Parse(currentItem.cItem.itemMaxQuantity) - int.Parse(currentItem.cItem.itemCurrentQuantity));                        
                        if (avai <= 0)
                        {
                            itemAvailable.Text = "Quantity Available : Sold out!";
                            purchaseButton.Text = "Sold Out";
                            purchaseButton.BackgroundColor = Color.FromHex("#ff3232");
                            purchaseButton.IsEnabled = false;
                        }
                        else
                        {
                            itemAvailable.Text = "Quantity Available : " + avai.ToString();                            
                        }
                    }
                }

                if (!string.IsNullOrEmpty(currentItem.iItem.itemShippingType))
                    shipping.Text = currentItem.iItem.itemShippingType;

                if (!string.IsNullOrEmpty(currentItem.iItem.itemRefund))
                    refund.Text = currentItem.iItem.itemRefund;

                currentItem.cItem.itemCurrentQuantity = "1";
            }
            if(!currentItem.Available(1))
            {
                purchaseButton.Text = "Sold Out!";
                purchaseButton.IsEnabled = false;                
            }
        }

        public void PurchaseItemFunction(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).AddItemToCart(this, null);
            //((HomeLayout)App.Current.MainPage).CreateCatalogPurchase(this, null);
        }
    }
}
