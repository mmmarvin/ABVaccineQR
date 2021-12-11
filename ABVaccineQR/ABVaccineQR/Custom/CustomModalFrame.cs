using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ABVaccineQR.Custom
{
    public class CustomModalFrame : CustomModal
    {
        public CustomModalFrame()
        {
            BackgroundColor = Color.White;

            m_layout = new StackLayout();
            m_frame = new Frame()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.White,
                CornerRadius = 0,
                Padding = new Thickness(15),
                Content = m_layout
            };

            Content = new StackLayout()
            {
                Padding = new Thickness(50, 100, 50, 100),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    m_frame
                }
            };

            Appearing += AppearingEvent;
            Disappearing += DisappearingEvent;
            Closing += DisappearingEvent;
        }

        public Frame Frame
        {
            get
            {
                return m_frame;
            }
        }

        public IList<View> Children
        {
            get
            {
                return m_layout.Children;
            }
        }

        public void AppearingEvent(object s, EventArgs e)
        {
            Animation fade_animation = new Animation(f => Opacity = f, 0.2, 1, Easing.SinInOut);
            fade_animation.Commit(m_frame, "frame_fade_animation", 100);
        }

        public void DisappearingEvent(object s, EventArgs e)
        {
            Animation fade_animation = new Animation(f => Opacity = f, 1, 0, Easing.SinInOut);
            fade_animation.Commit(m_frame, "frame_fade_animation", 100);
        }

        private StackLayout m_layout;
        private Frame       m_frame;
    }
}
