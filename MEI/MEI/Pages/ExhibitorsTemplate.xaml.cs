using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ExhibitorsTemplate : ViewCell
    {        
        public ExhibitorsTemplate()
        {
            View = new ExhibitorsTemplateView();
            //TapGestureRecognizer bookmarkTap = new TapGestureRecognizer();
            //bookmarkTap.Tapped += BookMark;
            //bookmarkPersonButton.GestureRecognizers.Add(bookmarkTap);
        }

        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreateExhibitorDetail(this, null);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((ExhibitorsTemplateView)View).SetExhibitors((ExhibitorGroup)BindingContext);
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }



        public string Getid()
        {
            return ((ExhibitorsTemplateView)View).id;
        }


    }

    public partial class ExhibitorsTemplateView:ContentView
    {
        public bool isBookmarked = false;
        public string id;
        public ExhibitorGroup currentExhibitor = new ExhibitorGroup();
        
        public ExhibitorsTemplateView()
        {
            InitializeComponent();
        }

        public void SetExhibitors(ExhibitorGroup _exhibitor)
        {
            currentExhibitor = _exhibitor;
            var exhibitorCompany = currentExhibitor.company;
            if (!string.IsNullOrEmpty(exhibitorCompany.companyLogo))
            {
                companyLogo.IsVisible = true;
                companyLogo.Source = exhibitorCompany.companyLogo;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(exhibitorCompany.companyName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                companyLogo.IsVisible = false;
                companyLogo.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(exhibitorCompany.companyName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (!string.IsNullOrEmpty(exhibitorCompany.companyName))               
                companyName.Text = exhibitorCompany.companyName;
            else                
                companyName.Text = "";        
            id = _exhibitor.exhibitor.exhibitorID;
            //CheckBookmark(((HomeLayout)App.Current.MainPage).currentUser.userBookmarks.isBookmarked(currentExhibitor));
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


        //public void SetExhibitorClick(EventHandler ev)
        //{
        //    TapGestureRecognizer exhibitorClick = new TapGestureRecognizer();
        //    exhibitorClick.Tapped += ev;
        //}

        public void BookMark(object s, EventArgs e)
        {

            //isBookmarked = !isBookmarked;
            //if (!isBookmarked)
            //{
            //    bookmarkIcon.Source = "addicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.RemoveExhibitors(currentExhibitor);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
            //else
            //{
            //    bookmarkIcon.Source = "checkyellowicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.AddExhibitor(currentExhibitor);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
        }
    }
}
