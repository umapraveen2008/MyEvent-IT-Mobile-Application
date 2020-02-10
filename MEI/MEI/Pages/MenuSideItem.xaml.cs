using MEI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class MenuSideItem : StackLayout
    {
        TapGestureRecognizer itemTapped = new TapGestureRecognizer();
        string buttonText = string.Empty;
        string buttonImageName = string.Empty;

        public static readonly BindableProperty ItemIconProperty = BindableProperty.Create(nameof(ItemIcon), typeof(string),typeof(MenuSideItem),"");

        public string ItemIcon
        {
            get { return (string)GetValue(ItemIconProperty); }
            set { SetValue(ItemIconProperty, value); }
        }

        public static readonly BindableProperty ItemIDProperty = BindableProperty.Create(nameof(ItemID), typeof(int), typeof(MenuSideItem), 0);

        public int ItemID
        {
            get { return (int)GetValue(ItemIDProperty); }
            set { SetValue(ItemIDProperty, value); }
        }

        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(MenuSideItem), Color.White);

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        EventHandler ClickEvent;


        public MenuSideItem()
        {
            InitializeComponent();            
            ResetButton();
        }
        
        public int GetItemID()
        {
            return menuId;
        }       

        public string GetItemName()
        {
            return buttonText;
        }

        public int menuId;
        public void SetMenuItem(EventHandler _clickEvent)
        {            
            buttonImage.Source = ItemIcon;            
            buttonImage.HeightRequest = 30;
            buttonImage.WidthRequest = 30;            
            menuId = ItemID;
            ClickEvent = _clickEvent;
        }

        public void OnSideButtonClick(object sender,EventArgs e)
        {
            ClickEvent?.Invoke(this, null);
        }

        public void DisableButton()
        {
            selectedButton.IsEnabled = false;
        }

        public void EnableButton()
        {
            selectedButton.IsEnabled = true;
        }

        public void SetButton()
        {         
            selectedButton.BackgroundColor = SelectedColor;            
            selectedButton.CornerRadius = Device.RuntimePlatform == Device.iOS ? 10 : 15;
            selected.BackgroundColor = SelectedColor;
        }

        public void ResetButton()
        {
            selectedButton.BackgroundColor = Color.FromHex("#505f6d");
            selectedButton.CornerRadius = Device.RuntimePlatform == Device.iOS ? 10 : 20;
            selected.BackgroundColor = Color.Transparent; 
        }
    }
}
