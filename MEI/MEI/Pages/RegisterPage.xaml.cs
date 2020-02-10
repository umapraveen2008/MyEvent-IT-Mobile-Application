using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Threading;
using MEI.Controls;

namespace MEI.Pages
{
    public partial class RegisterPage : GradientPage
    {
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

        public RegisterPage()
        {
            InitializeComponent();
            webOverlay.IsVisible = false;
            //loginHere.Text = "<u><b>LOGIN HERE</b></u>";
            //EULA.Text = "<u>EULA</u>";
            //privacyPolicy.Text = "<u>Privacy Policy</u>";
            TapGestureRecognizer loginHereTap = new TapGestureRecognizer(),switchPanelTap = new TapGestureRecognizer();
            switchPanelTap.Tapped += (s, e) => {                
                SwitchPanel();
                if (s == EULA)
                {                 
                    ShowEULA();
                }
                else if (s == privacyPolicy)
                {                 
                    ShowPrivacyPolicy();
                }

            };
            loginHereTap.Tapped += (s, e) =>
            {
                Application.Current.MainPage = new LoginPage();
            };
            loginHere.GestureRecognizers.Add(loginHereTap);
            backButton.GestureRecognizers.Add(switchPanelTap);
            EULA.GestureRecognizers.Add(switchPanelTap);
            privacyPolicy.GestureRecognizers.Add(switchPanelTap);          
        }

        public void ShowEULA()
        {
            webHeader.Text = "EULA";
            webLinks.Source = "http://www.myeventit.com/eula.html";
        }
        
        public void ShowPrivacyPolicy()
        {
            webHeader.Text = "Privacy Policy";
            webLinks.Source = "http://www.myeventit.com/privacypolicy/";
        }

        public void SwitchPanel()
        {
            if(webOverlay.IsVisible)
            {
                webOverlay.IsVisible = false;
                registerPage.IsVisible = true;
            }
            else
            {
                webOverlay.IsVisible = true;
                registerPage.IsVisible = false;
            }
        }

        public async void CheckSignUp(object sender,EventArgs e)
        {
            if (string.IsNullOrEmpty(fName.Text))
            {
                await DisplayAlert("Alert", "Required First Name", "OK");
                return;
            }
            if (string.IsNullOrEmpty(lName.Text))
            {
                await DisplayAlert("Alert", "Required Last Name", "OK");
                return;
            }
            if (string.IsNullOrEmpty(emailAddress.Text))
            {
                await DisplayAlert("Alert", "Required Email Address", "OK");
                return;
            }
            if (!BaseFunctions.IsValidEmail(emailAddress.Text))
            {
                await DisplayAlert("Alert", "Enter a valid email", "OK");
                return;
            }

            if (string.IsNullOrEmpty(password.Text))
            {
                await DisplayAlert("Alert", "Required Password", "OK");
                return;
            }
            if (string.IsNullOrEmpty(retypePassword.Text))
            {
                await DisplayAlert("Alert", "Please Re-type Password", "OK");
                return;
            }
            if (password.Text != retypePassword.Text)
            {
                await DisplayAlert("Alert", "Password doesn't match", "OK");
                return;
            }

            

            bool retry = false;
            do
            {
                try
                {                    
                    string address = "http://www.myeventit.com/PHP/RegisterUser.php/";
                    var client = App.serverData.GetHttpClient();
                    ServerUser user = new ServerUser();
                    user.userFirstName = fName.Text;
                    user.userLastName = lName.Text;
                    user.userEmail = emailAddress.Text.ToLower();
                    user.userPrivacy = "False";
                    user.userBookmarks = new BookMark();
                    user.userBookmarks.speakers = new List<string>();
                    user.userBookmarks.exhibitors = new List<string>();
                    user.userBookmarks.sponsors = new List<string>();
                    user.userBookmarks.people = new List<string>();
                    user.userBookmarks.session = new List<string>();
                    user.userNotes = new List<string>();
                    user.userAddress = "";
                    user.userCompany = "";
                    user.userDescription = "";
                    user.userID = "";
                    user.userImage = "";
                    user.userPhone = "";
                    user.userWebsite = "";
                    user.userFacebook = "";
                    user.userTwitter = "";
                    user.userGplus = "";
                    user.userLinkedIn = "";
                    user.userCustomerID = new List<string>();
                    //user.userCustomerTokenList = new List<UserCard>();
                    var userString = JsonConvert.SerializeObject(user);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("user", userString));
                    postData.Add(new KeyValuePair<string, string>("password", password.Text));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    CancellationToken c = new CancellationToken();
                    HttpResponseMessage result = await client.PostAsync(address, content, c)  ;
                    var isRegistered = await result.Content.ReadAsStringAsync() ;
                    if (isRegistered.ToString() == "exists")
                    {
                        await DisplayAlert("Alert", emailAddress.Text + " is already registered!!", "OK");
                        return;
                    }
                    else if (isRegistered.ToString() == "false")
                    {
                        await DisplayAlert("Alert", "Registration failed", "OK");
                        return;
                    }
                    else if(isRegistered.ToString() == "mailFail")
                    {
                        var k  = await DisplayAlert("Alert", "Confirmation link sending failed", "Resend Confirmation Link", "OK");
                        if(k)
                        {
                            await App.serverData.ResendVerficationEmail(user.userEmail);                           
                        }
                        return;
                    }
                    else
                    {                        
                        await DisplayAlert("Success", "You are successfully registered. A Confirmation link will be sent to your email.", "Ok");
                        Application.Current.MainPage = new LoginPage();

                    }
                }
                catch (Exception ex)
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

        }
    }
}
