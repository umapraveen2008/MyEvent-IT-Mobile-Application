using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class DomainDetailPage : ContentView
    {
        public ServerDomain currentDomain;
        ServerSubscription subscription;
        bool subscribed = false;
        public bool following = false;

        public DomainDetailPage()
        {
            InitializeComponent();
        }

        public async void SetDomainDetails(ServerDomain domain)
        {
            following = false;
            subscribed = false;
            followButton.IsVisible = false;            
            followButton.BackgroundColor = Color.FromHex("#31c3ee");
            currentDomain = domain;
            if(currentDomain.domainDonation != null)
            {
                if(currentDomain.domainDonation == "Yes")
                {
                    donateDomainButton.IsVisible = true;
                }
            }
            if (!string.IsNullOrEmpty(currentDomain.domainLogo))
            {
                domainLogo.IsVisible = true;
                domainLogo.Source = currentDomain.domainLogo;
                //logoGrid.BackgroundColor = Color.Transparent;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(domain.domainName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                domainLogo.IsVisible = false;
                //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(domain.domainName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            if (!string.IsNullOrEmpty(currentDomain.domainAddress))
                domainAddress.Text = currentDomain.domainAddress;
            else
                domainAddress.IsVisible = false;
            domainName.Text = currentDomain.domainName;
            domainDescription.Text = currentDomain.domainDescription;
            if (currentDomain.domainType == "Public")
            {
                domainType.Text = "Open Domain";
            }
            else if(currentDomain.domainType == "Private")
            {
                domainType.Text = "Private Domain";
            }
            else
            {
                domainType.Text = "Subscription ( "+currentDomain.domainSubscriptionType+" Payment )";
            }

            if (currentDomain.domainType != "Subscription")
            {
                if (App.serverData.mei_user.registeredDomainList.Where(x => x.firmID == currentDomain.firmID).ToList().Count > 0)
                {
                    following = true;
                    followButton.Text = "UnFollow";
                    followButton.BackgroundColor = Color.FromHex("#ff3232");
                    followButton.IsVisible = true;
                }
               
                if (!following)
                {
                    if (currentDomain.domainType == "Private")
                    {

                        followButton.Text = "Request for Access";
                        followButton.IsVisible = true;
                    }
                    else
                    {
                        followButton.Text = "Follow Domain";
                        followButton.IsVisible = true;
                    }
                }
                if (App.serverData.mei_user.userRequestedDomainList.Contains(currentDomain.firmID))
                {
                    followButton.Text = "Cancel Request";
                    followButton.BackgroundColor = Color.FromHex("#ff3232");
                    followButton.IsVisible = true;
                }
            }
            else
            {
               
                subscription = App.serverData.mei_user.userSubscriptionList.Find(x => x.planID == currentDomain.subscriptionPlanID);
                if (subscription != null)
                {
                    following = true;
                    subscribed = true;
                    string subStatus = await App.serverData.GetSubscriptionStatus(subscription.subscriptionID, "sub_status");
                    if(subStatus=="Canceled")
                    {
                        followButton.IsEnabled = false;
                        followButton.Text = "Subscription canceled";
                        followButton.BackgroundColor = Color.FromHex("#ff3232");
                        followButton.IsVisible = true;
                    }
                    else
                    {
                        followButton.IsEnabled = true;
                        followButton.BackgroundColor = Color.FromHex("#ff3232");
                        followButton.Text = "Cancel Subscription";
                        followButton.IsVisible = true;
                    }
                }
                else
                {
                    following = false;
                    followButton.Text = "Subscribe";
                    followButton.IsVisible = true;                    
                }
                if (App.serverData.mei_user.userRequestedDomainList.Where(x => x == currentDomain.firmID).ToList().Count > 0)
                {
                    following = true;
                    followButton.Text = "UnFollow";
                    followButton.BackgroundColor = Color.FromHex("#ff3232");
                    followButton.IsVisible = true;
                }
            }     
        }

        public void DonateDomainFunction(object sender,EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateDonationPurchase(this, null);
        }

        public async void RequestToFollow(object sender,EventArgs e)
        {
            
            if (App.serverData.mei_user.userRequestedDomainList.Contains(currentDomain.firmID))
            {
                if (await App.Current.MainPage.DisplayAlert("Confirmation", "Do you want to cancel your request for this domain?", "Yes", "No"))
                {
                    await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Canceling request..");
                    await App.serverData.CancelDomainRequest(currentDomain.firmID);
                    await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Cancelling request..");
                    SetDomainDetails(currentDomain);                    
                }
                return;
            }
            if (!following)
            {
                if (currentDomain.domainType == "Private")
                {
                    if (await App.Current.MainPage.DisplayAlert("Confirmation", "Do you want to request access for this domain?", "Yes", "No"))
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Processing request..");
                        bool requested = await App.serverData.RequestDomain(currentDomain.firmID);
                        await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Cancelling request..");
                        SetDomainDetails(currentDomain);
                    }
                    else
                    {
                        return;
                    }
                }
                else if (currentDomain.domainType == "Subscription")
                {
                    if (App.serverData.mei_user.userAddressList.Count == 0)
                    {
                        var alert = await App.Current.MainPage.DisplayAlert("Alert", "Address book is empty please add atleast one shipping address to have transaction", "Go to settings", "OK");
                        if (alert)
                        {
                            ((HomeLayout)App.Current.MainPage).CreateSettingScreen();
                            ((HomeLayout)App.Current.MainPage).ClearOtherScreens();
                            return;
                        }
                        else
                        {
                            return;
                        }

                    }
                    if (App.serverData.mei_user.userCustomerTokenList.Count == 0)
                    {
                        var alert = await App.Current.MainPage.DisplayAlert("Alert", "You need to have atleast one card.", "Go to settings", "OK");
                        if (alert)
                        {
                            ((HomeLayout)App.Current.MainPage).CreateSettingScreen();
                            ((HomeLayout)App.Current.MainPage).ClearOtherScreens();
                            return;
                        }
                        else
                        {
                            return;
                        }

                    }
                    if (currentDomain.subscriptionIDStatus == "Approved")
                    {
                        ((HomeLayout)App.Current.MainPage).CreateDomainSubscriptionPurchase(currentDomain);
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", " Sorry. You cannot subscribe to this domain at this moment.", "Ok");
                    }
                    

                }
                else
                {
                    if (await App.Current.MainPage.DisplayAlert("Confirmation", "Do you want to follow this domain?", "Yes", "No"))
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Adding domain to your profile..");
                        bool requested = await App.serverData.FollowDomain(currentDomain.firmID);
                        ((HomeLayout)App.Current.MainPage).SetRegisteredDomainList();
                        SetDomainDetails(currentDomain);
                    }
                    else
                    {
                        return;
                    }
                  
                }
            }
            else 
            {
                if (currentDomain.domainType != "Subscription")
                {
                    if (await App.Current.MainPage.DisplayAlert("Confirmation", "Do you want to unfollow this domain?", "Yes", "No"))
                    {
                        await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Removing domain from your profile...");
                        await App.serverData.UnFollowDomain(currentDomain.firmID);
                        App.serverData.mei_user.currentDomainIndex = 0;
                        App.serverData.mei_user.currentEventIndex = 0;
                        ((HomeLayout)App.Current.MainPage).SetRegisteredDomainList();
                        SetDomainDetails(currentDomain);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (!subscribed)
                    {
                        if (await App.Current.MainPage.DisplayAlert("Confirmation", "Do you want to unfollow this domain?", "Yes", "No"))
                        {
                            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Removing domain from your profile...");
                            await App.serverData.UnFollowDomain(currentDomain.firmID);
                            App.serverData.mei_user.currentDomainIndex = 0;
                            App.serverData.mei_user.currentEventIndex = 0;
                            ((HomeLayout)App.Current.MainPage).SetRegisteredDomainList();
                            SetDomainDetails(currentDomain);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        var k = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure to cancel this subscription", "Yes", "No");
                        if (k)
                        {
                            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Canceling your subscription...");
                            var j = await App.serverData.UnSubscibeForDomain(subscription.subscriptionID);
                            if (j)
                            {
                                await App.Current.MainPage.DisplayAlert("Alert", "Subscription is canceled successfully.", "Ok");
                                SetDomainDetails(currentDomain);
                            }
                        }
                    }
                }
            }
            
            ((HomeLayout)App.Current.MainPage).CheckDomainList();
        }
    }
}
