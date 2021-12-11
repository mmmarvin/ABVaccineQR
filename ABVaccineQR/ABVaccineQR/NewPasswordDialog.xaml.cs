using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using ABVaccineQR.Custom;

namespace ABVaccineQR
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPasswordDialog : CustomModalFrame
    {
        public NewPasswordDialog()
        {
            InitializeComponent();
            Appearing += (s, e) =>
            {
                c_Password1Entry.Focus();
            };
        }

        public void Btn_OKClicked(object s, EventArgs e)
        {
            if(c_Password1Entry.Text != "" && c_Password1Entry.Text == c_Password2Entry.Text && c_Password1Entry.Text.Length > 1)
            {
                m_result.SetResult(c_Password1Entry.Text);
            }
            else
            {
                c_Password1Entry.Focus();
            }
        }

        public Task<string> WaitForResult()
        {
            m_result = new TaskCompletionSource<string>();
            return m_result.Task;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private TaskCompletionSource<string> m_result;
    }
}