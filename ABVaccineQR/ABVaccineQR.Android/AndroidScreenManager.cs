using Android.Views;
using Xamarin.Forms;
using ABVaccineQR.PlatformDependent;
using ABVaccineQR.Droid;

[assembly: Dependency(typeof(AndroidScreenManager))]
namespace ABVaccineQR.Droid
{
    public class AndroidScreenManager : IScreenManager
    {
        public void KeepScreenOn()
        {
            MainActivity.Instance.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}