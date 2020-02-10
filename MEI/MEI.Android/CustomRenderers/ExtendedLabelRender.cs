using System.ComponentModel;
using Xamarin.Forms;
using MEI.Droid.CustomRenderers;
using Android.Text;
using System;
using MEI.Controls;
using Android.App;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics;
using MEI.Pages;
using MEI;
using Android.Content;
using Android.Views;

[assembly: ExportRenderer(typeof(Label), typeof(ExtendedLabelRender))]
[assembly: ExportRenderer(typeof(Button), typeof(ExtendedButtonRender))]
[assembly: ExportRenderer(typeof(GradientPage), typeof(GradientContentPageRenderer))]
//[assembly: ExportRenderer(typeof(TabbedPage),typeof(CustomTabRenderer))]
[assembly: ExportRenderer(typeof(Picker), typeof(CustomPickerRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.ProgressBar), typeof(CustomProgressBarRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
[assembly: ExportRenderer(typeof(Switch), typeof(CustomSwitchRenderer))]

namespace MEI.Droid.CustomRenderers
{ 
    public class CustomSwitchRenderer : SwitchRenderer
    {
        Context _context;
        public CustomSwitchRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);
            Control.TrackDrawable.SetColorFilter(Xamarin.Forms.Color.FromHex("#bfc9d3").ToAndroid(), PorterDuff.Mode.SrcAtop);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control.Checked)
                Control.ThumbDrawable.SetColorFilter(Xamarin.Forms.Color.FromHex("#31c3ee").ToAndroid(), PorterDuff.Mode.SrcAtop);
            else
                Control.ThumbDrawable.SetColorFilter(Xamarin.Forms.Color.FromHex("#505f6d").ToAndroid(), PorterDuff.Mode.SrcAtop);
        }

    }

    public class CustomEntryRenderer : EntryRenderer
    {
        Context _context;
        public CustomEntryRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.Gravity = Android.Views.GravityFlags.CenterVertical;
        }
    }

    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        Context _context;
        public CustomProgressBarRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);
            Control.ProgressDrawable.SetColorFilter(Xamarin.Forms.Color.White.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);
        }
    }

    public class CustomPickerRenderer : ViewRenderer<Picker, Android.Widget.Spinner>
    {
        Picker picker;
        Context _context;
        public CustomPickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
            //e.NewElement.SelectedIndex = e.OldElement.SelectedIndex;
            picker = e.NewElement;
            picker.SelectedIndex = e.NewElement.SelectedIndex;
            IList<string> scaleNames = e.NewElement.Items;
            Android.Widget.Spinner spinner = new Android.Widget.Spinner(this.Context);
            spinner.ItemSelected += new EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var scaleAdapter = new Android.Widget.ArrayAdapter<string>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, scaleNames);
            spinner.SetMinimumHeight((int)Element.Height);
            scaleAdapter.SetDropDownViewResource(Resource.Layout.spinner_custom_item);
            spinner.Adapter = scaleAdapter;
            spinner.SetSelection(e.NewElement.SelectedIndex);
            spinner.SetMinimumHeight((int)picker.Height);
            base.SetNativeControl(spinner);
        }

        private void spinner_ItemSelected(object sender, Android.Widget.AdapterView.ItemSelectedEventArgs e)
        {
            picker.SelectedIndex = (e.Position);
        }
    }

    public class ExtendedLabelRender : LabelRenderer
    {
        Context _context;

        public ExtendedLabelRender(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var view = Element as Label;
            var control = Control;
            if (control.Text.Contains("</") || control.Text.Contains("&nbsp") || control.Text.Contains("<br>"))
            {
                control.TextFormatted = Html.FromHtml(control.Text,FromHtmlOptions.ModeLegacy);
                control.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
            }
        }



        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);

            var view = Element as Label;
            var control = Control;

            if (control.Text.Contains("</") || control.Text.Contains("&nbsp") || control.Text.Contains("<br>"))
            {
                control.TextFormatted = Html.FromHtml(control.Text,FromHtmlOptions.ModeLegacy);
                control.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
            }
        }

    }

    public class ExtendedButtonRender : ButtonRenderer
    {
        Context _context;
        public ExtendedButtonRender(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var view = Element as Button;
            var control = Control;
            if (control.Text.Contains("</") || control.Text.Contains("&nbsp") || control.Text.Contains("<br>"))
            {
                if (!control.Text.Contains("<b>"))
                {
                    control.Text = "<b>" + control.Text + "</b>";
                }
                control.TextFormatted = Html.FromHtml(control.Text,FromHtmlOptions.ModeLegacy);
            }
        }


        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            var view = Element as Button;
            var control = Control;

            if (control.Text.Contains("</") || control.Text.Contains("&nbsp") || control.Text.Contains("<br>"))
            {
                control.TextFormatted = Html.FromHtml(control.Text,FromHtmlOptions.ModeLegacy);
            }
        }

    }

    public class GradientContentPageRenderer : PageRenderer
    {
        Context _context;
        public GradientContentPageRenderer(Context context) : base(context)
        {
            _context = context;
        }

        //protected override void OnElementChanged(VisualElementChangedEventArgs e)
        //{
        //    base.OnElementChanged(e);

        //    if (e.OldElement == null) // perform initial setup
        //    {
        //        var page = e.NewElement as Page;
        //        var gradientLayer = new CAGradientLayer();
        //        gradientLayer.Frame = View.Bounds;
        //        gradientLayer.Colors = new CGColor[] { page.StartColor.ToCGColor(), page.EndColor.ToCGColor() };
        //        View.Layer.InsertSublayer(gradientLayer, 0);
        //    }
        //}
        private Xamarin.Forms.Color StartColor { get; set; }
        private Xamarin.Forms.Color EndColor { get; set; }
        protected override void DispatchDraw(
           global::Android.Graphics.Canvas canvas)
        {
            var gradient = new Android.Graphics.LinearGradient(0, 0, 0, Height,
                this.StartColor.ToAndroid(),
                this.EndColor.ToAndroid(),
                Android.Graphics.Shader.TileMode.Mirror);
            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };
            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                var page = e.NewElement as GradientPage;
                this.StartColor = Xamarin.Forms.Color.FromRgba(49, 238, 218, 255);
                this.EndColor = Xamarin.Forms.Color.FromRgba(49, 195, 238, 255);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"          ERROR: ", ex.Message);
            }
        }
    }

    //public class CustomTabRenderer : TabbedRenderer
    //{
    //    private Dictionary<Int32, Int32> icons = new Dictionary<Int32, Int32>();
    //    protected override void DispatchDraw(Android.Graphics.Canvas canvas)
    //    {
    //        getIcons();
    //        setIcons();
    //        base.DispatchDraw(canvas);
    //    }
    //    private void getIcons()
    //    {
    //        if (Element == null)
    //            return;
    //        int id = 0;
    //        foreach (var t in Element.Children)
    //        {
    //            if (icons.ContainsKey(id))
    //                continue;
    //            if (t.Icon != "" && t.Icon != null)
    //            {
    //                icons.Add(id, ResourceIdFromString(t.Icon));
    //            }
    //            id++;
    //        }
    //    }
    //    private int ResourceIdFromString(String name)
    //    {
    //        name = name.ToLower()
    //            .Replace(".png", "")
    //            .Replace(".jpg", "")
    //            .Replace(".jpeg", "")
    //            .Replace(".gif", "")
    //            .Replace(".ico", "");
    //        Type type = typeof(Android.Resource.Drawable);
    //        foreach (var p in type.GetFields())
    //        {
    //            if (p.Name.ToLower() == name)
    //                return (int)p.GetValue(null);
    //        }
    //        return 0;
    //    }


    //    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    //    {                        
    //        base.OnLayout(changed, l, t, r, b);        

    //    }


    //    private void setIcons()
    //    {
    //        var activity = this.Context as Activity;
    //        if (activity != null && activity.ActionBar != null)
    //        {
    //            for (int i = 0; i < activity.ActionBar.TabCount; i++)
    //            {
    //                if (!icons.ContainsKey(i))
    //                    continue;
    //                ActionBar.Tab tab = activity.ActionBar.GetTabAt(i);
    //                if (tab.Icon == null)
    //                {
    //                    tab.SetIcon(icons[i]);
    //                }
    //            }
    //        }
    //    }
    //}

}