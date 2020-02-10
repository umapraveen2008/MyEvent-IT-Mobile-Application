using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class NotesTemplate : ViewCell
    {
        public NotesTemplate()
        {
            View = new NotesTemplateView();
        }
        
        public string GetID()
        {
            return ((NotesTemplateView)View).id;
        }
        
        public void ShowDetails()
        {
            ((HomeLayout)App.Current.MainPage).CreateNoteDetail(this, null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(BindingContext != null)
                ((NotesTemplateView)View).SetDetails();
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ((NotesTemplateView)View).SetDetails();
        }

        protected override void OnTapped()
        {
            base.OnTapped();
            ShowDetails();
            ((ListView)this.Parent).SelectedItem = null;
        }
                
    }

    public partial class NotesTemplateView :ContentView
    {
        public string id;
        public ServerNote currentNote = new ServerNote();

        public NotesTemplateView()
        {
            InitializeComponent();
            TapGestureRecognizer t = new TapGestureRecognizer();
            t.Tapped += RemoveNote;
            noteDelete.GestureRecognizers.Add(t);
        }

        public void SetDetails()
        {
            if (BindingContext != null)
            {
                var note = BindingContext as ServerNote;
                id = note.noteID;
                currentNote = note;
                var dt = DateTime.ParseExact(note.noteDateTime, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat);
                notesTime.Text = dt.ToString("hh:mm tt");
                string s = "";
                if (!string.IsNullOrEmpty(currentNote.userNote))
                {
                    if (currentNote.userNote.ToCharArray().Length > 50)
                        s = currentNote.userNote.Substring(0, 50);
                    else
                        s = currentNote.userNote;
                }
                noteDescription.Text = s;
                switch(note.userNoteTag.noteTag)
                {
                    case NoteTag.Exhibitor:
                        noteTag.TextColor = Color.FromHex("#2bc33d");
                        break;
                    case NoteTag.Speaker:
                        noteTag.TextColor = Color.FromHex("#039dfd");
                        break;
                    case NoteTag.Sponsor:
                        noteTag.TextColor = Color.FromHex("#f59000");
                        break;
                    case NoteTag.User:
                        noteTag.TextColor = Color.FromHex("#eb346d");
                        break;
                    case NoteTag.Session:
                        noteTag.TextColor = Color.FromHex("#bc1eca");
                        break;                    
                }
                if (note.userNoteTag.noteTag != NoteTag.Note)
                {
                    noteTag.Text = "Tagged with " + note.userNoteTag.noteTag.ToString().ToLower();
                }
                else
                {
                    tagIcon.Source = "";
                    noteTag.Text = "";
                }
            }
        }
        public void RemoveNote(object sender, EventArgs e)
        {
            ((HomeLayout)App.Current.MainPage).RemoveNote(this, null, currentNote.noteID);
        }
    }
}
