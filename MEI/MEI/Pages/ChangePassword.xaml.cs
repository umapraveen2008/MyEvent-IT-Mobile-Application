using Newtonsoft.Json;
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
    public partial class ChangePassword : ContentView
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        public async void ChangePasswordOnServer(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(oldpassword.Text))
            {
                await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Old password field is empty", "OK");
                return;
            }
            if (string.IsNullOrEmpty(newpassword.Text))
            {
                await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "New Password field is empty", "OK");
                return;
            }

            try
            {
                string address = "http://www.myeventit.com/PHP/ChangePassword.php/";
                var client = App.serverData.GetHttpClient();
                ServerUser user = App.serverData.mei_user.currentUser;
                string userString = JsonConvert.SerializeObject(user);
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("user", userString));
                postData.Add(new KeyValuePair<string, string>("newpassword", newpassword.Text));
                postData.Add(new KeyValuePair<string, string>("oldpassword", oldpassword.Text));
                HttpContent content = new FormUrlEncodedContent(postData);
                CancellationToken c = new CancellationToken();
                HttpResponseMessage result = await client.PostAsync(address, content, c);
                var isRegistered = await result.Content.ReadAsStringAsync();
                if (isRegistered.ToString() == "true")
                {
                    await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Your password has been changed", "OK");
                    return;
                }
                else
                {
                    await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Old password is wrong", "OK");
                    return;
                }
            }
            catch(Exception exception)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Something went wrong please retry..", "Ok");
            }
        }
    }
}
