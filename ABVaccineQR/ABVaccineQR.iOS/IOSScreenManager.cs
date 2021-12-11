using UIKit;
using Xamarin.Forms;
using ABVaccineQR.PlatformDependent;
using ABVaccineQR.iOS;

[assembly: Dependency(typeof(IOSScreenManager))]
namespace ABVaccineQR.iOS
{
    public class IOSScreenManager : IScreenManager
    {
        public void KeepScreenOn()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }
    }
}