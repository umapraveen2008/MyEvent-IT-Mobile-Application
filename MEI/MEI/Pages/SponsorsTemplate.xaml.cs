using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{    

    public partial class SponsorsTemplate : ViewCell
    {
        
        public SponsorsTemplate()
        {
            View = new SponsorsTemplateView();            
        }
        
        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreateSponsorDetail(this, null);
        }        

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((SponsorsTemplateView)View).SetSponsorDetails((SponsorGroup)BindingContext);
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }

        public string Getid()
        {
            return ((SponsorsTemplateView)View).id;
        }


    }

    public partial class SponsorsTemplateView : ContentView
    {
        public bool isBookmarked = false;
        public string id;
        public SponsorGroup currentSponsor = new SponsorGroup();

        public SponsorsTemplateView()
        {
            InitializeComponent();
            //TapGestureRecognizer bookmarkTap = new TapGestureRecognizer();
            //bookmarkTap.Tapped += BookMark;
            //bookmarkPersonButton.GestureRecognizers.Add(bookmarkTap);
        }

        public void BookMark(object s, EventArgs e)
        {
            isBookmarked = !isBookmarked;

            //if (!isBookmarked)
            //{
            //    bookmarkIcon.Source = "addicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.RemoveSponsor(currentSponsor);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
            //else
            //{
            //    bookmarkIcon.Source = "checkyellowicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.AddSponsor(currentSponsor);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
        }


        public void SetSponsorDetails(SponsorGroup _company)
        {
            currentSponsor = _company;
            var company = _company.company;
            //this.ImageSource = company.companyLogo;
            //this.Text = company.companyName;
            if (!string.IsNullOrEmpty(company.companyName))
                companyName.Text = company.companyName;
            else
                companyName.Text = "";
            if (!string.IsNullOrEmpty(company.companyLogo))
            {
                companyLogo.Source = company.companyLogo;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(company.companyName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {             
                companyLogo.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(company.companyName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            id = _company.sponsor.sponsorID;

            //CheckBookmark(((HomeLayout)App.Current.MainPage).currentUser.userBookmarks.isBookmarked(currentSponsor));
        }

        public void CheckBookmark(bool isMarked)
        {
            //isBookmarked = isMarked;
            //if (isMarked)
            //{
            //    bookmarkIcon.Source = "checkyellowicon.png";
            //}
            //else
            //{
            //    bookmarkIcon.Source = "addicon.png";
            //}
        }
       

        public void SetSponsorClick(EventHandler ev)
        {
            TapGestureRecognizer sponsorClick = new TapGestureRecognizer();
            sponsorClick.Tapped += ev;
        }
    }
}
