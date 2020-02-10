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
    public partial class SponsorsDetailPage : ContentView
    {
        public string id;
        public string websiteURL="http://www.cgsolutionsgroup.com";
        public bool isBookmarked = false;
        public SponsorGroup currentSponsor =new SponsorGroup();
        public SponsorsTemplate sponsorTemp = new SponsorsTemplate();        
        public SponsorsDetailPage()
        {
            InitializeComponent();
            AddTap(AddNotesButton, (s, e) => { ((HomeLayout)App.Current.MainPage).CreateNewNote(this, null, currentSponsor); });
            AddTap(bookmarkButton, BookMark);
            AddTap(vCardButton, App.createContact);
            AddSocialTaps();
        }

        void AddTap(ContentView f, EventHandler e)
        {
            TapGestureRecognizer t = new TapGestureRecognizer();
            t.Tapped += e;
            f.GestureRecognizers.Add(t);
        }

        public async void SponsorDetials(SponsorGroup _sponsor,SponsorsTemplate parentSponsor)
        {
            await((HomeLayout)App.Current.MainPage).SetLoading(true, "loading sponsor...");
            currentSponsor = _sponsor;
            sponsorTemp = parentSponsor;
            VcardContact c_user = new VcardContact();
            c_user.FirstName = currentSponsor.company.companyName;
            c_user.LastName = currentSponsor.company.companyName;
            c_user.company = currentSponsor.company.companyName;
            c_user.phoneNumber = currentSponsor.company.companyPhone;
            c_user.email = currentSponsor.company.companyEmail;
            App.contactuser = c_user;
            if (currentSponsor.company != null)
            {
                CheckSocialVisiblilty();
                if (!string.IsNullOrEmpty(currentSponsor.company.companyLogo))
                {
                    sponsorLogo.Source = currentSponsor.company.companyLogo;
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentSponsor.company.companyName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    sponsorLogo.Source = "";
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentSponsor.company.companyName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                if (!string.IsNullOrEmpty(currentSponsor.company.companyName))
                    sponsorName.Text = currentSponsor.company.companyName;
                else
                    sponsorName.Text = "";
                if (currentSponsor.sponsor.sponsorFields.Count > 0)
                   BaseFunctions.GetCustomFields(customFieldsLayout,currentSponsor.sponsor.sponsorFields);
                if (!string.IsNullOrEmpty(currentSponsor.company.companyWebsite))
                    websiteURL = currentSponsor.company.companyWebsite;
                id = _sponsor.sponsor.sponsorID;
                if (!string.IsNullOrEmpty(currentSponsor.company.companyDescription))
                {
                    description.Text = currentSponsor.company.companyDescription;
                    emptyList.IsVisible = false;
                }
                else
                {
                    emptyList.IsVisible = true;
                    description.ParentView.IsVisible = false;
                }
            }
            CheckBookmark(App.serverData.mei_user.currentUser.userBookmarks.isBookmarked(currentSponsor));
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
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSponsor.company.companyFacebook, "Facebook");
        }

        public void OpenTwitter(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSponsor.company.companyTwitter, "Twitter");
        }

        public void OpenGplus(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSponsor.company.companyGplus, "Instagram");
        }

        public void OpenLinkedIn(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSponsor.company.companyLinkedIn, "LinkedIn");
        }

        public void OpenWebiste(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSponsor.company.companyWebsite, "Website");
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
            SetVisibility(VisitWebsite, !string.IsNullOrEmpty(currentSponsor.company.companyWebsite));
            SetVisibility(facebookButton, !string.IsNullOrEmpty(currentSponsor.company.companyFacebook));
            SetVisibility(twitterButton, !string.IsNullOrEmpty(currentSponsor.company.companyTwitter));
            SetVisibility(gmailButton, !string.IsNullOrEmpty(currentSponsor.company.companyGplus));
            SetVisibility(linkedInButton, !string.IsNullOrEmpty(currentSponsor.company.companyLinkedIn));
            SetVisibility(vCardButton, !string.IsNullOrEmpty(currentSponsor.company.companyPhone));
        }

        public void BookMark(object s, EventArgs e)
        {
            isBookmarked = !isBookmarked;
            if (!isBookmarked)
            {
                bookmarkIcon.Source = "mei_bookmark_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.RemoveSponsor(currentSponsor);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
            else
            {
                bookmarkIcon.Source = "mei_bookmarked_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.AddSponsor(currentSponsor);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
        }
        
    }
}
