using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class BookmarksPage : ContentView
    {
        public ServerUser user = new ServerUser();
        public BookmarksPage()
        {
            InitializeComponent();
            user = App.serverData.mei_user.currentUser;
            HomeLayout h = ((HomeLayout)App.Current.MainPage);
            SetClickHandlers(h.CreateSpeakersBookMark, h.CreatePeopleBookMark, h.CreateSessionsBookMark, h.CreateExhibitorsBookMark, h.CreateSponsorsBookMark);
        }

        public async void SetBookmarksCount()
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Loading your bookmarks...");
            user = App.serverData.mei_user.currentUser;
            peopleBCount.Text = (await BaseFunctions.GetPeopleCount(user.userBookmarks.people)).ToString();
            sessionsBCount.Text = (await BaseFunctions.GetSessionCount(user.userBookmarks.session)).ToString();
            speakersBCount.Text = (await BaseFunctions.GetSpeakersCount(user.userBookmarks.speakers)).ToString();
            exhibitorsBCount.Text = (await BaseFunctions.GetExhibitorsCount(user.userBookmarks.exhibitors)).ToString();
            sponsorsBCount.Text = (await BaseFunctions.GetSponsorsCount(user.userBookmarks.sponsors)).ToString();
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }




        public void SetClickHandlers(EventHandler speakers, EventHandler peoples, EventHandler sessions, EventHandler exhibitors, EventHandler sponsors)
        {
            TapGestureRecognizer peopleTaped = new TapGestureRecognizer();
            peopleTaped.Tapped += peoples;
            peopleButton.GestureRecognizers.Add(peopleTaped);

            TapGestureRecognizer sessionTaped = new TapGestureRecognizer();
            sessionTaped.Tapped += sessions;
            sessionsButton.GestureRecognizers.Add(sessionTaped);

            TapGestureRecognizer sponsorsTapped = new TapGestureRecognizer();
            sponsorsTapped.Tapped += sponsors;
            sponsorsButton.GestureRecognizers.Add(sponsorsTapped);

            TapGestureRecognizer speakersTapped = new TapGestureRecognizer();
            speakersTapped.Tapped += speakers;
            speakersButton.GestureRecognizers.Add(speakersTapped);

            TapGestureRecognizer exhibitorsTapped = new TapGestureRecognizer();
            exhibitorsTapped.Tapped += exhibitors;
            exhibitorsButton.GestureRecognizers.Add(exhibitorsTapped);
        }
    }
}
