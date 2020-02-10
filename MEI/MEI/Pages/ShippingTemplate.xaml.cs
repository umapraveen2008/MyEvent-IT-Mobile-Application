using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ShippingTemplate: ViewCell
    {

        public ShippingTemplate()
        {
            View = new ShippingTemplateView(RefreshList);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((ShippingTemplateView)View).SetPaymentDetails((BillingInformation)BindingContext);
        }

        void RefreshList(object sender,EventArgs e)
        {
            ((ListView)this.Parent).BeginRefresh();
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ((ListView)this.Parent).SelectedItem = null;
        }
    }

    public partial class ShippingTemplateView : ContentView
    {
        int id = 0;
        public BillingInformation card = new BillingInformation();
        public EventHandler refreshList ;
        public ShippingTemplateView(EventHandler e)
        {
            InitializeComponent();
            refreshList = e;
        }

        public async void MakePrimaryFunction(object sender, EventArgs e)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Making primary address");
            App.serverData.mei_user.userAddressList.Remove(card);
            App.serverData.mei_user.userAddressList.Insert(0, card);
            await BaseFunctions.UpdateShippingAddress(App.serverData.mei_user.userAddressList);
            refreshList(this, null);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Making primary address");
        }

        public void EditFunction(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateShippingInformation(id);
        }

        public void SetPaymentDetails(BillingInformation _card)
        {
            card = _card;
            id = App.serverData.mei_user.userAddressList.IndexOf(card);
            if (id == 0)
                makePrimaryButton.IsEnabled = false;            
           FullName.Text = card.firstName + " " + card.lastName;
            shippingAddressLine1.Text = card.addressLine1;
            //shippingAddressLine2.Text = card.addressLine2;
            ShippingCity.Text = card.city;
            shippingState.Text = card.state;
            shippingPostalCode.Text = card.postalCode;
        }


    }
}
