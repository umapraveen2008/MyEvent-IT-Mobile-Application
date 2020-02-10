using System.Collections.Generic;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MEI
{
    public class VcardContact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string company { get; set; }
    }

    public class ServerTransaction
    {
        public string userID { get; set; } = "";
        public string firmID { get; set; } = "";
        public string itemID { get; set; } = "";
        public string transactionID { get; set; } = "";
        public string transactionName { get; set; } = "";
        public string transactionImage { get; set; } = "";
        public string transactionPrices { get; set; } = "";
        public string transactionType { get; set; } = "";
        public string transactionRefund { get; set; } = "false";
        public string transactionHaveRefund { get; set; } = "No";
        public string transactionPrice { get; set; } = "0.00";
        public string transactionQuantity { get; set; } = "1";
        public string transactionTokenID { get; set; } = "";
        public string transactionTracking { get; set; } = "";
        public string transactionMerchantID { get; set; } = "";
        public string transactionShippingAddress { get; set; } = "";
        public string transactionBillingAddress { get; set; } = "";
        public string transactionShippingType { get; set; } = "";
        public string transactionServiceFee { get; set; } = "";
        public string transactionDate { get; set; } = "";
    }

    public class ServerInventoryItem
    {
        public string firmID { get; set; }
        public string universal { get; set; }
        public string itemID { get; set; }
        public string itemName { get; set; }
        public string itemType { get; set; }
        public string itemPrice { get; set; }
        public string itemImage { get; set; }
        public string itemMaxQuantity { get; set; }
        public string itemCurrentQuantity { get; set; }
        public string itemDescription { get; set; }
        public string itemShippingType { get; set; }
        public string itemRefund { get; set; }
    }

    public class ServerCatalogGroup
    {
        public bool IsUniversal
        {
            get
            {
                return iItem.universal == "Yes";
            }
        }

        public bool Available(int c)
        {            
            if (IsUniversal)
            {
                if (iItem.itemMaxQuantity == "-1")
                    return true;
                return c <= int.Parse(iItem.itemMaxQuantity);
            }
            else
            {
                if (cItem.itemMaxQuantity == "-1")
                    return true;
                return c <= int.Parse(cItem.itemMaxQuantity);
            }
        }
        public ServerCatalogItem cItem { get; set; }
        public ServerInventoryItem iItem { get; set; }
    }

    public class ServerCatalogItem
    {
        public string itemID { get; set; }
        public string eventID { get; set; }
        public string itemMaxQuantity { get; set; }
        public string itemPrice { get; set; }
        public string itemCurrentQuantity { get; set; }
    }

    public class ServerDomain
    {
        public string firmID { get; set; } = "";
        public string domainLogo { get; set; } = "";
        public string domainName { get; set; } = "";
        public string domainType { get; set; } = "";
        public string domainSubscriptionType { get; set; } = "";
        public string subscriptionPlanID { get; set; } = "";
        public string domainAmount { get; set; } = "";
        public string domainTwitter { get; set; } = "";
        public string domainEmail { get; set; } = "";
        public string domainGplus { get; set; } = "";
        public string domainFacebook { get; set; } = "";
        public string domainLinkedIn { get; set; } = "";
        public string domainDescription { get; set; } = "";
        public string domainPhone { get; set; } = "";
        public string domainAddress { get; set; } = "";
        public string domainMerchantID { get; set; } = "";
        public string domainCity { get; set; } = "";
        public string domainPostal { get; set; } = "";
        public string domainCountry { get; set; } = "";
        public string domainState { get; set; } = "";
        public string domainDonation { get; set; } = "";
        public string BT_customerID { get; set; } = "";
        public string BT_cusomterToken { get; set; } = "";
        public List<string> BT_transactionList { get; set; } = new List<string>();
        public string BT_subscriptionID { get; set; } = "";
        public string planID { get; set; } = "";
        public string subscriptionIDStatus { get; set; } = "";
    }

    public class ServerEvent
    {
        public string firmID { get; set; } = "";
        public string eventID { get; set; } = "";
        public string eventLogo { get; set; } = "";
        public string eventName { get; set; } = "";
        public string eventStatus { get; set; } = "";
        public string eventStartDate { get; set; } = "";
        public string eventStartTime { get; set; } = "";
        public string eventEndDate { get; set; } = "";
        public string eventEndTime { get; set; } = "";
        public string eventType { get; set; } = "";
        public string eventSubscriptionType { get; set; } = "";
        public string eventSubscriptionPlanID { get; set; } = "";
        public string eventSubscriptionAmount { get; set; } = "";
        public string eventSubscriptionIDStatus { get; set; } = "";
        public string eventDescription { get; set; } = "";
        public string eventVenueName { get; set; } = "";
        public int eventAttendanceLimit { get; set; } = 0;
        public List<string> eventUsers { get; set; } = new List<string>();
        public string eventAddress { get; set; } = "";
        public string eventPublishStatus { get; set; } = "";
        public List<ServerFloorMap> eventFloorMap { get; set; } = new List<ServerFloorMap>();
        public ServerVenueMap eventVenueMap { get; set; } = new ServerVenueMap();
        public ServerFAQModule eventFaq { get; set; } = new ServerFAQModule();
        public string eventAbout { get; set; } = "";
    }

    public class ServerVenueMap
    {
        public string venueMapName { get; set; }
        public string venueAddress { get; set; }
        public List<ServerMarkerPoints> venuePoints { get; set; }

    }


    public class ServerMarkerPoints
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class ServerFAQModule
    {
        public ServerWifiDetails wifiDetails { get; set; }
        public List<ServerQandA> eventQandA { get; set; }
        public string eventTerms { get; set; }
        public string eventPolicy { get; set; }
        public string eventCodeOfConduct { get; set; }
    }


    public class ServerWifiDetails
    {
        public string name { get; set; }
        public string password { get; set; }
    }

    public class ServerQandA
    {
        public string question { get; set; }
        public string answer { get; set; }
    }

    public class ServerRSVP
    {
        public string eventID { get; set; }
        public string userID { get; set; }
        public string firmID { get; set; }
        public string rsvpStatus { get; set; }
    }

    public class ServerFloorMap
    {
        public string eventFloorMapName { get; set; }
        public string eventFloorMapURL { get; set; }
        public string eventFloorMapIndex { get; set; }
    }

    public class ServerCompany
    {
        public string companyID { get; set; }
        public string firmID { get; set; }
        public string companyLogo { get; set; }
        public string companyName { get; set; }
        public string companyWebsite { get; set; }
        public string companyEmail { get; set; }
        public string companyDescription { get; set; }
        public string companyFacebook { get; set; }
        public string companyTwitter { get; set; }
        public string companyGplus { get; set; }
        public string companyLinkedIn { get; set; }
        public string companyType { get; set; }
        public string companyPhone { get; set; }
        public string companyAddress { get; set; }
    }

    public class ServerExhibitor
    {
        public string eventID { get; set; }
        public string firmID { get; set; }
        public List<ServerCustomField> exhibitorFields { get; set; }
        public string exhibitorID { get; set; }
        public string exhibitorCompanyID { get; set; }

    }

    public class ExhibitorGroup
    {
        public ServerExhibitor exhibitor { get; set; }
        public ServerCompany company { get; set; }
    }

    public class ServerSponsor
    {
        public string eventID { get; set; }
        public string firmID { get; set; }
        public List<ServerCustomField> sponsorFields { get; set; }
        public string sponsorID { get; set; }
        public string sponsorCompanyID { get; set; }
    }

    public class ServerSubscription
    {
        public string planID { get; set; }
        public string firmID { get; set; }
        public string subscriptionID { get; set; }
        public string subscriptionDate { get; set; }
        public string subscriptionType { get; set; }
    }

    public class ServerCustomField
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class SponsorGroup
    {
        public ServerSponsor sponsor { get; set; }
        public ServerCompany company { get; set; }
    }

    public class UserProfile
    {
        public int currentDomainIndex { get; set; } = 0;
        public int currentEventIndex { get; set; } = 0;
        public ServerUser currentUser { get; set; } = new ServerUser();
        public List<ServerDomain> registeredDomainList { get; set; } = new List<ServerDomain>();
        public List<string> userRequestedDomainList { get; set; } = new List<string>();
        public List<ServerTransaction> userTransactionList { get; set; } = new List<ServerTransaction>();
        public List<UserCard> userCustomerTokenList { get; set; } = new List<UserCard>();
        public List<BillingInformation> userAddressList { get; set; } = new List<BillingInformation>();
        public List<ServerNote> noteList { get; set; } = new List<ServerNote>();
        public List<ServerSubscription> userSubscriptionList { get; set; } = new List<ServerSubscription>();
        public ObservableCollection<ExhibitorGroup> b_exhibitorList = new ObservableCollection<ExhibitorGroup>();
        public ObservableCollection<SponsorGroup> b_sponsorList = new ObservableCollection<SponsorGroup>();
        public ObservableCollection<ServerSpeaker> b_speakerList = new ObservableCollection<ServerSpeaker>();
        public ObservableCollection<ServerUser> b_peopleList = new ObservableCollection<ServerUser>();
        public ObservableCollection<ServerSession> b_sessionList = new ObservableCollection<ServerSession>();
    }

    public class ServerSession
    {
        public string sessionStartTime { get; set; }
        public string sessionName { get; set; }
        public string eventID { get; set; }
        public string firmID { get; set; }
        public string sessionID { get; set; }
        public string sessionTrack { get; set; }
        public string sessionLocation { get; set; }
        public string sessionEndTime { get; set; }
        public string sessionDescription { get; set; }
        public List<string> sessionSpeakers { get; set; }
        public List<string> sessionCatalogItems { get; set; }
        public List<string> sessionSponsors { get; set; }
        public List<string> sessionExhibitors { get; set; }
        public int sessionDay { get; set; }
    }

    public class ServerVoteOption
    {
        public string voteOption { get; set; }
        public List<string> votedUsers { get; set; }
    }

    public class ServerComment
    {
        public string commentMessage { get; set; }
        public List<string> commentLikes { get; set; }
    }

    public class ServerEventPost
    {
        public string time { get; set; }
        public string postType { get; set; }
        public List<string> postUsers { get; set; }
        public List<ServerVoteOption> voteOptions { get; set; }
        public List<ServerComment> comments { get; set; }
        public List<string> likeUsers { get; set; }
        public string postPicture { get; set; }
        public string postMessage { get; set; }
        public string postHeader { get; set; }
        public string firmID { get; set; }
        public string postID { get; set; }
        public string eventID { get; set; }
    }

    public class BillingInformation
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
    }

    public class CardObject
    {
        public string cardName { get; set; }
        public string cardNumber { get; set; }
        public string card4Digits { get; set; }
        public string cardExpMonth { get; set; }
        public string cardType { get; set; }
        public string cardExpYear { get; set; }
        public string cardCVV { get; set; }
        public string cardImageURL { get; set; }
        public string cardBillingFirstName { get; set; }
        public string cardBillingLastName { get; set; }
        public string cardBillingAddress { get; set; }
        public string cardBillingCity { get; set; }
        public string cardBillingState { get; set; }
        public string cardBillingZipCode { get; set; }
    }

    public class UserCard
    {
        public string cardToken { get; set; }
        public CardObject card { get; set; }
    }

    public class ServerUser
    {
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string userEmail { get; set; }
        public string userPosition { get; set; }
        public string userImage { get; set; }
        public string userPhone { get; set; }
        public string userWebsite { get; set; }
        public string userFacebook { get; set; }
        public string userGplus { get; set; }
        public string userTwitter { get; set; }
        public string userLinkedIn { get; set; }
        public string userAddress { get; set; }
        public string userCity { get; set; }
        public string userState { get; set; }
        public string userPostal { get; set; }
        public string userCompany { get; set; }
        public string userPrivacy { get; set; }
        public string userID { get; set; } = "";
        public string userDescription { get; set; }
        public List<string> userCustomerID { get; set; }
        //public List<UserCard> userCustomerTokenList { get; set; }
        public BookMark userBookmarks { get; set; }
        public List<string> userNotes { get; set; }
    }

    public class RegisteredDomainViewModel : INotifyPropertyChanged
    {
        public ServerDomain domain;
        Color bgColor;
        public bool haveUnread = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public RegisteredDomainViewModel()
        {
            this.bgColor = Color.White;
        }

        public bool HaveUnread
        {
            set
            {
                if (haveUnread != value)
                {
                    haveUnread = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("HaveUnread"));
                    }
                }
            }
            get
            {
                return haveUnread;
            }
        }

        public Color BGColor
        {
            set
            {
                if (bgColor != value)
                {
                    bgColor = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("BGColor"));
                    }
                }
            }
            get
            {
                return bgColor;
            }
        }
    }


    public class ServerUserFeeback
    {
        public string userID { get; set; }
        public string firmID { get; set; }
        public string userFeedback { get; set; }
        public string userFeedbackDate { get; set; }

    }

    public class ServerReportBug
    {
        public string userID { get; set; }
        public string firmID { get; set; }
        public string userBug { get; set; }
        public string userBugDate { get; set; }

    }

    public class ServerSpeaker
    {
        public string speakerID { get; set; }
        public string speakerImage { get; set; }
        public string firmID { get; set; }
        public string speakerLastName { get; set; }
        public string speakerFirstName { get; set; }
        public string speakerPosition { get; set; }
        public string speakerCompany { get; set; }
        public string speakerDescription { get; set; }
        public string speakerEmail { get; set; }
        public string speakerPhone { get; set; }
        public string speakerTwitter { get; set; }
        public string speakerFacebook { get; set; }
        public string speakerWebsite { get; set; }
        public string speakerGplus { get; set; }
        public string speakerLinkedIn { get; set; }
    }

    public class ServerNote
    {
        public string userID { get; set; }
        public string noteID { get; set; }
        public string noteDateTime { get; set; }
        public string userNote { get; set; }
        public ServerNoteTag userNoteTag { get; set; }
    }

    public class ServerNoteTag
    {
        public string tagID { get; set; }
        public NoteTag noteTag { get; set; }
    }
}
