using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class FAQPage : StackLayout
    {
        public FAQPage()
        {
            InitializeComponent();
            copyPasswordButton.Clicked += App.CopyToClipBoard;            
        }

        public async void CreateFAQ()
        {
            ServerFAQModule fqMod = (await ((HomeLayout)App.Current.MainPage).GetCurrentDomainEventFromServer()).s_event.eventFaq;
            if (fqMod.wifiDetails.name == "")
                wifiModule.IsVisible = false;
            else
                wifiModule.IsVisible = true;
            if (fqMod.eventQandA.Count == 0)
                FAQQuestions.IsVisible = false;
            else
                FAQQuestions.IsVisible = true;

            wifiUsername.Text = fqMod.wifiDetails.name;
            wifiPassword.Text = fqMod.wifiDetails.password;
            questionsList.Children.Clear();
            for(int i = 0;i<fqMod.eventQandA.Count;i++)
            {
                CommonQuestionTemplate q = new CommonQuestionTemplate();
                q.SetQandA(fqMod.eventQandA[i]);
                questionsList.Children.Add(q);
            }
            termsAndConditions.GestureRecognizers.Clear();
            codeOfConduct.GestureRecognizers.Clear();
            privacyPolicy.GestureRecognizers.Clear();
            SetApplicationTerms(fqMod.eventTerms);
            SetPrivacyPolicy(fqMod.eventPolicy);
            SetCodeOfConduct(fqMod.eventCodeOfConduct);
           await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        public void SetApplicationTerms(string url)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateWebView(s, e, url, "Event Terms"); };
            termsAndConditions.GestureRecognizers.Add(tap);
        }

        public void SetPrivacyPolicy(string url)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateWebView(s, e, url, "Event Policy"); };
            privacyPolicy.GestureRecognizers.Add(tap);
        }

        public void SetCodeOfConduct(string url)
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateWebView(s, e, url, "Event C.o.C"); };
            codeOfConduct.GestureRecognizers.Add(tap);
        }

    }

    
}
