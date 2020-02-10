using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class NotesPage : ContentView
    {
        NoteViewModel s;

        public NotesPage()
        {
            InitializeComponent();
            CreateNotes();
            notesParent.ItemTemplate = new DataTemplate(typeof(NotesTemplate));
            notesParent.RowHeight = 80;
            notesParent.IsGroupingEnabled = true;
            newNoteButton.Clicked += (s,e) => { ((HomeLayout)App.Current.MainPage).CreateNewNote(this, null, null); };
        }

        public void CreateNotes()
        {
            //groups.Clear();            
            //speakerParent.Children.Clear();                  
            CreateList(App.serverData.mei_user.noteList);          
        }

        public DateTime GetTime(DateTime time)
        {
            return DateTime.Parse(time.ToString("hh:mm tt"));
        }

        public void CreateList(IList<ServerNote> notes)
        {
            try
            {
                if (notes.Count > 0)
                {
                    emptyList.IsVisible = false;
                    notesParent.IsVisible = true;
                    s = new NoteViewModel(notes, SetupList(notes));
                    notesParent.ItemsSource = s.notesGroup;                                                       
                }
                else
                {
                    emptyList.IsVisible = true;
                    notesParent.IsVisible =false;
                }
                
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine("Creating server note failed...");
            }
        }

        ObservableCollection<Grouping<string, ServerNote>> SetupList(IList<ServerNote> notes)
        {
            ObservableCollection<Grouping<string, ServerNote>> notesGrouped = new ObservableCollection<Grouping<string, ServerNote>>();
            try
            {
                var sorted = from note in notes
                             orderby GetTime(DateTime.ParseExact(note.noteDateTime, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat)).TimeOfDay descending
                             group note by DateTime.ParseExact(note.noteDateTime,"MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture.DateTimeFormat).ToString("MM/dd/yyyy") into noteGroup
                             select new Grouping<string, ServerNote>(noteGroup.Key, noteGroup);

                notesGrouped = new ObservableCollection<Grouping<string, ServerNote>>(sorted);
                
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine("Setting up server note list failed...");
            }
            return notesGrouped;
        }

        public class NoteViewModel
        {
            public IList<ServerNote> notes { get; set; }
            public ObservableCollection<Grouping<string, ServerNote>> notesGroup { get; set; }

            public NoteViewModel(IList<ServerNote> _notes, ObservableCollection<Grouping<string, ServerNote>> _notesGroup)
            {
                try
                {                    

                    notes = _notes;
                    notesGroup = new ObservableCollection<Grouping<string, ServerNote>>(_notesGroup.OrderByDescending(a => DateTime.ParseExact(a.Key, "MM/dd/yyyy", CultureInfo.CurrentCulture.DateTimeFormat)));
                }
                catch (Exception e)
                {
                    Debug.WriteLine("NoteViewModel loading failed...");
                }
            }

            
        }
               
    }
}
