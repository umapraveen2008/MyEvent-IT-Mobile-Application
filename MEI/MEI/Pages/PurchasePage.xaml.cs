using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PurchasePage : ContentView
    {
        public string id;
        public string typeOfSubscription;
        int itemLimit = 0;
        public ServerTransaction currentTransaction = new ServerTransaction();
        private UserCard card = new UserCard();
        private BillingInformation billingInformation = new BillingInformation();
        public int cardIndex = 0;
        public int shippingIndex = 0;
        public EventHandler closePage;
        ServerUser currentUser = new ServerUser();

        public PurchasePage()
        {
            InitializeComponent();
            var nCard = new TapGestureRecognizer();
            var pcard = new TapGestureRecognizer();
            var nAddress = new TapGestureRecognizer();
            var pAddress = new TapGestureRecognizer();
            nCard.Tapped += (s, e) =>
            {
                cardIndex++;
                SetCardDetails();
            };

            pcard.Tapped += (s, e) =>
            {
                cardIndex--;
                SetCardDetails();
            };
            nAddress.Tapped += (s, e) =>
            {
                shippingIndex++;
                SetShippingDetails();
            };
            pAddress.Tapped += (s, e) =>
            {
                shippingIndex--;
                SetShippingDetails();
            };

            nextCard.GestureRecognizers.Add(nCard);
            previousCard.GestureRecognizers.Add(pcard);
            nextAddress.GestureRecognizers.Add(nAddress);
            previousAddress.GestureRecognizers.Add(pAddress);
        }

        public void SetTransaction(ServerTransaction transaction)
        {
            currentTransaction = transaction;
            //currentTransaction.transactionPrice = "0.00";
            switch (currentTransaction.transactionType)
            {
                case "Sale":
                    SetSaleTransaction();
                    break;
                case "Donation":
                    SetDonationTransaction();
                    break;
            }
            /*if (transaction.transactionHaveRefund == "Yes")
            {
                purchaseRefund.Text = "Refund Available";
            }
            else
            {
                purchaseRefund.Text = "Refund Not Available";
            }*/
            SetCardDetails();
            SetShippingDetails();
            SetItemPrice();
        }

        public void SetDomainSubscriptionTransaction(ServerTransaction transaction, string transactionSubscriptionType)
        {
            typeOfSubscription = transactionSubscriptionType;
            currentTransaction = transaction;
            SetSubscriptionTransaction();
            /*if (transaction.transactionHaveRefund == "Yes")
            {
                purchaseRefund.Text = "Refund Available";
            }
            else
            {
                purchaseRefund.Text = "Refund Not Available";
            }*/
            SetCardDetails();
            SetShippingDetails();
            SetItemPrice();
        }

        public void SetCardDetails()
        {
            currentUser = App.serverData.mei_user.currentUser;
            if (currentUser != null)
            {
                card = App.serverData.mei_user.userCustomerTokenList[cardIndex];
                purchaseCardType.Text = card.card.cardType;
                purchaseCardNumber.Text = card.card.cardNumber;
                purchaseCardExpMonth.Text = card.card.cardExpMonth;
                purchaseCardExpYear.Text = card.card.cardExpYear;
                currentTransaction.transactionBillingAddress = card.card.cardBillingAddress + "," + card.card.cardBillingCity + "," + card.card.cardBillingState + "," + card.card.cardBillingZipCode;
            }
            if (App.serverData.mei_user.userCustomerTokenList.Count > 1)
            {
                if (cardIndex == App.serverData.mei_user.userCustomerTokenList.Count - 1)
                {
                    nextCard.IsVisible = false;
                }
                else
                {
                    nextCard.IsVisible = true;
                }
                if (cardIndex == 0)
                {
                    previousCard.IsVisible = false;
                }
                else
                {
                    previousCard.IsVisible = true;
                }
            }
            else
            {
                nextCard.IsVisible = false;
                previousCard.IsVisible = false;
            }
        }

        public void SetShippingDetails()
        {
            currentUser = App.serverData.mei_user.currentUser;
            if (currentUser != null)
            {
                billingInformation = App.serverData.mei_user.userAddressList[shippingIndex];
                shippingFirstName.Text = billingInformation.firstName;
                shippingLastName.Text = billingInformation.lastName;
                shippingAddressLine1.Text = billingInformation.addressLine1;
                if (!string.IsNullOrEmpty(billingInformation.addressLine2))
                    shippingAddressLine2.Text = billingInformation.addressLine2;
                else
                    shippingAddressLine2.IsVisible = false;
                shippingCity.Text = billingInformation.city;
                shippingState.Text = billingInformation.state;
                shippingZipCode.Text = billingInformation.postalCode;
                if (!string.IsNullOrEmpty(billingInformation.addressLine2))
                    currentTransaction.transactionShippingAddress = shippingAddressLine1.Text + "," + shippingAddressLine2.Text + "," + shippingCity.Text + "," + shippingState.Text + "," + shippingZipCode.Text;
                else
                    currentTransaction.transactionShippingAddress = shippingAddressLine1.Text + "," + shippingCity.Text + "," + shippingState.Text + "," + shippingZipCode.Text;
            }
            if (App.serverData.mei_user.userAddressList.Count > 1)
            {
                if (shippingIndex == App.serverData.mei_user.userAddressList.Count - 1)
                {
                    nextAddress.IsVisible = false;
                }
                else
                {
                    nextAddress.IsVisible = true;
                }
                if (shippingIndex == 0)
                {
                    previousAddress.IsVisible = false;
                }
                else
                {
                    previousAddress.IsVisible = true;
                }
            }
            else
            {
                nextAddress.IsVisible = false;
                previousAddress.IsVisible = false;
            }
        }

        public void SetItemPrice()
        {
            if (!string.IsNullOrEmpty(currentTransaction.transactionPrice))
            {
                productPrice.Text = "$" + currentTransaction.transactionPrice;
                currentTransaction.transactionServiceFee = BaseFunctions.GetServiceFee(currentTransaction.transactionPrice).ToString();
            }
            else
            {
                productPrice.Text = "$0.00";
            }

        }
        public async void SetSaleTransaction()
        {
            //item = BaseFunctions.GetCatalogGroup(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().catalogList,currentTransaction.itemID);
            // if (item != null)
            // {

            //productName.Text = currentTransaction.transactionName;            
            foreach (ServerCatalogGroup item in App.AppCart)
            {
                TransactionItemList.Children.Add(new PurchaseItemInformation(item));
            }
            TransactionImagePanel.IsVisible = false;
            quantityOrAmountLayout.IsVisible = false;
            productCategory.Text = "Sale";
            /*if (item.cItem.itemMaxQuantity != "-1")
            {
                itemLimit = (int.Parse(item.cItem.itemMaxQuantity) - int.Parse(item.cItem.itemCurrentQuantity));
                quantityOrAmountVariable.Text = "Set Quantity (" + itemLimit.ToString() + " Available)";
            }
            else
            {
                itemLimit = -1;
                quantityOrAmountVariable.Text = "Set Quantity ( No Limit )";
            }*/

            purchaseAmountOrQuantity.Text = currentTransaction.transactionQuantity;
            //currentTransaction.transactionHaveRefund = item.iItem.itemRefund;
            //currentTransaction.transactionPrice = (int.Parse(purchaseAmountOrQuantity.Text) * double.Parse(currentTransaction.transactionPrice)).ToString();                
            /*purchaseAmountOrQuantity.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(purchaseAmountOrQuantity.Text))
                {
                    purchaseAmountOrQuantity.Text = Regex.Replace(purchaseAmountOrQuantity.Text, @"[^\d]", String.Empty);
                    if(purchaseAmountOrQuantity.Text!=""&&itemLimit != -1)
                    {
                        if(int.Parse(purchaseAmountOrQuantity.Text)>itemLimit)
                        {
                            purchaseAmountOrQuantity.Text = itemLimit.ToString();
                        }
                    }
                    currentTransaction.transactionPrice = (int.Parse(purchaseAmountOrQuantity.Text) * double.Parse(item.cItem.itemPrice)).ToString();
                    ();
                }
            };*/
            SetItemPrice();
            //purchaseShippingType.Text = item.iItem.itemShippingType;
            //currentTransaction.transactionShippingType = purchaseShippingType.Text;                     
            //}
        }

        public void SetDonationTransaction()
        {
            proceedButton.Text = "Donate";
            TransactionItemList.IsVisible = false;
            /*if (!string.IsNullOrEmpty(currentTransaction.transactionImage))
            {
                productImage.Source = currentTransaction.transactionImage;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentTransaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                productImage.Source = "";
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentTransaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }*/
            productName.Text = currentTransaction.transactionName;
            quantityOrAmountVariable.Text = "Donation Amount";
            purchaseAmountOrQuantity.Placeholder = "0.00";
            purchaseAmountOrQuantity.Text = "";
            purchaseAmountOrQuantity.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(purchaseAmountOrQuantity.Text))
                {
                    if (purchaseAmountOrQuantity.Text.Contains("."))
                    {
                        if (purchaseAmountOrQuantity.Text.ToCharArray().ToList().IndexOf('.') <= purchaseAmountOrQuantity.Text.Length - 2)
                            purchaseAmountOrQuantity.Text = Math.Round(decimal.Parse(purchaseAmountOrQuantity.Text), 2, MidpointRounding.AwayFromZero).ToString();
                    }
                }
                currentTransaction.transactionPrice = purchaseAmountOrQuantity.Text;
                SetItemPrice();
            };
            productCategory.Text = "Donation";
            //purchaseShippingType.Text = "Not available";
            currentTransaction.transactionShippingType = "Not available";
        }

        public void SetSubscriptionTransaction()
        {
            if (!string.IsNullOrEmpty(currentTransaction.transactionImage))
            {
                productImage.Source = currentTransaction.transactionImage;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentTransaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                productImage.Source = "";
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentTransaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            quantityOrAmountLayout.IsVisible = false;
            productName.Text = currentTransaction.transactionName;
            productCategory.Text = "Subscription ( " + currentTransaction.transactionTracking + " Payment )";
            proceedButton.Text = "Subscribe";
            //purchaseShippingType.Text = "Not available";
            currentTransaction.transactionShippingType = "Not available";
        }

        public async void ProceedFunction(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(cardCvv.Text))
            //{
            //    await App.Current.MainPage.DisplayAlert("Alert", "Please enter CVV", "Ok");
            //    return;
            //}

            if (currentTransaction.transactionType != "Subscription")
            {
                if (string.IsNullOrEmpty(purchaseAmountOrQuantity.Text))
                {
                    if (currentTransaction.transactionType == "Donation")
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Please enter your Donation Amount", "Ok");
                        return;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Please Set Quantity for purchase", "Ok");
                        return;
                    }
                }
                if (currentTransaction.transactionType == "Donation")
                {
                    if (double.Parse(purchaseAmountOrQuantity.Text) < 5.00)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Donation Amount should be minimum of $5.00", "Ok");
                        return;
                    }
                }
                var confirm = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure", "Ok", "Cancel");
                if (confirm)
                {
                    ((HomeLayout)App.Current.MainPage).ProceedPurchase(currentTransaction, currentTransaction.transactionType == "Donation");
                }
            }
            else
            {
                var confirm = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure", "Ok", "Cancel");
                if (confirm)
                {
                    ((HomeLayout)App.Current.MainPage).ProceedSubscriptionPurchase(currentTransaction, typeOfSubscription);
                }
            }
        }
    }
}
