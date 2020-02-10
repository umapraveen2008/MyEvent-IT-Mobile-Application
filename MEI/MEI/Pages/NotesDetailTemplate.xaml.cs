using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class NotesDetailTemplate : ContentView
    {

        public ServerNote currentNote = new ServerNote();
        public NotesTemplate parentNotes = new NotesTemplate();

        public NotesDetailTemplate()
        {
            InitializeComponent();
            noteEditor.Focus();
        }
        
        public async void SetNote()
        {
            await ((HomeLayout)App.Current.MainPage).SetLoading(true, "loading note...");
            tagObject.IsVisible = true;
            if (currentNote.userNoteTag != null)
            {
                switch (currentNote.userNoteTag.noteTag)
                {
                    case NoteTag.Exhibitor:
                        ServerCompany tagECompany = (await App.serverData.GetOneExhibitor(currentNote.userNoteTag.tagID)).company;
                        tagDescription.Text = tagDescription.Text + "exhibitor:";
                        if (tagECompany != null)
                        {
                            if (!string.IsNullOrEmpty(tagECompany.companyLogo))
                            {
                                tagImage.Source = tagECompany.companyLogo;
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagECompany.companyName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            else
                            {
                                tagImage.Source = "";
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagECompany.companyName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            tagName.Text = tagECompany.companyName;
                        }
                        else
                        {
                            logoGrid.IsVisible = false;
                            tagName.Text = "This exhibitor is removed from MyEvent it.";
                        }
                        break;
                    case NoteTag.Sponsor:
                        tagDescription.Text = tagDescription.Text + "sponsor:";
                        ServerCompany tagSCompany = (await App.serverData.GetOneSponsor(currentNote.userNoteTag.tagID)).company;
                        if (tagSCompany != null)
                        {
                            if (!string.IsNullOrEmpty(tagSCompany.companyLogo))
                            {
                                tagImage.Source = tagSCompany.companyLogo;
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagSCompany.companyName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            else
                            {
                                tagImage.Source = "";
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagSCompany.companyName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            tagName.Text = tagSCompany.companyName;
                        }
                        else
                        {
                            logoGrid.IsVisible = false;
                            tagName.Text = "This sponsor is removed from MyEvent it.";
                        }
                        break;
                    case NoteTag.Speaker:
                        tagDescription.Text = tagDescription.Text + "speaker:";
                        ServerSpeaker tagSpeaker = await App.serverData.GetOneSpeaker(currentNote.userNoteTag.tagID);
                        if (tagSpeaker != null)
                        {
                            if (!string.IsNullOrEmpty(tagSpeaker.speakerImage))
                            {
                                tagImage.Source = tagSpeaker.speakerImage;
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagSpeaker.speakerFirstName +" "+tagSpeaker.speakerLastName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            else
                            {
                                tagImage.Source = "";
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagSpeaker.speakerFirstName + " " + tagSpeaker.speakerLastName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            tagName.Text = tagSpeaker.speakerFirstName + " " + tagSpeaker.speakerLastName;
                        }
                        else
                        {
                            logoGrid.IsVisible = false;
                            tagName.Text = "This speaker is removed from MyEvent it.";
                        }
                        break;
                    case NoteTag.User:
                        tagDescription.Text = tagDescription.Text + "user:";
                        ServerUser tagUser = await App.serverData.GetUserWithID(currentNote.userNoteTag.tagID);
                        if (tagUser != null)
                        {
                            if (!string.IsNullOrEmpty(tagUser.userImage))
                            {
                                tagImage.Source = tagUser.userImage;
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagUser.userFirstName + " " + tagUser.userLastName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            else
                            {
                                tagImage.Source = "";
                                Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                                string init = initials.Replace(tagUser.userFirstName + " " + tagUser.userLastName, "$1");
                                if (init.Length > 3)
                                    init = init.Substring(0, 3);
                                logoText.Text = init.ToUpper();
                            }
                            tagName.Text = tagUser.userFirstName + " " + tagUser.userLastName;
                        }
                        else
                        {
                            logoGrid.IsVisible = false;
                            tagName.Text = "This user is removed from MyEvent it.";
                        }
                        break;
                    case NoteTag.Session:
                        tagDescription.Text = tagDescription.Text + "session:";
                        ServerSession tagSession = await App.serverData.GetOneSession(currentNote.userNoteTag.tagID);
                        if (tagSession != null)
                        {                            
                            tagImage.Source = "";
                            Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                            string init = initials.Replace(tagSession.sessionName, "$1");
                            if (init.Length > 3)
                                init = init.Substring(0, 3);
                            logoText.Text = init.ToUpper();                            
                            tagName.Text = tagSession.sessionName;
                        }
                        else
                        {
                            logoGrid.IsVisible = false;
                            tagName.Text = "This session is removed from MyEvent it.";
                        }
                        break;
                    case NoteTag.Note:
                        tagObject.IsVisible = false;
                        break;
                }
            }
            await Task.Delay(1000);
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "loading note...");
        }


        public void SetNote(ServerNote note, NotesTemplate _noteParent)
        {
            currentNote = note;            
            parentNotes = _noteParent;
            if (!string.IsNullOrEmpty(note.userNote))
            {
                noteEditor.Text = currentNote.userNote;
            }
            SetNote();
        }
        
        public void ResetFocus()
        {
            noteEditor.Unfocus();
        }

        public async void SaveNote(object sender,EventHandler e)
        {
            if (currentNote.userNote != noteEditor.Text)
            {
                currentNote.userNote = noteEditor.Text;
                for (int i = 0; i < App.serverData.mei_user.noteList.Count; i++)
                {
                    if (App.serverData.mei_user.noteList[i].noteID == currentNote.noteID)
                    {
                        var cTime = DateTime.Now;
                        currentNote.noteDateTime = cTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                        App.serverData.mei_user.noteList[i] = currentNote;
                        await  ((HomeLayout)App.Current.MainPage).SetLoading(true, "Saving note...");
                        await BaseFunctions.EditNoteInServer(currentNote);
                        await  ((HomeLayout)App.Current.MainPage).SetLoading(false, "Deleting note...");
                    }
                }
            }
            noteEditor.Unfocus();
            //if(parentNotes!=null)
            //parentNotes.SetDetails();
            e(this,null);
        }       

    }
}
