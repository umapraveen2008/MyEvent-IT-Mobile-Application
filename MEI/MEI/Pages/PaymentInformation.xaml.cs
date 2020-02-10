using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PaymentInformation : ContentView
    {

        ServerUser currentUser = new ServerUser();
        public int currentIndex = 0;
        public EventHandler closePage;
        List<string> states = new List<string> {"AL","AK","AZ","AR","CA","CO","CT"
                                    ,"DE"
                                    ,"DC"
                                    ,"FL"
                                    ,"GA"
                                    ,"HI"
                                    ,"ID"
                                    ,"IL"
                                    ,"IN"
                                    ,"IA"
                                    ,"KS"
                                    ,"KY"
                                    ,"LA"
                                    ,"ME"
                                    ,"MD"
                                    ,"MA"
                                    ,"MI"
                                    ,"MN"
                                    ,"MS"
                                    ,"MO"
                                    ,"MT"
                                    ,"NE"
                                    ,"NV"
                                    ,"NH"
                                    ,"NJ"
                                    ,"NM"
                                    ,"NY"
                                    ,"NC"
                                    ,"ND"
                                    ,"OH"
                                    ,"OK"
                                    ,"OR"
                                    ,"PA"
                                    ,"RI"
                                    ,"SC"
                                    ,"SD"
                                    ,"TN"
                                    ,"TX"
                                    ,"UT"
                                    ,"VT"
                                    ,"VA"
                                    ,"WA"
                                    ,"WV"
                                    ,"WI"
                                    ,"WY"};
        public PaymentInformation()
        {
            InitializeComponent();
            for (int i = 0; i < states.Count; i++)
                billingState.Items.Add(states[i]);
            BaseFunctions.SetTextChangeFunction(cardCvv, 4);
            BaseFunctions.SetTextChangeFunction(cardMonth, 2);
            BaseFunctions.SetTextChangeFunction(cardYear, 4);
            AcceptTerms.Text = "Agree <a href='http://www.myeventit.com/termsandconditions/'>terms and conditions</a>";
            AcceptPolicy.Text = "Agree <a href='http://www.myeventit.com/privacypolicy/'>privacy policy</a>";
        }

        public void SetPaymentInformation(int index)
        {
            currentUser = App.serverData.mei_user.currentUser;
            currentIndex = index;
            if (App.serverData.mei_user.userCustomerTokenList.Count > index)
            {
                ((StackLayout)agreeTermSwitch.Parent).IsVisible = false;
                ((StackLayout)agreePrivacySwitch.Parent).IsVisible = false;
                NameOnCard.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardName;
                cardNumber.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardNumber;
                cardNumber.IsEnabled = false;
                cardMonth.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardExpMonth;
                cardYear.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardExpYear;
                cardCvv.Text = "";
                billingFirstName.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardBillingFirstName;
                billingLastName.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardBillingLastName;                
                billingAddressLine.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardBillingAddress;
                billingCity.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardBillingCity;
                billingPostalCode.Text = App.serverData.mei_user.userCustomerTokenList[index].card.cardBillingZipCode;
                billingState.SelectedIndex = states.IndexOf(App.serverData.mei_user.userCustomerTokenList[index].card.cardBillingState);                
                cardNumber.BackgroundColor = Color.FromHex("#505f6d");
                deleteCardButton.IsVisible = true;                
                
            }
            else
            {
                deleteCardButton.IsVisible = false;
                billingFirstName.Text = currentUser.userFirstName;
                billingLastName.Text = currentUser.userLastName;                
                billingAddressLine.Text = currentUser.userAddress;
                billingCity.Text = currentUser.userCity;
                billingState.SelectedIndex = states.IndexOf(currentUser.userState);
                billingPostalCode.Text = currentUser.userPostal;
            }
        }

        public async void DeleteCard(object sender, EventArgs e)
        {
            var k = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure?", "Yes", "No");
            if (k)
            {
                bool token = await BaseFunctions.DeleteBraintreeUser(currentIndex, currentUser);
                if (token)
                {
                    App.serverData.mei_user.currentUser.userCustomerID.RemoveAt(currentIndex);
                    await BaseFunctions.SaveUserToServer();
                    App.serverData.mei_user.userCustomerTokenList.RemoveAt(currentIndex);
                    App.serverData.SaveUserDataToLocal();
                    ((HomeLayout)App.Current.MainPage).paymentList.PullToRefresh();
                    closePage(this, null);
                }
            }
        }


        public async Task<bool> SaveUserToBrainTree()
        {
            if (string.IsNullOrEmpty(NameOnCard.Text))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Required Name on Card", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(cardNumber.Text))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Required Card Number", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(cardMonth.Text))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Required Expiration Month of Card", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(cardYear.Text))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Required Expiration Year of Card", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(cardCvv.Text))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Required CVV", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingFirstName.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required First Name for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingLastName.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Last Name for billing information", "OK");
                return false;
            }
          
            if (string.IsNullOrEmpty(billingAddressLine.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Address for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingCity.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required City for billing information", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(billingPostalCode.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Postal / Zip code for billing information", "OK");
                return false;
            }
            bool returnValue = false;
            if (App.serverData.mei_user.userCustomerTokenList.Count > currentIndex)
            {
                UserCard currentCard = new UserCard();
                currentCard.card = App.serverData.mei_user.userCustomerTokenList[currentIndex].card;
                currentCard.cardToken = App.serverData.mei_user.userCustomerTokenList[currentIndex].cardToken;
                currentCard.card.cardName = NameOnCard.Text;
                currentCard.card.cardExpMonth = cardMonth.Text;
                currentCard.card.cardExpYear = cardYear.Text;
                currentCard.card.cardCVV = cardCvv.Text;
                currentCard.card.cardBillingFirstName = billingFirstName.Text;
                currentCard.card.cardBillingLastName = billingLastName.Text;                
                currentCard.card.cardBillingAddress = billingAddressLine.Text;
                //currentCard.billingInformation.addressLine2 = billingAddressLine2.Text;
                currentCard.card.cardBillingCity = billingCity.Text;
                currentCard.card.cardBillingZipCode = billingPostalCode.Text;
                currentCard.card.cardBillingState = states[billingState.SelectedIndex];
                string token = await BaseFunctions.UpdateBraintreeUser(currentIndex, currentUser, currentCard);
                if (token == "updated")
                {
                    App.serverData.mei_user.userCustomerTokenList[currentIndex] = currentCard;
                    ((HomeLayout)App.Current.MainPage).paymentList.PullToRefresh();
                    returnValue = true;

                }
                else
                {
                    returnValue = false;
                }
            }
            else
            {
                if (!agreeTermSwitch.IsToggled)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Please agree terms and conditions to add a card to your account.", "OK");
                    return false;
                }

                if (!agreePrivacySwitch.IsToggled)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Please agree privacy to add a card to your account.", "OK");
                    return false;
                }

                List<UserCard> list = new List<UserCard>();
                UserCard currentCard = new UserCard();
                currentCard.card = new CardObject();
                currentCard.card.cardName = NameOnCard.Text;
                currentCard.card.cardNumber = cardNumber.Text;
                currentCard.card.cardExpMonth = cardMonth.Text;
                currentCard.card.cardExpYear = cardYear.Text;
                currentCard.card.cardCVV = cardCvv.Text;
                currentCard.card.cardBillingFirstName = billingFirstName.Text;
                currentCard.card.cardBillingLastName = billingLastName.Text;
                currentCard.card.cardBillingAddress = billingAddressLine.Text;
                currentCard.card.cardBillingCity = billingCity.Text;
                currentCard.card.cardBillingState = states[billingState.SelectedIndex];
                currentCard.card.cardBillingZipCode = billingPostalCode.Text;
                string token = "not assigned";
                token = await BaseFunctions.AddUserToBraintree(currentIndex, currentUser, currentCard.card);
                if (token != "")
                {
                    string customerID = token.Split("&"[0])[0];
                    string customerTokenID = token.Split("&"[0])[1];
                    string cardType = token.Split("&"[0])[2];
                    currentUser.userCustomerID.Add(customerID);
                    App.serverData.mei_user.currentUser = currentUser;
                    await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Adding card to your profile...");
                    await App.serverData.CreateUserTokenList();
                    ((HomeLayout)App.Current.MainPage).paymentList.PullToRefresh();
                    await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Saving to server...");
                    returnValue = await BaseFunctions.SaveUserToServer();
                }
                else
                {
                    returnValue = false;
                }
            }
            return returnValue;
        }
    }
}
