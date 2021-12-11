using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ABVaccineQR.Renderer;

namespace ABVaccineQR
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NoAnimationNavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
