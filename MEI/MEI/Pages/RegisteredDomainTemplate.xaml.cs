using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class RegisteredDomainTemplate : ViewCell
    {

        public RegisteredDomainTemplate()
        {
            //if (((HomeLayout)App.Current.MainPage).GetDomainListPage() == this.Parent)
            //{                               
            //}           
            View = new RegisteredDomainTemplateView();

        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                ((RegisteredDomainTemplateView)View).SetExhibitors((RegisteredDomainViewModel)BindingContext);
            }
        }
        
        public void ShowDetails()
        {          
            if (App.firmID != ((RegisteredDomainTemplateView)View).currentDomain.firmID)
            {
                ((HomeLayout)App.Current.MainPage).SetDomain(this, null);
            }
            else
            {
                ((HomeLayout)App.Current.MainPage).IsPresented = true;
            }          
        }



        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }
         
    }

    public partial class RegisteredDomainTemplateView : ContentView
    {
        public ServerDomain currentDomain;
        public RegisteredDomainViewModel currentItem;


        public RegisteredDomainTemplateView()
        {
            InitializeComponent();
            TapGestureRecognizer viewGest = new TapGestureRecognizer();
            viewGest.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateDomainDetail(currentDomain); };
            viewDomain.GestureRecognizers.Add(viewGest);
            TapGestureRecognizer viewDomainPosts = new TapGestureRecognizer();
            viewDomainPosts.Tapped += (s, e) =>
            {
                int i = App.serverData.mei_user.registeredDomainList.FindIndex(x => x.firmID == currentDomain.firmID);
                ((HomeLayout)App.Current.MainPage).CreateDomainPosts(i);
            };
            viewDomainNotifications.GestureRecognizers.Add(viewDomainPosts);
        }

        public async void SetExhibitors(RegisteredDomainViewModel _domain)
        {
            currentItem = _domain;
            currentDomain = currentItem.domain;
            companyName.Text = currentItem.domain.domainName;
            if (!string.IsNullOrEmpty(currentDomain.domainLogo))
            {
                companyLogo.IsVisible = true;
                companyLogo.Source = currentDomain.domainLogo;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentDomain.domainName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                companyLogo.IsVisible = false;
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentDomain.domainName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            _domain.HaveUnread = await App.serverData.CheckForNewNotifications(currentDomain.firmID);
            unReadNotification.IsVisible = _domain.HaveUnread;
        }        
    }
}
