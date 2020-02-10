using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class MenuBottom : StackLayout
    {
        
        public MenuBottom()
        {
            InitializeComponent();
            TapGestureRecognizer userProfileTap = new TapGestureRecognizer();
            userProfileTap.Tapped += (s,e)=> { ((HomeLayout)App.Current.MainPage).CreateUserDetail(); };
            userProfile.GestureRecognizers.Add(userProfileTap);
            logoGrid.GestureRecognizers.Add(userProfileTap);

            TapGestureRecognizer settingsTapped = new TapGestureRecognizer();
            settingsTapped.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).CreateSettingScreen(); };
            settingsButton.GestureRecognizers.Add(settingsTapped);

            TapGestureRecognizer domainTapped = new TapGestureRecognizer();
            domainTapped.Tapped += (s, e) => { ((HomeLayout)App.Current.MainPage).ShowDomainPage(); };
            domainButton.GestureRecognizers.Add(domainTapped);
        }
        
        public void SetUser(ServerUser user)
        {
            if (!string.IsNullOrEmpty(user.userImage))
            {
                userProfilePic.IsVisible = true;
                userProfilePic.Source = user.userImage;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(user.userFirstName + " " + user.userLastName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            else
            {
                userProfilePic.Source = "";
                userProfilePic.IsVisible = false;
                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                string init = initials.Replace(user.userFirstName + " " + user.userLastName, "$1");
                if (init.Length > 3)
                    init = init.Substring(0, 3);
                logoText.Text = init.ToUpper();
            }
            userName.Text = user.userFirstName + " " + user.userLastName;
            //userPosition.Text = user.userPosition;
            //userCompany.Text = user.userCompany;            
        }

        public void SetButtonDetails(EventHandler _peopleEV, EventHandler _notesEV, EventHandler _bookmarksEV)
        {
            TapGestureRecognizer peopleTap = new TapGestureRecognizer();
            TapGestureRecognizer notesTap = new TapGestureRecognizer();
            TapGestureRecognizer bookmarksTap = new TapGestureRecognizer();

            peopleTap.Tapped += _peopleEV;
            notesTap.Tapped += _notesEV;
            bookmarksTap.Tapped += _bookmarksEV;

            //peopleButton.GestureRecognizers.Add(peopleTap);
            notesButton.GestureRecognizers.Add(notesTap);
            bookmarksButton.GestureRecognizers.Add(bookmarksTap);

        }
    }
}
