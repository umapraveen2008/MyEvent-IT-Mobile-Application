using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class CatalogTemplate : ViewCell
    {

        public CatalogTemplate()
        {
            View = new CatalogTemplateView();
        }

        public string GetID()
        {
            return ((CatalogTemplateView)View).id;
        }

        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreateCatalogDetail(this, null);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((CatalogTemplateView)View).SetSpeakerDetails((ServerCatalogGroup)BindingContext);
        }

        public ServerCatalogGroup GetCurrentCatalogItem()
        {
           return ((CatalogTemplateView)View).catalogGroup;
        }
        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }
    }

    public partial class CatalogTemplateView : ContentView
    {

        public bool isBookmarked = false;
        public string id;
        public ServerCatalogGroup catalogGroup = new ServerCatalogGroup();

        public CatalogTemplateView()
        {
            InitializeComponent();
        }

        public void SetSpeakerDetails(ServerCatalogGroup _catalogGroupItem)
        {
            catalogGroup = _catalogGroupItem;
            if (!string.IsNullOrEmpty(catalogGroup.iItem.itemName))
                itemName.Text = catalogGroup.iItem.itemName;
            else
                itemName.Text = "";
            if (!string.IsNullOrEmpty(catalogGroup.iItem.itemImage))
            {                
                itemImage.Source = catalogGroup.iItem.itemImage;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(catalogGroup.iItem.itemName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                itemImage.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(catalogGroup.iItem.itemName , "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (!string.IsNullOrEmpty(catalogGroup.cItem.itemPrice))
                itemPrice.Text = "$"+catalogGroup.cItem.itemPrice;
            else
                itemPrice.Text = "$0";                     
        }

        
    }
}
