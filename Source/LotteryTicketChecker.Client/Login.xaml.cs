namespace Client
{
    using System;
    using System.Threading;
    using System.Windows;

    using MaterialDesignThemes.Wpf;

    using Properties;

    public partial class Login
    {
        private readonly LotteryTicketChecker checker;

        public Login(LotteryTicketChecker checker)
        {
            this.checker = checker;
            this.InitializeComponent();
        }

        private async void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            this.tbResult.Visibility = Visibility.Collapsed;
            this.tbEmail.IsEnabled = false;
            this.passwordBox.IsEnabled = false;
            this.btnLogin.IsEnabled = false;

            try
            {
                var result = await this.checker.LoginAsync(this.tbEmail.Text, this.passwordBox.Password, CancellationToken.None);
                if (result)
                {
                    Settings.Default.Username = this.tbEmail.Text;
                    Settings.Default.Password = this.passwordBox.Password;
                    Settings.Default.Save();
                    
                    DialogHost.CloseDialogCommand.Execute(true, null);
                }
                else
                {
                    this.tbResult.Visibility = Visibility.Visible;
                    this.tbResult.Text = "Грешно име или парола";

                    this.tbEmail.IsEnabled = true;
                    this.passwordBox.IsEnabled = true;
                    this.btnLogin.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                this.tbResult.Visibility = Visibility.Visible;
                this.tbResult.Text = "Неуспешен опит за връзка";
            }
        }
    }
}
