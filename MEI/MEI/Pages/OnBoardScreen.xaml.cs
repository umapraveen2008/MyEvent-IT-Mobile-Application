using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class OnBoardScreen : CarouselPage
    {
        public List<ContentPage> carList = new List<ContentPage>();
        int currentPageIndex = 0;

        public struct OnBoard
        {
            public string imageSource;
            public string imageDescription;
            public string buttonText;
            public string imageHeader;
        }

        public OnBoardScreen()
        {
            InitializeComponent();            
            for(int i = 0;i<5;i++)
            {
                OnBoardTemplate n = new OnBoardTemplate();
                n.SetPage(GetOnBoard(i), NextPage, i);
                carList.Add(n);
                this.Children.Add(n);                
            }
            this.CurrentPageChanged += (sender,e) => {
                for (int i =0;i<carList.Count;i++)
                {
                    if(carList[i] == this.CurrentPage)
                    {
                        if(currentPageIndex<i)
                        {
                            currentPageIndex++;
                        }
                        else if(currentPageIndex>i)
                        {
                            currentPageIndex--;
                        }
                    }
                }
                    };
        }

        public void NextPage(object sender,EventArgs e)
        {
            if(carList.Count-1 == ((OnBoardTemplate)((Button)sender).Parent.Parent.Parent.Parent).id)
            {
                 if(App.Current.MainPage.GetType() != typeof(LoginPage))App.Current.MainPage = new LoginPage();
            }
            else
            {
                currentPageIndex++;
                this.CurrentPage = carList[currentPageIndex];
            }
        }
        
        public OnBoard GetOnBoard(int id)
        {
            OnBoard nOnboard = new OnBoard();
            nOnboard.imageSource = "obs"+(id+1).ToString()+".png";
            switch(id)
            {
                case 0:
                    nOnboard.imageDescription = "Use the top left icon to open the event menu, use the top right icon to open event selection page. ";//"Use hamburger icon to access event menu.<br>Use dotted menu to access all events panel <br>";
                    nOnboard.imageHeader = "Navigation Bar";
                    nOnboard.buttonText = "Next";
                    break;
                case 1:
                    nOnboard.imageDescription = "All the available events are shown on this screen. Select one of them to access.";
                    nOnboard.imageHeader = "Event Selection";
                    nOnboard.buttonText = "Next";
                    break;
                case 2:
                    nOnboard.imageDescription = "This is the event menu, here you can access your settings, notes, bookmarks and also look at other registered users. You can also access relative information of the selected event.";
                    nOnboard.imageHeader = "Event Menu";
                    nOnboard.buttonText = "Next";
                    break;
                case 3:
                    nOnboard.imageDescription = "Click on the star to bookmark sessions, speakers etc.";
                    nOnboard.imageHeader = "Bookmark";
                    nOnboard.buttonText = "Next";
                    break;
                case 4:
                    nOnboard.imageDescription = "All of your bookmarks can be accessed here.";
                    nOnboard.imageHeader = "Your Bookmarks";
                    nOnboard.buttonText = "Finish";
                    break;
                    
                   
            }
            return nOnboard;
        }
    }
}
