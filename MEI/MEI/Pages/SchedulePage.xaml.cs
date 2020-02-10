using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class SchedulePage : ContentView
    {
        public List<DayButtonTemplate> scheduleDays = new List<DayButtonTemplate>();
        public List<ServerSession> eventSessions = new List<ServerSession>();
        public List<ListView> eventListViews = new List<ListView>();
        public List<int> daySessioncount = new List<int>();
        DomainEvent current = new DomainEvent();
        public SessionViewModel s;
        public int currentDay = 0;
        public bool bookMarked = false;
        private Command loadSessionCommand;
        bool canSearch = false;

        public SchedulePage()
        {
            InitializeComponent();
            peopleSearch.TextChanged += OnSearchTextChange;
            sessionParent.ItemTemplate = new DataTemplate(typeof(ScheduleItem));
            sessionParent.RowHeight = 100;
            sessionParent.IsGroupingEnabled = true;
        }

        public Command LoadSessionsCommand
        {
            get
            {
                return loadSessionCommand ?? (loadSessionCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            sessionParent.IsRefreshing = true;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading your bookmarked sessions...");
            if (bookMarked)
            {
                sessionParent.IsVisible = true;
                peopleSearch.BackgroundColor = Color.FromHex("#31c3ee");
                sessionParent.IsPullToRefreshEnabled = false;
                sessionParent.IsGroupingEnabled = false;
                dayScrollView.IsVisible = false;
                peopleSearch.IsVisible = false;
                sessionParent.ChildRemoved += (s, e) =>
                {
                    if (App.serverData.mei_user.b_sessionList.Count > 0)
                    {
                        sessionParent.IsVisible = true;
                        emptyList.IsVisible = false;
                        //ListParent.IsVisible = true;
                    }
                    else
                    {
                        emptyList.IsVisible = true;
                        sessionParent.IsVisible = false;
                        //ListParent.IsVisible = false;
                    }
                };
                sessionParent.ItemsSource = App.serverData.mei_user.b_sessionList;
                await Task.Delay(1000);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                //CreateBookmarkList(App.serverData.b_sessionList.ToList());
            }
            else
            {
                sessionParent.IsVisible = false;
                DomainEvent current = await ((HomeLayout)App.Current.MainPage).GetCurrentEventAfterSyncingSessions(true);
                if (!current.s_event.eventStartDate.Equals(current.s_event.eventEndDate))
                {
                    if (BaseFunctions.GetMonth(current.s_event.eventStartDate) == BaseFunctions.GetMonth(current.s_event.eventEndDate))
                        SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + " - " + BaseFunctions.getDayYear(current.s_event.eventEndDate));
                    else
                        SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + " - " + BaseFunctions.getMonthDay(current.s_event.eventEndDate) + ", " + BaseFunctions.getYear(current.s_event.eventEndDate));
                }
                else
                    SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + ", " + BaseFunctions.getYear(current.s_event.eventEndDate));
                CreateList(current.s_event.eventStartDate, current.s_event.eventEndDate, current.sessionList);
            }

            sessionParent.IsRefreshing = false;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "IsVisible")
                currentDay = 0;
        }

        public async void OnSearchTextChange(object sender, EventArgs e)
        {
            if (canSearch)
            {
                current = await ((HomeLayout)App.Current.MainPage).GetCurrentEventAfterSyncingSessions(false);
                if (!current.s_event.eventStartDate.Equals(current.s_event.eventEndDate))
                {
                    if (BaseFunctions.GetMonth(current.s_event.eventStartDate) == BaseFunctions.GetMonth(current.s_event.eventEndDate))
                        SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + " - " + BaseFunctions.getDayYear(current.s_event.eventEndDate));
                    else
                        SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + " - " + BaseFunctions.getMonthDay(current.s_event.eventEndDate) + ", " + BaseFunctions.getYear(current.s_event.eventEndDate));
                }
                else
                    SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + ", " + BaseFunctions.getYear(current.s_event.eventEndDate));
                CreateList(current.s_event.eventStartDate, current.s_event.eventEndDate, current.sessionList);
            }
        }


        public async void CreateSchedule(bool isBookmarked)
        {
            canSearch = false;
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading event schedule...");
            bookMarked = isBookmarked;
            peopleSearch.Text = "";
            if (isBookmarked)
            {
                emptyText.Text = "You haven't bookmarked any session yet";

                emptyList.IsVisible = false;
                dayScrollView.IsVisible = false;
                peopleSearch.BackgroundColor = Color.FromHex("#31c3ee");
                sessionParent.IsPullToRefreshEnabled = false;
                sessionParent.IsGroupingEnabled = false;
                peopleSearch.IsVisible = false;
                sessionParent.ItemsSource = App.serverData.mei_user.b_sessionList;
                if (App.serverData.mei_user.b_sessionList.Count > 0)
                {
                    sessionParent.IsVisible = true;
                    emptyList.IsVisible = false;
                    //ListParent.IsVisible = true;
                }
                else
                {
                    emptyList.IsVisible = true;
                    sessionParent.IsVisible = false;
                    //ListParent.IsVisible = false;
                }
                sessionParent.ChildRemoved += (s, e) =>
                {
                    if (App.serverData.mei_user.b_sessionList.Count > 0)
                    {
                        sessionParent.IsVisible = true;
                        emptyList.IsVisible = false;
                        //ListParent.IsVisible = true;
                    }
                    else
                    {
                        emptyList.IsVisible = true;
                        sessionParent.IsVisible = false;
                        //ListParent.IsVisible = false;
                    }
                };
                await Task.Delay(1000);
                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
                //CreateBookmarkList(App.serverData.b_sessionList.ToList());
            }
            else
            {
                sessionParent.IsVisible = false;
                current = await ((HomeLayout)App.Current.MainPage).GetCurrentEventAfterSyncingSessions(false);
                if (!current.s_event.eventStartDate.Equals(current.s_event.eventEndDate))
                {
                    if (BaseFunctions.GetMonth(current.s_event.eventStartDate) == BaseFunctions.GetMonth(current.s_event.eventEndDate))
                        SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + " - " + BaseFunctions.getDayYear(current.s_event.eventEndDate));
                    else
                        SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + " - " + BaseFunctions.getMonthDay(current.s_event.eventEndDate) + ", " + BaseFunctions.getYear(current.s_event.eventEndDate));
                }
                else
                    SetEventDateHeader(BaseFunctions.getMonthDay(current.s_event.eventStartDate) + ", " + BaseFunctions.getYear(current.s_event.eventEndDate));
                CreateList(current.s_event.eventStartDate, current.s_event.eventEndDate, current.sessionList);
            }
        }




        public void CreateList(string startDate, string endDate, List<ServerSession> sessions)
        {

            scheduleDays.Clear();
            DaysList.Children.Clear();
            eventSessions = sessions;
            DateTime start = DateTime.ParseExact(startDate, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            DateTime end = DateTime.ParseExact(endDate, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            int totalDays = 1;
            if (start != end)
                totalDays = (int)end.Subtract(start).TotalDays + 1;
            for (int i = 0; i < totalDays; i++)
            {
                DayButtonTemplate day = new DayButtonTemplate();
                DateTime today = start.AddDays(i);
                day.SetButtonText(today.ToString("MMM") + " " + today.Day.ToString(), i, CreateCurrentList);
                scheduleDays.Add(day);
                DaysList.Children.Add(day);
            }
            scheduleDays[currentDay].SetButton();
            CurrentList();
            date.IsVisible = true;
        }

        public async void CurrentList()
        {
            eventListViews.Clear();
            daySessioncount.Clear();
            if (ListParent.Children.Count > 1)
            {
                for (int i = ListParent.Children.Count - 1; i > 1; i--)
                {
                    ListParent.Children.RemoveAt(i);
                }
            }
            for (int j = 0; j < DaysList.Children.Count; j++)
            {
                ListView n = new ListView(ListViewCachingStrategy.RecycleElement);
                n.GroupHeaderTemplate = sessionParent.GroupHeaderTemplate;
                n.HorizontalOptions = sessionParent.HorizontalOptions;
                n.VerticalOptions = sessionParent.VerticalOptions;
                n.HasUnevenRows = sessionParent.HasUnevenRows;
                n.IsPullToRefreshEnabled = true;
                n.RefreshCommand = LoadSessionsCommand;
                n.SeparatorColor = sessionParent.SeparatorColor;
                n.SeparatorVisibility = SeparatorVisibility.Default;
                if (j != currentDay)
                    n.IsVisible = false;
                ListParent.Children.Add(n);
                List<ServerSession> currentDaySessions = new List<ServerSession>();
                for (int i = 0; i < eventSessions.Count; i++)
                {
                    if (eventSessions[i].sessionDay == j)
                    {
                        currentDaySessions.Add(eventSessions[i]);
                    }
                }
                eventListViews.Add(n);
                daySessioncount.Add(currentDaySessions.Count);
                if (j == currentDay)
                {
                    if (currentDaySessions.Count > 0)
                    {
                        eventListViews[j].IsVisible = true;
                        emptyList.IsVisible = false;
                    }
                    else
                    {
                        eventListViews[j].IsVisible = false;
                        emptyList.IsVisible = true;
                    }
                }
               await  CreateList(currentDaySessions, n);
            }
            await Task.Delay(1000);
            await((HomeLayout)App.Current.MainPage).SetLoading(false, "");
            canSearch = true;
        }

        public void ResetButtons()
        {
            for (int i = 0; i < scheduleDays.Count; i++)
            {
                scheduleDays[i].ResetButton();
                eventListViews[i].IsVisible = false;
            }

        }

        public void CreateCurrentList(object sender, EventArgs e)
        {
            ResetButtons();
            currentDay = ((DayButtonTemplate)sender).buttonID;
            ((DayButtonTemplate)sender).SetButton();
            if (daySessioncount.Count > 0)
            {
                if (daySessioncount[currentDay] > 0)
                {
                    eventListViews[currentDay].IsVisible = true;
                    emptyList.IsVisible = false;
                    //ListParent.IsVisible = true;
                }
                else
                {
                    emptyList.IsVisible = true;
                    eventListViews[currentDay].IsVisible = false;
                    //ListParent.IsVisible = false;
                }
            }
        }


        public void SetEventDateHeader(string _date)
        {
            if (!string.IsNullOrEmpty(_date))
                date.Text = _date;
        }


        public void CreateBookmarkList(IList<ServerSession> sessions)
        {
            dayScrollView.IsVisible = false;
            CreateBookMarkList(sessions);
        }

        //public void AddSessionBookmark(Session session)
        //{
        //    ScheduleTimeTemplate day = null;
        //    if (scheduleTimes.Count > 0)
        //    {
        //        for (int i = 0; i < scheduleTimes.Count; i++)
        //        {
        //            if (session.sessionDate.getFullYear() == scheduleTimes[i].scheduleHeader)
        //            {
        //                day = scheduleTimes[i];
        //            }
        //        }
        //    }

        //    if (day == null)
        //    {
        //        day = new ScheduleTimeTemplate();
        //        day.SetHeader(session.sessionDate.getFullYear());
        //        scheduleTimes.Add(day);
        //        scheduleParent.Children.Add(day);
        //    }

        //    day.AddSession(session, false);
        //}

        public async Task<bool> CreateList(IList<ServerSession> sessions, ListView n)
        {
            List<ServerSession> filterPeople = new List<ServerSession>();
            if (!string.IsNullOrEmpty(peopleSearch.Text))
            {
                for (int i = 0; i < sessions.Count; i++)
                {
                    if (sessions[i].sessionName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                        sessions[i].sessionStartTime.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                        sessions[i].sessionLocation.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                        sessions[i].sessionTrack.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase)) //||                        ContainsSpeaker(sessions[i].sessionSpeakers))
                    {
                        filterPeople.Add(sessions[i]);
                    }
                }
            }
            else
            {
                filterPeople = sessions as List<ServerSession>;
            }
            filterPeople.RemoveAll(x => x == null);
            s = new SessionViewModel(filterPeople, SetupList(filterPeople));
            n.ItemTemplate = new DataTemplate(typeof(ScheduleItem));
            n.RowHeight = 100;
            n.IsGroupingEnabled = true;
            n.ItemsSource = s.speakersGroup;
            return true;
        }

        //public bool ContainsSpeaker(List<string> speakers)
        //{

        //    if (speakers != null)
        //    {
        //        if (speakers.Count > 0)
        //        {
        //            for (int i = 0; i < speakers.Count; i++)
        //            {
        //                var currentSpeaker = sp
        //                if (speakers[i].speaker.FirstName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) || speakers[i].speaker.LastName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase)
        //                        || speakers[i].speaker.company.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) || speakers[i].speaker.Position.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
        //                        speakers[i].speakerType.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}

        public async void CreateBookMarkList(IList<ServerSession> sessions)
        {
            List<ServerSession> filterPeople = new List<ServerSession>();
            if (!string.IsNullOrEmpty(peopleSearch.Text))
            {
                for (int i = 0; i < sessions.Count; i++)
                {
                    if (sessions[i].sessionName.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                        sessions[i].sessionStartTime.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                        sessions[i].sessionLocation.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                        sessions[i].sessionTrack.Contains(peopleSearch.Text, StringComparison.OrdinalIgnoreCase))//||     ContainsSpeaker(sessions[i].speaker))
                    {
                        filterPeople.Add(sessions[i]);
                    }
                }
            }
            else
            {
                filterPeople = sessions as List<ServerSession>;
            }
            filterPeople.RemoveAll(x => x == null);
            for (int i = 0; i < filterPeople.Count; i++)
            {
                filterPeople[i].eventID = (await App.serverData.GetSingleEventData(filterPeople[i].eventID)).eventName;
            }
            SessionBookMarkModel sb = new SessionBookMarkModel(filterPeople, SetupListBookMark(filterPeople));
            sessionParent.ItemsSource = sb.speakersGroup;
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "");
        }


        ObservableCollection<Grouping<string, ServerSession>> SetupList(IList<ServerSession> speakers)
        {

            var sorted = from speaker in speakers
                         orderby speaker.sessionName
                         group speaker by speaker.sessionStartTime into speakerGroup
                         select new Grouping<string, ServerSession>(speakerGroup.Key, speakerGroup);

            var speakersGrouped = new ObservableCollection<Grouping<string, ServerSession>>(sorted);

            return speakersGrouped;
        }

        ObservableCollection<Grouping<string, ServerSession>> SetupListBookMark(IList<ServerSession> speakers)
        {

            var sorted = from speaker in speakers
                         orderby speaker.sessionName
                         group speaker by speaker.eventID into speakerGroup
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
                speakersGroup = new ObservableCollection<Grouping<string, ServerSession>>(_speakersGroup.OrderBy(a => GetTimeDuration(a.Key).TimeOfDay));
                Debug.WriteLine(speakersGroup.ToString());
            }
            public DateTime GetTimeDuration(string endTime)
            {
                return DateTime.Parse(endTime);
            }
        }

        public class SessionBookMarkModel
        {
            public IList<ServerSession> speakers { get; set; }
            public ObservableCollection<Grouping<string, ServerSession>> speakersGroup { get; set; }

            public SessionBookMarkModel(IList<ServerSession> _speakers, ObservableCollection<Grouping<string, ServerSession>> _speakersGroup)
            {
                speakers = _speakers;
                speakersGroup = _speakersGroup;
            }
        }
    }

}
