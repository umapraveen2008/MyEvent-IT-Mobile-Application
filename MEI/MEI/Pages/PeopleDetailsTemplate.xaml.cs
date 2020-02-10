using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class PeopleDetailsTemplate : ContentView
    {
        public bool isBookmarked;
        public ServerUser currentUser = new ServerUser();
        public PeopleSpeakerTemplate parentPeople = new PeopleSpeakerTemplate();


        public PeopleDetailsTemplate()
        {
            InitializeComponent();            
            AddTap(AddNotesButton, (s, e) => { ((HomeLayout)App.Current.MainPage).CreateNewNote(this, null, currentUser); });            
            AddTap(bookmarkButton, BookMark);
            AddTap(vCardButton, App.createContact);
            AddSocialTaps();
        }

        void AddTap(ContentView f,EventHandler e)
        {
            TapGestureRecognizer t = new TapGestureRecognizer();
            t.Tapped += e;
            f.GestureRecognizers.Add(t);
        }

        public async void SetPeopleCheck(ServerUser user,PeopleSpeakerTemplate parentPep)
        {
            await((HomeLayout)App.Current.MainPage).SetLoading(true, "loading user...");
            currentUser = user;
            if(parentPep != null)
            parentPeople = parentPep;
            VcardContact c_user = new VcardContact();
            c_user.FirstName = currentUser.userFirstName;
            c_user.LastName = currentUser.userLastName;
            c_user.company = currentUser.userCompany;
            c_user.phoneNumber = currentUser.userPhone;
            c_user.email = currentUser.userEmail;
            App.contactuser = c_user;
            if (currentUser != null)
            {
                CheckSocialVisiblilty();
                if (!string.IsNullOrEmpty(user.userImage))
                {
                    peopleImage.Source = user.userImage;
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentUser.userFirstName + " " + currentUser.userLastName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    peopleImage.Source = "";
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentUser.userFirstName + " " + currentUser.userLastName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                if (!string.IsNullOrEmpty(user.userFirstName))
                    fullNameText.Text = user.userFirstName + " " + user.userLastName;
                else
                    fullNameText.Text = "";
                if (!string.IsNullOrEmpty(user.userPosition))
                    positionText.Text = user.userPosition;
                else
                    positionText.Text = "";
                if (!string.IsNullOrEmpty(user.userCompany))
                    companyText.Text = user.userCompany;
                else
                    companyText.Text = "";
                if (!string.IsNullOrEmpty(user.userDescription))
                {
                    bioView.IsVisible = true;
                    emptyList.IsVisible = false;
                    bioDescriptionText.Text = user.userDescription;
                }
                else
                {
                    bioView.IsVisible = false;
                    emptyList.IsVisible = true;
                    //bioDescriptionText.Text = "Bio unavailable";
                }
            }
            CheckBookmark(App.serverData.mei_user.currentUser.userBookmarks.isBookmarked(currentUser));
            await Task.Delay(1000);
            await((HomeLayout)App.Current.MainPage).SetLoading(false, "");
        }

        public void CheckBookmark(bool isMarked)
        {
            isBookmarked = isMarked;
            if (isMarked)
            {
                bookmarkIcon.Source = "mei_bookmarked_active.png";
            }
            else
            {
                bookmarkIcon.Source = "mei_bookmark_active.png";
            }

        }

        void AddSocialTaps()
        {
            AddTap(VisitWebsite, OpenWebiste);
            AddTap(facebookButton, OpenFacebook);
            AddTap(twitterButton, OpenTwitter);
            AddTap(gmailButton, OpenGplus);
            AddTap(linkedInButton, OpenLinkedIn);
        }

        public void OpenFacebook(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentUser.userFacebook, "Facebook");
        }

        public void OpenTwitter(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentUser.userTwitter, "Twitter");
        }

        public void OpenGplus(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentUser.userGplus, "Instagram");
        }

        public void OpenLinkedIn(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentUser.userLinkedIn, "LinkedIn");
        }

        public void OpenWebiste(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentUser.userWebsite, "Website");
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
            SetVisibility(VisitWebsite, !string.IsNullOrEmpty(currentUser.userWebsite));
            SetVisibility(facebookButton, !string.IsNullOrEmpty(currentUser.userFacebook));
            SetVisibility(twitterButton, !string.IsNullOrEmpty(currentUser.userTwitter));
            SetVisibility(gmailButton, !string.IsNullOrEmpty(currentUser.userGplus));
            SetVisibility(linkedInButton, !string.IsNullOrEmpty(currentUser.userLinkedIn));
            SetVisibility(vCardButton, !string.IsNullOrEmpty(currentUser.userPhone));
        }

        public void BookMark(object s, EventArgs e)
        {
            isBookmarked = !isBookmarked;
            if (!isBookmarked)
            {
                bookmarkIcon.Source = "mei_bookmark_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.RemovePeople(currentUser);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
            else
            {
                bookmarkIcon.Source = "mei_bookmarked_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.AddPeople(currentUser);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
        }
        
    }
}
