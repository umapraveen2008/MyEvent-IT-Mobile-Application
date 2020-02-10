using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class EditProfile : ContentView
    {

        ServerUser currentUser = new ServerUser();
        string oldImage = "";
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


        public EditProfile()
        {
            InitializeComponent();
            App.uploadStream = null;
            for (int i = 0; i < states.Count; i++)
                state.Items.Add(states[i]);
            currentUser = App.serverData.mei_user.currentUser;
            TapGestureRecognizer logoTap = new TapGestureRecognizer();
            FirstName.Text = currentUser.userFirstName;
            LastName.Text = currentUser.userLastName;
            position.Text = currentUser.userPosition;
            company.Text = currentUser.userCompany;
            phone.Text = currentUser.userPhone;
            website.Text = currentUser.userWebsite;
            address.Text = currentUser.userAddress;
            city.Text = currentUser.userCity;
            state.SelectedIndex = states.IndexOf(currentUser.userState);
            postal.Text = currentUser.userPostal;
            shortBio.Text = currentUser.userDescription;
            userFacebook.Text = currentUser.userFacebook;
            userTwitter.Text = currentUser.userTwitter;
            userLinkedIn.Text = currentUser.userLinkedIn;
            userGPlus.Text = currentUser.userGplus;
            oldImage = currentUser.userImage;
            if (!string.IsNullOrEmpty(currentUser.userImage))
                logo.Source = currentUser.userImage;
            App.uploadImageStream = logo;
            logoTap.Tapped += App.cropImage;
            logo.GestureRecognizers.Add(logoTap);
        }


        async Task<string> UploadImageToServer()
        {
            try
            {
                string address = "http://www.myeventit.com/PHP/uploadImage.php/";
                var client = App.serverData.GetHttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                var imageContent = new ByteArrayContent(App.uploadStream);
                imageContent.Headers.ContentType =
                    MediaTypeHeaderValue.Parse("image/jpeg");

                content.Add(imageContent, "file", "image.jpg");
                CancellationToken c = new CancellationToken();
                HttpResponseMessage result = await client.PostAsync(address, content, c);
                var isRegistered = await result.Content.ReadAsStringAsync();
                if (isRegistered == "false")
                {
                    await ((HomeLayout)App.Current.MainPage).DisplayAlert("Alert", "Upload Failed", "Ok");
                    return "false";
                }
                else
                {
                    return isRegistered.ToString();
                }
            }
            catch (Exception exception)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Something went wrong please retry..", "Ok");
                return "false";
            }
        }

        async Task<bool> DeleteOldimage()
        {
            try
            {
                string address = "http://www.myeventit.com/PHP/DeleteOldImage.php/";
                var client = App.serverData.GetHttpClient();

                string deleteFileName = oldImage.Replace("http://www.myeventit.com/PHP/", "");

                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("filename", deleteFileName));

                HttpContent content = new FormUrlEncodedContent(postData);
                CancellationToken c = new CancellationToken();
                HttpResponseMessage result = await client.PostAsync(address, content, c);
                var isRegistered = await result.Content.ReadAsStringAsync();
                if (isRegistered != "true")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Something went wrong please retry..", "Ok");
                return false;
            }
        }

        public async Task<bool> SaveUserToDB()
        {
            if (App.uploadStream != null)
            {
                string uploadImage = await UploadImageToServer();
                if (uploadImage.ToString() != "false")
                {
                    currentUser.userImage = uploadImage.ToString();
                }
            }
            currentUser.userFirstName = FirstName.Text;
            currentUser.userLastName = LastName.Text;
            currentUser.userPosition = position.Text;
            currentUser.userCompany = company.Text;
            currentUser.userPhone = phone.Text;
            currentUser.userWebsite = website.Text;
            currentUser.userAddress = address.Text;
            currentUser.userDescription = shortBio.Text;
            currentUser.userGplus = userGPlus.Text;
            currentUser.userFacebook = userFacebook.Text;
            currentUser.userCity = city.Text;
            currentUser.userState = states[state.SelectedIndex];
            currentUser.userPostal = postal.Text;
            currentUser.userLinkedIn = userLinkedIn.Text;
            currentUser.userTwitter = userTwitter.Text;
            App.serverData.mei_user.currentUser = currentUser;
            App.serverData.SaveUserDataToLocal();
            if (!string.IsNullOrEmpty(oldImage) && currentUser.userImage != oldImage)
            {
                var k = await DeleteOldimage();
                Debug.WriteLine("Old Image deletion" + k.ToString());
            }
            return await BaseFunctions.SaveUserToServer();
        }
    }
}
