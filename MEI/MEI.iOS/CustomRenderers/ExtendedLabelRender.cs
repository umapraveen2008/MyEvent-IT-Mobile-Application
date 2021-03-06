﻿using CoreGraphics;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreAnimation;
using MEI.iOS.CustomRenderers;
using MEI.Controls;

[assembly: ExportRenderer(typeof(Label), typeof(AwesomeHyperLinkLabelRenderer))]
[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebViewRenderer))]
//[assembly: ExportRenderer(typeof(ViewCell), typeof(CustomViewCellRenderer))]
[assembly: ExportRenderer(typeof(ListView), typeof(CustomListRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.ProgressBar), typeof(CustomProgressBarRenderer))]
[assembly: ExportRenderer(typeof(GradientPage), typeof(GradientContentPageRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntry))]
//[assembly: ExportRenderer(typeof(ScrollView), typeof(CustomScrollViewRenderer))]
namespace MEI.iOS.CustomRenderers
{
    public class CustomScrollViewRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            //DelaysContentTouches = false;
        }
    }

    public class CustomEntry : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.TintColor = Color.White.ToUIColor();
            //Control.BorderStyle = UITextBorderStyle.RoundedRect;
            Control.Layer.BorderWidth = 1f;
            Control.Layer.BorderColor = Color.White.ToCGColor();
            if (Element != null)
            {
                if (Element.BackgroundColor != null)
                    Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
                else
                    Control.BackgroundColor = Color.Transparent.ToUIColor();
            }
            else
            {
                Control.BackgroundColor = Color.Transparent.ToUIColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element != null)
            {
                if (Element.BackgroundColor != null)
                    Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
                else
                    Control.BackgroundColor = Color.Transparent.ToUIColor();
            }
            else
            {
                Control.BackgroundColor = Color.Transparent.ToUIColor();
            }
        }
    }

    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);
            Control.ProgressTintColor = Color.White.ToUIColor();
        }
    }


    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer()
        {

        }
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            var view = (UIWebView)NativeView;
            view.ScrollView.ScrollEnabled = true;
            view.ScalesPageToFit = true;
        }

    }

    public class GradientContentPageRenderer : PageRenderer
    {
        public CAGradientLayer gradient;
        public GradientContentPageRenderer() : base()
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            gradient = new CAGradientLayer();
            gradient.Frame = View.Bounds;
            gradient.NeedsDisplayOnBoundsChange = true;
            gradient.MasksToBounds = true;
            gradient.Colors = new CGColor[] { Xamarin.Forms.Color.FromRgba(49, 238, 218, 255).ToCGColor(), Xamarin.Forms.Color.FromRgba(49, 195, 238, 255).ToCGColor()
            };
            View.Layer.InsertSublayer(gradient, 0);

        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            gradient.Frame = View.Bounds;
        }
    }

    public class CustomListRenderer : ListViewRenderer
    {
        public CustomListRenderer()
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            Control.AllowsSelection = false;
            Control.UserInteractionEnabled = true;
        }
    }


    public class CustomViewCellRenderer : ViewCellRenderer
    {
        public CustomViewCellRenderer()
        {
            
        }
                
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {

            if (reusableCell != null)
            {
                reusableCell.PrepareForReuse();
                reusableCell.UserInteractionEnabled = true;
            }
            return base.GetCell(item, reusableCell, tv);
        }


    }



    public class AwesomeHyperLinkLabelRenderer : ViewRenderer
    {
        public AwesomeHyperLinkLabelRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            var view = (Label)Element;
            if (view == null) return;
            view.VerticalOptions = LayoutOptions.Center;
            UITextView uiLabel = new UITextView(new CGRect(0, 0, view.Width, view.Height));
            SetNativeControl(uiLabel);
            switch (view.HorizontalTextAlignment)
            {
                case TextAlignment.Center:
                    uiLabel.TextAlignment = UITextAlignment.Center;
                    break;
                case TextAlignment.Start:
                    uiLabel.TextAlignment = UITextAlignment.Left;
                    break;
                case TextAlignment.End:
                    uiLabel.TextAlignment = UITextAlignment.Right;
                    break;

            }
            uiLabel.TextContainer.LineFragmentPadding = 0;
            uiLabel.TextContainerInset = UIEdgeInsets.Zero;
            uiLabel.ShouldInteractWithUrl += (tView, req, type) =>
            {
                try
                {
                    UIApplication.SharedApplication.OpenUrl(req);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return true;
            };
            uiLabel.BackgroundColor = view.BackgroundColor.ToUIColor();

            uiLabel.ClearsContextBeforeDrawing = true;
            uiLabel.ClearsOnInsertion = true;
            uiLabel.AutosizesSubviews = true;
            uiLabel.TextColor = view.TextColor.ToUIColor();
            uiLabel.Font = UIFont.SystemFontOfSize((float)view.FontSize);
            uiLabel.DataDetectorTypes = UIDataDetectorType.Link;
            uiLabel.Editable = false;
            uiLabel.UserInteractionEnabled = true;
            uiLabel.NeedsUpdateConstraints();
            uiLabel.SetNeedsLayout();
            if (view != null)
                SetUIText(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            var view = (Label)Element;
            if (view == null) return;

            if (Control != null)
            {
                ((UITextView)Control).ClearsContextBeforeDrawing = true;
                float topCorrect = (float)(((UITextView)Control).Bounds.Height - ((UITextView)Control).ContentSize.Height);

                topCorrect = topCorrect / 2;
                //topCorrect = topCorrect < 0.0 ? (float) 0.0 : topCorrect;

                if (((UITextView)Control).ContentSize.Height < ((UITextView)Control).Bounds.Height)
                {
                    ((UITextView)Control).SetContentOffset(new CGPoint(0, -topCorrect), true);
                }
            }
            if (view != null)
                SetUIText(view);
        }


        public void SetUIText(Label view)
        {
            if (view != null)
            {
                if (Control != null && view.Text != null)
                {
                    ((UITextView)Control).ClearsContextBeforeDrawing = true;
                    if (view.Text.Contains("</") || view.Text.Contains("&nbsp") || view.Text.Contains("<br>"))
                    {
                        NSAttributedStringDocumentAttributes attr = new NSAttributedStringDocumentAttributes();
                        var nsError = new NSError();
                        attr.DocumentType = NSDocumentType.HTML;


                        string text = "<div style = 'font-size:15px; font-family: Lato'>" + view.Text + "</div>";
                        var myHtmlData = NSData.FromString(text, NSStringEncoding.Unicode);
                        NSAttributedString htmlText = new NSAttributedString(myHtmlData, attr, ref nsError);
                        ((UITextView)Control).AttributedText = htmlText;
                        ((UITextView)Control).Font = view.Font.ToUIFont();
                        ((UITextView)Control).TextColor = view.TextColor.ToUIColor();
                        ((UITextView)Control).UserInteractionEnabled = true;
                        ((UITextView)Control).ScrollEnabled = true;
                        ((UITextView)Control).ScrollEnabled = false;
                    }
                    else
                    {
                        ((UITextView)Control).Text = view.Text;
                        ((UITextView)Control).ScrollEnabled = true;
                        ((UITextView)Control).ScrollEnabled = false;
                        ((UITextView)Control).UserInteractionEnabled = false;
                    }

                    ((UITextView)Control).NeedsUpdateConstraints();
                    ((UITextView)Control).SetNeedsLayout();
                }
            }
        }
    }

    public class ExtendedLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = e.NewElement as Label;
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = Element as Label;
        }

        private void UpdateUi(Label view)
        {
            //var underline = view.IsUnderline ? NSUnderlineStyle.Single : NSUnderlineStyle.None;



            if (view.TextColor != Color.Default)
            {
                this.Control.TextColor = view.TextColor.ToUIColor();
            }

            switch (view.HorizontalTextAlignment)
            {
                case TextAlignment.Center:
                    this.Control.TextAlignment = UITextAlignment.Center;
                    break;
                case TextAlignment.End:
                    this.Control.TextAlignment = UITextAlignment.Right;
                    break;
                case TextAlignment.Start:
                    this.Control.TextAlignment = UITextAlignment.Left;
                    break;
            }

            //switch (view.YAlign)
            //{
            //    case TextAlignment.Start:
            //        break;
            //    case TextAlignment.End:
            //        break;
            //    case TextAlignment.Center:
            //        break;
            //}

            //this.Control.AttributedText = new NSMutableAttributedString(view.Text,
            //    this.Control.Font,
            //    underlineStyle: underline,
            //    strikethroughStyle: strikethrough,
            //    shadow: dropShadow);
            //;

            NSAttributedStringDocumentAttributes attr = new NSAttributedStringDocumentAttributes();
            var nsError = new NSError();
            attr.DocumentType = NSDocumentType.HTML;


            if (Control.Text.Contains("</") || Control.Text.Contains("&nbsp") || Control.Text.Contains("<br>"))
            {
                Control.UserInteractionEnabled = true;
                string text = "<div style = 'font-size:15px; font-family: Opensans' >" + view.Text + "</div>";
                var myHtmlData = NSData.FromString(text, NSStringEncoding.Unicode);
                NSAttributedString htmlText = new NSAttributedString(myHtmlData, attr, ref nsError);
                Control.AttributedText = htmlText;
                //UITextView uilabel = new UITextView(new CGRect(0, 0, view.Width, view.Height));
                //uilabel.AttributedText = htmlText;
                //uilabel.Editable = false;
                //uilabel.DataDetectorTypes = UIDataDetectorType.All;
                //SetNativeControl(uilabel);
            }
        }
    }
}
