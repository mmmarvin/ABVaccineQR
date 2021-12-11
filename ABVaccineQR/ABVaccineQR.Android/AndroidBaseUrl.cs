using Xamarin.Forms;
using ABVaccineQR.PlatformDependent;
using ABVaccineQR.Droid;

[assembly: Dependency(typeof(AndroidBaseUrl))]
namespace ABVaccineQR.Droid
{
    public class AndroidBaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}