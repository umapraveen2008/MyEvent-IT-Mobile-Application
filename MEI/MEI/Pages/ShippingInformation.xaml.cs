using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ShippingInformation : ContentView
    {
        ServerUser currentUser = new ServerUser();
        public int currentIndex = 0;
        public EventHandler closePage;
        List<string> states = new List<string> {"AL","AK","AZ","AR","CA","CO","CT"
                                    ,"DE"
                                    ,"DC"
                                    ,"FL"
                                    ,"GA"
                                    ,"HI"
                                    ,"ID"
                                    ,"IL"
                                    ,"IN"
                                    ,"IA"
                                    ,"KS"
                                    ,"KY"
                                    ,"LA"
                                    ,"ME"
                                    ,"MD"
                                    ,"MA"
                                    ,"MI"
                                    ,"MN"
                                    ,"MS"
                                    ,"MO"
                                    ,"MT"
                                    ,"NE"
                                    ,"NV"
                                    ,"NH"
                                    ,"NJ"
                                    ,"NM"
                                    ,"NY"
                                    ,"NC"
                                    ,"ND"
                                    ,"OH"
                                    ,"OK"
                                    ,"OR"
                                    ,"PA"
                                    ,"RI"
                                    ,"SC"
                                    ,"SD"
                                    ,"TN"
                                    ,"TX"
                                    ,"UT"
                                    ,"VT"
                                    ,"VA"
                                    ,"WA"
                                    ,"WV"
                                    ,"WI"
                                    ,"WY"};

        public ShippingInformation()
        {
            InitializeComponent();
        }

        public void SetPaymentInformation(int index)
        {
            for (int i = 0; i < states.Count; i++)
                billingState.Items.Add(states[i]);
            currentUser = App.serverData.mei_user.currentUser;
            currentIndex = index;
            if (App.serverData.mei_user.userAddressList.Count > index)
            {
                billingFirstName.Text = App.serverData.mei_user.userAddressList[index].firstName;
                billingLastName.Text = App.serverData.mei_user.userAddressList[index].lastName;
                billingEmail.Text = App.serverData.mei_user.userAddressList[index].email;
                billingPhoneNumber.Text = App.serverData.mei_user.userAddressList[index].phone;
                billingAddressLine1.Text = App.serverData.mei_user.userAddressList[index].addressLine1;
                billingAddressLine2.Text = App.serverData.mei_user.userAddressList[index].addressLine2;
                billingCity.Text = App.serverData.mei_user.userAddressList[index].city;
                billingPostalCode.Text = App.serverData.mei_user.userAddressList[index].postalCode;
                billingState.SelectedIndex = states.IndexOf(App.serverData.mei_user.userAddressList[index].state);
                deleteAddressButton.IsVisible = true;
            }
            else
            {
                deleteAddressButton.IsVisible = false;
                billingFirstName.Text = currentUser.userFirstName;
                billingLastName.Text = currentUser.userLastName;
                billingEmail.Text = currentUser.userEmail;
                billingPhoneNumber.Text = currentUser.userPhone;
                billingAddressLine1.Text = currentUser.userAddress;
                billingCity.Text = currentUser.userCity;
                billingState.SelectedIndex = states.IndexOf(currentUser.userState);
                billingPostalCode.Text = currentUser.userPostal;
            }
        }

        public async void DeleteShippingAddress(object sender,EventArgs e)
        {
            var k = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure?", "Yes", "No");
            if(k)
            {
                App.serverData.mei_user.userAddressList.RemoveAt(currentIndex);
                bool token = await BaseFunctions.UpdateShippingAddress(App.serverData.mei_user.userAddressList);
                if (token)
                {
                    ((HomeLayout)App.Current.MainPage).shippingList.PullToRefresh();
                    closePage(this, null);
                }                
            }
        }


        public async Task<bool> SaveShippingAddress()
        {
            if (string.IsNullOrEmpty(billingFirstName.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required First Name for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingLastName.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Last Name for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingEmail.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Email for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingPhoneNumber.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Phone Number for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingAddressLine1.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Address for billing information", "OK");
                return false;
            }
            if (string.IsNullOrEmpty(billingCity.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required City for billing information", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(billingPostalCode.Text))
            {

                await App.Current.MainPage.DisplayAlert("Alert", "Required Postal / Zip code for billing information", "OK");
                return false;
            }
            bool returnValue = false;
            if (App.serverData.mei_user.userAddressList.Count > currentIndex)
            {
                BillingInformation billingInformation = new BillingInformation();
               
                billingInformation.firstName = billingFirstName.Text;
                billingInformation.lastName = billingLastName.Text;
                billingInformation.email = billingEmail.Text;
                billingInformation.phone = billingPhoneNumber.Text;
                billingInformation.addressLine1 = billingAddressLine1.Text;
                billingInformation.addressLine2 = billingAddressLine2.Text;
                billingInformation.city = billingCity.Text;
                billingInformation.postalCode = billingPostalCode.Text;
                billingInformation.state = states[billingState.SelectedIndex];
                billingInformation.country = "US";
                App.serverData.mei_user.userAddressList[currentIndex] = billingInformation;
                bool token = await BaseFunctions.UpdateShippingAddress(App.serverData.mei_user.userAddressList);
                if (token)
                {
                    ((HomeLayout)App.Current.MainPage).shippingList.PullToRefresh();
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }
            }
            else
            {
                List<BillingInformation> list = App.serverData.mei_user.userAddressList;

                BillingInformation billingInformation = new BillingInformation();

                billingInformation.firstName = billingFirstName.Text;
                billingInformation.lastName = billingLastName.Text;
                billingInformation.email = billingEmail.Text;
                billingInformation.phone = billingPhoneNumber.Text;
                billingInformation.addressLine1 = billingAddressLine1.Text;
                billingInformation.addressLine2 = billingAddressLine2.Text;
                billingInformation.city = billingCity.Text;
                billingInformation.postalCode = billingPostalCode.Text;
                billingInformation.state = states[billingState.SelectedIndex];
                billingInformation.country = "US";
                list.Add(billingInformation);      
                bool token = await BaseFunctions.AddShippingAddress(list);
                if (token)
                {
                    returnValue =  true;
                    App.serverData.mei_user.userAddressList = list;
                    ((HomeLayout)App.Current.MainPage).shippingList.PullToRefresh();
                }
                else
                {
                    returnValue = false;
                }
            }
            return returnValue;
        }
    }
}
