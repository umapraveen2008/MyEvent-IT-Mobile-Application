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
    public partial class PurchaseTemplate : ViewCell
    {

        public PurchaseTemplate()
        {
            View = new PurchaseTemplateView();
        }

        public ServerTransaction GetPurchase()
        {
            return ((PurchaseTemplateView)View).transaction;
        }

        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreatePurchaseDetail(((PurchaseTemplateView)View).transaction);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((PurchaseTemplateView)View).SetTransactionDetails((ServerTransaction)BindingContext);
        }
        
        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }
    }

    public partial class PurchaseTemplateView : ContentView
    {
        
        public string id;
        public ServerTransaction transaction = new ServerTransaction();

        public PurchaseTemplateView()
        {
            InitializeComponent();
        }

        public void SetTransactionDetails(ServerTransaction _transaction)
        {
            transaction = _transaction;
            if (!string.IsNullOrEmpty(transaction.transactionName))
                purchaseName.Text = transaction.transactionName;
            else
                purchaseName.Text = "Item name not available";
            if (!string.IsNullOrEmpty(transaction.transactionType))
                purchaseDate.Text = DateTime.ParseExact(transaction.transactionDate, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MMM dd, yyyy");
            else
                purchaseDate.Text = "Date not generated";
            if (!string.IsNullOrEmpty(transaction.transactionID))
            {
                purchaseID.Text = "ID : "+transaction.transactionID;
            }
            else
                purchaseID.Text = "ID not generated";
            if (!string.IsNullOrEmpty(transaction.transactionImage))
            {
                purchaseImage.Source = transaction.transactionImage;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(transaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                purchaseImage.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(transaction.transactionName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (!string.IsNullOrEmpty(transaction.transactionPrice))
                purchasePrice.Text = "$" + transaction.transactionPrice;
            else
                purchasePrice.Text = "$0.00";
        }


    }
}
