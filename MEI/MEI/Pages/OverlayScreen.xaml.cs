using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class OverlayScreen : ContentView
    {
        View child =new ContentView();
        public OverlayScreen()
        {
            InitializeComponent();
            ((HomeLayout)App.Current.MainPage).OnAppear();
            TapGestureRecognizer onBackTap = new TapGestureRecognizer();
            onBackTap.Tapped += OnBack;
            backButton.GestureRecognizers.Add(onBackTap);
            this.BindingContext = App.AppCart;
        }

        public void ChangeBackIcon(string icon,Thickness x)
        {
            backButton.Padding = x;
            backiconImage.Source = icon;
        }


        public async void OnBack(object sender,EventArgs e)
        {
            try
            {
                if (child != null)
                {
                    if (child.GetType() == typeof(NotesDetailTemplate))
                    {
                        ((NotesDetailTemplate)child).ResetFocus();
                        if (string.IsNullOrEmpty(((NotesDetailTemplate)child).currentNote.userNote))
                        {
                            var k = await App.Current.MainPage.DisplayAlert("Alert", "Empty Note, this note will be deleted", "Ok", "Cancel");
                            if (k)
                            {
                                for (int i = 0; i < App.serverData.mei_user.noteList.Count; i++)
                                {
                                    if (App.serverData.mei_user.noteList[i].noteID == ((NotesDetailTemplate)child).currentNote.noteID)
                                    {
                                        App.serverData.mei_user.noteList.RemoveAt(i);
                                    }
                                }
                                await ((HomeLayout)App.Current.MainPage).SetLoading(true, "Deleting note...");
                                await BaseFunctions.DeletNoteFromServer(((NotesDetailTemplate)child).currentNote);
                                await ((HomeLayout)App.Current.MainPage).SetLoading(false, " note...");
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    if (child.GetType() == typeof(FeedbackPage))
                        ((FeedbackPage)child).ResetFocus();
                    if (this.Parent.GetType() == typeof(RelativeLayout))
                    {
                        ((RelativeLayout)this.Parent).Children.Remove(this);
                    }
                    ((HomeLayout)App.Current.MainPage).ResetCurrentPage();
                }
            }
            catch(Exception ex)
            {
              //  await App.Current.MainPage.DisplayAlert("Crash", ex.ToString(), "OK");
            }
        }

        public void SetScreen(View childLayout,string heading,bool showCart = true)
        {
            child = childLayout;
            titleHeader.Text = heading;            
            cartButton.IsVisible = showCart;            
            ParentLayout.Children.Add(childLayout);
        }

        public void CreateCartPage(object sender,EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).CreateCartPage(this, null);
        }

        public void TitleRightBar(string image,EventHandler tapEvent)
        {
            titleRightButton.IsVisible = true;            
            titleRightImage.Source = image;
            TapGestureRecognizer rightTap = new TapGestureRecognizer();
            rightTap.Tapped += tapEvent;
            titleRightButton.GestureRecognizers.Add(rightTap);

        }
    }
}
