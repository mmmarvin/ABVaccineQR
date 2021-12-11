using System;
using System.Threading.Tasks;
using System.Text;
using Xamarin.Forms;

namespace ABVaccineQR
{
    public static class DialogFactory
    {
        public async static Task<string> DisplayNewPasswordPromptAsync()
        {
            var custom_prompt = new NewPasswordDialog();
            await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
            var res = await custom_prompt.WaitForResult();
            custom_prompt.Close();

            return res;
        }

        public async static Task<string> DisplayPasswordPromptAsync()
        {
            var custom_prompt = new PasswordDialog();
            await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
            var res = await custom_prompt.WaitForResult();
            custom_prompt.Close();

            return res;
        }
    }
}
