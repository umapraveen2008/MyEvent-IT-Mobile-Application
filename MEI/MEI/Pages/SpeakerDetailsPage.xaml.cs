using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class SpeakerDetailsPage : ContentView
    {
        public SessionViewModel s;
        public bool isBookmarked=false;
        public ServerSpeaker currentSpeaker = new ServerSpeaker();
        public SpeakerTemplate speakerTemp = new SpeakerTemplate();

        public SpeakerDetailsPage()
        {
            InitializeComponent();            
            AddTap(AddNotesButton, (s, e) => { ((HomeLayout)App.Current.MainPage).CreateNewNote(this, null, currentSpeaker); });            
            ((HomeLayout)App.Current.MainPage).isDuration = true;
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

        public async void SetSpeakerDetail(ServerSpeaker speaker,SpeakerTemplate parentSpeaker)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading speaker...");
            currentSpeaker = speaker;
            speakerTemp = parentSpeaker;
            VcardContact c_user = new VcardContact();
            c_user.FirstName = currentSpeaker.speakerFirstName;
            c_user.LastName = currentSpeaker.speakerLastName;
            c_user.company = currentSpeaker.speakerCompany;
            c_user.phoneNumber = currentSpeaker.speakerPhone;
            c_user.email = currentSpeaker.speakerEmail;
            App.contactuser = c_user;
            if (speaker != null)
            {
                CheckSocialVisiblilty();
                if (!string.IsNullOrEmpty(speaker.speakerFirstName) || !string.IsNullOrEmpty(speaker.speakerLastName))
                    speakerFullNameText.Text = speaker.speakerFirstName + " " + speaker.speakerLastName;
                else
                    speakerFullNameText.Text = "";
                if (!string.IsNullOrEmpty(speaker.speakerPosition))
                    positionText.Text = speaker.speakerPosition;
                else
                    positionText.Text = "";
                if (!string.IsNullOrEmpty(speaker.speakerCompany))
                    companyText.Text = speaker.speakerCompany;
                else
                    companyText.Text = "";
                if (!string.IsNullOrEmpty(speaker.speakerImage))
                {
                    speakerIcon.Source = speaker.speakerImage;                    
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(speaker.speakerFirstName + " " + speaker.speakerLastName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    speakerIcon.Source = "";
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(speaker.speakerFirstName + " " + speaker.speakerLastName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
            }
            if (((HomeLayout)App.Current.MainPage).bookmarksPage != null)
            {
                if (!((HomeLayout)App.Current.MainPage).bookmarksPage.IsVisible)
                    CreateSessionList(await BaseFunctions.GetSpeakerCurrentEventSessions(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().sessionList, speaker.speakerID));
                else
                    CreateSessionList(await App.serverData.GetSpeakerAllEventsSessions(speaker.speakerID));
            }
            else
            {
                CreateSessionList(await BaseFunctions.GetSpeakerCurrentEventSessions(((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().sessionList, speaker.speakerID));
            }
            CheckBookmark(App.serverData.mei_user.currentUser.userBookmarks.isBookmarked(currentSpeaker));
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
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

        public void CreateSessionList(IList<ServerSession> sessions)
        {
            if (sessions.Count > 0)
            {
                emptyList.IsVisible = false;
                speakerSessionList.IsVisible = true;
            }
            else
            {
                speakerSessionList.IsVisible = false;
                emptyList.IsVisible = true;
            }
            s = new SessionViewModel(sessions, SetupList(sessions));
            speakerSessionList.ItemTemplate = new DataTemplate(typeof(ScheduleItem));
            speakerSessionList.RowHeight = 80;
            speakerSessionList.IsGroupingEnabled = false;
            speakerSessionList.ItemsSource = sessions;
        }

        ObservableCollection<Grouping<string, ServerSession>> SetupList(IList<ServerSession> speakers)
        {

            var sorted = from speaker in speakers
                         orderby speaker.sessionName
                         group speaker by speaker.sessionTrack into speakerGroup
                         select new Grouping<string, ServerSession>(speakerGroup.Key, speakerGroup);

            var speakersGrouped = new ObservableCollection<Grouping<string, ServerSession>>(sorted);

            return speakersGrouped;
        }


        public class SessionViewModel
        {
            public IList<ServerSession> speakers { get; set; }
            public ObservableCollection<Grouping<string, ServerSession>> speakersGroup { get; set; }

            public SessionViewModel(IList<ServerSession> _speakers, ObservableCollection<Grouping<string, ServerSession>> _speakersGroup)
            {
                speakers = _speakers;
                speakersGroup = new ObservableCollection<Grouping<string, ServerSession>>(_speakersGroup.OrderBy(a => a.Key));
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
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSpeaker.speakerFacebook, "Facebook");
        }

        public void OpenTwitter(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSpeaker.speakerTwitter, "Twitter");
        }

        public void OpenGplus(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSpeaker.speakerGplus, "Instagram");
        }

        public void OpenLinkedIn(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSpeaker.speakerLinkedIn, "LinkedIn");
        }

        public void OpenWebiste(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateWebView(sender, e, currentSpeaker.speakerWebsite, "Website");
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
            SetVisibility(VisitWebsite, !string.IsNullOrEmpty(currentSpeaker.speakerWebsite));
            SetVisibility(facebookButton, !string.IsNullOrEmpty(currentSpeaker.speakerFacebook));
            SetVisibility(twitterButton, !string.IsNullOrEmpty(currentSpeaker.speakerTwitter));
            SetVisibility(gmailButton, !string.IsNullOrEmpty(currentSpeaker.speakerGplus));
            SetVisibility(linkedInButton, !string.IsNullOrEmpty(currentSpeaker.speakerLinkedIn));
            SetVisibility(vCardButton, !string.IsNullOrEmpty(currentSpeaker.speakerPhone));
        }

        public void BookMark(object s, EventArgs e)
        {
            isBookmarked = !isBookmarked;
            if (!isBookmarked)
            {

                bookmarkIcon.Source = "mei_bookmark_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.RemoveSpeaker(currentSpeaker);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
            else
            {
                bookmarkIcon.Source = "mei_bookmarked_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.AddSpeaker(currentSpeaker);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
        }
        
    }
}
