using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class CommonQuestionTemplate : StackLayout
    {
        public CommonQuestionTemplate()
        {
            InitializeComponent();
        }

        public void SetQandA(ServerQandA qandA)
        {
            question.Text = "Q: "+ qandA.question;
            answer.Text = "A: "+ qandA.answer;
        }
    }
}
