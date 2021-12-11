using Foundation;
using Xamarin.Forms;
using ABVaccineQR.PlatformDependent;
using ABVaccineQR.iOS;

[assembly: Dependency(typeof(IOSBaseUrl))]
namespace ABVaccineQR.iOS
{
    public class IOSBaseUrl : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}