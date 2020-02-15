using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PurchaseDetailPage : ContentView
    {
        public string id;
        public ServerTransaction transaction = new ServerTransaction();
        string domainEmail = "";
        
        public PurchaseDetailPage()
        {
            InitializeComponent();
        }

        public async void PurchaseDetail(ServerTransaction _transaction)
        {          
            transaction = _transaction;
            domainEmail = (await App.serverData.GetDomainFromServer(transaction.firmID)).domainEmail;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading transaction...");
            if (string.IsNullOrEmpty(domainEmail))
            {
                contactSender.IsVisible = false;
            }
            if (!string.IsNullOrEmpty(transaction.transactionName))
                purchaseItemName.Text = transaction.transactionName.Replace("@",",");
            else
                purchaseItemName.Text = "Item name not available";
            if (!string.IsNullOrEmpty(transaction.transactionType))
                purchaseDate.Text = DateTime.ParseExact(transaction.transactionDate, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MMM dd, yyyy");
            else
                purchaseDate.Text = "Date not generated";
            if (!string.IsNullOrEmpty(transaction.transactionID))
                purchaseID.Text = transaction.transactionID;
            else
                purchaseID.Text = "ID not generated";
            if (!string.IsNullOrEmpty(transaction.transactionImage))
            {
                purchaseItemImage.Source = transaction.transactionImage;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(transaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                purchaseItemImage.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(transaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (transaction.transactionType != "Subscription")
            {
                purchaseType.Text = transaction.transactionType;
            }
            else
            {
                purchaseType.Text = transaction.transactionType + "( " + transaction.transactionTracking + " Payment )";
            }
            if (!string.IsNullOrEmpty(transaction.transactionPrice))
                purchaseAmount.Text = "$" + transaction.transactionPrice;
            else
                purchaseAmount.Text = "$0.00";
            if (!string.IsNullOrEmpty(transaction.transactionID))
            {
                purchasePaymentCardType.Text = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_cardType");
            }
            if (!string.IsNullOrEmpty(transaction.transactionShippingAddress))
                purchaseShippingAddress.Text = transaction.transactionShippingAddress;
            else
                purchaseShippingAddress.Text = "Address not provided";
            if (!string.IsNullOrEmpty(transaction.transactionShippingType))
                purchaseShippingType.Text = transaction.transactionShippingType;
            else
                purchaseShippingType.Text = "Type not specified";
            if (!string.IsNullOrEmpty(transaction.transactionTracking))
                purchaseTracking.Text = transaction.transactionTracking;
            else
                purchaseTracking.Text = "Sender yet to provide tracking details.";

            if (transaction.transactionHaveRefund == "Yes")
            {
                purchaseRefund.Text = "Refund Available";
            }
            else
            {
                purchaseRefund.Text = "Refund Not Available";
            }

            if (transaction.transactionType != "Sale")
            {
                ((StackLayout)purchaseQuantity.Parent).IsVisible = false;
                shippingInformation.IsVisible = false;
            }
            else
            {
                purchaseQuantity.Text = transaction.transactionQuantity.Replace("@", "-");
            }

            if (transaction.transactionType == "Subscription")
            {                
                cancelSubscriptionLayout.IsVisible = true;
                string subStatus = await App.serverData.GetSubscriptionStatus(transaction.transactionMerchantID, "sub_status");
                if (subStatus.Equals("Canceled") || subStatus.Equals("Expired") && transaction.transactionTracking != "Single")
                {
                    cancelSubscription.IsEnabled = false;
                    if(subStatus.Equals("Canceled"))
                        cancelSubscription.Text = "Subscription Canceled";
                    else
                        cancelSubscription.Text = "Subscription Expired";
                }
                else if(transaction.transactionTracking == "Single")
                {
                    cancelSubscriptionLayout.IsVisible = false;
                }                
            }
            else
            {
                cancelSubscriptionLayout.IsVisible = false;
            }
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
        }

        public async void EmailInvoiceFunction(object sender, EventArgs e)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Sending email...");
            string cardType = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_cardType");
            string card4 = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_last4");            
            if (transaction.transactionType == "Sale")
            {                
                bool sent = await BaseFunctions.EmailSaleInvoice(transaction, cardType + " ending with " + card4);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending email...");
                if (sent)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Invoice has been sent to your email", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Failed", "Something went wrong please retry.", "Ok");
                }
            }
            else if(transaction.transactionType == "Donation")
            {
                bool sent = await BaseFunctions.EmailDonationInvoice(transaction, cardType + " ending with " + card4);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending email...");
                if (sent)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Invoice has been sent to your email", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Failed", "Something went wrong please retry.", "Ok");
                }
            }
        }

        public void ContactSenderFunction(object sender, EventArgs e)
        {            
            string subject = "Order ID : " + transaction.transactionID;
            Device.OpenUri(new Uri("mailto:"+domainEmail+ "?subject="+subject));
        }

        public async void CancelSubscriptionFunction(object sender, EventArgs e)
        {
            var k = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure", "Yes", "No");
            if(k)
            {
                var j = await App.serverData.UnSubscibeForDomain(transaction.transactionMerchantID);
                if(j)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Subscription is canceled successfully.", "Ok");
                    cancelSubscription.IsEnabled = false;
                    cancelSubscription.Text = "Subscription Canceled";
                }
            }
        }
    }
}
