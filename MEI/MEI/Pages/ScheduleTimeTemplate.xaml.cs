using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class ScheduleTimeTemplate : ContentView
    {
        public string scheduleHeader;

        public ScheduleTimeTemplate()
        {
            InitializeComponent();
        }

        public void SetHeader(string _header)
        {
            scheduleHeader = _header;
            if (!string.IsNullOrEmpty(_header))
                time.Text = _header;
        }

        //public void AddSession(Session session,bool isDuration)
        //{
        //    ScheduleItem sItem = new ScheduleItem();
        //    sItem.SetEventITem(session,isDuration);
        //    sItem.SetSessionClick(((HomeLayout)App.Current.MainPage).CreateSessionDetail);
        //    scheduleParent.Children.Add(sItem);
        //}
    }
}
