using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using ABVaccineQR.PlatformDependent;
using ABVaccineQR.Renderer;

namespace ABVaccineQR
{
    public partial class MainPage : ContentPage
    {
        class DebugData
        {
            public string Text { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            c_AboutButton.Source = ImageSource.FromResource("ABVaccineQR.Resources.help_icon.png");
            Appearing += OnFocus;

            
        }

        public void Close()
        {
            Process.GetCurrentProcess().Kill();
        }

        public async void OnFocus(object s, EventArgs e)
        {
            var smallest_dimension = Math.Min(DeviceDisplay.MainDisplayInfo.Width, DeviceDisplay.MainDisplayInfo.Height);
            var qr_width = smallest_dimension - 20;
            c_QRImage.WidthRequest = qr_width;
            c_QRImage.HeightRequest = qr_width;
            c_QRImage.BarcodeOptions.Width = (int)qr_width;
            c_QRImage.BarcodeOptions.Height = (int)qr_width;

            if(m_firstRun)
            {
                m_firstRun = false;

                var res = Global.QRManager.Load();
                if (res == QRManager.ELoadResult.FileDoesNotExist || res == QRManager.ELoadResult.InvalidFile)
                {
                    c_ButtonLayout.IsVisible = true;
                }
                else if (res == QRManager.ELoadResult.NeedPassword)
                {
                    bool load_success = false;
                    int i = 0;
                    while (i < 3)
                    {
                        var password = await DialogFactory.DisplayPasswordPromptAsync();
                        Global.QRManager.SetPassword(password);
                        //while (!password_set)
                        //{
                        //    await DisplayAlert("Error", "Cannot set password, please try again", "OK");

                        //    password = await DialogFactory.DisplayPasswordPromptAsync();
                        //    password_set = Global.QRManager.SetPassword(password);
                        //}

                        res = Global.QRManager.Load();
                        if (res == QRManager.ELoadResult.Success)
                        {
                            load_success = true;
                            break;
                        }
                        else if (res == QRManager.ELoadResult.InvalidPassword)
                        {
                            await DisplayAlert("Error", "Invalid master password!", "OK");
                            ++i;
                        }
                        else
                        {
                            await DisplayAlert("Error", "An unknown error has occured. Exiting...", "OK");
                            Close();
                            return;
                        }
                    }

                    if (!load_success)
                    {
                        await DisplayAlert("Error", "Exceeded maximum tries", "OK");
                        Close();
                        return;
                    }
                    else
                    {
                        LoadQR();
                        c_ImageLayout.IsVisible = true;
                    }
                }
                else if (res == QRManager.ELoadResult.Success)
                {
                    LoadQR();
                    c_ImageLayout.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Error", "An unknown error has occured.", "OK");
                }
            }

            DependencyService.Get<IScreenManager>().KeepScreenOn();
        }

        public async void Btn_ScanVaccineQR(object s, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            //var result = new DebugData() { Text = "https://www.google.com/" };
            if (result != null)
            {
                var res = await DisplayAlert("Confirmation", "Do you want to encrypt your QR code?", "Yes", "No");
                if(res)
                {
                    var password = await DialogFactory.DisplayNewPasswordPromptAsync();
                    Global.QRManager.SetPassword(password);
                    //while (!password_set)
                    //{
                    //    await DisplayAlert("Error", "Cannot set password, please try again", "OK");

                    //    password = await DialogFactory.DisplayNewPasswordPromptAsync();
                    //    password_set = Global.QRManager.SetPassword(password);
                    //}
                    Global.QRManager.QR = result.Text;
                    Global.QRManager.Save();
                }
                else
                {
                    Global.QRManager.QR = result.Text;
                    Global.QRManager.Save();
                }
                
                LoadQR();
                c_ButtonLayout.IsVisible = false;
                c_ImageLayout.IsVisible = true;
            }
        }

        public async void Btn_AboutClicked(object s, EventArgs e)
        {
            await ((NoAnimationNavigationPage)Application.Current.MainPage).PushAsync(new About());
        }

        private void LoadQR()
        {
            c_QRImage.BarcodeValue = Global.QRManager.QR;
            //c_QRLabel.Text = Global.QRManager.QR;
        }

        private bool m_firstRun = true;
    }
}
