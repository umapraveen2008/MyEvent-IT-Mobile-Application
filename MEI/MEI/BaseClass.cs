using MEI.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MEI
{

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (!string.IsNullOrEmpty(source))
            {
                return source.IndexOf(toCheck, comp) >= 0;
            }
            else
            {
                return false;
            }
        }
    }

    public enum NoteTag
    {
        User, Speaker, Exhibitor, Sponsor, Note, Session
    }

    //public enum PeopleType
    //{
    //    User, Speaker
    //}


    public enum EventAttendStatus
    {
        Attending, NotAttending, maybe
    }

    //public class Speaker
    //{
    //    public ServerSpeaker speaker { get; set; }
    //    public string speakerID { get; set; }
    //  //  public List<Session> sessionList { get; set; }
    //}


    public enum EventStatus
    {
        UpComing, Current, Expired
    }

    public class Grouping<K, T> : ObservableCollection<T>
    {
        private Task<string> key;
        private IGrouping<Task<string>, ServerSession> speakerGroup;

        public K Key { get; private set; }

        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
                this.Items.Add(item);
        }

        public Grouping(Task<string> key, IGrouping<Task<string>, ServerSession> speakerGroup)
        {
            this.key = key;
            this.speakerGroup = speakerGroup;
        }
    }

    public class BookMark
    {
        public List<string> session { get; set; }
        public List<string> people { get; set; }
        public List<string> speakers { get; set; }
        public List<string> sponsors { get; set; }
        public List<string> exhibitors { get; set; }

        public bool isBookmarked(object bookMarkObject)
        {            
            if (typeof(ServerSession) == bookMarkObject.GetType())
            {
                if (session.Find(x => x == ((ServerSession)bookMarkObject).sessionID) == null)
                    return false;
                else
                    return true;
            }
            if (typeof(ServerUser) == bookMarkObject.GetType())
            {
                if (people.Find(x => x == ((ServerUser)bookMarkObject).userID) == null)
                    return false;
                else
                    return true;
            }
            if (typeof(ServerSpeaker) == bookMarkObject.GetType())
            {
                if (speakers.Find(x => x == ((ServerSpeaker)bookMarkObject).speakerID) == null)
                    return false;
                else
                    return true;
            }
            if (typeof(ExhibitorGroup) == bookMarkObject.GetType())
            {
                if (exhibitors.Find(x => x == ((ExhibitorGroup)bookMarkObject).exhibitor.exhibitorID) == null)
                    return false;
                else
                    return true;
            }
            if (typeof(SponsorGroup) == bookMarkObject.GetType())
            {
                if (sponsors.Find(x => x == ((SponsorGroup)bookMarkObject).sponsor.sponsorID) == null)
                    return false;
                else
                    return true;
            }
            return false;
        }


        public async void AddSession(ServerSession s)
        {
            if (session.Find(x => x == s.sessionID) == null)
            {
                session.Add(s.sessionID);
                App.serverData.mei_user.b_sessionList.Add(s);
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_ADDED, "You have added " + s.sessionName + " to bookmarks", MEIStrings.MEI_OK);
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }
        }



        public async void RemoveSession(ServerSession s)
        {
            if (session.Find(x => x == s.sessionID) != null)
            {
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_REMOVED, "You have removed " + s.sessionName + " from bookmarks", MEIStrings.MEI_OK);
                session.Remove(session.Find(x => x == s.sessionID));
                App.serverData.mei_user.b_sessionList.Remove(App.serverData.mei_user.b_sessionList.Single(x => x.sessionID == s.sessionID));
            }
            try
            {
                var k = await BaseFunctions.SaveUserToServer();
            }
            catch
            {
            }
        }

        public async void AddPeople(ServerUser s)
        {
            if (people.Find(x => x == s.userID) == null)
            {
                people.Add(s.userID);
                App.serverData.mei_user.b_peopleList.Add(s);
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_ADDED, "You have added " + s.userFirstName + " " + s.userLastName + " to bookmarks", MEIStrings.MEI_OK);
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }
        }

        public async void RemovePeople(ServerUser s)
        {
            if (people.Find(x => x == s.userID) != null)
            {
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_REMOVED, "You have removed " + s.userFirstName + " " + s.userLastName + " from bookmarks", MEIStrings.MEI_OK);
                people.Remove(people.Find(x => x == s.userID));
                App.serverData.mei_user.b_peopleList.Remove(App.serverData.mei_user.b_peopleList.Single(x => x.userID == s.userID));
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }
        }

        public async void AddSpeaker(ServerSpeaker s)
        {
            if (speakers.Find(x => x == s.speakerID) == null)
            {
                speakers.Add(s.speakerID);
                App.serverData.mei_user.b_speakerList.Add(s);
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_ADDED, "You have added " + s.speakerFirstName + " " + s.speakerLastName + " to bookmarks", MEIStrings.MEI_OK);
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }
        }

        public async void RemoveSpeaker(ServerSpeaker s)
        {
            if(speakers.Find(x => x == s.speakerID)!=null)
            { 
                    await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_REMOVED, "You have removed " + s.speakerFirstName + " " + s.speakerLastName + " from bookmarks", MEIStrings.MEI_OK);
                    speakers.Remove(speakers.Find(x => x == s.speakerID));
                App.serverData.mei_user.b_speakerList.Remove(App.serverData.mei_user.b_speakerList.Single(x => x.speakerID == s.speakerID));
                try
                    {
                        var k = await BaseFunctions.SaveUserToServer();
                    }
                    catch
                    {

                    }
            }
        }

        public async void AddExhibitor(ExhibitorGroup s)
        {
            if (exhibitors.Find(x => x == s.exhibitor.exhibitorID) == null)
            {
                exhibitors.Add(s.exhibitor.exhibitorID);
                App.serverData.mei_user.b_exhibitorList.Add(s);
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_ADDED, "You have added " + s.company.companyName + " to bookmarks", MEIStrings.MEI_OK);
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }
        }

        public async void RemoveExhibitors(ExhibitorGroup s)
        {
            if (exhibitors.Find(x => x == s.exhibitor.exhibitorID) != null)
            {
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_REMOVED, "You have removed " + s.company.companyName + " from bookmarks", MEIStrings.MEI_OK);
                exhibitors.Remove(exhibitors.Find(x => x == s.exhibitor.exhibitorID));
                App.serverData.mei_user.b_exhibitorList.Remove(App.serverData.mei_user.b_exhibitorList.Single(x => x.exhibitor.exhibitorID == s.exhibitor.exhibitorID));
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }                     
        }

        public async void AddSponsor(SponsorGroup s)
        {
           
            if (sponsors.Find(x => x == s.sponsor.sponsorID) == null)
            {
                sponsors.Add(s.sponsor.sponsorID);
                App.serverData.mei_user.b_sponsorList.Add(s);
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_ADDED, "You have added " + s.company.companyName + " to bookmarks", MEIStrings.MEI_OK);
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }
        }

        public async void RemoveSponsor(SponsorGroup s)
        {

            if (sponsors.Find(x => x == s.sponsor.sponsorID) != null)
            {
                await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_BOOKMARK_REMOVED, "You have removed " + s.company.companyName + " from bookmarks", MEIStrings.MEI_OK);
                sponsors.Remove(sponsors.Find(x => x == s.sponsor.sponsorID));
                App.serverData.mei_user.b_sponsorList.Remove(App.serverData.mei_user.b_sponsorList.Single(x => x.sponsor.sponsorID == s.sponsor.sponsorID));
                try
                {
                    var k = await BaseFunctions.SaveUserToServer();
                }
                catch
                {

                }
            }            
        }

    }


    public class AppCartList : ObservableCollection<ServerCatalogGroup>, INotifyPropertyChanged
    {
        public void AddItem(ServerCatalogGroup sGroup)
        {
            this.Add(sGroup);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));                
            }
        }

        public void RemoveItem(ServerCatalogGroup sGroup)
        {
            this.Remove(sGroup);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));                
            }
        }

        public string Quantity {
            get
            {
                string i = string.Empty;
                int k = 0;
                foreach (ServerCatalogGroup item in this)
                {
                    i += item.cItem.itemCurrentQuantity.ToString();
                    k++;
                    if (k < this.Count)
                        i += "@";
                }
                return i;
            }
        }

        public string Names
        {
            get
            {
                string j = string.Empty;
                int k = 0;
                foreach (ServerCatalogGroup item in this)
                {
                    j += item.iItem.itemName.ToString();
                    k++;
                    if (k < this.Count)
                        j += "@";
                }
                return j;
            }
        }

        public string Prices
        {
            get
            {
                string j = string.Empty;
                int k = 0;
                foreach (ServerCatalogGroup item in this)
                {
                    j += item.cItem.itemPrice.ToString();
                    k++;
                    if (k < this.Count)
                        j += "@";
                }
                return j;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class AdminPost
    {
        public string header;
        public List<ServerUser> likes;
        public List<string> comments;
        public string postTime;
        public string postDescription;
        public string postImage;
        public string postLink;
    }


    public class BaseFunctions
    {
        public async static Task<int> GetPeopleCount(List<string> ids)
        {
            int count = 0;
            List<string> tempids = ids;
            for (int i = 0; i < ids.Count; i++)
            {
                if(await App.serverData.GetUserWithID(ids[i])!=null)
                    count++;            
            }
            return count;
        }

        public async static Task<int> GetSessionCount(List<string> ids)
        {
            int count = 0;
            List<string> tempids = ids;
            for (int i = 0; i < ids.Count; i++)
            {
                if (await BaseFunctions.SessionExists(ids[i]))
                    count++;
            }
            return count;
        }
        public async static Task<int> GetSponsorsCount(List<string> ids)
        {
            int count = 0;
            List<string> tempids = ids;
            for (int i = 0; i < ids.Count; i++)
            {
                if (await BaseFunctions.SponsorExists(ids[i]))
                    count++;
            }
            return count;
        }
        public async static Task<int> GetSpeakersCount(List<string> ids)
        {
            int count = 0;
            List<string> tempids = ids;
            for (int i = 0; i < ids.Count; i++)
            {
                if (await BaseFunctions.SpeakerExists(ids[i]))
                    count++;
             
            }
            return count;
        }
        public async static Task<int> GetExhibitorsCount(List<string> ids)
        {
            int count = 0;
            List<string> tempids = ids;
            for (int i = 0; i < ids.Count; i++)
            {
                if (await BaseFunctions.ExhibitorExists(ids[i]))
                    count++;
               
            }            
            return count;
        }

        public async static Task<bool> ExhibitorExists(string id)
        {
           if(await App.serverData.GetOneExhibitor(id)!=null)
                {
                    return true;
                }            
            return false;
        }

        public async static Task<bool> SessionExists(string id)
        {
            if (await App.serverData.GetOneSession(id) != null)
            {
                return true;
            }
            return false;
        }

        public async static Task<bool> SpeakerExists(string id)
        {
            if (await App.serverData.GetOneSpeaker(id) != null)
            {
                return true;
            }
            return false;
        }



        public async static Task<bool> SponsorExists(string id)
        {
            if (await App.serverData.GetOneSponsor(id) != null)
            {
                return true;
            }
            return false;
        }

 
        public static void SetTextChangeFunction(Entry entry,int length)
        {
            entry.TextChanged += (s, e) =>
            {
                if (entry.Text.Length > length)
                {
                    entry.Text = entry.Text.Substring(0, length);
                }
            };
        }

        public static void GetCustomFields(StackLayout stack,List<ServerCustomField> customFields)
        {            
            
            for(int i = 0; i < customFields.Count;i++)
            {
                Label lab = new Label { HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromHex("#505f6d"),Text= customFields[i].type + " : " + customFields[i].value};
                lab.FontSize = Device.GetNamedSize(NamedSize.Small, lab);
                stack.Children.Add(lab);
            }
        }
        public static string GetDuration(string startTime, string endTime)
        {
            if (!string.IsNullOrEmpty(startTime) || !string.IsNullOrEmpty(endTime))
            {
                if (!string.IsNullOrEmpty(endTime) && !string.IsNullOrEmpty(startTime))
                {
                    TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));

                    if (duration.Hours != 0)
                    {
                        if (duration.Minutes != 0)
                            return duration.Hours.ToString() + " Hour(s) " + duration.Minutes.ToString() + " Minute(s)";
                        else
                            return duration.Hours.ToString() + " Hour(s)";
                    }
                    else
                    {
                        return duration.Minutes.ToString() + " Minute(s)";
                    }
                }
                else
                {
                    return "-";
                }
            }
            return "-";
        }

        public async static Task<string> GetDateFullYear(string _eventID, int day)
        {
            ServerEvent _event = await App.serverData.GetSingleEventData(_eventID);
            DateTime t = GetDateTime(_event.eventStartDate);
            t.AddDays(day);
            return t.ToString("MMMM") + " " + t.ToString("dd") + " " + t.ToString("yyyy"); ;         
        }

        public static ServerNote GetNote(string id)
        {
            for (int i = 0; i < App.serverData.mei_user.noteList.Count; i++)
            {
                if (App.serverData.mei_user.noteList[i].noteID == id)
                {
                    return App.serverData.mei_user.noteList[i];
                }
            }
            return null;
        }

        //public static AppDate GetAppDateFromString(string stringDate)
        //{
        //    AppDate aDate = new AppDate();
        //    aDate.day = int.Parse(stringDate.Split("-"[0])[1]);
        //    aDate.month = int.Parse(stringDate.Split("-"[0])[0]);
        //    aDate.year = int.Parse(stringDate.Split("-"[0])[2]);
        //    return aDate;
        //}

        public static DateTime GetDateTime(string date)
        {
            return DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
        }

        public static DateTime GetDateTimeFull(string date)
        {
            return DateTime.ParseExact(date, "MM-dd-yyyy h:mm tt", CultureInfo.CurrentCulture.DateTimeFormat);
        }
        public static string getMonthDay(string date)
        {
            DateTime t = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            return t.ToString("MMMM") + " " + t.ToString("dd");
        }

        public static string getFullYear(string date)
        {
            DateTime t = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            return t.ToString("MMMM") + " " + t.ToString("dd") + " " + t.ToString("yyyy");
        }

        public static string getDayYear(string date)
        {
            DateTime t = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            return t.ToString("dd") + ", " + t.ToString("yyyy");
        }

        public static string getYear(string date)
        {
            DateTime t = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            return t.ToString("yyyy");
        }
        public static int GetMonth(string date)
        {
            string mm = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MM");
            return int.Parse(mm);
        }

        public static int GetDay(string date)
        {
            return int.Parse(DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat).ToString("dd"));
        }

        public static int GetYear(string date)
        {
            return int.Parse(DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat).ToString("yyyy"));
        }



        public static int GetMonth3Letters(string date)
        {
            return int.Parse(DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MMM"));
        }

        public static ServerInventoryItem GetInventoryItem(List<ServerInventoryItem> list,string id)
        {
            return list.Find(x => x.itemID == id);
        }
              

        public static ServerCatalogItem GetCatalogItem(ServerInventoryItem item)
        {
            ServerCatalogItem cItem = new ServerCatalogItem();
            cItem.itemID = item.itemID;
            cItem.itemPrice = item.itemPrice;
            cItem.itemMaxQuantity = item.itemMaxQuantity;
            cItem.itemCurrentQuantity = item.itemCurrentQuantity;
            cItem.eventID = "";
            return cItem;
        }
  

        public static ServerNote GetExistingNoteWithTag(string id)
        {
            for (int i = 0; i < App.serverData.mei_user.noteList.Count; i++)
            {
                if (App.serverData.mei_user.noteList[i].userNoteTag.tagID == id)
                {
                    return App.serverData.mei_user.noteList[i];
                }
            }
            return null;
        }
     
        public static async Task<bool> CheckEmail(string email)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string address = "http://www.myeventit.com/PHP/CheckEmail.php/";
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("email", email));

                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(address, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString().Equals(MEIStrings.MEI_EXISTS))
                    {
                        retry = false;
                        returnObject = true;
                    }
                    else
                    {
                        retry = false;
                        await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_ALERT, email + " isn't registerd yet!!", "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_ALERT,MEIStrings.MEI_NO_INTERNET , MEIStrings.MEI_RETRY, MEIStrings.MEI_CANCEL);
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                        retry = false;
                    }

                }

            } while (retry);
            return returnObject;
        }

        public static async Task<int> RSVPCount(string eventid)
        {
            int returnObject = 0;
            bool retry = false;
            do
            {
                try
                {
                    HttpClientHandler h = new HttpClientHandler();
                    using (HttpClient client = new HttpClient(h))
                    {                        
                        string addressUrl = "http://www.myeventit.com/PHP/GetRSVPCount.php/";

                        var postData = new List<KeyValuePair<string, string>>();
                        postData.Add(new KeyValuePair<string, string>(MEIStrings.MEI_EVENTID,eventid));
                        HttpContent content = new FormUrlEncodedContent(postData);
                        CancellationToken c = new CancellationToken();
                        HttpResponseMessage result = await client.PostAsync(addressUrl, content, c);
                        var isRegistered = await result.Content.ReadAsStringAsync() ;

                        if (isRegistered.ToString() != MEIStrings.MEI_NOTHING)
                        {
                            List<ServerRSVP> rsvpList = JsonConvert.DeserializeObject<List<ServerRSVP>>(isRegistered);
                            int count = 0;
                            if (rsvpList != null)
                            {
                                for (int i = 0; i < rsvpList.Count; i++)
                                {
                                    if (rsvpList[i].rsvpStatus == MEIStrings.MEI_YES)
                                        count++;
                                }
                            }
                            retry = false;
                            returnObject = count;
                        }
                        else
                        {
                            retry = false;
                            //await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Getting RSVP failed", "OK");                            
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert(MEIStrings.MEI_ALERT, MEIStrings.MEI_NO_INTERNET, MEIStrings.MEI_RETRY, MEIStrings.MEI_CANCEL);
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<ServerRSVP> GetRSVPStatus(string eventid)
        {
            ServerRSVP returnObject = null;
            bool retry = false;
            do
            {
                try
                {
                    HttpClientHandler h = new HttpClientHandler();
                    using (HttpClient client = new HttpClient(h))
                    {
                        string addressUrl = "http://www.myeventit.com/PHP/GetRSVP.php/";

                        var postData = new List<KeyValuePair<string, string>>();
                        postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                        postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                        HttpContent content = new FormUrlEncodedContent(postData);
                        CancellationToken c = new CancellationToken();
                        HttpResponseMessage result = await client.PostAsync(addressUrl, content, c);
                        var isRegistered = await result.Content.ReadAsStringAsync() ;

                        if (isRegistered.ToString() != "nothing")
                        {
                            retry = false;
                            returnObject = JsonConvert.DeserializeObject<ServerRSVP>(isRegistered);
                        }
                        else
                        {
                            //await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Getting RSVP failed", "OK");                            
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static double GetServiceFee(string price)
        {
            double serviceFee = Math.Round(((double.Parse(price, CultureInfo.CurrentCulture.DateTimeFormat) * 6.9) / 100), 2) + 0.3;
            return serviceFee;
        }

        public static ServerCatalogGroup GetCatalogGroup(List<ServerCatalogGroup> list,string id)
        {
            return list.Find(x => x.iItem.itemID == id);
        }     


        public static UserCard GetUserCard(List<UserCard> cardList,string token)
        {            
            for(int i = 0; i< cardList.Count;i++)
            {
                if (cardList[i].cardToken == token)
                    return cardList[i];
            }
            return null;
        }

        public static bool CheckBool(string bString)
        {
            switch (bString)
            {
                case "True":
                    {
                        return true;
                    }
                case "False":
                    {
                        return false;
                    }
            }
            return false;

        }

        public static async Task<bool> EmailSaleInvoice(ServerTransaction transaction,string paidBy)
        {
            bool returnObject = false;
            bool retry = false;
            var currentUser = App.serverData.mei_user.currentUser;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/email_user_sale_invoice.php/";
                    var client = App.serverData.GetHttpClient();
                    string authorizationCode = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_processAuthorization");                    
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user_name", currentUser.userFirstName +" "+currentUser.userLastName));
                    postData.Add(new KeyValuePair<string, string>("user_email", currentUser.userEmail));
                    postData.Add(new KeyValuePair<string, string>("user_order_id", transaction.transactionID));
                    postData.Add(new KeyValuePair<string, string>("user_item_name", transaction.transactionName));                    
                    postData.Add(new KeyValuePair<string, string>("user_item_type",  transaction.transactionType));
                    postData.Add(new KeyValuePair<string, string>("user_item_qty", transaction.transactionQuantity));
                    postData.Add(new KeyValuePair<string, string>("user_event_img", transaction.transactionImage));
                    postData.Add(new KeyValuePair<string, string>("user_item_price", transaction.transactionPrices));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_total", "$" + transaction.transactionPrice));
                    postData.Add(new KeyValuePair<string, string>("user_item_tax", "$0.00"));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_grand_total", "$" + transaction.transactionPrice));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_date", DateTime.ParseExact(transaction.transactionDate, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MMM dd, yyyy")));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_paid_by", paidBy));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_confirm_num", authorizationCode));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_shipping_type", transaction.transactionShippingType));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_shipping_address", transaction.transactionShippingAddress));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_billing_address", transaction.transactionBillingAddress));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c);
                    var isRegistered = await result.Content.ReadAsStringAsync();
                    if (isRegistered.ToString() == "InvoiceSent")
                    {
                        returnObject = true;
                    }                   
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> EmailDonationInvoice(ServerTransaction transaction, string paidBy)
        {
            bool returnObject = false;
            bool retry = false;
            var currentUser = App.serverData.mei_user.currentUser;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/email_user_donation_invoice.php/";
                    var client = App.serverData.GetHttpClient();
                    string authorizationCode = await App.serverData.GetTransactionInformation(transaction.transactionID, "trans_processAuthorization");
                    string domainName = (await App.serverData.GetDomainFromServer(transaction.firmID)).domainName;
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user_name", currentUser.userFirstName + " " + currentUser.userLastName));
                    postData.Add(new KeyValuePair<string, string>("user_email", currentUser.userEmail));
                    postData.Add(new KeyValuePair<string, string>("domain_name", domainName));
                    postData.Add(new KeyValuePair<string, string>("user_order_id", transaction.transactionID));
                    postData.Add(new KeyValuePair<string, string>("user_item_name", transaction.transactionName));
                    postData.Add(new KeyValuePair<string, string>("user_item_type", transaction.transactionType));
                    postData.Add(new KeyValuePair<string, string>("user_event_img", transaction.transactionImage));
                    postData.Add(new KeyValuePair<string, string>("user_donation_price", "$" + transaction.transactionPrice));                    
                    postData.Add(new KeyValuePair<string, string>("user_dontationTransactionFee_tax", "$0.00"));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_grand_total", "$" + transaction.transactionPrice));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_date", DateTime.ParseExact(transaction.transactionDate, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MMM dd, yyyy")));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_paid_by", paidBy));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_confirm_num", authorizationCode));
                    postData.Add(new KeyValuePair<string, string>("user_item_order_billing_address", transaction.transactionBillingAddress));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c);
                    var isRegistered = await result.Content.ReadAsStringAsync();
                    if (isRegistered.ToString() == "InvoiceSent")
                    {
                        returnObject = true;
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }


        public static async Task<bool> SaveUserToServer()
        {
            bool returnObject = false;
            bool retry = false;
            var currentUser = App.serverData.mei_user.currentUser;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/EditUser.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(currentUser);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "true")
                    {
                        App.serverData.SaveUserDataToLocal();
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Saving profile failed", "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
                return true;
            else
                return false;
        }

        public static async Task<string> MerchantSale(ServerTransaction transaction)
        {
            string returnObject = "";
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/merchantSale.php/";
                    var client = App.serverData.GetHttpClient();
                    var transactionString = JsonConvert.SerializeObject(transaction);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("transaction", transactionString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (!isRegistered.ToString().Contains(" ") && isRegistered.ToString() != "Declined")
                    {                                                   
                        returnObject = isRegistered.ToString();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", isRegistered.ToString(), "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;

        }

        public static async Task<bool> AddShippingAddress(List<BillingInformation> list)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/AddShippingAddress.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(list);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", App.userID));
                    postData.Add(new KeyValuePair<string, string>("billingList", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString()== "1")
                    {
                        returnObject = true;
                        App.serverData.SaveUserDataToLocal();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", isRegistered.ToString(), "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> UpdateShippingAddress(List<BillingInformation> list)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/UpdateShippingAddress.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(list);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", App.userID));
                    postData.Add(new KeyValuePair<string, string>("billingList", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "1")
                    {
                        returnObject = true;
                        App.serverData.SaveUserDataToLocal();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", isRegistered.ToString(), "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> EditCatalogItem(ServerCatalogGroup item)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {

                    string addressUrl = "";
                    string userString = "";
                    if (item.iItem.universal == "Yes")
                    {
                        userString = JsonConvert.SerializeObject(item.iItem);
                        addressUrl = "http://www.myeventit.com/PHP/EditCatalogItem.php/";
                    }
                    else
                    {
                        userString = JsonConvert.SerializeObject(item.cItem);
                        addressUrl = "http://www.myeventit.com/PHP/EditEventCatalogItem.php/";
                    }
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("item", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (!isRegistered.ToString().Contains("1"))
                    {
                        returnObject = true;
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<ServerEventPost> GetPost(string id)
        {
            ServerEventPost returnObject = null;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/GetOnePost.php/";
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("post", id));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<ServerEventPost>(isRegistered.ToString()); ;
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> EditPost(ServerEventPost post)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/EditEventUpdates.php/";
                    var client = App.serverData.GetHttpClient();
                    var postString = JsonConvert.SerializeObject(post);
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("post", postString));                    
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (!isRegistered.ToString().Contains("1"))
                    {
                        returnObject = true;
                    }             
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<string> AddUserToBraintree(int index,ServerUser user,CardObject card)
        {
            string returnObject = "";
            bool retry = false;            
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/RegisterUserInBraintree.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(user);
                    var cardString = JsonConvert.SerializeObject(card);
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userString));
                    postData.Add(new KeyValuePair<string, string>("index", index.ToString()));
                    postData.Add(new KeyValuePair<string, string>("card", cardString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (!isRegistered.ToString().Contains(" "))
                    {
                        returnObject = isRegistered.ToString();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", isRegistered.ToString(), "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<string> UpdateBraintreeUser(int index,ServerUser user, UserCard card)
        {
            string returnObject = "";
            bool retry = false;            
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/UpdateUserInBraintree.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(user);
                    var cardString = JsonConvert.SerializeObject(card.card);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userString));
                    postData.Add(new KeyValuePair<string, string>("index", index.ToString()));
                    postData.Add(new KeyValuePair<string, string>("card", cardString));
                    postData.Add(new KeyValuePair<string, string>("cardToken", card.cardToken));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (!isRegistered.ToString().Contains(" "))
                    {
                        returnObject = isRegistered.ToString();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", isRegistered.ToString(), "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> DeleteBraintreeUser(int index,ServerUser user)
        {
            bool returnObject = false;
            bool retry = false;            
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/DeleteUserInBraintree.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(user);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userString));
                    postData.Add(new KeyValuePair<string, string>("index", index.ToString()));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "Deleted")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", isRegistered.ToString(), "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> AddNoteToServer(ServerNote note)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Adding Note...");
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/AddNotes.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(note);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("note", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "true")
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending Feedback...");
                        return true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Sending Feedback...");
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Saving notes failed", "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }

                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> SendReportToServer(ServerReportBug bug)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/AddReportBug.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(bug);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", App.firmID));
                    postData.Add(new KeyValuePair<string, string>("bug", userString));
                    postData.Add(new KeyValuePair<string, string>("user", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "true")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Sending report failed", "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }

                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> SendFeedBackToServer(ServerUserFeeback note)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/AddFeedback.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(note);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", App.firmID));
                    postData.Add(new KeyValuePair<string, string>("feedback", userString));
                    postData.Add(new KeyValuePair<string, string>("user", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "true")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Sending feedback failed", "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }

                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> EditNoteInServer(ServerNote note)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/EditNotes.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(note);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("note", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "true")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Editing notes failed", "OK");
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public static async Task<bool> DeletNoteFromServer(ServerNote note)
        {
            bool returnObject = false;
            bool retry = false;
            do
            {
                try
                {
                    string addressUrl = "http://www.myeventit.com/PHP/DeleteNotes.php/";
                    var client = App.serverData.GetHttpClient();
                    var userString = JsonConvert.SerializeObject(note);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("note", userString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(addressUrl, content, c) ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "true")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Deleting Notes Failed", "OK");
                    }
                }
                catch (Exception e)
                {
                    retry = await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Deleting note failed", "Retry", "OK");
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }


        public async static Task<List<ServerSession>> GetSessionList(List<string> idList)
        {
            List<ServerSession> tSession = new List<ServerSession>();
            for (int i = 0; i < idList.Count; i++)
            {
                tSession.Add(await App.serverData.GetOneSession(idList[i]));
            }
            return tSession;
        }
        public async static Task<List<ExhibitorGroup>> GetExhibitorsList(List<string> idList)
        {
            List<ExhibitorGroup> tSession = new List<ExhibitorGroup>();
            for (int i = 0; i < idList.Count; i++)
            {
                tSession.Add(await App.serverData.GetOneExhibitor(idList[i]));
            }
            return tSession;
        }
        public async static Task<List<SponsorGroup>> GetSponsorsList(List<string> idList)
        {
            List<SponsorGroup> tSession = new List<SponsorGroup>();
            for (int i = 0; i < idList.Count; i++)
            {
                tSession.Add(await App.serverData.GetOneSponsor(idList[i]));
            }
            return tSession;
        }

        public async static Task<List<ServerSpeaker>> GetSpeakersList(List<string> idList)
        {
            List<ServerSpeaker> tSession = new List<ServerSpeaker>();
            for (int i = 0; i < idList.Count; i++)
            {
                tSession.Add(await App.serverData.GetOneSpeaker(idList[i]));
            }
            return tSession;
        }

        public async static Task<List<ServerSession>> GetSpeakerCurrentEventSessions(List<ServerSession> sessions,string speakerID)
        {
            List<ServerSession> tSession = new List<ServerSession>();            
            ServerSpeaker speaker = await App.serverData.GetOneSpeaker(speakerID);
            for (int j = 0; j < sessions.Count; j++)
            {
                if (sessions[j].sessionSpeakers.Contains(speakerID))
                {
                    tSession.Add(sessions[j]);
                }
            }
            return tSession;
        }


        public static EventStatus GetEventStatus(DomainEvent _event)
        {
            DateTime s = DateTime.ParseExact(_event.s_event.eventStartDate, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            DateTime e = DateTime.ParseExact(_event.s_event.eventEndDate, "MM-dd-yyyy", CultureInfo.CurrentCulture.DateTimeFormat);
            if (e.Subtract(DateTime.Now).TotalDays <= -1)
                return EventStatus.Expired;
            else if (s.Subtract(DateTime.Now).TotalDays > 0)
                return EventStatus.UpComing;
            else
                return EventStatus.Current;
        }
    }

    //public class FloorMap
    //{
    //    public string mapName { get; set; }
    //    public string map { get; set; }
    //}

    public class DomainGroup
    {
        public ServerDomain domain { get; set; } = new ServerDomain();
        public List<DomainEvent> domainEvents { get; set; } = new List<DomainEvent>();        
        public List<ServerInventoryItem> inventoryList { get; set; } = new List<ServerInventoryItem>();
        public List<ServerEventPost> userPosts { get; set; } = new List<ServerEventPost>();
        public List<ServerEventPost> userNotifications { get; set; } = new List<ServerEventPost>();
        public List<ServerUser> users { get; set; } = new List<ServerUser>();
    }

    public class DomainEvent
    {
        public ServerEvent s_event { get; set; }
        public bool isSetted { get; set; } = false;
        public List<ServerSession> sessionList { get; set; } = new List<ServerSession>();
        public List<ServerSpeaker> speakers { get; set; } = new List<ServerSpeaker>();
        public List<ServerEventPost> eventPostList { get; set; } = new List<ServerEventPost>();
        public List<string> users { get; set; } = new List<string>();
        public List<ExhibitorGroup> exhibitors { get; set; } = new List<ExhibitorGroup>();
        public List<SponsorGroup> sponsors { get; set; } = new List<SponsorGroup>();
        public List<ServerRSVP> rsvpUserList { get; set; } = new List<ServerRSVP>();
        public List<ServerCatalogGroup> catalogList { get; set; } = new List<ServerCatalogGroup>();
    }
}
