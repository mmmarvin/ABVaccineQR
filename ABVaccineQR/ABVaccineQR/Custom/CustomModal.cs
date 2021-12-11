using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ABVaccineQR.Custom
{
    public class CustomModal : ContentPage
    {
        public CustomModal()
        {
        }

        public async void Close()
        {
            var ev = new EventArgs();
            foreach(var e in m_events)
            {
                e(this, ev);
            }

            await Navigation.PopModalAsync(false);
        }

        protected override bool OnBackButtonPressed()
        {
            Close();
            return true;
        }

        public event EventHandler Closing
        {
            add
            {
                m_events.Add(value);
            }

            remove
            {
                m_events.Remove(value);
            }
        }

        private List<EventHandler> m_events = new List<EventHandler>();
    }
}
