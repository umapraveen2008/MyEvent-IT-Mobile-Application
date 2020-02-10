using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ScheduleItem : ViewCell
    {


        public ScheduleItem()
        {
            View = new ScheduleItemView();
        }

        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreateSessionDetail(this, null);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                ((ScheduleItemView)View).SetEventITem((ServerSession)BindingContext, ((HomeLayout)App.Current.MainPage).isDuration);
            }
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }


        public string Getid()
        {
            return ((ScheduleItemView)View).id;
        }
        
    }

    public partial class ScheduleItemView : ContentView
    {
        public bool isBookmarked = false;
        public string id;
        public bool isDuration = false;
        public ServerSession currentSession = new ServerSession();

        public ScheduleItemView()
        {
            InitializeComponent();
        }

        public void SetEventITem(ServerSession _session, bool _isDuration)
        {
            currentSession = _session;
            isDuration = _isDuration;
            id = _session.sessionID;
            timingColor.BackgroundColor = GetTimeColor(currentSession.sessionStartTime);
            if (!string.IsNullOrEmpty(_session.sessionName))
                sessionName.Text = _session.sessionName;
            else
                sessionName.Text = "";
            if (!string.IsNullOrEmpty(_session.sessionLocation))
                location.Text = _session.sessionLocation;
            else
                location.Text = "";

            if (isDuration)
                time.Text = BaseFunctions.GetDuration(_session.sessionStartTime, _session.sessionEndTime);
            else
                time.Text = _session.sessionStartTime + " - " + _session.sessionEndTime;
            //CheckBookmark(((HomeLayout)App.Current.MainPage).currentUser.userBookmarks.isBookmarked(currentSession));
        }

        public Color GetTimeColor(string time)
        {
            if (time.Contains("AM"))
                return Color.FromHex("#FEBD11");
            else
                return Color.FromHex("#683081");
        }
    }
}
