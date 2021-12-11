using Android.Content;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ABVaccineQR.Renderer;
using ABVaccineQR.Droid.Renderer;

[assembly: ExportRenderer(typeof(NoAnimationNavigationPage), typeof(NoAnimationNavigationRenderer))]
namespace ABVaccineQR.Droid.Renderer
{
    public class NoAnimationNavigationRenderer : NavigationRenderer
    {
        public NoAnimationNavigationRenderer(Context context) : base(context)
        {
        }

        protected override Task<bool> OnPushAsync(Page page, bool animated)
        {
            return base.OnPushAsync(page, false);
        }

        protected override Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            return base.OnPopViewAsync(page, false);
        }
    }
}