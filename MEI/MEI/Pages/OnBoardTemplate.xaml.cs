using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class OnBoardTemplate : ContentPage
    {
        public int id;
        public OnBoardTemplate()
        {
            InitializeComponent();
        }

        public void SetPage(OnBoardScreen.OnBoard onboard,EventHandler ev,int m_id)
        {
            id = m_id;
            pageImage.Source = onboard.imageSource;
            Description.Text = onboard.imageDescription;
            nextButton.Text = onboard.buttonText;
            headerText.Text = onboard.imageHeader;
            nextButton.Clicked += ev;
        }
    }
}
