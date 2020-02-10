using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PaymentTemplate : ViewCell
    {

        public PaymentTemplate()
        {
            View = new PaymentTemplateView(RefreshList);
        }
            
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((PaymentTemplateView)View).SetPaymentDetails((UserCard)BindingContext);
        }
        
        public void RefreshList(object sender, EventArgs e)
        {
            ((ListView)this.Parent).BeginRefresh();
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ((ListView)this.Parent).SelectedItem = null;
        }
    }

    public partial class PaymentTemplateView : ContentView
    {
        int id=0;
        public UserCard card = new UserCard();
        EventHandler refreshList;
        public PaymentTemplateView(EventHandler e)
        {
            refreshList = e;
            InitializeComponent();
        }

        public async void MakePrimaryFunction(object sender, EventArgs e)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Making primary card");
            List<string> userTokens = App.serverData.mei_user.currentUser.userCustomerID;
            int index = App.serverData.mei_user.userCustomerTokenList.IndexOf(card);
            string id = userTokens[index];
            userTokens.Remove(id);
            userTokens.Insert(0, id);
            App.serverData.mei_user.userCustomerTokenList.Remove(card);
            App.serverData.mei_user.userCustomerTokenList.Insert(0, card);
            App.serverData.mei_user.currentUser.userCustomerID = userTokens;
            await BaseFunctions.SaveUserToServer();
            refreshList(this, null);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Making primary address");
        }

        public void EditFunction(object sender,EventArgs e)
        {            
            ((HomeLayout)App.Current.MainPage).CreatePaymentInformation(id);
        }

        public void SetPaymentDetails(UserCard _card)
        {           
            card = _card;
            cardImage.Source = card.card.cardImageURL;
            id = App.serverData.mei_user.userCustomerTokenList.IndexOf(card);
            if (id == 0)
                makePrimaryButton.IsEnabled = false;
            if (!string.IsNullOrEmpty(card.card.cardNumber))
                cardNumber.Text = card.card.cardNumber;
            if (!string.IsNullOrEmpty(card.card.cardName))
                cardName.Text = card.card.cardName;
            if (!string.IsNullOrEmpty(card.card.cardType))
                cardType.Text = card.card.cardType;            
        }


    }
}
