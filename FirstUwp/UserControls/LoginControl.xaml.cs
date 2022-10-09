﻿using FirstUwp.Classes;
using FirstUwp.Helpers;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI;
using System.Drawing;
using Windows.UI.Xaml.Media;
using System.Threading;
using System.Threading.Tasks;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FirstUwp.UserControls
{
    public sealed partial class LoginControl : UserControl
    {
        /// <summary>
        /// Események a login kezeléséhez
        /// </summary>
        public event EventHandler LoginAccepted;
        public event EventHandler LoginCancelled;
        public event EventHandler LoginUserInteraction;

        string nfcId;
        int ledPinGreen = 18;
        int ledPinRed = 10;

        private GpioController gpioController = null;
        private NfcReader nfcReader = null;


        /// <summary>
        /// Azt állítja be, hogy a 'Mégsem" gomb alklamzható-e
        /// </summary>
        public bool CanCancel { get; set; } = false;

        private DispatcherTimer _timer = new DispatcherTimer();
        private DispatcherTimer RfidTimer = new DispatcherTimer();

        public LoginControl()
        {
            this.Loaded += LoginControl_Loaded;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;

            RfidTimer.Interval = TimeSpan.FromSeconds(1);
            RfidTimer.Tick += RfidTimer_Tick;

            this.InitializeComponent();

            Message.Text = "";

            this.CharacterReceived += LoginControl_CharacterReceived;
            this.PivotControl.SelectionChanged += PivotControl_SelectionChanged;

            int Index = -1;
            if (LocalSettingsHelper.Get("LoginIndex", ref Index))
            {
                PivotControl.SelectedIndex = 0;
            }
        }

        private void LoginControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                nfcReader = new NfcReader();
                gpioController = new GpioController();

                /*gpioController.OpenPin(ledPinGreen, PinMode.Output);
                gpioController.Write(ledPinGreen, PinValue.Low);

                gpioController.OpenPin(ledPinRed, PinMode.Output);
                gpioController.Write(ledPinRed, PinValue.Low);*/
            }
            catch (Exception en)
            {
                Debug.WriteLine(en.Message);
            }
        }

        private void _timer_Tick(object sender, object e)
        {
            _timer.Stop();

            switch (PivotControl.SelectedIndex)
            {
                case 0:
                    PinText.SelectAll();
                    PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    break;

                case 1:
                    RfidTimer.Start();
                    break;
            }

        }

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RfidTimer.Stop();
            _timer.Start();
            LocalSettingsHelper.Set("LoginIndex", PivotControl.SelectedIndex);
        }

        private void LoginControl_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            if (args.Character == 13) // Enter
            {
                switch (PivotControl.SelectedIndex)
                {
                    case 0:
                        PinOk_Click(sender, null);
                        break;
                    case 1:
                        break;
                }
            }
        }

        private void UserInteraction()
        {
            var eventHandler = this.LoginUserInteraction;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }
        
        private void Input(string inputString)
        {
            UserInteraction();
            PinText.Password += inputString;
        }

        private void Pin1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("1");
        }

        private void Pin2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("2");
        }

        private void Pin3_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("3");
        }

        private void Pin4_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("4");
        }

        private void Pin5_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("5");
        }

        private void Pin6_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("6");
        }

        private void Pin7_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("7");
        }

        private void Pin8_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("8");
        }

        private void Pin9_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("9");
        }

        private void Pin0_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("0");
        }

        private void PinBackSpace_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PinText.Password = "";
            PinText.SelectAll();
            PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }


        private async void PinOk_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UserInteraction();

            //gpioController.Write(ledPinGreen, PinValue.High);

            
            Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByCode(PinText.Password);
            
            if (Repository.Repository.LoggedInUser != null)
            {
                var eventHandler = this.LoginAccepted;
                if (eventHandler != null)
                {
                    eventHandler(this, EventArgs.Empty);
                }

                if (Repository.Repository.LoggedInUser.LoginId == 200 || Repository.Repository.LoggedInUser.LoginId == 201)
                {
                    Debug.WriteLine($"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó belépett!");
                    Message.Text = $"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó belépett!";
                    Message.Foreground = new SolidColorBrush(Colors.Green);
                    await Task.Delay(4000);
                    Message.Text = "";

                }
                else if (Repository.Repository.LoggedInUser.LoginId == 300 || Repository.Repository.LoggedInUser.LoginId == 301)
                {
                    Debug.WriteLine($"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó kilépett!");
                    Message.Text = $"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó kilépett!";
                    Message.Foreground = new SolidColorBrush(Colors.Green);
                    await Task.Delay(4000);
                    Message.Text = "";
                }
                else
                {
                    Debug.WriteLine($"A megadott kóddal nem lehet belépni!");
                    Message.Text = $"A megadott kóddal nem lehet belépni!";
                    Message.Foreground = new SolidColorBrush(Colors.Yellow);
                    await Task.Delay(4000);
                    Message.Text = "";
                }

                //gpioController.Write(ledPinGreen, PinValue.Low);
            }
            else
            {
                Message.Text = "Az adatbázis nem elérhető!";
                Message.Foreground = new SolidColorBrush(Colors.Red);
                PinText.SelectAll();
                PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }

        private async void RfidTimer_Tick(object sender, object e)
        {
            RfidTimer.Stop();
            try
            {
                nfcId = nfcReader.GetNfcId();

                if (!string.IsNullOrEmpty(nfcId))
                {

                    //gpioController.Write(ledPinGreen, PinValue.High);
                    //gpioController.Write(ledPinRed, PinValue.Low);

                    Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByNFC_Id(nfcId);


                    if (Repository.Repository.LoggedInUser != null)
                    {

                        var eventHandler = this.LoginAccepted;
                        if (eventHandler != null)
                        {
                            eventHandler(this, EventArgs.Empty);
                        }

                        if (Repository.Repository.LoggedInUser.LoginId == 200 || Repository.Repository.LoggedInUser.LoginId == 201)
                        {
                            Debug.WriteLine($"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó belépett!");
                            Message.Text = $"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó belépett!";
                            Message.Foreground = new SolidColorBrush(Colors.Green);
                            
                        }
                        else if (Repository.Repository.LoggedInUser.LoginId == 300 || Repository.Repository.LoggedInUser.LoginId == 301)
                        {
                            Debug.WriteLine($"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó kilépett!");
                            Message.Text = $"A(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó kilépett!";
                            Message.Foreground = new SolidColorBrush(Colors.Green);
                            
                        }
                        else if(Repository.Repository.LoggedInUser.IsActive == false)
                        {
                            Debug.WriteLine($"Ön jelenleg inaktív állapotban van!");
                            Message.Text = $"Ön jelenleg inaktív állapotban van!";
                            Message.Foreground = new SolidColorBrush(Colors.Yellow);
                            
                        }
                    }
                    else if(Repository.Repository.LoggedInUser.IsActive.Equals(false))
                    {
                        Debug.WriteLine("Az adatbázis nem elérhető!");
                        Message.Text = $"Az adatbázis nem elérhető!";
                        Message.Foreground = new SolidColorBrush(Colors.Red);
                        
                    }

                    //gpioController.Write(ledPinGreen, PinValue.Low);
                }
                //Debug miatt van csak!
                else
                {
                    Debug.WriteLine("Nincs beolvasott NFC kód!");
                    //gpioController.Write(ledPinRed, PinValue.High);
                }

                await Task.Delay(2000);
                Message.Text = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
               
                RfidTimer.Start();
            }
        }

    }
}