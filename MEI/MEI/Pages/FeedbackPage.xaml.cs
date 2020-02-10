using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class FeedbackPage : ContentView
    {
        public FeedbackPage()
        {
            InitializeComponent();
        }

        public void ResetFocus()
        {
            noteEditor.Unfocus();
        }

        public void SetReportHeader()
        {
            header.Text = "Bug Description";
        }

        public void SetFeedBackHeader(string domainName)
        {
            header.Text = "Feedback for "+domainName;
        }

        public async void ReportBug(object sender,EventHandler e)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Submitting bug report...");
            ServerReportBug fback = new ServerReportBug();
            fback.userID = App.userID;
            fback.firmID = App.firmID;
            fback.userBugDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
            fback.userBug = noteEditor.Text;
            var k = await BaseFunctions.SendReportToServer(fback);
            if (k)
            {
                await App.Current.MainPage.DisplayAlert("Successful", "Thank you for reporting.", "Close");
            }
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending bug report...");
            e(this, null);
        }

        public async void SendFeedback(object sender, EventHandler e)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Sending Feedback...");
            ServerUserFeeback fback = new ServerUserFeeback();
            fback.userID = App.userID;
            fback.firmID = App.firmID;
            fback.userFeedbackDate = DateTime.Now.ToString("MM-dd-yy");
            fback.userFeedback = noteEditor.Text;
            var k = await BaseFunctions.SendFeedBackToServer(fback);
            if (k)
            {
                await App.Current.MainPage.DisplayAlert("Successful", "Thank you for your feedback.", "Close");
            }
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending Feedback...");
            e(this, null);
        }
    }
}
