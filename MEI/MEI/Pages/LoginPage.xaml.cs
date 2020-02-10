using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net.Http;

using System.Threading;
using MEI.Controls;

namespace MEI.Pages
{
    public partial class LoginPage : GradientPage
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

        public LoginPage()
        {
            InitializeComponent();
            App.checkForUpdates = false;
            App.eventCreation = false;
            //username.Text = "umapraveen1991@gmail.com";
            //password.Text = "eventit";

            //signUpHere.Text = "<u>SIGNUP HERE</u>";
            LoadLoginPage();
            showPassword.IsVisible = false;
            password.TextChanged += (s, e) => { if (password.Text.Length > 0) showPassword.IsVisible = true; else showPassword.IsVisible = false; };
            TapGestureRecognizer t = new TapGestureRecognizer();
            t.Tapped += (s, e) =>
            {
                password.IsPassword = !password.IsPassword;
                if (password.IsPassword)
                {
                    showPasswordIcon.Source = "mei_showicon_w.png";
                    //   showPassword.Text = "Show";                    
                }
                else
                {
                    showPasswordIcon.Source = "mei_hideicon_w.png";                    
                    // showPassword.Text = "Hide";                    
                }

            };
            showPassword.GestureRecognizers.Add(t);       
        }


        public async void DataLoaded()
        {
            try
            {
                App.GetUserEvent(this, null);
                
                if (App.GetUser != String.Empty)
                {
                     loadingText.Text = "Loading your Profile...";
                    App.userID = App.GetUser;
                    loading.IsVisible = true;
                    loginForm.IsVisible = false;
                   
                    if (App.AppResume)
                    {
                        loadingText.Text = "Syncing subscriptions...";
                        await progressBar.ProgressTo(0.2, 250, Easing.Linear);                        
                        App.serverData.GetLocalToVariable();
                        App.serverData.mei_user.currentUser = await App.serverData.GetUserWithID(App.userID);
                        //await progressBar.ProgressTo(0.4, 250, Easing.Linear);
                        //loadingText.Text = "Getting your requested Domains...";
                        //App.serverData.GetRequestedDomains();
                        //await progressBar.ProgressTo(0.6, 250, Easing.Linear);
                        //loadingText.Text = "Getting your notes...";
                        //App.serverData.SyncUserNotes();
                        loadingText.Text = "Syncing registered domains...";
                        await App.serverData.GetRegisteredDomain();
                        await progressBar.ProgressTo(0.8, 250, Easing.Linear);
                        loadingText.Text = "Syncing shipping addresses...";
                        
                        App.serverData.GetUserAddressList(); 
                        App.serverData.CreateUserTokenList();
                        //await progressBar.ProgressTo(0.9, 250, Easing.Linear);
                        loadingText.Text = "Syncing subscriptions...";
                        await App.serverData.GetUserSubscriptions();
                        await progressBar.ProgressTo(1, 250, Easing.Linear);
                        Application.Current.MainPage = new HomeLayout();
                    }
                    else
                    {
                        App.FirstTime = true;
                        loading.IsVisible = false;
                        loginForm.IsVisible = true;
                    }

                }
                else
                {
                    loading.IsVisible = false;
                    loginForm.IsVisible = true;
                }
            }
            catch(Exception e)
            {
                await DisplayAlert("User ID", e.ToString(), "OK");                
                loading.IsVisible = false;
                loginForm.IsVisible = true;
            }
        }

        public void NoInternetLogin()
        {
            loading.IsVisible = false;
            loginForm.IsVisible = true;
        }

        public void LoadLoginPage()
        {
            if (App.AppHaveInternet)
            {
                DataLoaded();
            }
            else
            {
                NoInternetLogin();
            }
            TapGestureRecognizer signUpHereTap = new TapGestureRecognizer();
            signUpHereTap.Tapped += (s, e) =>
            {
                Application.Current.MainPage = new RegisterPage();
            };
            signUpHere.GestureRecognizers.Add(signUpHereTap);

            TapGestureRecognizer forgotPasswordTap = new TapGestureRecognizer();
            forgotPasswordTap.Tapped += (s, e) =>
            {
                Application.Current.MainPage = new ForgotPassword(username.Text);
            };
            forgotPassword.GestureRecognizers.Add(forgotPasswordTap);
        }

        public async void CheckLogin(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(username.Text))
            {
                await DisplayAlert("Alert", "Email Required", "OK");
                return;
            }
            if (!BaseFunctions.IsValidEmail(username.Text))
            {
                await DisplayAlert("Alert", "Enter a valid email", "OK");
                return;
            }
            if (string.IsNullOrEmpty(password.Text))
            {
                await DisplayAlert("Alert", "Password Required", "OK");
                return;
            }
            

            loadingText.Text = "Checking with server...";
            loading.IsVisible = true;
            loginForm.IsVisible = false;
            
        
                bool retry = false;
                do
                {
                    try
                    {
                        string address = "http://www.myeventit.com/PHP/CheckUserLogin.php/";
                        var client = App.serverData.GetHttpClient();
                        var postData = new List<KeyValuePair<string, string>>();
                        postData.Add(new KeyValuePair<string, string>("email", username.Text.ToLower()));
                        postData.Add(new KeyValuePair<string, string>("password", password.Text));
                        HttpContent content = new FormUrlEncodedContent(postData);
                        CancellationToken c = new CancellationToken();
                        HttpResponseMessage result = await client.PostAsync(address, content, c);
                        var isRegistered = await result.Content.ReadAsStringAsync();
                        if (isRegistered.ToString() != "false" && isRegistered.ToString() != "Inactive")
                        {
                            App.serverData.allDomainEvents = new List<DomainGroup>();
                            App.serverData.mei_user = new UserProfile();
                            App.AppHaveInternet = true;
                            App.userID = isRegistered.ToString();
                            App.SaveUser(this, null);                            
                            loadingText.Text = "Loading your Profile...";
                            loading.IsVisible = true;
                            loginForm.IsVisible = false;
                            await progressBar.ProgressTo(0.2, 250, Easing.Linear);
                            loadingText.Text = "Syncing registered Domains...";
                            var regDomains = await App.serverData.GetRegisteredDomain();                            
                            await progressBar.ProgressTo(0.4, 250, Easing.Linear);
                            loadingText.Text = "Syncing requested Domains...";
                            App.serverData.mei_user.currentUser = await App.serverData.GetUserWithID(App.userID);
                            App.serverData.CreateUserTokenList();
                            await App.serverData.GetRequestedDomains();
                            await progressBar.ProgressTo(0.6, 250, Easing.Linear);
                            loadingText.Text = "Syncing notes...";
                            App.serverData.SyncUserNotes();
                            await progressBar.ProgressTo(0.8, 250, Easing.Linear);
                            loadingText.Text = "Syncing shipping addresses...";
                            App.serverData.GetUserAddressList();
                            await progressBar.ProgressTo(0.9, 250, Easing.Linear);
                            loadingText.Text = "Syncing subscriptions...";
                            App.serverData.GetUserSubscriptions();
                            await progressBar.ProgressTo(1, 250, Easing.Linear);
                            App.registerPhoneToServer(this, null);
                            Application.Current.MainPage = new HomeLayout();
                        }
                        else
                        {
                            if (isRegistered.ToString() == "false")
                            {
                                App.AppHaveInternet = true;
                                await DisplayAlert("Alert", "User email or Password is Invalid", "OK");
                                loading.IsVisible = false;
                                loginForm.IsVisible = true;
                                return;
                            }
                            else
                            {
                                App.AppHaveInternet = true;
                                var k = await DisplayAlert("Alert", "Your account is still unverified.","Resend Confirmation Link","Ok");
                                if(k)
                                {
                                var j = await App.serverData.ResendVerficationEmail(username.Text);
                                if(j)
                                {
                                  await DisplayAlert("Alert", "Confirmation link will be sent to your email.", "Ok");
                                }
                                }
                                loading.IsVisible = false;
                                loginForm.IsVisible = true;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(System.Net.WebException))
                            retry = await App.Current.MainPage.DisplayAlert("Alert", "No internet connection found. Please check your internet.", "Retry", "Cancel");
                        else
                            retry = true;
                        if (!retry)
                        {
                            App.AppHaveInternet = false;
                            loading.IsVisible = false;
                            loginForm.IsVisible = true;
                        }
                    }
                } while (retry);               
        }
    }
}
