
using MEI.Controls;
using MEI.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using Plugin.Media;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Net;

namespace MEI
{
    public partial class HomeLayout : MasterDetailPage
    {
        public StackLayout _panel;
        public double xDelta, currePosition;
        public ObservableCollection<RegisteredDomainViewModel> registeredDomain = new ObservableCollection<RegisteredDomainViewModel>();
        public bool inAnimation = false;
        public List<string> tabMenuItems = new List<string> {
            "About Event",
            "Event Updates",
            "Speakers",
            "Attendees",
            "Schedule",
            "Exhibitors",
            "Sponsors",
            "Catalog",
            "Floor Map",
            "Venue Map",
            "Event FAQ" };
        public List<string> sideMenuIcons = new List<string> {
            "mei_aboutdomainicon_w.png",
            "mei_timelineicon_w.png",
            "mei_speakericon_w.png",
            "mei_attendeeicon_w.png",
            "mei_dateicon_w.png",
            "mei_exhibitoricon_w.png",
            "mei_sponsoricon_w.png",
            "mei_shoppingbagicon_w.png",
            "mei_flooricon_w.png",
            "mei_venueicon_w.png",
            "mei_faqicon_w.png" };
        public bool updateCheckRunning = false;
        public List<EventItem> eventMenu = new List<EventItem>();
        public object currentScreen;
        //public List<ServerUser> currentEventPeople = new List<ServerUser>();        
        public bool seeingForNewDomains = false;
        public PeopleDetailsTemplate peopleDetail;
        public UnderConstructionPage pageUnderCon;
        public bool isLoading = false;
        public SpeakerPage speakerPage;
        public ExhibitorsPage exhibitorsPage;
        public EventHomePage eventHomePage;
        public SchedulePage schedulePage;
        public SponsorsPage sponsorsPage;
        public FAQPage faqPage;
        public PurchasePage purchasePage;
        public Cart cartPage;
        public AboutPage aboutPage;
        public EventUpdates eventUpdatesScreen;
        public SettingsPage settingsScreen;
        public PeoplePage peoplePage;
        public CatalogList catalogPage;
        public EventMap venuMap;
        public EditProfile editProfile;
        public PaymentInformation paymentInformation;
        public ChangePassword changePassword;
        public PaymentList paymentList;
        public ShippingList shippingList;
        public NotesPage notesPage;
        public FloorMapPage floorPage;
        public BookmarksPage bookmarksPage;
        public ServerDomain currentDomain;
        public ShippingInformation shippingInformation;
        public PurchaseHistory purchaseHistory;
        public bool _PanelShowing = true;
        public bool isDuration = false;
        public List<object> historyScreens = new List<object>();
        private Command loadDomainCommand;


        public void ClearOtherScreens()
        {
            for (int i = 0; i < mainPage.Children.Count; i++)
            {
                if (mainPage.Children[i] != isLoadingView && mainPage.Children[i] != domainsListPage && mainPage.Children[i] != PrimaryScreen)
                    mainPage.Children.RemoveAt(i);
            }

            if (mainPage.Children.IndexOf(domainsListPage) > 0 && domainsListPage.IsVisible)
            {
                ////indicator.IsVisible = true;
            }

        }

        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }

        //public async void CheckForNewEvents()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            updateCheckRunning = true;
        //            Debug.WriteLine("Update Checking Wait....");
        //            await Task.Delay(60000);
        //            Debug.WriteLine("Update Checking....");
        //            if (!App.eventCreation && App.AppHaveInternet)
        //            {
        //                var k = await App.serverData.GetDomainEventData();
        //                if (allEvents.Count != App.serverData.eventList.Count)
        //                {
        //                    var update = await DisplayAlert("Alert", "There are changes to an event(s). Do you want to update now?", "Yes", "No");
        //                    if (update)
        //                    {
        //                        await SetLoading(true, "Syncing with new events...");
        //                        CreateEvents();
        //                    }
        //                    else
        //                    {
        //                        await DisplayAlert("Alert", "To update later please restart the App.", "Ok");
        //                        App.checkForUpdates = false;
        //                    }
        //                }
        //                else
        //                {
        //                    allEvents = await SyncEventsList();
        //                }
        //            }
        //            if (!App.checkForUpdates && !App.AppHaveInternet)
        //                break;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(e.ToString());
        //    }
        //    await SetLoading(false, "....");
        //}

        //async Task<List<DomainEvent>> CreateDummyData()
        //{
        //    try
        //    {               
        //        events = await EventCreation.GetCurrentEvents();
        //        if (App.serverData.mei_user.currentEventIndex >= events.Count)
        //            App.serverData.mei_user.currentEventIndex = 0;
        //        if (events.Count > 0)
        //        {
        //            events.Sort((a, b) => GetDateTime(b.startDate).CompareTo(GetDateTime(a.startDate)));
        //        }
        //        return events;
        //    }
        //    catch(Exception e)
        //    {
        //        Debug.WriteLine(e.ToString());
        //    }
        //    return null;
        //}


        public async void CreateEvents()
        {
            try
            {
                if (!App.eventCreation)
                {
                    await SetProgressBar(.1);
                    await SetLoading(true, "Getting Users from server..");
                    await SetProgressBar(.3);
                    await SetLoading(true, "Getting Domains from server");
                    await SetProgressBar(.5);
                    //SetLoading(true, "Getting user registered domain..");
                    //await App.serverData.GetRegisteredDomain();
                    //await SetProgressBar(.4);
                    //SetLoading(true, "Getting user requested domain..");
                    //await App.serverData.GetRequestedDomains();
                    //await SetProgressBar(.5);                    
                    sideMenuBottom.SetUser(App.serverData.mei_user.currentUser);
                    App.serverData.SyncBookmarkList();
                    App.serverData.SaveUserDataToLocal();
                    //new List<ServerDomain>(App.serverData.registeredDomainList.OrderBy(a => GetSort(a)));              
                }

                if (App.serverData.mei_user.registeredDomainList.Count > 0)
                {
                    eventMenu.Clear();
                    eventList.Children.Clear();
                    bool domainCheck = App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count == await EventCreation.GetDomainEventCount(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domain.firmID);
                    if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents != null && domainCheck)
                    {
                        App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domain = await App.serverData.GetDomainFromServer(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domain.firmID);
                        currentDomain = App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domain;
                    }
                    else
                    {
                        currentDomain = App.serverData.mei_user.registeredDomainList[App.serverData.mei_user.currentDomainIndex];
                        if (App.firmID == "")
                            App.firmID = App.serverData.mei_user.registeredDomainList[0].firmID;
                        await SetProgressBar(.7);
                        await SetLoading(true, "Syncing current domain events...");
                        DomainGroup dGroup = App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex];
                        dGroup.domainEvents = new List<DomainEvent>();
                        List<ServerEvent> events = await SyncEventsList(dGroup.domain.firmID);

                        for (int i = 0; i < events.Count; i++)
                        {
                            DomainEvent dEvent = new DomainEvent();
                            dEvent.s_event = events[i];
                            dGroup.domainEvents.Add(dEvent);
                        }
                        App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex] = dGroup;
                        App.serverData.SaveDomainDBToLocal();
                    }
                    if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count > 0)
                    {
                        eventListParent.IsVisible = true;
                        emptyEventList.IsVisible = false;
                        sideEventMenuSection.IsVisible = true;
                        CreateEventList();
                        //await Task.Delay(100);
                        //if (aboutPage == null & App.firmID != "")
                        //    CreateAboutScreen();
                    }
                    else
                    {
                        emptyEventList.IsVisible = true;
                        eventListParent.IsVisible = false;
                        sideEventMenuSection.IsVisible = false;
                        //await DisplayAlert("Alert", "No Events in Current Domain", "Ok");
                        //App.firmID = "";
                        await SetLoading(false, "");
                        //IsGestureEnabled = false;
                    }
                    aboutDomainLabel.Text = "About " + currentDomain.domainName;
                    aboutDomainSideButton.IsVisible = true;
                }
                else
                {
                    
                    mainPage.RaiseChild(domainsListPage);
                    domainsListPage.IsVisible = true;
                    aboutDomainSideButton.IsVisible = false;
                    sideEventMenuSection.IsVisible = false;
                    eventListParent.IsVisible = false;
                    eventList.Children.Clear();
                    ////indicator.IsVisible = true;
                    await SetLoading(false, "");
                    if (App.FirstTime)
                    {
                        OpenEducation(this,null);
                        App.FirstTime = false;
                    }
                    //IsGestureEnabled = false;
                }
                //if(!updateCheckRunning)             
                //    CheckForNewEvents();
                if (App.eventCreation)
                    IsPresented = true;
                App.eventCreation = true;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void SettingsMenu(object sender, EventArgs e)
        {
            this.IsPresented = !this.IsPresented;
        }

        public void HomeMenu(object sender, EventArgs e)
        {
            //this.IsPresented = !this.IsPresented;
            CreateEventHomePage();
        }

        public void SetTapButtons()
        {
            TapGestureRecognizer domainUsersTap = new TapGestureRecognizer();
            domainUsersTap.Tapped += CreateDomainPeople;
            domainUserDirectory.GestureRecognizers.Add(domainUsersTap);
            TapGestureRecognizer settingsTap = new TapGestureRecognizer();
            settingsTap.Tapped += SettingsMenu;
            TapGestureRecognizer homeTap = new TapGestureRecognizer();
            homeTap.Tapped += HomeMenu;
            TapGestureRecognizer eventsTap = new TapGestureRecognizer();
            TapGestureRecognizer feedbackTapped = new TapGestureRecognizer();
            TapGestureRecognizer aboutTap = new TapGestureRecognizer();
            aboutTap.Tapped += DomainCurrentDomainDetail;
            eventsTap.Tapped += BackToEvent;
            feedbackTapped.Tapped += (s, e) => { CreateFeedbackPage(this, null); };
            settingsButton.GestureRecognizers.Add(settingsTap);
            homeButton.GestureRecognizers.Add(homeTap);
            feedbackButton.GestureRecognizers.Add(feedbackTapped);
            backToScreenButton.GestureRecognizers.Add(eventsTap);
            aboutDomainSideButton.GestureRecognizers.Add(aboutTap);
            sideMenuBottom.SetButtonDetails(CreatePeoplesScreen, CreateNotesScreen, CreateBookmarksScreen);
        }

        public async void CreateEventList()
        {
            for (int i = 0; i < App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count; i++)
            {
                EventItem e = new EventItem();
                e.setEventITem(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[i], SetEvent, i);
                eventMenu.Add(e);
                eventList.Children.Add(e);
            }
            await SetCurrentEvent();
        }

        public void ResetAllEvents()
        {
            for (int i = 0; i < eventMenu.Count; i++)
            {
                eventMenu[i].resetButton();
            }
        }

        public async void SetDomain(object sender, EventArgs e)
        {
            App.firmID = ((RegisteredDomainTemplateView)((RegisteredDomainTemplate)sender).View).currentDomain.firmID;
            App.serverData.mei_user.currentDomainIndex = App.serverData.mei_user.registeredDomainList.IndexOf(App.serverData.mei_user.registeredDomainList.Find(x => x.firmID == App.firmID));
            App.serverData.mei_user.currentEventIndex = 0;
            App.AppCart = new AppCartList();
            await SetLoading(true, "Getting Domain data...");
            for (int i = 0; i < registeredDomain.Count; i++)
            {
                if (App.serverData.mei_user.currentDomainIndex == i)
                    registeredDomain[i].BGColor = Color.FromHex("#ebeff2");
                else
                    registeredDomain[i].BGColor = Color.White;
            }
            //this.IsGestureEnabled = false;
            CreateEvents();
        }

        public async void SetEvent(object sender, EventArgs e)
        {
            //await Task.Delay(200);
            //if (eventCarousel.CurrentPage == eventPage)
            //{                 
            await SetLoading(true, "Switching event...");
            bool create = false;
            if (App.serverData.mei_user.currentEventIndex != ((EventItem)sender).eventIndex)
            {
                create = true;
                ResetButtons();
                for (int i = 0; i < sideMenuSection.Children.Count; i++)
                {
                    ((MenuSideItem)sideMenuSection.Children[i]).DisableButton();
                    ((MenuSideItem)sideMenuSection.Children[i]).Opacity = 0.5;
                }
                ResetAllEvents();
            }
            App.serverData.mei_user.currentEventIndex = ((EventItem)sender).eventIndex;
            eventMenu[App.serverData.mei_user.currentEventIndex].SetLoading(true);
            eventMenu[App.serverData.mei_user.currentEventIndex].setButton();
            await eventMenu[App.serverData.mei_user.currentEventIndex].CheckLock(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
            if (((EventItem)sender).isLocked)
            {
                domainsListPage.IsVisible = true;
                ////indicator.IsVisible = true;
                mainPage.RaiseChild(domainsListPage);
                if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventType != "Subscription")
                {
                    if (((EventItem)sender).requested)
                    {
                        var k = await DisplayAlert("Private Event", "Your request is in process.", "Cancel Permission", "Ok");
                        if (k)
                        {
                            ((EventItem)sender).requested = !await App.serverData.CancelRequestForEvent(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventID);
                        }
                    }
                    else
                    {
                        var k = await DisplayAlert("Private Event", "This is a private event. Do you want to request admin for Permission?", "Request Permission", "Cancel");
                        if (k)
                        {
                            ((EventItem)sender).requested = await App.serverData.RequestForEvent(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventID);
                        }
                    }
                }
                else
                {
                    var k = await DisplayAlert("Private Event", "This is subscription event. Do you want to subscribe?", "Yes", "No");
                    if (k)
                    {
                        CreateEventSubscriptionPurchase(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event);
                    }
                }
                eventMenu[App.serverData.mei_user.currentEventIndex].SetLoading(false);
                await SetLoading(false, "");
            }
            else
            {
                if (create)
                {
                    await SetCurrentEvent();
                    CreateEventHomePage();
                    //SetSideMenuItem((MenuSideItem)sideMenuSection.Children[0]);
                    ////indicator.IsVisible = false;
                    domainsListPage.IsVisible = false;
                }
                else
                {
                    CreateEventHomePage();
                    domainsListPage.IsVisible = false;
                    eventMenu[App.serverData.mei_user.currentEventIndex].SetLoading(false);
                    await SetLoading(false, "Switching event...");
                }

            }

            //AnimatePanel();
            ////this.IsPresented = false;
            //}
        }

        public void OpenEducation(object s,EventArgs e)
        {
            Education education = new Education();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(education, "How to",false);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
        }

        public void SearchDomains(object s, EventArgs e)
        {
            seeingForNewDomains = true;
            DomainList domainDetail = new DomainList();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(domainDetail, "Domain Search");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
        }

        public ExhibitorGroup GetCurrentExhibitor(string id)
        {
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].exhibitors.Find(x => x.exhibitor.exhibitorID == id);
        }

        public SponsorGroup GetCurrentSponsor(string id)
        {
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sponsors.Find(x => x.sponsor.sponsorID == id);
        }

        public ServerSession GetCurrentSession(string id)
        {
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sessionList.Find(x => x.sessionID == id);
        }

        public ServerSpeaker GetCurrentSpeaker(string id)
        {
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].speakers.Find(x => x.speakerID == id);
        }

        public DomainEvent GetCurrentEvent(string id)
        {
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Find(x => x.s_event.eventID == id);
        }

        public async Task<bool> SetCurrentEvent()
        {
            ResetAllEvents();
            eventMenu[App.serverData.mei_user.currentEventIndex].setButton();
            await SetProgressBar(.8);
            await SetLoading(true, "Getting current event data...");
            bool createEvent = true;
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventType == "Private")
            {
                var isRegistered = await App.serverData.IsRegisteredForEvent(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventID);
                if (!isRegistered)
                {
                    createEvent = false;
                }
            }
            if (createEvent)
            {
                for (int i = 0; i < sideMenuSection.Children.Count; i++)
                {
                    ((MenuSideItem)sideMenuSection.Children[i]).EnableButton();
                    ((MenuSideItem)sideMenuSection.Children[i]).Opacity = 1;
                }
                await SetLoading(false, "");
            }
            else
            {
                //currentEvent = App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex];
                //CreateAboutScreen();
                for (int i = 0; i < sideMenuSection.Children.Count; i++)
                {
                    ((MenuSideItem)sideMenuSection.Children[i]).DisableButton();
                    ((MenuSideItem)sideMenuSection.Children[i]).Opacity = 0.5;
                }
                domainsListPage.IsVisible = true;
                ////indicator.IsVisible = true;
                mainPage.RaiseChild(domainsListPage);
                await SetLoading(false, "");
            }
            eventMenu[App.serverData.mei_user.currentEventIndex].SetLoading(false);
            App.serverData.SaveUserDataToLocal();
            //var k = await ResetPeopleList();
            //eventDetailBar.SetEventDetails(currentEvent);
            ResetCurrentScreen();
            return true;
        }

        public void ResetCurrentScreen()
        {
            cartItemCount.Text = App.AppCart.Count.ToString();
            //if (eventUpdatesScreen != null)
            //{
            //    if (eventUpdatesScreen.IsVisible)
            //    {

            //    }
            //}
            //if (settingsScreen != null)
            //{
            //    if (settingsScreen.IsVisible)
            //    {

            //    }
            //}
            //if (peoplePage != null)
            //{
            //    if (peoplePage.IsVisible)
            //    {
            //        peoplePage.CreatePeople(false);
            //    }
            //}
            //if (aboutPage != null)
            //{
            //    if (aboutPage.IsVisible)
            //    {
            //        //   aboutPage.SetAbout();
            //    }
            //}
            //if (faqPage != null)
            //{
            //    if (faqPage.IsVisible)
            //    {
            //        faqPage.CreateFAQ();
            //    }
            //}
            //if (floorPage != null)
            //{
            //    if (floorPage.IsVisible)
            //    {
            //        floorPage.CreateMapScreen(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventFloorMap);
            //    }
            //}
            //if (venuMap != null)
            //{
            //    if (venuMap.IsVisible)
            //    {
            //        venuMap.GenerateMap();
            //    }
            //}
            //if (sponsorsPage != null)
            //{
            //    if (sponsorsPage.IsVisible)
            //    {
            //        sponsorsPage.CreateSponsors(false);
            //    }
            //}
            //if (speakerPage != null)
            //{
            //    if (speakerPage.IsVisible)
            //    {
            //        speakerPage.CreateSpeakers(false);
            //    }
            //}
            //if (exhibitorsPage != null)
            //{
            //    if (exhibitorsPage.IsVisible)
            //    {
            //        exhibitorsPage.CreateExhibitors(false);
            //    }
            //}
            //if (schedulePage != null)
            //{
            //    if (schedulePage.IsVisible)
            //    {
            //        schedulePage.currentDay = 0;
            //        schedulePage.CreateSchedule(false);
            //    }
            //}

            if (notesPage != null)
            {
                if (notesPage.IsVisible)
                {
                    notesPage.CreateNotes();
                }
            }
            if (bookmarksPage != null)
            {
                if (bookmarksPage.IsVisible)
                {
                    bookmarksPage.SetBookmarksCount();
                }
            }
            CheckIndicator();
        }

        public Command LoadDomainCommand
        {
            get
            {
                return loadDomainCommand ?? (loadDomainCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            //if (contactsParent.IsRefreshing)
            //  return;
            registeredDomainList.IsRefreshing = true;
            await App.serverData.GetRegisteredDomain();
            CheckDomainList();
            registeredDomainList.IsRefreshing = false;
        }

        public void CheckDomainList()
        {
            if (App.serverData.mei_user.registeredDomainList.Count > 0)
            {
                IsGestureEnabled = true;
                emptyList.IsVisible = false;
                registeredDomainList.IsVisible = true;
                //registeredDomainList.SelectedItem = App.serverData.mei_user.currentDomainIndex;
            }
            else
            {
                emptyList.IsVisible = true;
                registeredDomainList.IsVisible = false;
                //IsGestureEnabled = false;
            }
        }

        public string GetSort(ServerDomain s)
        {
            if (s != null)
            {
                return s.domainName;
            }
            return string.Empty;
        }

        public void ResetRegisteredDomainList()
        {
            registeredDomain.Clear();
            List<DomainGroup> modified = new List<DomainGroup>();
            for (int i = 0; i < App.serverData.mei_user.registeredDomainList.Count; i++)
            {
                if (App.serverData.allDomainEvents.Find(x => x.domain.firmID == App.serverData.mei_user.registeredDomainList[i].firmID) != null)
                    modified.Add(App.serverData.allDomainEvents.Find(x => x.domain.firmID == App.serverData.mei_user.registeredDomainList[i].firmID));
            }
            App.serverData.allDomainEvents = modified;
            if (App.serverData.mei_user.currentDomainIndex > App.serverData.mei_user.registeredDomainList.Count - 1)
                App.serverData.mei_user.currentDomainIndex = 0;

            if (App.serverData.allDomainEvents.Count > 0)
            {
                if (App.serverData.mei_user.currentDomainIndex > App.serverData.allDomainEvents.Count - 1)
                    App.serverData.mei_user.currentDomainIndex = 0;
                if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count > 0)
                {
                    if (App.serverData.mei_user.currentEventIndex > App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count - 1)
                        App.serverData.mei_user.currentEventIndex = 0;
                }
                else
                {
                    App.serverData.mei_user.currentEventIndex = 0;
                }
            }
            for (int i = 0; i < App.serverData.mei_user.registeredDomainList.Count; i++)
            {
                RegisteredDomainViewModel model = new RegisteredDomainViewModel();
                model.domain = App.serverData.mei_user.registeredDomainList[i];
                if (App.serverData.mei_user.currentDomainIndex == i)
                    model.BGColor = Color.FromHex("#ebeff2");
                else
                    model.BGColor = Color.White;
                registeredDomain.Add(model);
                if (App.serverData.allDomainEvents.Find(x => x.domain.firmID == App.serverData.mei_user.registeredDomainList[i].firmID) == null)
                {
                    DomainGroup dGroup = new DomainGroup();
                    dGroup.domain = App.serverData.mei_user.registeredDomainList[i];
                    dGroup.domainEvents = new List<DomainEvent>();
                    App.serverData.allDomainEvents.Add(dGroup);
                    App.serverData.SaveDomainDBToLocal();
                }
            }
            registeredDomainList.ItemsSource = registeredDomain;
        }

        public void SetRegisteredDomainList()
        {

            registeredDomain.Clear();
            List<DomainGroup> modified = new List<DomainGroup>();
            for (int i = 0; i < App.serverData.mei_user.registeredDomainList.Count; i++)
            {
                if (App.serverData.allDomainEvents.Find(x => x.domain.firmID == App.serverData.mei_user.registeredDomainList[i].firmID) != null)
                    modified.Add(App.serverData.allDomainEvents.Find(x => x.domain.firmID == App.serverData.mei_user.registeredDomainList[i].firmID));
            }
            App.serverData.allDomainEvents = modified;
            if (App.serverData.mei_user.currentDomainIndex > App.serverData.mei_user.registeredDomainList.Count - 1)
                App.serverData.mei_user.currentDomainIndex = 0;

            if (App.serverData.allDomainEvents.Count > 0)
            {
                if (App.serverData.mei_user.currentDomainIndex > App.serverData.allDomainEvents.Count - 1)
                    App.serverData.mei_user.currentDomainIndex = 0;
                if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count > 0)
                {
                    if (App.serverData.mei_user.currentEventIndex > App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents.Count - 1)
                        App.serverData.mei_user.currentEventIndex = 0;
                }
                else
                {
                    App.serverData.mei_user.currentEventIndex = 0;
                }
            }
            for (int i = 0; i < App.serverData.mei_user.registeredDomainList.Count; i++)
            {
                RegisteredDomainViewModel model = new RegisteredDomainViewModel();
                model.domain = App.serverData.mei_user.registeredDomainList[i];
                if (App.serverData.mei_user.currentDomainIndex == i)
                    model.BGColor = Color.FromHex("#ebeff2");
                else
                    model.BGColor = Color.White;
                registeredDomain.Add(model);
                if (App.serverData.allDomainEvents.Find(x => x.domain.firmID == App.serverData.mei_user.registeredDomainList[i].firmID) == null)
                {
                    DomainGroup dGroup = new DomainGroup();
                    dGroup.domain = App.serverData.mei_user.registeredDomainList[i];
                    dGroup.domainEvents = new List<DomainEvent>();
                    App.serverData.allDomainEvents.Add(dGroup);
                    App.serverData.SaveDomainDBToLocal();
                }
            }
            registeredDomainList.ItemsSource = registeredDomain;
            CheckDomainList();
            CreateEvents();
            App.serverData.SaveUserDataToLocal();
        }

        public HomeLayout()
        {
            InitializeComponent();
            TapGestureRecognizer indicTap = new TapGestureRecognizer();
            indicTap.Tapped += (s, e) => { this.IsPresented = true; };
            TapGestureRecognizer resync = new TapGestureRecognizer();
            resync.Tapped += (s, e) => { ReSyncDomain(); };
            resyncDomains.GestureRecognizers.Add(resync);
            //indicator.GestureRecognizers.Add(indicTap);
            sideEventMenuSection.IsVisible = false;
            registeredDomainList.ItemTemplate = new DataTemplate(typeof(RegisteredDomainTemplate));
            registeredDomainList.RowHeight = 80;
            SetRegisteredDomainList();
            CheckDomainList();
            CreateMenu();
            App.checkForUpdates = true;
            ParentLayout.MinimumHeightRequest = this.Detail.Height;
            App.inHome = true;
            registeredDomainList.RefreshCommand = LoadDomainCommand;
            SetTapButtons();
        }

        public async Task<List<ServerEvent>> SyncEventsList(string firmID)
        {
            List<ServerEvent> events = new List<ServerEvent>();
            try
            {
                events = await EventCreation.GetDomainEvents(firmID);
                events.Sort((a, b) => BaseFunctions.GetDateTime(b.eventStartDate).CompareTo(BaseFunctions.GetDateTime(a.eventStartDate)));

            }
            catch
            {

            }
            return events;
        }

        public void OnSideMenuItemClicked(object sender, EventArgs e)
        {
            ResetButtons();
            this.IsPresented = false;
            SetSideMenuItem((MenuSideItem)sender);
        }

        public void SetSideMenuItem(int id)
        {
            ResetButtons();
            MenuSideItem item = (MenuSideItem)sideMenuSection.Children[id];
            SetSideMenuItem(item);
        }

        public void SetSideMenuItem(MenuSideItem item)
        {
            screenName.Text = tabMenuItems[item.ItemID];
            item.SetButton();
            CreateScreen(item.ItemID);
        }

        public void SetScreen(Type objectType)
        {
            indicator.IsVisible = true;
            if (eventHomePage != null)
            {
                eventHomePage.IsVisible = isSameType(typeof(EventHomePage), objectType);
            }
            if (eventUpdatesScreen != null)
            {
                eventUpdatesScreen.IsVisible = isSameType(typeof(EventUpdates), objectType);
            }
            if (catalogPage != null)
            {
                catalogPage.IsVisible = isSameType(typeof(CatalogList), objectType);
            }
            if (settingsScreen != null)
            {
                indicator.IsVisible = false;
                settingsScreen.IsVisible = isSameType(typeof(SettingsPage), objectType);
            }
            if (peoplePage != null)
            {
                peoplePage.IsVisible = isSameType(typeof(PeoplePage), objectType);
            }
            if (aboutPage != null)
            {
                aboutPage.IsVisible = isSameType(typeof(AboutPage), objectType);
            }
            if (faqPage != null)
            {
                faqPage.IsVisible = isSameType(typeof(FAQPage), objectType);
            }
            if (pageUnderCon != null)
            {
                pageUnderCon.IsVisible = isSameType(typeof(UnderConstructionPage), objectType);
            }
            if (venuMap != null)
            {
                venuMap.IsVisible = isSameType(typeof(EventMap), objectType);
            }
            if (bookmarksPage != null)
            {
                bookmarksPage.IsVisible = isSameType(typeof(BookmarksPage), objectType);
            }
            if (sponsorsPage != null)
            {
                sponsorsPage.IsVisible = isSameType(typeof(SponsorsPage), objectType);
            }
            if (speakerPage != null)
            {
                speakerPage.IsVisible = isSameType(typeof(SpeakerPage), objectType);
            }
            if (exhibitorsPage != null)
            {
                exhibitorsPage.IsVisible = isSameType(typeof(ExhibitorsPage), objectType);
            }
            if (notesPage != null)
            {
                notesPage.IsVisible = isSameType(typeof(NotesPage), objectType);
            }
            if (schedulePage != null)
            {
                schedulePage.IsVisible = isSameType(typeof(SchedulePage), objectType);
            }
            if (floorPage != null)
            {
                floorPage.IsVisible = isSameType(typeof(FloorMapPage), objectType);
            }
            ClearOtherScreens();
        }

        public void SetSecondaryScreen(Type objectType)
        {
            if (peopleDetail != null)
                peopleDetail.IsVisible = isSameType(typeof(PeopleDetailsTemplate), objectType);
            if (editProfile != null)
                editProfile.IsVisible = isSameType(typeof(EditProfile), objectType);
            if (changePassword != null)
                changePassword.IsVisible = isSameType(typeof(ChangePassword), objectType);
        }

        bool isSameType(Type obj1, Type obj2)
        {
            if (obj1 == obj2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async void CreateScreen(int id)
        {
            feedbackButton.IsVisible = true;
            homeButton.IsVisible = true;
            SetScreen(typeof(int));
            switch (id)
            {

                case 0:
                    {
                        await SetLoading(true, "Loading event about...");
                        CreateAboutScreen();
                    }
                    break;
                case 1:
                    {
                        CreateEventsUpdateScreen();
                    }
                    break;
                case 2:
                    {
                        await SetLoading(true, "Loading event speakers...");
                        CreateSpeakersScreen();
                    }
                    break;
                case 3:
                    {
                        await SetLoading(true, "Loading event attendees...");
                        CreatePeoplesScreen(this, null);
                    }
                    break;
                case 4:
                    {
                        await SetLoading(true, "Loading event sessions...");
                        CreateScheduleScreen();
                    }
                    break;
                case 5:
                    {
                        await SetLoading(true, "Loading event exibitors...");
                        CreateExhibitorsScreen();
                    }
                    break;
                case 6:
                    {
                        await SetLoading(true, "Loading event sponsors...");
                        CreateSponsorsScreen();
                    }
                    break;
                case 7:
                    {
                        await SetLoading(true, "Loading event catalog...");
                        CreateCatalogScreen();
                    }
                    break;
                case 8:
                    {
                        await SetLoading(true, "Loading event floormaps...");
                        CreateFloorMapScreen();
                    }
                    break;
                case 9:
                    {
                        await SetLoading(true, "Loading event venue maps...");
                        CreateVenueMapScreen();
                    }
                    break;
                case 10:
                    {
                        await SetLoading(true, "Loading event FAQs...");
                        CreateEventFAQScreen();
                    }
                    break;
            }
            mainPage.RaiseChild(ParentLayout);
            domainsListPage.IsVisible = false;
            ////indicator.IsVisible = false;
        }

        public void CreateEventHomePage()
        {
            //this.IsPresented = false;
            screenName.Text = GetCurrentDomainEvent().s_event.eventName;
            homeButton.IsVisible = true;
            feedbackButton.IsVisible = true;
            if (eventHomePage == null)
            {
                eventHomePage = new EventHomePage();
                ParentLayout.Children.Add(eventHomePage);
            }
            IsPresented = false;
            ResetButtons();
            SetCurrentScreen(eventHomePage, ParentLayout);
        }

        public async void CreateEventsUpdateScreen()
        {
            //this.IsPresented = false;
            if (eventUpdatesScreen == null)
            {
                eventUpdatesScreen = new EventUpdates();
                ParentLayout.Children.Add(eventUpdatesScreen);
            }
            SetCurrentScreen(eventUpdatesScreen, ParentLayout);
            await SetLoading(true, "Syncing event posts...");
            eventUpdatesScreen.PullToRefresh();
        }

        public void CreateSpeakersScreen()
        {
            //var k = await  App.serverData.GetSpeakerData();
            //currentEvent.speakers = await EventCreation.GetEventSpeakers(currentEvent.eventID);
            //this.IsPresented = false;
            if (speakerPage == null)
            {
                speakerPage = new SpeakerPage();
                ParentLayout.Children.Add(speakerPage);
            }
            speakerPage.CreateSpeakers(false);
            SetCurrentScreen(speakerPage, ParentLayout);
        }

        public void CreateScheduleScreen()
        {
            //var k = await App.serverData.GetSessionData();
            //currentEvent.sessionList = await EventCreation.GetEventSessions(currentEvent.eventID);
            //this.IsPresented = false;            
            if (schedulePage == null)
            {
                schedulePage = new SchedulePage();
                ParentLayout.Children.Add(schedulePage);
            }
            schedulePage.CreateSchedule(false);
            isDuration = true;
            SetCurrentScreen(schedulePage, ParentLayout);
        }

        public void CreateCatalogScreen()
        {
            //var k = await App.serverData.GetExhibitorData();
            //currentEvent.exhibitors = await EventCreation.GetEventExhibitors(currentEvent.eventID);
            //this.IsPresented = false;  
            if (catalogPage == null)
            {
                catalogPage = new CatalogList();
                ParentLayout.Children.Add(catalogPage);
            }
            catalogPage.PullToRefresh();

            SetCurrentScreen(catalogPage, ParentLayout);

        }

        public void CreateExhibitorsScreen()
        {
            //var k = await App.serverData.GetExhibitorData();
            //currentEvent.exhibitors = await EventCreation.GetEventExhibitors(currentEvent.eventID);
            //this.IsPresented = false;  
            if (exhibitorsPage == null)
            {
                exhibitorsPage = new ExhibitorsPage();
                ParentLayout.Children.Add(exhibitorsPage);
            }
            exhibitorsPage.CreateExhibitors(false);

            SetCurrentScreen(exhibitorsPage, ParentLayout);

        }

        public void CreateFloorMapScreen()
        {
            //this.IsPresented = false;
            if (floorPage == null)
            {
                floorPage = new FloorMapPage();
                ParentLayout.Children.Add(floorPage);
            }
            floorPage.CreateMapScreen(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventFloorMap);

            SetCurrentScreen(floorPage, ParentLayout);
        }

        public void CreateSponsorsScreen()
        {
            //var k = await App.serverData.GetSponsorData();
            //currentEvent.sponsors = await EventCreation.GetEventSponsor(currentEvent.eventID);
            //this.IsPresented = false;
            if (sponsorsPage == null)
            {
                sponsorsPage = new SponsorsPage();
                ParentLayout.Children.Add(sponsorsPage);
            }
            sponsorsPage.CreateSponsors(false);
            SetCurrentScreen(sponsorsPage, ParentLayout);
        }

        public void CreateVenueMapScreen()
        {
            //this.IsPresented = false;
            if (venuMap == null)
            {
                venuMap = new EventMap();
                parentNoScrollLayout.Children.Add(venuMap);

            }
            venuMap.GenerateMap();
            SetCurrentScreen(venuMap, parentNoScrollLayout);
        }

        public void CreateComingSoon(string pageName)
        {
            //this.IsPresented = false;
            if (pageUnderCon == null)
            {
                pageUnderCon = new UnderConstructionPage();
                pageUnderCon.SetDescription(pageName);
                parentNoScrollLayout.Children.Add(pageUnderCon);
            }
            pageUnderCon.SetDescription(pageName);
            SetCurrentScreen(pageUnderCon, parentNoScrollLayout);
        }

        public void CreateEventFAQScreen()
        {
            //this.IsPresented = false;
            if (faqPage == null)
            {
                faqPage = new FAQPage();
                ParentLayout.Children.Add(faqPage);
            }
            faqPage.CreateFAQ();
            SetCurrentScreen(faqPage, ParentLayout);
        }

        public async void ReSyncDomain()
        {

            //if (App.serverData.mei_user.registeredDomainList.Count > 0)
            //{
            var k = await DisplayAlert("Confirmation", "Are you sure to resync your domains", "Yes", "No");
            if (k)
            {
                await SetLoading(true, "Resyncing domains...");
                var register = await App.serverData.GetRegisteredDomain();
                var request = await App.serverData.GetRequestedDomains();
                SetRegisteredDomainList();
            }
            //}
            //else
            //{
            //    await DisplayAlert("Confirmation", "You are not following any domains to sync", "Ok");
            //}
        }

        public void CreateAboutScreen()
        {
            //this.IsPresented = false;
            if (aboutPage == null)
            {
                aboutPage = new AboutPage();
                ParentLayout.Children.Add(aboutPage);
            }
            aboutPage.SetAbout();
            SetCurrentScreen(aboutPage, ParentLayout);
        }

        public async Task<List<ServerEventPost>> GetCurrentEventPosts()
        {
            App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventPosts(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
            App.serverData.SaveDomainDBToLocal();
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].eventPostList;
        }

        public async Task<List<ServerSpeaker>> GetCurrentEventSpeakers(bool fromDatabase)
        {
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].speakers.Count == 0 || fromDatabase)
            {
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventSpeakers(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
                App.serverData.SaveDomainDBToLocal();
            }
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].speakers;
        }

        public async Task<List<ServerEventPost>> GetCurrentDomainUserPosts(int id)
        {
            App.serverData.allDomainEvents[id] = await EventCreation.SyncDomainPosts(App.serverData.allDomainEvents[id]);
            App.serverData.SaveDomainDBToLocal();

            return App.serverData.allDomainEvents[id].userPosts;
        }

        public async Task<List<ServerSession>> GetCurrentEventSessions(bool fromDatabase)
        {
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sessionList.Count == 0 || fromDatabase)
            {
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventSessions(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
                App.serverData.SaveDomainDBToLocal();
            }
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sessionList;
        }

        public async Task<DomainEvent> GetCurrentEventAfterSyncingSessions(bool fromDatabase)
        {
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sessionList.Count == 0 || fromDatabase)
            {
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventSessions(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
                App.serverData.SaveDomainDBToLocal();
            }
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex];
        }

        public DomainEvent GetCurrentDomainEvent()
        {
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex];
        }

        public async Task<DomainEvent> GetCurrentDomainEventFromServer()
        {
            App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event = await EventCreation.GetDomainEvent(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].s_event.eventID);
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex];
        }

        public async Task<List<ExhibitorGroup>> GetCurrentEventExhibitors(bool fromDatabase)
        {
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].exhibitors.Count == 0 || fromDatabase)
            {
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventExhibitors(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
                App.serverData.SaveDomainDBToLocal();
            }
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].exhibitors;
        }

        public async Task<List<SponsorGroup>> GetCurrentEventSponsors(bool fromDatabase)
        {
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sponsors.Count == 0 || fromDatabase)
            {
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventSponsors(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
                App.serverData.SaveDomainDBToLocal();
            }
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].sponsors;
        }

        public async Task<List<ServerUser>> GetCurrentEventAttendees(bool fromDatabase)
        {
            App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex] = await EventCreation.SyncDomainUsers(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex]);
            App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventUsers(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex]);
            App.serverData.SaveDomainDBToLocal();
            return await GetEventUsersFromFirmUsers(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].users, App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].users);
        }

        public async Task<List<ServerUser>> GetCurrentDomainUsers(bool fromDatabase)
        {
            App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex] = await EventCreation.SyncDomainUsers(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex]);
            App.serverData.SaveDomainDBToLocal();
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].users;
        }

        public async Task<List<ServerUser>> GetEventUsersFromFirmUsers(List<ServerUser> domainUsers, List<string> eventUsers)
        {
            List<ServerUser> returnList = new List<ServerUser>();
            foreach (string id in eventUsers)
            {
                ServerUser user = domainUsers.Find(x => x.userID == id);
                if (user == null)
                    user = await App.serverData.GetUserWithID(id);
                returnList.Add(user);
            }
            returnList.RemoveAll(x => x == null);
            return returnList;
        }

        public async Task<List<ServerCatalogGroup>> GetCurrentEventCatalog(bool fromDatabase)
        {
            if (App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].catalogList.Count == 0 || fromDatabase)
            {
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].inventoryList = await App.serverData.GetEventInventoryData(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domain.firmID);
                App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex] = await EventCreation.SyncEventCatalog(App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex], App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].inventoryList);
                App.serverData.SaveDomainDBToLocal();
            }
            return App.serverData.allDomainEvents[App.serverData.mei_user.currentDomainIndex].domainEvents[App.serverData.mei_user.currentEventIndex].catalogList;
        }

        public async void CreatePeoplesScreen(object s, EventArgs e)
        {
            screenName.Text = "Attendees";
            await SetLoading(true, "Getting people information...");
            //this.IsPresented = false;
            if (peoplePage == null)
            {
                peoplePage = new PeoplePage();
                ParentLayout.Children.Add(peoplePage);
                peoplePage.CreatePeople(false);
            }
            else
            {
                peoplePage.CreatePeople(false);
            }
            SetCurrentScreen(peoplePage, ParentLayout);
        }

        public void CreateNotesScreen(object s, EventArgs e)
        {
            // SetLoading(true, "Loading notes...");
            ResetButtons();
            feedbackButton.IsVisible = false;
            homeButton.IsVisible = false;
            screenName.Text = "Notes";
            this.IsPresented = false;
            if (notesPage == null)
            {
                notesPage = new NotesPage();
                ParentLayout.Children.Add(notesPage);
            }
            else
            {
                notesPage.CreateNotes();
            }
            mainPage.RaiseChild(PrimaryScreen);
            ////indicator.IsVisible = false;
            SetCurrentScreen(notesPage, ParentLayout);

        }

        public async void CreateBookmarksScreen(object s, EventArgs e)
        {
            await SetLoading(true, "Loading bookmarks...");
            feedbackButton.IsVisible = false;
            homeButton.IsVisible = false;
            ResetButtons();
            screenName.Text = "Bookmarks";
            this.IsPresented = false;
            if (bookmarksPage == null)
            {
                bookmarksPage = new BookmarksPage();
                ParentLayout.Children.Add(bookmarksPage);
                bookmarksPage.SetBookmarksCount();
            }
            else
            {
                bookmarksPage.SetBookmarksCount();
            }
            mainPage.RaiseChild(PrimaryScreen);
            ////indicator.IsVisible = false;
            SetCurrentScreen(bookmarksPage, ParentLayout);

        }



        public void CreatePaymentInformation(int index)
        {
            paymentInformation = new PaymentInformation();
            paymentInformation.SetPaymentInformation(index);
            OverlayScreen os = new OverlayScreen();
            paymentInformation.closePage = os.OnBack;
            os.SetScreen(paymentInformation, "Payment Information");
            os.TitleRightBar("mei_saveicon_w.png", SavePaymentInformation);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
            /////indicator.IsVisible = false;
        }

        public void CreateShippingInformation(int index)
        {
            shippingInformation = new ShippingInformation();
            shippingInformation.SetPaymentInformation(index);
            OverlayScreen os = new OverlayScreen();
            shippingInformation.closePage = os.OnBack;
            os.SetScreen(shippingInformation, "Shipping Information");
            os.TitleRightBar("mei_saveicon_w.png", SaveShippingInformation);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreatePaymentList(object sender, EventArgs e)
        {
            paymentList = new PaymentList();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(paymentList, "Payment List");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateShippingList(object sender, EventArgs e)
        {
            shippingList = new ShippingList();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(shippingList, "Shipping List");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
            ////indicator.IsVisible = false;
        }

        public async void SavePaymentInformation(object sender, EventArgs e)
        {
            await SetLoading(true, "Saving payment information..");
            bool r = await paymentInformation.SaveUserToBrainTree();
            if (r)
                paymentInformation.closePage?.Invoke(this, null);
            await SetLoading(false, "");
        }

        public async void SaveShippingInformation(object sender, EventArgs e)
        {
            await SetLoading(true, "Saving shipping information..");
            bool r = await shippingInformation.SaveShippingAddress();
            if (r)
                shippingInformation.closePage?.Invoke(this, null);
            await SetLoading(false, "");
        }

        public void CreateEditProfile(object s, EventArgs e)
        {

            //this.IsPresented = false;
            editProfile = new EditProfile();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(editProfile, "Edit Profile");
            os.TitleRightBar("mei_saveicon_w.png", SaveProfile);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
            ////indicator.IsVisible = false;

        }

        public async void SaveProfile(object sender, EventArgs e)
        {
            await SetLoading(true, "Saving user profile...");
            bool r = await editProfile.SaveUserToDB();
            if (r == true)
            {
                sideMenuBottom.SetUser(App.serverData.mei_user.currentUser);
                mainPage.Children.Remove((ContentView)sender);
                ClearOtherScreens();
            }
            await SetLoading(false, "");
        }


        public void CreateChangePassword(object s, EventArgs e)
        {
            //this.IsPresented = false;
            changePassword = new ChangePassword();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(changePassword, "Change Password");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateSettingScreen()
        {
            indicator.IsVisible = false;
            ResetButtons();
            feedbackButton.IsVisible = false;
            homeButton.IsVisible = false;
            screenName.Text = "Settings";
            this.IsPresented = false;
            if (settingsScreen == null)
            {
                settingsScreen = new SettingsPage();
                ParentLayout.Children.Add(settingsScreen);
            }

            mainPage.RaiseChild(PrimaryScreen);
            ////indicator.IsVisible = false;
            SetCurrentScreen(settingsScreen, ParentLayout);
        }

        public void CreateSpeakerDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            SpeakerTemplate sTemplate = (SpeakerTemplate)s;
            SpeakerDetailsPage speakerDetail = new SpeakerDetailsPage();
            speakerDetail.SetSpeakerDetail(((SpeakerTemplateView)sTemplate.View).currentSpeaker, sTemplate);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(speakerDetail, "Speaker Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public ListView GetDomainListPage()
        {
            return registeredDomainList;
        }

        public void DomainCurrentDomainDetail(object s, EventArgs e)
        {
            this.IsPresented = false;
            DomainDetailPage domainDetail = new DomainDetailPage();
            domainDetail.SetDomainDetails(currentDomain);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(domainDetail, currentDomain.domainName);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateDomainDetail(ServerDomain _domain)
        {

            DomainDetailPage domainDetail = new DomainDetailPage();
            domainDetail.SetDomainDetails(_domain);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(domainDetail, _domain.domainName);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }
        public ServerDomain notificationDomain;


        public async void CreateDomainPosts(int index,ServerDomain cDomain)
        {

            DomainNotifications domainDetail = new DomainNotifications();
            notificationDomain = cDomain;
            await domainDetail.CreateUpdates(index);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(domainDetail, "Domain Notifications");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }


        public void CreateSessionDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            ScheduleItem sessionTemplate = (ScheduleItem)s;
            SessionDetailPage sessionDetail = new SessionDetailPage();
            sessionDetail.SessionDetails(((ScheduleItemView)sessionTemplate.View).currentSession, sessionTemplate);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(sessionDetail, "Session Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
        }

        public void CreateSponsorDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            isDuration = false;
            SponsorsTemplate sponsorTemplate = (SponsorsTemplate)s;
            SponsorsDetailPage sponsorDetail = new SponsorsDetailPage();
            sponsorDetail.SponsorDetials(((SponsorsTemplateView)sponsorTemplate.View).currentSponsor, sponsorTemplate);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(sponsorDetail, "Sponsor Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateCatalogDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            CatalogTemplate template = (CatalogTemplate)s;
            CatalogDetailPage detail = new CatalogDetailPage();
            detail.CatalogDetails(template.GetCurrentCatalogItem());
            OverlayScreen os = new OverlayScreen();
            detail.closePage = os.OnBack; os.SetScreen(detail, "Item Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateCartItemDetail(object s,EventArgs e)
        {
            CartItemTemplate template = s as CartItemTemplate;
            CatalogDetailPage detail = new CatalogDetailPage();
            detail.CatalogDetails(template.GetCurrentCatalogItem());
            OverlayScreen os = new OverlayScreen();
            detail.closePage = os.OnBack; os.SetScreen(detail, "Item Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
        }

        public void CreatePurchaseHistory(object s, EventArgs e)
        {
            purchaseHistory = new PurchaseHistory();
            OverlayScreen os = new OverlayScreen();
            purchaseHistory.CreatePurchaseList();
            os.SetScreen(purchaseHistory, "Order History");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
        }

        public void CreateDomainPeople(object s, EventArgs e)
        {
            DomainUserDirectory domainUserDirectory = new DomainUserDirectory();
            OverlayScreen os = new OverlayScreen();
            domainUserDirectory.CreatePeople();
            os.SetScreen(domainUserDirectory, "Domain Directory");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return mainPage.Width; })
                    , Constraint.RelativeToParent((p) => { return mainPage.Height; }));
        }


        public void CreatePurchaseDetail(ServerTransaction transaction)
        {
            PurchaseDetailPage detail = new PurchaseDetailPage();
            detail.PurchaseDetail(transaction);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(detail, "Order Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateCartPage(object s,EventArgs e)
        {
            Cart cart = new Cart();            
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(cart, "Order Detail",false);
            cart.closePage = os.OnBack;
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
        }

        public async void AddItemToCart(object s, EventArgs e)
        {
            CatalogDetailPage page = s as CatalogDetailPage;            
            if (App.AppCart.Contains(page.currentItem))
            {
                await DisplayAlert("Alert", "Item is already added to cart.", "OK");
                return;
            }
            else
            {
                App.AppCart.AddItem(page.currentItem);
                await DisplayAlert("Alert", "Item added to cart.", "OK");
            }
        }

        string GetItemIDsForPurchase
        {
            get
            {
                string return_string = string.Empty;
                int count = 0;
                foreach (ServerCatalogGroup item in App.AppCart)
                {
                    return_string += item.iItem.itemID;
                    count++;
                    if (count != App.AppCart.Count)
                    {
                        return_string += "@";
                    }
                }
                return return_string;
            }
        }

        string GetTransacationPriceForCart
        {
            get
            {
                double total = 0;
                foreach (ServerCatalogGroup item in App.AppCart)
                {
                    total += (int.Parse(item.cItem.itemCurrentQuantity) * double.Parse(item.cItem.itemPrice));
                }
                return total.ToString();
            }
        }
        public async void CreateCatalogPurchase(object s, EventArgs e)
        {
            //this.IsPresented = false;
            if (App.serverData.mei_user.userAddressList.Count == 0)
            {
                var alert = await DisplayAlert("Alert", "Address book is empty please add atleast one shipping address to have transaction", "Go to settings", "OK");
                if (alert)
                {
                    CreateSettingScreen();
                    ClearOtherScreens();
                    return;
                }
                else
                {
                    return;
                }

            }
            if (App.serverData.mei_user.userCustomerTokenList.Count == 0)
            {
                var alert = await DisplayAlert("Alert", "You need to have atleast one card.", "Go to settings", "OK");
                if (alert)
                {
                    CreateSettingScreen();
                    ClearOtherScreens();
                    return;
                }
                else
                {
                    return;
                }

            }
            Cart template = s as Cart;
            purchasePage = new PurchasePage();
            ServerTransaction transaction = new ServerTransaction();
            transaction.userID = App.serverData.mei_user.currentUser.userID;
            transaction.firmID = currentDomain.firmID;
            transaction.itemID = GetItemIDsForPurchase;
            transaction.transactionName = App.AppCart.Names;
            transaction.transactionType = "Sale";
            transaction.transactionImage = App.AppCart[0].iItem.itemImage;
            transaction.transactionMerchantID = currentDomain.domainMerchantID;
            transaction.transactionPrices = App.AppCart.Prices;
            transaction.transactionTokenID = App.serverData.mei_user.userCustomerTokenList[0].cardToken;
            transaction.transactionPrice = GetTransacationPriceForCart;
            transaction.transactionQuantity = App.AppCart.Quantity;
            var cTime = DateTime.Now;
            transaction.transactionDate = cTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            purchasePage.SetTransaction(transaction);
            OverlayScreen os = new OverlayScreen();
            purchasePage.closePage = os.OnBack;
            template.closePage?.Invoke(this, null);
            os.SetScreen(purchasePage, "Transaction",false);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        

        public async void ProceedPurchase(ServerTransaction transaction, bool donation)
        {
            await SetLoading(true, "Order in progress.Please dont close your app or minimize...");
            string transactionID = await BaseFunctions.MerchantSale(transaction);
            if (transactionID != "")
            {
                if (!donation)
                {
                    foreach (ServerCatalogGroup item in App.AppCart)
                    {
                        //ServerCatalogItem cItem = item.cItem;
                        //ServerInventoryItem iItem = item.iItem;
                        //if (iItem.universal == "Yes")
                        //{
                        //    iItem.itemCurrentQuantity = (int.Parse(iItem.itemCurrentQuantity)).ToString();
                        //}
                        //else
                        //{
                        //    cItem.itemCurrentQuantity = (int.Parse(cItem.itemCurrentQuantity) + int.Parse(transaction.transactionQuantity)).ToString();
                        //}
                        //item.cItem = cItem;
                        //item.iItem = iItem;
                        if(item.IsUniversal)
                        {
                            if(item.iItem.itemMaxQuantity != "-1")
                            {
                                item.iItem.itemMaxQuantity = (int.Parse(item.iItem.itemMaxQuantity) - int.Parse(item.cItem.itemCurrentQuantity)).ToString();
                            }
                        }
                        else{
                            if (item.cItem.itemMaxQuantity != "-1")
                            {
                                item.cItem.itemMaxQuantity = (int.Parse(item.cItem.itemMaxQuantity) - int.Parse(item.cItem.itemCurrentQuantity)).ToString();
                            }
                        }
                        await BaseFunctions.EditCatalogItem(item);
                    }
                }
                transaction.transactionID = transactionID;
                await SetLoading(false, "Order in progress.Please wait...");
                await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Sending invoice to your email...");
                string cardType = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_cardType");
                string card4 = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_last4");
                if (transaction.transactionType == "Sale")
                {
                    bool sent = await BaseFunctions.EmailSaleInvoice(transaction, cardType + " ending with " + card4);

                }
                else if (transaction.transactionType == "Donation")
                {
                    bool sent = await BaseFunctions.EmailDonationInvoice(transaction, cardType + " ending with " + card4);
                }
                await App.Current.MainPage.DisplayAlert("Order ID : " + transactionID, "Your Order is successful", "Ok");
                ((HomeLayout)App.Current.MainPage).CreatePurchaseDetail(transaction);
                App.AppCart.Clear();
                purchasePage.closePage?.Invoke(this, null);
            }
            await SetLoading(false, "Order in progress.Please wait...");
        }

        public async void ProceedSubscriptionPurchase(ServerTransaction transaction, string subscriptionType)
        {
            await SetLoading(true, "Order in progress.Please wait...");
            string transactionID = await App.serverData.SubscribeForDomain(transaction, subscriptionType);
            if (transactionID != "")
            {
                var k = await App.serverData.GetUserSubscriptions();
                await SetLoading(false, "Order in progress.Please wait...");
                await App.Current.MainPage.DisplayAlert("Order ID : " + transactionID, "Your subscription is successful", "Ok");
                ((HomeLayout)App.Current.MainPage).CreatePurchaseDetail(transaction);
                purchasePage.closePage?.Invoke(this, null);
            }
            await SetLoading(false, "Transaction in progress.Please wait...");
        }

        public async void CreateDonationPurchase(object s, EventArgs e)
        {
            if (App.serverData.mei_user.userAddressList.Count == 0)
            {
                var alert = await DisplayAlert("Alert", "Address book is empty please add atleast one shipping address to have an order", "Go to settings", "OK");
                if (alert)
                {
                    CreateSettingScreen();
                    ClearOtherScreens();
                    return;
                }
                else
                {
                    return;
                }

            }
            if (App.serverData.mei_user.userCustomerTokenList.Count == 0)
            {
                var alert = await DisplayAlert("Alert", "You need to have atleast one card.", "Go to settings", "OK");
                if (alert)
                {
                    CreateSettingScreen();
                    ClearOtherScreens();
                    return;
                }
                else
                {
                    return;
                }

            }
            DomainDetailPage template = (DomainDetailPage)s;
            purchasePage = new PurchasePage();
            ServerTransaction transaction = new ServerTransaction();
            transaction.userID = App.serverData.mei_user.currentUser.userID;
            transaction.firmID = App.firmID;
            transaction.itemID = "";
            transaction.transactionName = template.currentDomain.domainName + " Donation";
            transaction.transactionType = "Donation";
            transaction.transactionImage = template.currentDomain.domainLogo;
            transaction.transactionMerchantID = currentDomain.domainMerchantID;
            transaction.transactionTokenID = App.serverData.mei_user.userCustomerTokenList[0].cardToken;
            transaction.transactionPrice = "0.00";
            var cTime = DateTime.Now;
            transaction.transactionDate = cTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            purchasePage.SetTransaction(transaction);
            OverlayScreen os = new OverlayScreen();
            purchasePage.closePage = os.OnBack;
            os.SetScreen(purchasePage, "Order");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }


        public void CreateDomainSubscriptionPurchase(ServerDomain cDomain)
        {
            purchasePage = new PurchasePage();
            ServerTransaction transaction = new ServerTransaction();
            transaction.userID = App.serverData.mei_user.currentUser.userID;
            transaction.firmID = cDomain.firmID;
            transaction.itemID = cDomain.subscriptionPlanID;
            transaction.transactionName = cDomain.domainName + " Subscription";
            transaction.transactionType = "Subscription";
            transaction.transactionImage = cDomain.domainLogo;
            transaction.transactionMerchantID = currentDomain.domainMerchantID;
            transaction.transactionTokenID = App.serverData.mei_user.userCustomerTokenList[0].cardToken;
            transaction.transactionPrice = cDomain.domainAmount;
            transaction.transactionTracking = cDomain.domainSubscriptionType;
            var cTime = DateTime.Now;
            transaction.transactionDate = cTime.ToString("MM/dd/yyyy hh:mm:ss tt");

            purchasePage.SetDomainSubscriptionTransaction(transaction, "Domain");
            OverlayScreen os = new OverlayScreen();
            purchasePage.closePage = os.OnBack;
            os.SetScreen(purchasePage, "Order");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateEventSubscriptionPurchase(ServerEvent cDomain)
        {
            purchasePage = new PurchasePage();
            ServerTransaction transaction = new ServerTransaction();
            transaction.userID = App.serverData.mei_user.currentUser.userID;
            transaction.firmID = cDomain.firmID;
            transaction.itemID = cDomain.eventSubscriptionPlanID;
            transaction.transactionName = cDomain.eventName + " Subscription";
            transaction.transactionType = "Subscription";
            transaction.transactionImage = cDomain.eventLogo;
            transaction.transactionMerchantID = currentDomain.domainMerchantID;
            transaction.transactionTokenID = App.serverData.mei_user.userCustomerTokenList[0].cardToken;
            transaction.transactionPrice = cDomain.eventSubscriptionAmount;
            transaction.transactionTracking = cDomain.eventSubscriptionType;
            var cTime = DateTime.Now;
            transaction.transactionDate = cTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            purchasePage.SetDomainSubscriptionTransaction(transaction, "Event");
            OverlayScreen os = new OverlayScreen();
            purchasePage.closePage = os.OnBack;
            os.SetScreen(purchasePage, "Order");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateNoteDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            NotesTemplate nTemplate = (NotesTemplate)s;
            NotesDetailTemplate notesDetail = new NotesDetailTemplate();
            notesDetail.SetNote(ReturnNote(nTemplate.GetID()), nTemplate);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(notesDetail, "Note");
            EventHandler ev = (sen, f) => { notesDetail.SaveNote(this, os.OnBack); };
            os.TitleRightBar("mei_saveicon_w.png", ev);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }


        public void CreateReportBugPage(object s, EventArgs e)
        {
            //this.IsPresented = false;            
            FeedbackPage feedbackPage = new FeedbackPage();
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(feedbackPage, "Report Bug");
            os.ChangeBackIcon("mei_closeicon_w.png", 10);
            feedbackPage.SetReportHeader();
            EventHandler ev = (sen, f) => { feedbackPage.ReportBug(this, os.OnBack); };
            os.TitleRightBar("mei_saveicon_w.png", ev);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public void CreateFeedbackPage(object s, EventArgs e)
        {
            //this.IsPresented = false;            
            FeedbackPage feedbackPage = new FeedbackPage();
            feedbackPage.SetFeedBackHeader(currentDomain.domainName);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(feedbackPage, "Feedback & Suggestion");

            os.ChangeBackIcon("mei_closeicon_w.png", 10);
            EventHandler ev = (sen, f) => { feedbackPage.SendFeedback(this, os.OnBack); };
            os.TitleRightBar("mei_saveicon_w.png", ev);
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            ////indicator.IsVisible = false;
        }

        public async void CreateNewNote(object s, EventArgs es, object tagObject)
        {
            //this.IsPresented = false;
            try
            {
                IsPresented = false;
                ServerNote n = new ServerNote();
                n.userNote = "";
                n.userID = App.serverData.mei_user.currentUser.userID;
                var cTime = DateTime.Now;
                n.noteDateTime = cTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                n.noteID = "et_" + App.serverData.mei_user.currentUser.userID + n.noteDateTime;
                ServerNote exNote = null;
                if (tagObject == null)
                {
                    n.userNoteTag = new ServerNoteTag();
                    n.userNoteTag.noteTag = NoteTag.Note;
                    n.userNoteTag.tagID = "";
                }
                else
                {
                    if (tagObject.GetType() == typeof(ServerSpeaker))
                    {
                        ServerSpeaker speaker = ((ServerSpeaker)tagObject);
                        exNote = BaseFunctions.GetExistingNoteWithTag(speaker.speakerID);
                        if (exNote == null)
                        {
                            n.userNoteTag = new ServerNoteTag();
                            n.userNoteTag.noteTag = NoteTag.Speaker;
                            n.userNoteTag.tagID = speaker.speakerID;
                        }
                        else
                        {
                            n = exNote;
                        }
                    }
                    if (tagObject.GetType() == typeof(SponsorGroup))
                    {
                        SponsorGroup speaker = ((SponsorGroup)tagObject);
                        exNote = BaseFunctions.GetExistingNoteWithTag(speaker.sponsor.sponsorID);
                        if (exNote == null)
                        {
                            n.userNoteTag = new ServerNoteTag();
                            n.userNoteTag.noteTag = NoteTag.Sponsor;
                            n.userNoteTag.tagID = speaker.sponsor.sponsorID;
                        }
                        else
                        {
                            n = exNote;
                        }
                    }
                    if (tagObject.GetType() == typeof(ExhibitorGroup))
                    {
                        ExhibitorGroup speaker = ((ExhibitorGroup)tagObject);
                        exNote = BaseFunctions.GetExistingNoteWithTag(speaker.exhibitor.exhibitorID);
                        if (exNote == null)
                        {
                            n.userNoteTag = new ServerNoteTag();
                            n.userNoteTag.noteTag = NoteTag.Exhibitor;
                            n.userNoteTag.tagID = speaker.exhibitor.exhibitorID;
                        }
                        else
                        {
                            n = exNote;
                        }
                    }
                    if (tagObject.GetType() == typeof(ServerUser))
                    {
                        ServerUser speaker = ((ServerUser)tagObject);
                        exNote = BaseFunctions.GetExistingNoteWithTag(speaker.userID);
                        if (exNote == null)
                        {
                            n.userNoteTag = new ServerNoteTag();
                            n.userNoteTag.noteTag = NoteTag.User;
                            n.userNoteTag.tagID = speaker.userID;
                        }
                        else
                        {
                            n = exNote;
                        }
                    }

                    if (tagObject.GetType() == typeof(ServerSession))
                    {
                        ServerSession speaker = ((ServerSession)tagObject);
                        exNote = BaseFunctions.GetExistingNoteWithTag(speaker.sessionID);
                        if (exNote == null)
                        {
                            n.userNoteTag = new ServerNoteTag();
                            n.userNoteTag.noteTag = NoteTag.Session;
                            n.userNoteTag.tagID = speaker.sessionID;
                        }
                        else
                        {
                            n = exNote;
                        }
                    }
                }
                if (exNote == null)
                {
                    if (App.serverData.mei_user.noteList == null)
                        App.serverData.mei_user.noteList = new List<ServerNote>();
                    App.serverData.mei_user.noteList.Add(n);
                    await SetLoading(true, "Creating note...");
                    await BaseFunctions.AddNoteToServer(n);
                    await SetLoading(false, "Creating ...");
                }
                NotesDetailTemplate notesDetail = new NotesDetailTemplate();
                notesDetail.SetNote(n, null);
                OverlayScreen os = new OverlayScreen();
                if (exNote == null)
                    os.SetScreen(notesDetail, "New Note");
                else
                    os.SetScreen(notesDetail, "Edit Note");
                EventHandler ev = (sen, f) => { notesDetail.SaveNote(this, os.OnBack); ResetCurrentScreen(); };
                os.TitleRightBar("mei_saveicon_w.png", ev);
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
                //indicator.IsVisible = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            //currentUser.userNotes.Add(n.noteID);
        }

        public async Task<bool> CheckDelete()
        {
            bool d = await DisplayAlert("Alert", "Are you sure you want to delete this note?", "Yes", "No");
            return d;
        }

        public void ShowAlert(string title, string message, string cancelText)
        {
            DisplayAlert("Photos Not Supported", "Permission not granted to photos.", "OK");
        }

        public async void RemoveNote(object sender, EventArgs e, string id)
        {
            try
            {
                bool x = await CheckDelete();
                if (x)
                {
                    for (int i = 0; i < App.serverData.mei_user.noteList.Count; i++)
                    {
                        if (App.serverData.mei_user.noteList[i].noteID == id)
                        {
                            await SetLoading(true, "Deleting note...");
                            await BaseFunctions.DeletNoteFromServer(App.serverData.mei_user.noteList[i]);
                            App.serverData.mei_user.noteList.RemoveAt(i);
                            await SetLoading(false, "Deleting note...");
                        }
                    }
                    notesPage.CreateNotes();
                }
            }
            catch
            {

            }
        }

        public void CreateExhibitorDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            ExhibitorsTemplate psTemplate = (ExhibitorsTemplate)s;
            ExhibitorsDetailsPage exhibitorDetail = new ExhibitorsDetailsPage();
            exhibitorDetail.ExhibitorDetials(((ExhibitorsTemplateView)psTemplate.View).currentExhibitor, psTemplate);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(exhibitorDetail, "Exhibitor Detail");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            //indicator.IsVisible = false;
        }

        public async void CreatePeopleBookMark(object s, EventArgs e)
        {
            if (await BaseFunctions.GetPeopleCount(App.serverData.mei_user.currentUser.userBookmarks.people) > 0)
            {
                //this.IsPresented = false;
                PeoplePage page = new PeoplePage();
                page.CreatePeople(true);
                OverlayScreen os = new OverlayScreen();
                os.SetScreen(page, "People Bookmarks");
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
                //indicator.IsVisible = false;
            }
            else
            {
                await DisplayAlert("No Bookmarks", "You haven't bookmarked any user!", "OK");
            }
        }

        public async void CreateSponsorsBookMark(object s, EventArgs e)
        {
            if (await BaseFunctions.GetSponsorsCount(App.serverData.mei_user.currentUser.userBookmarks.sponsors) > 0)
            {
                //this.IsPresented = false;
                SponsorsPage page = new SponsorsPage();
                page.CreateSponsors(true);
                OverlayScreen os = new OverlayScreen();
                os.SetScreen(page, "Sponsors Bookmarks");
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
                //indicator.IsVisible = false;
            }
            else
            {
                await DisplayAlert("No Bookmarks", "You haven't bookmarked any sponsor!", "OK");
            }
        }

        public async void CreateExhibitorsBookMark(object s, EventArgs e)
        {
            if (await BaseFunctions.GetExhibitorsCount(App.serverData.mei_user.currentUser.userBookmarks.exhibitors) > 0)
            {
                //this.IsPresented = false;
                ExhibitorsPage page = new ExhibitorsPage();
                page.CreateExhibitors(true);
                OverlayScreen os = new OverlayScreen();
                os.SetScreen(page, "Exhibitors Bookmarks");
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
                //indicator.IsVisible = false;
            }
            else
            {
                await DisplayAlert("No Bookmarks", "You haven't bookmarked any exhibitor!", "OK");
            }
        }

        public async void CreateSpeakersBookMark(object s, EventArgs e)
        {
            if (await BaseFunctions.GetSpeakersCount(App.serverData.mei_user.currentUser.userBookmarks.speakers) > 0)
            {
                //this.IsPresented = false;
                SpeakerPage page = new SpeakerPage();
                page.CreateSpeakers(true);
                OverlayScreen os = new OverlayScreen();
                os.SetScreen(page, "Speakers Bookmarks");
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
                //indicator.IsVisible = false;
            }
            else
            {
                await DisplayAlert("No Bookmarks", "You haven't bookmarked any speaker!", "OK");
            }
        }

        public async void CreateSessionsBookMark(object s, EventArgs e)
        {
            if (await BaseFunctions.GetSessionCount(App.serverData.mei_user.currentUser.userBookmarks.session) > 0)
            {
                //this.IsPresented = false;
                isDuration = false;
                SchedulePage page = new SchedulePage();
                page.CreateSchedule(true);
                OverlayScreen os = new OverlayScreen();
                os.SetScreen(page, "Sessions Bookmarks");
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
                //indicator.IsVisible = false;
            }
            else
            {
                await DisplayAlert("No Bookmarks", "You haven't bookmarked any session!", "OK");
            }
        }

        public void CreateUserDetail()
        {
            //this.IsPresented = false;
            ClearTopScreens();
            peopleDetail = new PeopleDetailsTemplate();
            peopleDetail.SetPeopleCheck(App.serverData.mei_user.currentUser, null);
            OverlayScreen os = new OverlayScreen();
            os.SetScreen(peopleDetail, "My Profile");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            //indicator.IsVisible = false;
            this.IsPresented = false;
        }

        public async void CreateWebView(object sender, EventArgs e, string url, string header)
        {
            //this.IsPresented = false;          
            if (!url.Contains("http://") && !url.Contains("https://"))
                url = "http://" + url;
            if (await isURLValid(url))
            {
                WebPage webPage = new WebPage();
                webPage.HorizontalOptions = LayoutOptions.FillAndExpand;
                webPage.VerticalOptions = LayoutOptions.FillAndExpand;
                webPage.SetURL(url);
                OverlayScreen os = new OverlayScreen();
                os.SetScreen(webPage, header);
                mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                        , Constraint.RelativeToParent((p) => { return p.Height; }));
            }
            else
            {
                await DisplayAlert("Alert", "Sorry invalid url!", "close");
            }
            //indicator.IsVisible = false;
        }

        public async Task<bool> isURLValid(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
        public async void CreatePeopleDetail(object s, EventArgs e)
        {
            //this.IsPresented = false;
            PeopleSpeakerTemplate psTemplate = (PeopleSpeakerTemplate)s;
            peopleDetail = new PeopleDetailsTemplate();
            peopleDetail.SetPeopleCheck(await App.serverData.GetUserWithID(psTemplate.GetID()), psTemplate);
            OverlayScreen os = new OverlayScreen();
            if (App.serverData.mei_user.currentUser.userID != psTemplate.GetID())
                os.SetScreen(peopleDetail, "Profile Detail");
            else
                os.SetScreen(peopleDetail, "My Profile");
            mainPage.Children.Add(os, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return p.Width; })
                    , Constraint.RelativeToParent((p) => { return p.Height; }));
            //indicator.IsVisible = false;
        }


        public ServerNote ReturnNote(string id)
        {
            return App.serverData.mei_user.noteList.Find(x => x.noteID == id);
        }

        public void ResetCurrentPage()
        {
            mainPage.Focus();
            ResetCurrentScreen();
        }

        public void SetCurrentScreen(object current, object parent)
        {
            currentScreen = current;
            //if (historyScreens.Contains(current))
            //    historyScreens.Remove(current);
            //historyScreens.Add(current);
            parentNoScrollLayout.IsVisible = false;
            ParentLayout.IsVisible = false;
            ((StackLayout)parent).IsVisible = true;
            SetScreen(currentScreen.GetType());
            //if(!App.gettingUpdates)
        }

        public void HistoryCurrentScreen(object current)
        {
            currentScreen = current;
            if (historyScreens.Contains(current))
                historyScreens.Remove(current);
            SetScreen(currentScreen.GetType());
        }

        public async Task<bool> CheckExit()
        {
            bool d = await DisplayAlert("Alert", "Are you sure you want to Exit?", "Yes", "No");
            return d;
        }

        public void ClearTopScreens()
        {
            for (int i = mainPage.Children.Count - 1; i >= 0; i--)
            {
                if (mainPage.Children[i] != domainsListPage)
                {
                    if (mainPage.Children[i] != isLoadingView)
                    {
                        if (mainPage.Children[i] != PrimaryScreen)
                        {
                            mainPage.Children.RemoveAt(mainPage.Children.Count - 1);
                            break;
                        }
                    }
                }
            }
            if (mainPage.Children.IndexOf(domainsListPage) > 0 && domainsListPage.IsVisible && mainPage.Children.Count == 3)
            {
                //indicator.IsVisible = true;
            }
        }

        public void CheckIndicator()
        {
            if (mainPage.Children.IndexOf(domainsListPage) > 0 && domainsListPage.IsVisible && mainPage.Children.Count <= 2)
            {
                //indicator.IsVisible = true;
            }
        }

        public async void OnBackPressed()
        {
            if (mainPage.Children.Count > 1)
            {
                for (int i = mainPage.Children.Count - 1; i >= 0; i--)
                {
                    if (mainPage.Children[i] != domainsListPage)
                    {
                        if (mainPage.Children[i] != isLoadingView)
                        {
                            if (mainPage.Children[i] != PrimaryScreen)
                            {
                                mainPage.Children.RemoveAt(mainPage.Children.Count - 1);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                bool x = await CheckExit();
                if (x)
                {
                    App.closeApp(this, null);
                }
            }
            if (IsPresented)
                IsPresented = false;

            if (mainPage.Children.IndexOf(domainsListPage) > 0 && domainsListPage.IsVisible && mainPage.Children.Count <= 2)
            {
                //indicator.IsVisible = true;
            }
        }

        public void ResetButtons()
        {
            ClearOtherScreens();
            for (int i = 0; i < sideMenuSection.Children.Count; i++)
            {
                ((MenuSideItem)sideMenuSection.Children[i]).ResetButton();
            }
        }

        public void CreateMenu()
        {
            for (int i = 0; i < sideMenuSection.Children.Count; i++)
            {
                ((MenuSideItem)sideMenuSection.Children[i]).SetMenuItem(OnSideMenuItemClicked);
            }
        }

        public void BackToEvent(object sender, EventArgs e)
        {
            IsPresented = true;
        }


        private bool PanelShowing
        {
            get
            {
                return _PanelShowing;
            }
            set
            {
                _PanelShowing = value;
            }
        }

        public void AnimatePanel()
        {
            _PanelShowing = !_PanelShowing;
            domainsListPage.IsVisible = _PanelShowing;
            //indicator.IsVisible = domainsListPage.IsVisible;

            if (domainsListPage.IsVisible)
                mainPage.RaiseChild(domainsListPage);
        }

        public void ShowDomainPage()
        {
            ResetButtons();
            domainsListPage.IsVisible = true;
            //indicator.IsVisible = true;
            mainPage.RaiseChild(domainsListPage);
            IsPresented = false;
        }

        public async Task<bool> SetProgressBar(double progressValue)
        {
            await progressBar.ProgressTo(progressValue, 250, Easing.Linear);
            return true;
        }


        public async Task<bool> SetLoading(bool loading, string text)
        {
            //ParentLayout.IsVisible = !loading;

            isLoadingView.IsVisible = loading;
            IsGestureEnabled = !loading;
            if (!isLoading && loading)
            {
                await progressBar.ProgressTo(.01, 250, Easing.Linear);
            }

            isLoading = loading;

            isLoadingView.InputTransparent = !loading;
            if (loading)
                loadingText.Text = text;
            else
                loadingText.Text = "loading...";
            return true;
        }

        public async void LogoutFunction(object s, EventArgs e)
        {
            var k = await DisplayAlert("Logout", "Are you sure?", "Yes", "No");
            if (k)
            {
                App.ResetUser(this, null);
                if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
            }
        }

        public void OnAppear()
        {
            //this.Animate("intro", (s) => Layout(new Rectangle(((1 - s) * Width), Y, Width, Height)), 16, 600, Easing.Linear, null, null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.Animate("intro", (s) => Layout(new Rectangle(((1 - s) * Width), Y, Width, Height)), 16, 600, Easing.Linear, null, null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            this.Animate("exit", (s) => Layout(new Rectangle((s * Width) * -1, Y, Width, Height)), 16, 600, Easing.Linear, null, null);
        }
    }
}