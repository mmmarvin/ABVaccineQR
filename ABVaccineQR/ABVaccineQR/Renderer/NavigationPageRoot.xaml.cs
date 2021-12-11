using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ABVaccineQR.Renderer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageRoot : ContentPage
    {
        public NavigationPageRoot(ContentPage content)
        {
            InitializeComponent();

            c_Content.Children.Add(content.Content);
            if(content.Title != null)
            {
                c_TitleLabel.Text = content.Title;
            }
        }

        private async void Btn_BackClicked(object sender, System.EventArgs e)
        {
            await ((NoAnimationNavigationPage)Application.Current.MainPage).PopAsync();
        }
    }
}