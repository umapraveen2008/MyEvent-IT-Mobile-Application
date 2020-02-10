using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class EventHomePage : ContentView
    {
        HomeLayout home;
        public EventHomePage()
        {
            InitializeComponent();
            home = ((HomeLayout)App.Current.MainPage);
        }

        public void CreateScreen(object sender,EventArgs e)
        {
            home.SetSideMenuItem(int.Parse(((Button)sender).AutomationId));
        }        
    }
}
