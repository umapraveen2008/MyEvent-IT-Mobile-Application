using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MEI.Controls
{
    public class PinchZoomContainer:ContentView
    {
        private double startScale, currentScale, xOffset, yOffset, startX, startY;

        public PinchZoomContainer()
        {
            if (Device.RuntimePlatform != Device.Android)
            {
                var pinchGesture = new PinchGestureRecognizer();
                pinchGesture.PinchUpdated += OnPinchUpdated;
                GestureRecognizers.Add(pinchGesture);

                var panGesture = new PanGestureRecognizer();
                panGesture.PanUpdated += OnPanUpdated;
                GestureRecognizers.Add(panGesture);
            }
        }

        public void ResetScale()
        {
            Content.Scale = 1;
            Content.TranslationX = 0;
            Content.TranslationY = 0;
            startScale = 0;
            currentScale = 0;
            xOffset = 0;
            yOffset = 0;
            startX = 0;
            startY = 0;
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    startX = e.TotalX;
                    startY = e.TotalY;
                    Content.AnchorX = 0;
                    Content.AnchorY = 0;
                    break;

                case GestureStatus.Running:
                    var maxTranslationX = Content.Scale * Content.Width - Content.Width;
                    Content.TranslationX = Math.Min(0, Math.Max(-maxTranslationX, xOffset + e.TotalX - startX));

                    var maxTranslationY = Content.Scale * Content.Height - Content.Height;
                    Content.TranslationY = Math.Min(0, Math.Max(-maxTranslationY, yOffset + e.TotalY - startY));

                    break;

                case GestureStatus.Completed:
                    xOffset = Content.TranslationX;
                    yOffset = Content.TranslationY;

                    break;
            }
        }

        public void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    // Store the current scale factor applied to the wrapped user interface element,
                    // and zero the components for the center point of the translate transform.
                    startScale = Content.Scale;
                    Content.AnchorX = 0;
                    Content.AnchorY = 0;

                    break;

                case GestureStatus.Running:
                    // Calculate the scale factor to be applied.
                    currentScale += (e.Scale - 1) * startScale;
                    currentScale = Math.Max(1, currentScale);

                    // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                    // so get the X pixel coordinate.
                    double renderedX = Content.X + xOffset;
                    double deltaX = renderedX / Width;
                    double deltaWidth = Width / (Content.Width * startScale);
                    double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                    // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                    // so get the Y pixel coordinate.
                    double renderedY = Content.Y + yOffset;
                    double deltaY = renderedY / Height;
                    double deltaHeight = Height / (Content.Height * startScale);
                    double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                    // Calculate the transformed element pixel coordinates.
                    double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                    double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                    // Apply translation based on the change in origin.
                    Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (currentScale - 1)));
                    Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (currentScale - 1)));

                    // Apply scale factor.
                    Content.Scale = currentScale;

                    break;

                case GestureStatus.Completed:
                    // Store the translation delta's of the wrapped user interface element.
                    xOffset = Content.TranslationX;
                    yOffset = Content.TranslationY;

                    break;
            }
        }
    }

    public static class ClampExtension
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
