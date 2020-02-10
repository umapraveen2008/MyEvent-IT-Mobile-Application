using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MEI.Pages;
using MEI.Droid.CustomRenderers;
using MEI.Controls;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.Graphics;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(CustomWebViewRenderer))]

namespace MEI.Droid.CustomRenderers
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        Context _context;
        public CustomWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control != null)
            {
                Control.Settings.BuiltInZoomControls = true;
                Control.Settings.DisplayZoomControls = true;
            }
            base.OnElementPropertyChanged(sender, e);
        }

    }
}