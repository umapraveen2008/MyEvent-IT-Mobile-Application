
using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ForgotPassword : GradientPage
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

        public ForgotPassword(string _email)
        {
            InitializeComponent();
            //loginHere.Text = "<u><b>LOGIN HERE</b></u>";
            if(!string.IsNullOrEmpty(_email))
            {
                email.Text = _email;
                retypeemail.Text = _email;
            }
            TapGestureRecognizer signUpHereTap = new TapGestureRecognizer();
            signUpHereTap.Tapped += (s, e) =>
            {
                Application.Current.MainPage = new LoginPage();
            };
            loginHere.GestureRecognizers.Add(signUpHereTap);
        }

        public async void SendEmail(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(email.Text))
            {
                await DisplayAlert("Alert", "Email Required", "OK");
                return;
            }
            if (string.IsNullOrEmpty(retypeemail.Text))
            {
                await DisplayAlert("Alert", "Confirm Email Required", "OK");
                return;
            }

            if (retypeemail.Text != email.Text)
            {
                await DisplayAlert("Alert", "Please check your email address", "OK");
                return;
            }
            bool retry = false;
            do
            {
                try
                {
                    if (await BaseFunctions.CheckEmail(email.Text))
                    {
                        string address = "http://www.myeventit.com/PHP/ResetPassword.php/";
                        var client = App.serverData.GetHttpClient();

                        var postData = new List<KeyValuePair<string, string>>();
                        postData.Add(new KeyValuePair<string, string>("useremail", email.Text.ToLower()));
                        HttpContent content = new FormUrlEncodedContent(postData);
                        CancellationToken c = new CancellationToken();
                        HttpResponseMessage result = await client.PostAsync(address, content, c)  ;
                        var isRegistered = await result.Content.ReadAsStringAsync() ;
                        if (isRegistered.ToString() == "true")
                        {
                            await DisplayAlert("Recovery Password", "New Password Sent to " + email.Text, "OK");
                            Application.Current.MainPage = new LoginPage();
                        }
                        else
                        {
                            await DisplayAlert("Alert", "User email isn't registered yet!", "OK");
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
                        if (App.Current.MainPage.GetType() != typeof(LoginPage)) App.Current.MainPage = new LoginPage();
                    }                    
                }
            } while (retry);
        }

       
    }
}
