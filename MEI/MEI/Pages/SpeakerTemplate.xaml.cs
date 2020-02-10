using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MEI.Pages
{
    public partial class SpeakerTemplate : ViewCell
    {

        public SpeakerTemplate()
        {
            View = new SpeakerTemplateView();       
            //TapGestureRecognizer bookmarkTap = new TapGestureRecognizer();
            //bookmarkTap.Tapped += BookMark;
            //bookmarkPersonButton.GestureRecognizers.Add(bookmarkTap);
            //currentUser = ((HomeLayout)App.Current.MainPage).currentUser;
        }
     
        public string GetID()
        {
            return ((SpeakerTemplateView)View).id;
        }

        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreateSpeakerDetail(this, null);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
                ((SpeakerTemplateView)View).SetSpeakerDetails((ServerSpeaker)BindingContext);
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }        
    }

    public partial class SpeakerTemplateView : ContentView
    {

        public bool isBookmarked = false;
        public string id;
        public ServerSpeaker currentSpeaker = new ServerSpeaker();

        public SpeakerTemplateView()
        {
            InitializeComponent();
        }

        public void SetSpeakerDetails(ServerSpeaker _speaker)
        {
            currentSpeaker = _speaker;
            if (!string.IsNullOrEmpty(_speaker.speakerFirstName) || !string.IsNullOrEmpty(_speaker.speakerLastName))
                speakerName.Text = _speaker.speakerFirstName + " " + _speaker.speakerLastName;
            else
                speakerName.Text = "";
            if (!string.IsNullOrEmpty(currentSpeaker.speakerImage))
            {
                speakerProfile.IsVisible = true;
                speakerProfile.Source = _speaker.speakerImage;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentSpeaker.speakerFirstName +" "+currentSpeaker.speakerLastName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                speakerProfile.IsVisible = false;
                speakerProfile.Source = "";
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(currentSpeaker.speakerFirstName + " " + currentSpeaker.speakerLastName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (!string.IsNullOrEmpty(_speaker.speakerPosition))
                position.Text = _speaker.speakerPosition;
            else
                position.Text = "";
            if (!string.IsNullOrEmpty(_speaker.speakerCompany))
                company.Text = _speaker.speakerCompany;
            else
                company.Text = "";
            id = _speaker.speakerID;
            //CheckBookmark(currentUser.userBookmarks.isBookmarked(currentSpeaker));
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


        public void BookMark(object s, EventArgs e)
        {
            //isBookmarked = !isBookmarked;
            //if (!isBookmarked)
            //{
            //    bookmarkIcon.Source = "addicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.RemoveSpeaker(currentSpeaker);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
            //else
            //{
            //    bookmarkIcon.Source = "checkyellowicon.png";
            //    ServerUser p = ((HomeLayout)App.Current.MainPage).currentUser;
            //    BookMark b = p.userBookmarks;
            //    b.AddSpeaker(currentSpeaker);
            //    p.userBookmarks = b;
            //    ((HomeLayout)App.Current.MainPage).currentUser = p;
            //}
        }
    }
}
