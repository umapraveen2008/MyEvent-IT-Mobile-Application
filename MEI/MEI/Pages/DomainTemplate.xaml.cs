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
    public partial class DomainTemplate : ViewCell
    {       

        public DomainTemplate()
        {     
            View = new DomainTemplateView();
        }
              
      

        protected override void OnParentSet()
        {
            base.OnParentSet();
            ((DomainTemplateView)View).SetParent(this.Parent,this);            
        }        

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {             
                ((DomainTemplateView)View).SetExhibitors((ServerDomain)BindingContext);               
            }         
        }
                

        public void ShowDetails()
        {
            if (((HomeLayout)App.Current.MainPage).GetDomainListPage() != this.Parent)
            {
                ((HomeLayout)App.Current.MainPage).CreateDomainDetail(((DomainTemplateView)View).currentDomain);
            }
            else
            {
                if (App.firmID != ((DomainTemplateView)View).currentDomain.firmID)
                    {
                        ((HomeLayout)App.Current.MainPage).SetDomain(this, null);
                    }
                
            }

        }


        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            if (((HomeLayout)App.Current.MainPage).GetDomainListPage() != this.Parent)
            {
                ((ListView)this.Parent).SelectedItem = null;
            }
        }
    
    }

    public partial class DomainTemplateView : ContentView
    {
        public ServerDomain currentDomain;
        public Element parent;
        public ViewCell cell;
        

        public DomainTemplateView()
        {
            InitializeComponent();
            TapGestureRecognizer viewGest = new TapGestureRecognizer();
            viewGest.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateDomainDetail(currentDomain); };
            viewDomain.GestureRecognizers.Add(viewGest);
        }

        public void SetParent(Element p,ViewCell c)
        {
            cell = c;
            parent = p;
            if (((HomeLayout)App.Current.MainPage).GetDomainListPage() != parent)
            {
               
                viewDomain.IsVisible = false;
            }
            else
            {
                viewDomain.IsVisible = true;
            }
        }



        public void SetExhibitors(ServerDomain _domain)
        {
            currentDomain = _domain;
            companyName.Text = _domain.domainName;         
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
        }


       

    }
}
