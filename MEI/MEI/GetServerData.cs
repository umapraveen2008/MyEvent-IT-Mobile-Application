using MEI.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MEI
{
    public class GetServerData
    {
        private bool isSecured = false;

        public UserProfile mei_user = new UserProfile();
        public List<DomainGroup> allDomainEvents = new List<DomainGroup>();

        private string baseURL = "www.myeventit.com/PHP/";

        private string httpAppend = "http://";
        private string securedHTTP = "https://";
        private string unSecuredHTTP = "http://";

        private string getDomain_URL = "SearchDomain.php";
        private string getSingleDomain_URL = "GetSingleDomain.php";
        private string getSingleUser_URL = "GetSingleUser.php";
        private string getRegisteredDomains_URL = "GetRegisteredDomains.php";
        private string getRequestedDomains_URL = "GetRequestedDomains.php";
        private string getSingleCompany_URL = "GetOneCompany.php";
        private string getDomainEvents_URL = "GetEventIDList.php";
        private string getSingleEvent_URL = "GetSingleEvent.php";
        private string getDomainInventory_URL = "GetInventory.php";

        private string getEventExhibitors_URL = "GetEventExhibitorIDList.php";
        private string getEventSponsors_URL = "GetEventSponsorIDList.php";
        private string getEventSessions_URL = "GetEventSessionIDList.php";
        private string getSpeakerSessions_URL = "GetSpeakerSession.php";
        private string getEventSpeakers_URL = "GetEventSpeakerIDList.php";

        private string getSingleExhibitor_URL = "GetOneExhibitor.php";
        private string getSingleSponsor_URL = "GetOneSponsor.php";
        private string getSingleSession_URL = "GetOneSession.php";
        private string getSingleSpeaker_URL = "GetOneSpeaker.php";
        private string getEventPosts_URL = "GetEventPosts.php";
        private string getEventRSVP_URL = "GetEventRSVP.php";
        private string getEventCatalog_URL = "GetCatalog.php";

        private string getDomainUserPosts_URL = "DomainUserPosts.php";

        private string resendActivationLink_URL = "ResendUserActivationCode.php";


        private string unfollowDomain_URL = "UnFollowDomain.php";
        private string getUserTransactions_URL = "GetUserTransactions.php";
        private string getUserAddressList_URL = "GetUserShippingAddress.php";
        private string getUserCardInformation_URL = "customerInfo.php";
        private string getTransactionInformation_URL = "transactionInfo.php";
        private string requestDomain_URL = "RequestDomain.php";
        private string followDomain_URL = "FollowDomain.php";
        private string getUserNotes_URL = "GetNotes.php";
        private string cancelDomainRequest_URL = "CancelDomainRequest.php";

        private string getFirmUsers_URL = "GetFirmUsers.php";

        private string isRequestedForEvent_URL = "isRequestedForEvent.php";
        private string isRegisteredForEvent_URL = "isRegisteredForEvent.php";
        private string userSubscriptions_URL = "GetUserSubscription.php";
        private string requestForEvent_URL = "RequestForEvent.php";
        private string cancelRequestForEvent_URL = "CancelRequestForEvent.php";
        private string unRegisterFromEvent_URL = "UnRegisterForEvent.php";
        private string subscribeForDomain_URL = "SubscribeForDomainEvent.php";
        private string unsubscribeForDomain_URL = "UnSubscribeForDomainEvent.php";

        private string getSubscriptionStatus_URL = "subscriptionInfo.php";

        public bool isLoaded = false;

        public GetServerData()
        {
            if (isSecured)
                httpAppend = securedHTTP + baseURL;
            else
                httpAppend = unSecuredHTTP + baseURL;
            // GetDomainEventData();            
        }

        public async Task<bool> CheckInternetConnection()
        {
            string CheckUrl = "http://google.com";
            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);
                iNetRequest.ContinueTimeout = 5000;
                WebResponse iNetResponse = await iNetRequest.GetResponseAsync();
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        public async Task<bool> SyncUserNotes()
        {
            var notes = await App.serverData.GetUserNotes();
            return true;
        }

        public async Task<ServerDomain> GetDomainFromServer(string firmID)
        {
            ServerDomain returnObject = new ServerDomain();
            bool retry = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getSingleDomain_URL, content, c);
                    var domainResponse = await result.Content.ReadAsStringAsync();
                    if (domainResponse.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<ServerDomain>(domainResponse.ToString());
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "No Domains have registered yet", "OK");
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

        public async Task<bool> CheckForNewNotifications(string firmID)
        {
            bool returnBool = false;
            DomainGroup dGroup = App.serverData.allDomainEvents.Find(x => x.domain.firmID == firmID);
            if (dGroup != null)
            {
                int currentNotificationCount = dGroup.userNotifications.Count;
                int notificationList = (await GetDomainUserPosts(firmID)).Count;
                returnBool = !(currentNotificationCount == notificationList);
            }
            return returnBool;
        }

        public async Task<List<ServerDomain>> SearchDomain(string searchString)
        {
            List<ServerDomain> returnObject = new List<ServerDomain>();
            bool retry = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("search", searchString));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getDomain_URL, content, c);
                    var domainResponse = await result.Content.ReadAsStringAsync();
                    if (domainResponse.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<ServerDomain>>(domainResponse.ToString());
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

        public async Task<bool> ResendVerficationEmail(string email)
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    mei_user.registeredDomainList.Clear();
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("email", email));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + resendActivationLink_URL, content, c);
                    var domainListResponse = await result.Content.ReadAsStringAsync();
                    if (domainListResponse.ToString() != "mailFail")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "Verifcation link sending failed", "Retry", "Cancel");
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

        public async void SyncBookmarkList()
        {
            mei_user.b_peopleList.Clear();
            ServerUser cUser = App.serverData.mei_user.currentUser;
            mei_user.b_exhibitorList = new ObservableCollection<ExhibitorGroup>(await BaseFunctions.GetExhibitorsList(cUser.userBookmarks.exhibitors));
            mei_user.b_sponsorList = new ObservableCollection<SponsorGroup>(await BaseFunctions.GetSponsorsList(cUser.userBookmarks.sponsors));
            mei_user.b_speakerList = new ObservableCollection<ServerSpeaker>(await BaseFunctions.GetSpeakersList(cUser.userBookmarks.speakers));
            for (int i = 0; i < cUser.userBookmarks.people.Count; i++)
            {
                mei_user.b_peopleList.Add(await App.serverData.GetUserWithID(cUser.userBookmarks.people[i]));
            }
            mei_user.b_sessionList = new ObservableCollection<ServerSession>(await BaseFunctions.GetSessionList(cUser.userBookmarks.session));
            //await App.Current.MainPage.DisplayAlert("Bookmarks", "Bookmarks loaded...", "Close");
        }

        public async Task<bool> GetRegisteredDomain()
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    mei_user.registeredDomainList.Clear();
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getRegisteredDomains_URL, content, c);
                    var domainListResponse = await result.Content.ReadAsStringAsync();
                    if (domainListResponse.ToString() != "null")
                    {
                        List<string> userRegisteredDomainList = JsonConvert.DeserializeObject<List<string>>(domainListResponse);
                        for (int i = 0; i < userRegisteredDomainList.Count; i++)
                        {
                            mei_user.registeredDomainList.Add(await GetDomainFromServer(userRegisteredDomainList[i]));
                        }
                        SaveUserDataToLocal();
                    }
                    returnObject = true;
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

        public async Task<bool> GetRequestedDomains()
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    mei_user.userRequestedDomainList.Clear();

                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getRequestedDomains_URL, content, c);
                    var domainListResponse = await result.Content.ReadAsStringAsync();
                    if (domainListResponse.ToString() != "null")
                        mei_user.userRequestedDomainList = JsonConvert.DeserializeObject<List<string>>(domainListResponse);

                    SaveUserDataToLocal();
                    returnObject = true;
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

        public async Task<ServerEvent> GetSingleEventData(string eventid)
        {
            ServerEvent returnObject = new ServerEvent();
            bool retry = false;
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage eventResponse = await client.PostAsync(httpAppend + getSingleEvent_URL, content, c);
                    var response = await eventResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<ServerEvent>(response.ToString());
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

        public async Task<List<string>> GetDomainEventData(string firmID)
        {
            List<string> returnObject = new List<string>();
            bool retry = false;
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage eventResponse = await client.PostAsync(httpAppend + getDomainEvents_URL, content, c);
                    var response = await eventResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<string>>(response.ToString());
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

        public async Task<ServerCompany> GetOneCompany(string companyID)
        {
            bool retry = false;
            ServerCompany returnObject = new ServerCompany();
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("companyid", companyID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage companyResponse = await client.PostAsync(httpAppend + getSingleCompany_URL, content, c);
                    var respone = await companyResponse.Content.ReadAsStringAsync();
                    if (respone.ToString() != "null")
                        returnObject = JsonConvert.DeserializeObject<ServerCompany>(respone.ToString());
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
                        if (App.Current.MainPage.GetType() != typeof(LoginPage))
                            App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public async Task<List<ServerInventoryItem>> GetEventInventoryData(string firmID)
        {
            bool retry = false;
            List<ServerInventoryItem> returnObject = new List<ServerInventoryItem>();
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage inventoryResponse = await client.PostAsync(httpAppend + getDomainInventory_URL, content, c);
                    var respone = await inventoryResponse.Content.ReadAsStringAsync();

                    if (respone.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<ServerInventoryItem>>(respone.ToString());
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
                        if (App.Current.MainPage.GetType() != typeof(LoginPage))
                            App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public async Task<List<ServerCatalogGroup>> GetEventCatalogData(List<ServerInventoryItem> inventoryList, string eventid)
        {
            bool retry = false;
            List<ServerCatalogGroup> returnObject = new List<ServerCatalogGroup>();
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage catalogResponse = await client.PostAsync(httpAppend + getEventCatalog_URL, content, c);
                    var response = await catalogResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        List<ServerCatalogItem> catList = JsonConvert.DeserializeObject<List<ServerCatalogItem>>(response.ToString());
                        if (catList != null)
                        {
                            for (int i = 0; i < inventoryList.Count; i++)
                            {
                                if (inventoryList[i].universal == "Yes")
                                {
                                    ServerCatalogGroup group = new ServerCatalogGroup();
                                    group.iItem = inventoryList[i];
                                    group.cItem = BaseFunctions.GetCatalogItem(group.iItem);
                                    returnObject.Add(group);
                                }
                            }
                            for (int i = 0; i < catList.Count; i++)
                            {
                                ServerCatalogGroup group = new ServerCatalogGroup();
                                group.cItem = catList[i];
                                group.iItem = BaseFunctions.GetInventoryItem(inventoryList, group.cItem.itemID);
                                returnObject.Add(group);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < inventoryList.Count; i++)
                        {
                            if (inventoryList[i].universal == "Yes")
                            {
                                ServerCatalogGroup group = new ServerCatalogGroup();
                                group.iItem = inventoryList[i];
                                group.cItem = BaseFunctions.GetCatalogItem(group.iItem);
                                returnObject.Add(group);
                            }
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
                        if (App.Current.MainPage.GetType() != typeof(LoginPage))
                            App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public async Task<List<ExhibitorGroup>> GetEventExhibitorData(string eventID)
        {
            bool retry = false;
            List<ExhibitorGroup> returnObject = new List<ExhibitorGroup>();
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventid", eventID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage exhibitorResponse = await client.PostAsync(httpAppend + getEventExhibitors_URL, content, c);
                    var response = await exhibitorResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        List<ServerExhibitor> exList = JsonConvert.DeserializeObject<List<ServerExhibitor>>(response.ToString());
                        if (exList != null)
                        {
                            for (int i = 0; i < exList.Count; i++)
                            {
                                ExhibitorGroup group = new ExhibitorGroup();
                                group.exhibitor = exList[i];
                                group.company = await GetOneCompany(group.exhibitor.exhibitorCompanyID);
                                returnObject.Add(group);
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public async Task<List<SponsorGroup>> GetEventSponsorData(string eventID)
        {
            bool retry = false;
            List<SponsorGroup> returnObject = new List<SponsorGroup>();
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventid", eventID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage sponsorResponse = await client.PostAsync(httpAppend + getEventSponsors_URL, content, c);
                    var response = await sponsorResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        List<ServerSponsor> spList = JsonConvert.DeserializeObject<List<ServerSponsor>>(response.ToString());
                        if (spList != null)
                        {
                            for (int i = 0; i < spList.Count; i++)
                            {
                                SponsorGroup group = new SponsorGroup();
                                group.sponsor = spList[i];
                                group.company = await GetOneCompany(group.sponsor.sponsorCompanyID);
                                returnObject.Add(group);
                            }
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

        public async Task<List<ServerSession>> GetEventSessionData(string eventID)
        {
            bool retry = false;
            List<ServerSession> returnObject = new List<ServerSession>();
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventid", eventID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage sessionResponse = await client.PostAsync(httpAppend + getEventSessions_URL, content, c);
                    var response = await sessionResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<ServerSession>>(response.ToString());
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

        public async Task<List<ServerSpeaker>> GetFirmSpeakers(string firmID)
        {
            bool retry = false;
            List<ServerSpeaker> returnObject = new List<ServerSpeaker>();
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + getEventSpeakers_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<ServerSpeaker>>(response.ToString());
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

        public async Task<string> GetTransactionInformation(string transactionID, string returnItem)
        {
            bool retry = false;
            string returnObject = "";
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("transaction", transactionID));
                    postData.Add(new KeyValuePair<string, string>("returnItem", returnItem));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getTransactionInformation_URL, content, c);
                    var cardResponse = await result.Content.ReadAsStringAsync();
                    returnObject = cardResponse;
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");                    
                    if (!retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }

            } while (retry);
            return returnObject;
        }

        public async Task<List<ServerEventPost>> GetDomainUserPosts(string firmID)
        {
            List<ServerEventPost> returnObject = new List<ServerEventPost>();
            bool retry = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    postData.Add(new KeyValuePair<string, string>("userID", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage eventPostRepsone = await client.PostAsync(httpAppend + getDomainUserPosts_URL, content, c);
                    var response = await eventPostRepsone.Content.ReadAsStringAsync();
                    List<ServerEventPost> ePosts = JsonConvert.DeserializeObject<List<ServerEventPost>>(response.ToString());
                    if (ePosts != null)
                    {
                        returnObject = ePosts;
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

        public async Task<List<ServerEventPost>> GetPostData(string eventID)
        {
            List<ServerEventPost> returnObject = new List<ServerEventPost>();
            bool retry = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventID", eventID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage eventPostRepsone = await client.PostAsync(httpAppend + getEventPosts_URL, content, c);
                    var response = await eventPostRepsone.Content.ReadAsStringAsync();
                    List<ServerEventPost> ePosts = JsonConvert.DeserializeObject<List<ServerEventPost>>(response.ToString());
                    if (ePosts != null)
                    {
                        returnObject = ePosts;
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


        public async Task<List<string>> GetDomainUsers(string firmid)
        {
            bool retry = false;
            List<string> returnObject = new List<string>();
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("firmid", firmid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getFirmUsers_URL, content, c);
                    var firmUserListResponse = await result.Content.ReadAsStringAsync();
                    
                    if (firmUserListResponse.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<string>>(firmUserListResponse);                        
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

        public async Task<List<ServerRSVP>> GetRSVPUserList(string eventid)
        {
            bool retry = false;
            List<ServerRSVP> returnObject = new List<ServerRSVP>();
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getEventRSVP_URL, content, c);
                    var noteListResponse = await result.Content.ReadAsStringAsync();
                    List<ServerRSVP> serverRSVPList = new List<ServerRSVP>();
                    if (noteListResponse.ToString() != "nothing")
                    {
                        serverRSVPList = JsonConvert.DeserializeObject<List<ServerRSVP>>(noteListResponse);
                        for (int i = 0; i < serverRSVPList.Count; i++)
                        {
                            if (serverRSVPList[i].rsvpStatus != "No")
                            {
                                returnObject.Add(serverRSVPList[i]);
                            }
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

        public async Task<bool> GetUserTransactions()
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userID", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getUserTransactions_URL, content, c);
                    var transactionResponse = await result.Content.ReadAsStringAsync();
                    if (transactionResponse.ToString() != "null")
                    {
                        mei_user.userTransactionList = JsonConvert.DeserializeObject<List<ServerTransaction>>(transactionResponse);
                    }
                    returnObject = true;
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

        public async Task<bool> GetUserAddressList()
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userID", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getUserAddressList_URL, content, c);
                    var addressResponse = await result.Content.ReadAsStringAsync();
                    if (addressResponse.ToString() != "null")
                    {
                        mei_user.userAddressList = JsonConvert.DeserializeObject<List<BillingInformation>>(addressResponse);
                    }
                    returnObject = true;
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

        public async Task<bool> CreateUserTokenList()
        {
            mei_user.userCustomerTokenList.Clear();
            ServerUser currentUser = App.serverData.mei_user.currentUser;
            for (int i = 0; i < currentUser.userCustomerID.Count; i++)
            {
                UserCard card = new UserCard();
                card.cardToken = await GetUserCardInformation(currentUser.userCustomerID[i], "cc_token");
                card.card = new CardObject();
                card.card.card4Digits = await GetUserCardInformation(currentUser.userCustomerID[i], "cc_last4");
                card.card.cardExpMonth = await GetUserCardInformation(currentUser.userCustomerID[i], "cc_expMonth");
                card.card.cardExpYear = await GetUserCardInformation(currentUser.userCustomerID[i], "cc_expYear");
                card.card.cardNumber = await GetUserCardInformation(currentUser.userCustomerID[i], "pmvf1_maskedNumber");
                card.card.cardName = await GetUserCardInformation(currentUser.userCustomerID[i], "cc_cardholderName");
                card.card.cardType = await GetUserCardInformation(currentUser.userCustomerID[i], "cc_cardType");
                card.card.cardImageURL = await GetUserCardInformation(currentUser.userCustomerID[i], "payment_imageUrl");
                card.card.cardBillingFirstName = await GetUserCardInformation(currentUser.userCustomerID[i], "ccbilling_firstName");
                card.card.cardBillingLastName = await GetUserCardInformation(currentUser.userCustomerID[i], "ccbilling_lastName");
                card.card.cardBillingAddress = await GetUserCardInformation(currentUser.userCustomerID[i], "ccbilling_streetAddress");
                card.card.cardBillingCity = await GetUserCardInformation(currentUser.userCustomerID[i], "ccbilling_locality");
                card.card.cardBillingState = await GetUserCardInformation(currentUser.userCustomerID[i], "ccbilling_region");
                card.card.cardBillingZipCode = await GetUserCardInformation(currentUser.userCustomerID[i], "ccbilling_postalCode");
                mei_user.userCustomerTokenList.Add(card);
            }
            return true;
        }

        public async Task<string> GetUserCardInformation(string userToken, string returnItem)
        {
            bool retry = false;
            string returnObject = "";
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userToken));
                    postData.Add(new KeyValuePair<string, string>("returnItem", returnItem));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getUserCardInformation_URL, content, c);
                    var cardResponse = await result.Content.ReadAsStringAsync();
                    returnObject = cardResponse;
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

        public async Task<ServerUser> GetUserWithID(string id)
        {
            ServerUser returnObject = null;
            bool retry = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", id));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage usersResponse = await client.PostAsync(httpAppend + getSingleUser_URL, content, c);
                    var response = await usersResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<ServerUser>(response.ToString());
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

        public async Task<bool> GetUserNotes()
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    mei_user.noteList.Clear();
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + getUserNotes_URL, content, c);
                    var noteListResponse = await result.Content.ReadAsStringAsync();
                    if (noteListResponse.ToString() != App.userID)
                        mei_user.noteList = JsonConvert.DeserializeObject<List<ServerNote>>(noteListResponse);
                    returnObject = true;
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

        public async Task<bool> RequestDomain(string firmID)
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + requestDomain_URL, content, c);
                    var requestDomain = await result.Content.ReadAsStringAsync();
                    if (requestDomain.ToString() != "false")
                    {
                        returnObject = true;
                        var requestDomainList = await GetRequestedDomains();
                        var registeredDomainList = await GetRegisteredDomain();
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

        public async Task<bool> FollowDomain(string firmID)
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + followDomain_URL, content, c);
                    var requestDomain = await result.Content.ReadAsStringAsync();
                    if (requestDomain.ToString() != "false")
                    {
                        returnObject = true;
                        var requestDomainList = await GetRequestedDomains();
                        var registeredDomainList = await GetRegisteredDomain();
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

        public async Task<bool> CancelDomainRequest(string firmID)
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + cancelDomainRequest_URL, content, c);
                    var requestDomain = await result.Content.ReadAsStringAsync();
                    if (requestDomain.ToString() != "false")
                    {
                        returnObject = true;
                        var requestDomainList = await GetRequestedDomains();
                        var registeredDomainList = await GetRegisteredDomain();
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

        public async Task<bool> UnFollowDomain(string firmID)
        {
            bool retry = false, returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("firmID", firmID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(httpAppend + unfollowDomain_URL, content, c);
                    var requestDomain = await result.Content.ReadAsStringAsync();
                    if (requestDomain.ToString() != "false")
                    {
                        returnObject = true;
                        var requestDomainList = await GetRequestedDomains();
                        var registeredDomainList = await GetRegisteredDomain();
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

        public async Task<ExhibitorGroup> GetOneExhibitor(string exhibitorID)
        {
            bool retry = false;
            ExhibitorGroup returnObject = null;
            do
            {
                try
                {

                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("exhibitorid", exhibitorID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage exhibitorResponse = await client.PostAsync(httpAppend + getSingleExhibitor_URL, content, c);
                    var response = await exhibitorResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = new ExhibitorGroup();
                        returnObject.exhibitor = JsonConvert.DeserializeObject<ServerExhibitor>(response.ToString());
                        returnObject.company = await GetOneCompany(returnObject.exhibitor.exhibitorCompanyID);

                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(System.Net.WebException))
                        retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                    else
                        retry = true;
                    if (retry)
                    {
                        App.AppHaveInternet = false;
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }
                }
            } while (retry);
            return returnObject;
        }

        public async Task<SponsorGroup> GetOneSponsor(string sponsorid)
        {
            bool retry = false;
            SponsorGroup returnObject = null;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("sponsorid", sponsorid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage sponsorResponse = await client.PostAsync(httpAppend + getSingleSponsor_URL, content, c);
                    var response = await sponsorResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = new SponsorGroup();
                        returnObject.sponsor = JsonConvert.DeserializeObject<ServerSponsor>(response.ToString());
                        returnObject.company = await GetOneCompany(returnObject.sponsor.sponsorCompanyID);
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

        public async Task<List<ServerSession>> GetSpeakerAllEventsSessions(string speakerID)
        {
            List<ServerSession> returnObject = new List<ServerSession>();
            bool retry = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("speakerid", speakerID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage sessionResponse = await client.PostAsync(httpAppend + getSpeakerSessions_URL, content, c);
                    var response = await sessionResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<List<ServerSession>>(response.ToString());
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

        public async Task<ServerSession> GetOneSession(string sessionID)
        {
            bool retry = false;
            ServerSession returnObject = null;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("sessionid", sessionID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage sessionResponse = await client.PostAsync(httpAppend + getSingleSession_URL, content, c);
                    var response = await sessionResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<ServerSession>(response.ToString());
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

        public async Task<ServerSpeaker> GetOneSpeaker(string speakerID)
        {
            bool retry = false;
            ServerSpeaker returnObject = null;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("speakerid", speakerID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + getSingleSpeaker_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        returnObject = JsonConvert.DeserializeObject<ServerSpeaker>(response.ToString());
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

        public async Task<bool> IsRegisteredForEvent(string eventid)
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + isRegisteredForEvent_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "False")
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

        public async Task<bool> IsRequestedForEvent(string eventid)
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + isRequestedForEvent_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "False")
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

        public async Task<bool> GetUserSubscriptions()
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + userSubscriptions_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "null")
                    {
                        mei_user.userSubscriptionList = JsonConvert.DeserializeObject<List<ServerSubscription>>(response.ToString());
                        for (int i = 0; i < mei_user.userSubscriptionList.Count; i++)
                        {
                            if (mei_user.userSubscriptionList[i].subscriptionType == "Domain")
                            {
                                if (mei_user.registeredDomainList.Find(x => x.firmID == mei_user.userSubscriptionList[i].firmID) == null)
                                {
                                    mei_user.registeredDomainList.Add(await GetDomainFromServer(mei_user.userSubscriptionList[i].firmID));
                                }
                            }
                        }
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

        public void SaveDomainDBToLocal()
        {
            App.allDomainEvents = JsonConvert.SerializeObject(allDomainEvents).ToString();
            App.SetDomainData(this, null);
        }


        public void SaveUserDataToLocal()
        {
            App.userProfileData = JsonConvert.SerializeObject(mei_user).ToString();
            App.SetUserData(this, null);
        }


        public async void GetLocalToVariable()
        {
            //await App.Current.MainPage.DisplayAlert("DomainData", App.allDomainEvents, "Ok");
            //await App.Current.MainPage.DisplayAlert("User Data", App.userProfileData, "Ok");
            if (!string.IsNullOrEmpty(App.allDomainEvents))
                allDomainEvents = JsonConvert.DeserializeObject<List<DomainGroup>>(App.allDomainEvents);
            if (!string.IsNullOrEmpty(App.userProfileData))
                mei_user = JsonConvert.DeserializeObject<UserProfile>(App.userProfileData);

        }

        public async Task<bool> RequestForEvent(string eventid)
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + requestForEvent_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "False")
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

        public async Task<bool> CancelRequestForEvent(string eventid)
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + cancelRequestForEvent_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "False")
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

        public async Task<bool> UnRegisterForEvent(string eventid)
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("eventid", eventid));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + unRegisterFromEvent_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() != "False")
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

        public async Task<string> SubscribeForDomain(ServerTransaction transaction, string subscriptionType)
        {
            bool retry = false;
            string returnObject = "";
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();
                    var transactionString = JsonConvert.SerializeObject(transaction);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("transaction", transactionString));
                    postData.Add(new KeyValuePair<string, string>("userid", App.userID));
                    postData.Add(new KeyValuePair<string, string>("tokenid", transaction.transactionTokenID));
                    postData.Add(new KeyValuePair<string, string>("planid", transaction.itemID));
                    postData.Add(new KeyValuePair<string, string>("firmid", transaction.firmID));
                    postData.Add(new KeyValuePair<string, string>("subscriptionType", subscriptionType));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + subscribeForDomain_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (!response.ToString().Contains(" "))
                    {
                        returnObject = response.ToString();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", response.ToString(), "OK");
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

        public async Task<bool> UnSubscibeForDomain(string subscriptionID)
        {
            bool retry = false;
            bool returnObject = false;
            do
            {
                try
                {
                    var client = App.serverData.GetHttpClient();

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("subscriptionid", subscriptionID));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + unsubscribeForDomain_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (response.ToString() == "cancelled")
                    {
                        returnObject = true;
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", response.ToString(), "OK");
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


        public HttpClient GetHttpClient()
        {

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://www.myeventit.com"),
            };


            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        public async Task<string> GetSubscriptionStatus(string subscriptionID, string returnItem)
        {
            bool retry = false;
            string returnObject = "";
            do
            {
                try
                {
                    var client = GetHttpClient();
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("subscription", subscriptionID));
                    postData.Add(new KeyValuePair<string, string>("returnItem", returnItem));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage speakerResponse = await client.PostAsync(httpAppend + getSubscriptionStatus_URL, content, c);
                    var response = await speakerResponse.Content.ReadAsStringAsync();
                    if (!response.ToString().Contains(" "))
                    {
                        returnObject = response.ToString();
                    }
                    else
                    {
                        await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", response.ToString(), "OK");
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
    }
}
