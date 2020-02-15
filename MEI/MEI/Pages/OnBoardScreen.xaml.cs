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
            nOnboard.imageSource = "mei_education_0" + (id+1).ToString()+".png";
            switch(id)
            {
                case 0:
                    nOnboard.imageDescription = "Search for Organizations to Join. ";//"Use hamburger icon to access event menu.<br>Use dotted menu to access all events panel <br>";
                    nOnboard.imageHeader = "Search";
                    nOnboard.buttonText = "Next";
                    break;
                case 1:
                    nOnboard.imageDescription = "Verify You are Joining the Correct Organization.";
                    nOnboard.imageHeader = "Select Organisation";
                    nOnboard.buttonText = "Next";
                    break;
                case 2:
                    nOnboard.imageDescription = "View all Events Within the Organization.";
                    nOnboard.imageHeader = "Event Menu";
                    nOnboard.buttonText = "Next";
                    break;
                case 3:
                    nOnboard.imageDescription = "View all Event Items.";
                    nOnboard.imageHeader = "Event Items";
                    nOnboard.buttonText = "Next";
                    break;
                case 4:
                    nOnboard.imageDescription = "Manage Settings Here.";
                    nOnboard.imageHeader = "Settings";
                    nOnboard.buttonText = "Finish";
                    break;
                    
                   
            }
            return nOnboard;
        }
    }
}
