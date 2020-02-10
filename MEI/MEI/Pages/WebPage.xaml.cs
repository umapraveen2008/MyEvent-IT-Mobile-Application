using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class WebPage : Grid
    {
        public WebPage()
        {
            InitializeComponent();
        }


        public void SetURL(string url)
        {
            webView.Source = url;
            webView.Navigated += (s, e) => {
                if(e.Result == WebNavigationResult.Success)
                {
                    loading.IsVisible = false;
                }
            };         
        }
    }
}
