using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{    
    public partial class PeopleSpeakerTemplate : ViewCell
    {
      
        public PeopleSpeakerTemplate()
        {
            View = new PeopleTemplateView();
            //TapGestureRecognizer bookmarkTap = new TapGestureRecognizer();
            //bookmarkTap.Tapped += BookMark;
            //bookmarkPersonButton.GestureRecognizers.Add(bookmarkTap);         
        }


        public string GetID()
        {
            return ((PeopleTemplateView)View).id;
        }
                
        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreatePeopleDetail(this, null);
        }

       


        protected override void OnBindingContextChanged()
        {         
            base.OnBindingContextChanged();
            if(BindingContext!=null)
            ((PeopleTemplateView)View).SetDetails();
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }       


    }

    public partial class PeopleTemplateView : ContentView
    {
        public PeopleTemplateView()
        {
            InitializeComponent();
        }

        public bool isBookmarked = false;
        public string id;
        public ServerUser currentPeople = new ServerUser();

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

        public void BookMark(object s, EventArgs e)
        {
            //isBookmarked = !isBookmarked;

            //if (!isBookmarked)
            //{
            //    bookmarkIcon.Source = "addicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.RemovePeople(currentPeople);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
            //else
            //{
            //    bookmarkIcon.Source = "checkyellowicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.AddPeople(currentPeople);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
        }

        public void SetDetails()
        {

            if (BindingContext != null)
            {
                ServerUser user = (ServerUser)BindingContext;
                id = user.userID;
                currentPeople = user;
                if (!string.IsNullOrEmpty(user.userImage))
                {
                    peopleImage.IsVisible = true;
                    peopleImage.Source = user.userImage;
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(user.userFirstName + " " + user.userLastName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    peopleImage.IsVisible = false;
                    peopleImage.Source = "";
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(user.userFirstName+" "+user.userLastName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                if (!string.IsNullOrEmpty(user.userFirstName))
                    fullName.Text = user.userFirstName + " " + user.userLastName;
                else
                    fullName.Text = "";
                if (!string.IsNullOrEmpty(user.userPosition))
                    position.Text = user.userPosition;
                else
                    position.Text = "";
                if (!string.IsNullOrEmpty(user.userCompany))
                    company.Text = user.userCompany;
                else
                    company.Text = "";
                //CheckBookmark(((HomeLayout)App.Current.MainPage).currentUser.userBookmarks.isBookmarked(currentPeople));
            }
        }
    }


   
}
