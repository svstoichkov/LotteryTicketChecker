namespace LotteryTicketChecker.Client
{
    using System;
    using System.Windows;

    using MaterialDesignThemes.Wpf;

    using Properties;

    public partial class MainWindow
    {
        private readonly Checker checker;

        public MainWindow()
        {
            this.InitializeComponent();
            this.checker = new Checker();

            if (string.IsNullOrWhiteSpace(Settings.Default.Username) || string.IsNullOrWhiteSpace(Settings.Default.Password))
            {
                this.dialogHost.DialogContent = new Login(this.checker);
                this.dialogHost.IsOpen = true;
            }
            else
            {
                this.progress.Visibility = Visibility.Visible;
                this.Login();
            }
        }

        private async void Login()
        {
            try
            {
                var result = await this.checker.Login(Settings.Default.Username, Settings.Default.Password);
                this.progress.Visibility = Visibility.Collapsed;

                if (result)
                {
                    this.tbResult.Text = "Успешен вход";
                    this.btnLogout.IsEnabled = true;
                    this.checker.Start();
                }
                else
                {
                    this.dialogHost.DialogContent = new Login(this.checker);
                    this.dialogHost.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                this.progress.Visibility = Visibility.Collapsed;
                this.tbResult.Text = "Неуспешен опит за връзка";
            }
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            this.tbResult.Text = string.Empty;
            this.dialogHost.DialogContent = new Login(this.checker);
            this.dialogHost.IsOpen = true;
        }

        public async void HandleBarcode(string barcode)
        {
            if (this.progress.Visibility != Visibility.Visible && !this.dialogHost.IsOpen)
            {
                this.tbResult.Text = string.Empty;
                this.progress.Visibility = Visibility.Visible;
                var result = await this.checker.Check(barcode);
                this.tbResult.Text = result;
                this.progress.Visibility = Visibility.Collapsed;

                this.SwatResultColorZoneMode();
            }
        }

        private void SwatResultColorZoneMode()
        {
            if (this.czResult.Mode == ColorZoneMode.PrimaryDark)
            {
                this.czResult.Mode = ColorZoneMode.PrimaryMid;
            }
            else
            {
                this.czResult.Mode = ColorZoneMode.PrimaryDark;
            }
        }
    }
}
