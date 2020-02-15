using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class EventUpdateTemplate : ViewCell
    {

        public EventUpdateTemplate()
        {
            View = new EventUpdateTemplateView();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                ((EventUpdateTemplateView)View).SetEventUpdate((ServerEventPost)BindingContext,this);
            }
            
        }

        public void SetViewHeight(object sender,EventArgs e)
        {
            this.Height = ((EventUpdateTemplateView)View).HeightRequest;
            this.ForceUpdateSize();
        }
        

        protected override void OnTapped()
        {
            base.OnTapped();
            ((ListView)this.Parent).SelectedItem = null;
        }

    }

    public partial class EventUpdateTemplateView : ContentView
    {
        string clickURL = "";
        ServerEventPost currentPost = new ServerEventPost();
        int votedOption = -1;

        public EventUpdateTemplateView()
        {
            InitializeComponent();            
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateWebView(this, null, clickURL, "Attachment"); };
            postImage.GestureRecognizers.Add(tap);
            TapGestureRecognizer like = new TapGestureRecognizer();
            like.Tapped += LikePost;
            likeImage.GestureRecognizers.Add(like);
        }

        public Button CreateButton(string text, bool isSelected)
        {
            Button b = new Button { BackgroundColor = b_BackgroundColor(isSelected), BorderColor = Color.FromHex("#31c3ee"), FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)), BorderRadius = 5, BorderWidth = 2, Text = text, TextColor = b_TextColor(isSelected) };
            b.Clicked += ButtonClick;
            return b;
        }

        public Color b_TextColor(bool isSelected)
        {
            if (!isSelected)
            {
                return Color.FromHex("#31c3ee");
            }
            else
                return Color.White;
        }

        public Color b_BackgroundColor(bool isSelected)
        {
            if (isSelected)
            {
                return Color.FromHex("#31c3ee");
            }
            else
                return Color.Transparent;
        }

        public async void ButtonClick(object sender, EventArgs e)
        {

            if (postList.Children.IndexOf((Button)sender) != votedOption)
            {
                var k = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure?", "Yes", "No");
                if (k)
                {
                    await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Updating vote..");
                    await ((HomeLayout)App.Current.MainPage).SetProgressBar(0.2);
                    currentPost = await BaseFunctions.GetPost(currentPost.postID);
                    await ((HomeLayout)App.Current.MainPage).SetProgressBar(0.4);
                    if (votedOption != -1)
                    {
                        ServerVoteOption vOption = currentPost.voteOptions[votedOption];
                        vOption.votedUsers.Remove(App.userID);
                        currentPost.voteOptions[votedOption] = vOption;
                    }
                    for (int i = 0; i < postList.Children.Count; i++)
                    {
                        if (sender == postList.Children[i] && i != votedOption)
                        {
                            ServerVoteOption voteOption = currentPost.voteOptions[i];
                            ((Button)postList.Children[i]).BackgroundColor = b_BackgroundColor(true);
                            ((Button)postList.Children[i]).TextColor = b_TextColor(true);
                            votedOption = i;
                            voteOption.votedUsers.Add(App.userID);
                            currentPost.voteOptions[i] = voteOption;

                            if (await BaseFunctions.EditPost(currentPost))
                            {
                                await ((HomeLayout)App.Current.MainPage).SetProgressBar(0.6);
                                await ((HomeLayout)App.Current.MainPage).DisplayAlert("Vote Updated", "Thank you for Voting...", "Close");
                            }
                        }
                        else
                        {
                            ((Button)postList.Children[i]).BackgroundColor = b_BackgroundColor(false);
                            ((Button)postList.Children[i]).TextColor = b_TextColor(false);

                        }
                    }
                    await ((HomeLayout)App.Current.MainPage).SetProgressBar(0.8);
                    await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Updating vote..");
                }
            }
        }

        public async void LikePost(object sender, EventArgs e)
        {

            //            currentPost = await BaseFunctions.GetPost(currentPost.postID);
            if (currentPost.likeUsers.Contains(App.userID))
            {
                currentPost.likeUsers.Remove(App.userID);
            }
            else
            {
                currentPost.likeUsers.Add(App.userID);
            }
            CheckLike();
            await BaseFunctions.EditPost(currentPost);
        }

        public void CheckLike()
        {
            if (currentPost.likeUsers.Contains(App.userID))
            {
                likeImage.Source = "mei_likeicon_r.png";
            }
            else
            {
                likeImage.Source = "mei_likeicon_g.png";
            }
            likeCount.Text = currentPost.likeUsers.Count.ToString();
        }

        public void SetEventUpdate(ServerEventPost post,EventUpdateTemplate parentViewCell)
        {
            double height = 150;
            postList.IsVisible = false;
            postList.Children.Clear();
            postHeader.Text = "";
            postInfo.Text = "";
            eventPost.Text = "";
            postImage.Source = "";
            postTime.Text = "";
            domainLogo.Source = "";
            clickURL = "";            
            votedOption = -1;            
            currentPost = post;
            loading.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");
            loading.BindingContext = postImage;
            if (post != null)
            {
                postImage.IsVisible = true;               
                ServerDomain currentDomain = ((HomeLayout)App.Current.MainPage).notificationDomain;
                postHeader.Text = currentDomain.domainName;
                if (!string.IsNullOrEmpty(currentDomain.domainLogo))
                {
                    domainLogo.IsVisible = true;
                    domainLogo.Source = currentDomain.domainLogo;
                    //logoGrid.BackgroundColor = Color.Transparent;
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentDomain.domainName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                else
                {
                    domainLogo.IsVisible = false;
                    //logoGrid.BackgroundColor = Color.FromHex("#31c3ee");
                    Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                    string init = initials.Replace(currentDomain.domainName, "$1");
                    if (init.Length > 3)
                        init = init.Substring(0, 3);
                    logoText.Text = init.ToUpper();
                }
                postInfo.Text = "Admin";
                eventPost.Text = post.postMessage.Trim();
                if (Device.RuntimePlatform == Device.iOS)
                {                 
                    height += (double)(eventPost.Text.ToString().Split("<br>"[0]).Count() * 20);
                }
                postTime.Text = post.time.Trim();
                if (!string.IsNullOrEmpty(post.postPicture))
                {
                    if (post.postPicture.ToString().Contains("png")|| post.postPicture.ToString().Contains("jpg")|| post.postPicture.ToString().Contains("jpeg"))
                    {
                        postImageLayout.IsVisible = true;
                        height = height + 200;
                        postImage.Source = new UriImageSource { Uri = new Uri(post.postPicture), CachingEnabled = false };
                        clickURL = post.postPicture;                       
                    }
                    else
                    {
                        clickURL = post.postPicture;
                        postImageLayout.IsVisible = false;

                        eventPost.Text = eventPost.Text+"<br><a href='" + post.postPicture + "'>Attachment<a>";
                        //TapGestureRecognizer tap = new TapGestureRecognizer();
                        //tap.Tapped += (s, e) => {
                        //    ((HomeLayout)App.Current.MainPage).CreateWebView(this, null, clickURL, "Attachment");
                        //};
                        //eventPost.GestureRecognizers.Add(tap);
                        height = height + 50;                 
                    }
                }
                else
                {
                    clickURL = "";
                    postImageLayout.IsVisible = false;
                }
                CheckLike();
                if (post.postType == "Voting" && post.voteOptions.Count > 1)
                {
                    postList.IsVisible = true;
                    for (int i = 0; i < post.voteOptions.Count; i++)
                    {
                        if (post.voteOptions[i].votedUsers.Contains(App.userID))
                        {
                            votedOption = i;
                            postList.Children.Add(CreateButton(post.voteOptions[i].voteOption, true));
                        }
                        else
                        {
                            postList.Children.Add(CreateButton(post.voteOptions[i].voteOption, false));
                        }
                        height = height + 60;
                    }
                }
                else
                {
                    postList.IsVisible = false;
                }
            }
            if (Device.RuntimePlatform == Device.iOS)
            {
                //this.HeightRequest = height + 30;                
                //parentViewCell.SetViewHeight(this, null);
            }
        }
    }
}
