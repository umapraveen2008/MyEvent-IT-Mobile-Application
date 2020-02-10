using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class SessionDetailPage : ContentView
    {
        public string id;
        public bool isBookmarked = false;
        public ServerSession currentSession = new ServerSession();
        public ScheduleItem parentSession = new ScheduleItem();

        public SessionDetailPage()
        {
            InitializeComponent();
            AddTap(AddNotesButton, (s, e) => { ((HomeLayout)App.Current.MainPage).CreateNewNote(this, null, currentSession); });
            AddTap(bookmarkButton, BookMark);
            //AddTap(bookmarkButton,BookMark);
        }

        void AddTap(ContentView f, EventHandler e)
        {
            TapGestureRecognizer t = new TapGestureRecognizer();
            t.Tapped += e;
            f.GestureRecognizers.Add(t);
        }

        public async void SessionDetails(ServerSession _session,ScheduleItem parentSes)
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading session...");
            currentSession = _session;
            parentSession = parentSes;
            id = _session.sessionID;
            if(_session.eventID.Contains(":"))
            sessionEvent.Text = (await App.serverData.GetSingleEventData(_session.eventID)).eventName;
            else
                sessionEvent.Text = _session.eventID;
            if (!string.IsNullOrEmpty(_session.sessionTrack))
                sessionCategory.Text = _session.sessionTrack;
            else
                sessionCategory.Text = "category not specified";
            if (!string.IsNullOrEmpty(_session.sessionName))
                sessionName.Text = _session.sessionName;
            else
                sessionName.Text = "";
            if (!string.IsNullOrEmpty(_session.sessionLocation))
                location.Text = _session.sessionLocation;
            else
                location.Text = "not specified";
            DateTime start = DateTime.ParseExact((await App.serverData.GetSingleEventData(_session.eventID)).eventStartDate, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            time.Text = start.AddDays(_session.sessionDay).ToString("MM-dd-yyyy")+" / "+_session.sessionStartTime + " - " + _session.sessionEndTime;
            if (!string.IsNullOrEmpty(_session.sessionDescription))
            {
                emptyList.IsVisible = false;
                description.Text = _session.sessionDescription;
            }
            else
            {
                description.IsVisible = false;
                emptyList.IsVisible = true;
            }
            CheckBookmark(App.serverData.mei_user.currentUser.userBookmarks.isBookmarked(currentSession));
            if (_session.sessionSpeakers.Count > 0)
            {
                sessionSpeakers.Text = "";
                for (int i = 0; i < _session.sessionSpeakers.Count; i++)
                {
                    ServerSpeaker company = await App.serverData.GetOneSpeaker(_session.sessionSpeakers[i]);
                    if(company!=null)
                    sessionSpeakers.Text = sessionSpeakers.Text + (i + 1).ToString() + ". " + company.speakerFirstName+" "+company.speakerLastName + "<br>";
                }
            }
            speakersLoading.IsVisible = false;
            sessionSpeakers.IsVisible = true;
            if (_session.sessionExhibitors.Count > 0)
            {
                sessionExhibitors.Text = "";
                for (int i = 0; i < _session.sessionExhibitors.Count; i++)
                {
                    ExhibitorGroup company = await App.serverData.GetOneExhibitor(_session.sessionExhibitors[i]);
                    if (company != null)
                        sessionExhibitors.Text = sessionExhibitors.Text + (i + 1).ToString() + ". " + company.company.CompanyName + "<br>";
                }
            }
            exhibitorsLoading.IsVisible = false;
            sessionExhibitors.IsVisible = true;
            if (_session.sessionSponsors.Count>0)
            {
                sessionSponsors.Text = "";
                for(int i =0;i<_session.sessionSponsors.Count;i++)
                {
                    SponsorGroup company = await App.serverData.GetOneSponsor(_session.sessionSponsors[i]);
                    if (company != null)
                        sessionSponsors.Text = sessionSponsors.Text + (i+1).ToString() + ". " + company.company.CompanyName+ "<br>";
                }              
            }
            sponsorsLoading.IsVisible = false;
            sessionSponsors.IsVisible = true;
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
        }

        //public string GetExhibitorCompanyID(string id)
        //{            
        //    List<ServerExhibitor> exhibitors = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().exhibitors;
        //    for(int i = 0;i<exhibitors.Count;i++)
        //    {
        //        if(exhibitors[i].exhibitorID == id)
        //        {
        //            return exhibitors[i].exhibitorCompanyID;
        //        }
        //    }
        //    return null;
        //}

        //public string GetSponsorCompanyID(string id)
        //{
        //    List<ServerSponsor> sponsors = ((HomeLayout)App.Current.MainPage).GetCurrentDomainEvent().sponsors;
        //    for (int i = 0; i < sponsors.Count; i++)
        //    {
        //        if (sponsors[i].sponsorID == id)
        //        {
        //            return sponsors[i].sponsorCompanyID;
        //        }
        //    }
        //    return null;
        //}


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

        public void BookMark(object s, EventArgs e)
        {
            isBookmarked = !isBookmarked;
            if (!isBookmarked)
            {
                bookmarkIcon.Source = "mei_bookmark_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.RemoveSession(currentSession);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
            else
            {
                bookmarkIcon.Source = "mei_bookmarked_active.png";
                ServerUser p = App.serverData.mei_user.currentUser;
                BookMark b = p.userBookmarks;
                b.AddSession(currentSession);
                p.userBookmarks = b;
                App.serverData.mei_user.currentUser = p;
            }
        }
        
    }
}
