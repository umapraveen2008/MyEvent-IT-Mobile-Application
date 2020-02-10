using System;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class DayButtonTemplate : Button
    {
        public EventHandler onClick;
        public int buttonID;

        public DayButtonTemplate()
        {
            InitializeComponent();
        }

        public void SetButtonText(string _day, int _buttonID, EventHandler _clickEvent)
        {
            if (!string.IsNullOrEmpty(_day))
                this.Text = _day;
            buttonID = _buttonID;
            this.Clicked += _clickEvent;
        }

        public void SetButton()
        {
            this.BackgroundColor = Color.FromHex("#31c3ee");
            this.TextColor = Color.White;
        }

        public void ResetButton()
        {
            this.BackgroundColor = Color.White;
            this.TextColor = Color.FromHex("#31c3ee");
        }
    }
}
