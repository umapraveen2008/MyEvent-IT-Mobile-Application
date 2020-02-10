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
    public partial class ExhibitorsDetailsPage : ContentView
    {
        public string id;
        public bool isBookmarked = false;
        public ExhibitorGroup currentExhibitor = new ExhibitorGroup();
        public ExhibitorsTemplate parentExhibitor = new ExhibitorsTemplate();        
        public ExhibitorsDetailsPage()
        {
            InitializeComponent();
            AddTap(AddNotesButton,(s,e)=> { ((HomeLayout)App.Current.MainPage).CreateNewNote(this, null, currentExhibitor); });
            //bookmarkButton.Clicked += BookMark;
            AddTap(bookmarkButton,BookMark);
            AddTap(vCardButton, App.createContact);
            AddSocialTaps();
        }

        void AddTap(ContentView f, EventHandler e)
        {
            TapGestureRecognizer t = new TapGestureRecognizer();
            t.Tapped += e;
            f.GestureRecognizers.Add(t);
        }

        void AddSocialTaps()
        {
            AddTap(VisitWebsite, OpenWebiste);
            AddTap(facebookButton, OpenFacebook);
            AddTap(twitterButton, OpenTwitter);
            AddTap(gmailButton, OpenGplus);
            AddTap(linkedInButton, OpenLinkedIn);
        }

        public void OpenFacebook(object sender ,EventArgs e)
        {            
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentExhibitor.company.companyFacebook, "Facebook");
            //Device.OpenUri(new Uri("fb://page/"+ currentExhibitor.company.companyFacebook));
        }

        public void OpenTwitter(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentExhibitor.company.companyTwitter, "Twitter");
            //Device.OpenUri(new Uri("twitter://user?user_id="+currentExhibitor.company.companyTwitter));
        }

        public void OpenGplus(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentExhibitor.company.companyGplus, "Instagram");
            //Device.OpenUri(new Uri(currentExhibitor.company.companyGplus));
        }

        public void OpenLinkedIn(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentExhibitor.company.companyLinkedIn, "LinkedIn");
        }

        public void OpenWebiste(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentExhibitor.company.companyWebsite, "Website");
        }

        public void SetVisibility(ContentView frame, bool visible)
        {
            frame.IsEnabled = visible;
            frame.InputTransparent = !visible;
            if (visible)
                frame.Opacity = 1;
            else
                frame.Opacity = 0.3f;
        }

        public void CheckSocialVisiblilty()
        {
            SetVisibility(VisitWebsite, !string.IsNullOrEmpty(currentExhibitor.company.companyWebsite));
            SetVisibility(facebookButton, !string.IsNullOrEmpty(currentExhibitor.company.companyFacebook));
            SetVisibility(twitterButton, !string.IsNullOrEmpty(currentExhibitor.company.companyTwitter));
            SetVisibility(gmailButton, !string.IsNullOrEmpty(currentExhibitor.company.companyGplus));
            SetVisibility(linkedInButton, !string.IsNullOrEmpty(currentExhibitor.company.companyLinkedIn));
            SetVisibility(vCardButton, !string.IsNullOrEmpty(currentExhibitor.company.companyPhone));
        }

        public async void ExhibitorDetials(ExhibitorGroup _exhibitor,ExhibitorsTemplate parentEx)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading exhibitor...");
            currentExhibitor = _exhibitor;
            parentExhibitor = parentEx;
            VcardContact c_user = new VcardContact();
            c_user.FirstName = currentExhibitor.company.CompanyName;
            c_user.LastName = currentExhibitor.company.CompanyName;
            c_user.company = currentExhibitor.company.CompanyName;
            c_user.phoneNumber = currentExhibitor.company.companyPhone;            
            c_user.email = currentExhibitor.company.companyEmail;
            App.contactuser = c_user;
            if (currentExhibitor.company != null && currentExhibitor != null)
            {
                CheckSocialVisiblilty();             
                if (!string.IsNullOrEmpty(currentExhibitor.company.companyLogo))
                {
                    exhibitorLogo.Source = currentExhibitor.company.companyLogo;
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentExhibitor.company.CompanyName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    exhibitorLogo.Source = "";
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentExhibitor.company.CompanyName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                if (!string.IsNullOrEmpty(currentExhibitor.company.CompanyName))
                    exhibitorName.Text = currentExhibitor.company.CompanyName;                
                if (currentExhibitor.exhibitor.exhibitorFields.Count>0)
                    BaseFunctions.GetCustomFields(customFieldsLayout,currentExhibitor.exhibitor.exhibitorFields);
                id = currentExhibitor.exhibitor.exhibitorID;
                if (!string.IsNullOrEmpty(currentExhibitor.company.companyDescription))
                {

                    description.Text = currentExhibitor.company.companyDescription;
                    emptyList.IsVisible = false;
                }
                else
                {
                    ((ContentView)description.Parent).IsVisible = false;
                    emptyList.IsVisible = true;
                }
                CheckBookmark(App.serverData.mei_user.currentUser.userBookmarks.isBookmarked(currentExhibitor));
            }
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "loading exhibitor...");
        }

        

        public void CheckBookmark(bool isMarked)
        {
            isBookmarked = isMarked;
            if (isMarked)
            {
                //bookmarkButton.Text = "Remove";
                bookmarkIcon.Source = "mei_bookmarked_active.png";
            }
            else
            {
                //bookmarkButton.Text = "Bookmark";
                bookmarkIcon.Source = "mei_bookmark_active.png";
            }
        }


        public void BookMark(object s, EventArgs e)
        {
            isBookmarked = !isBookmarked;
            if (!isBookmarked)
            {
                //bookmarkButton.Text = "Bookmark";
                bookmarkIcon.Source = "mei_bookmark_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.RemoveExhibitors(currentExhibitor);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
            else
            {
                bookmarkIcon.Source = "mei_bookmarked_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.AddExhibitor(currentExhibitor);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
        }
    }
}
