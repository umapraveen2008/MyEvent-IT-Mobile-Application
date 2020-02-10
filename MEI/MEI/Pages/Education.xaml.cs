using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class Education : ContentView
    {
        int totaleducation = 5;

        const string image_name = "mei_education_0";
        List<string> information = new List<string>()
        {
            "Search for Organizations to Join.",
            "Verify You are Joining the Correct Organization.",
            "View all Event Items.",
            "View all Events Within the Organization.",
            "Manage Settings Here."

        };
        int current_education = 1;
        public Education()
        {
            InitializeComponent();
            SetCurrentEducationImage();
        }

        public void NextEducation(object sender,EventArgs e)
        {
            current_education++;
            if(current_education >= totaleducation)
            {
                current_education = totaleducation;
            }
            SetCurrentEducationImage();
        }

        public void PreviousEducation(object sender,EventArgs e)
        {
            current_education--;
            if(current_education <= 1)
            {
                current_education = 1;
            }
            SetCurrentEducationImage();
        }

        void SetCurrentEducationImage()
        {
            educationImage.Source =  image_name + current_education.ToString();
            next.IsVisible = current_education != 1;
            previous.IsVisible = current_education != totaleducation;
            info.Text = information[current_education - 1];
        }
    }
}
