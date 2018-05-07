namespace Client
{
    using System.Text;
    using System.Windows.Input;

    public class BarcodeScannerHandler
    {
        private readonly global::Client.MainWindow window;
        private readonly StringBuilder barcodeReader;
        private readonly KeyConverter keyConverter;
        private bool isScanning;

        public BarcodeScannerHandler(global::Client.MainWindow window)
        {
            this.window = window;
            this.barcodeReader = new StringBuilder();
            this.keyConverter = new KeyConverter();

            window.PreviewKeyDown += this.OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.F11 || e.Key == Key.F20) && !this.isScanning)
            {
                this.isScanning = true;
                e.Handled = true;
            }
            else if ((e.Key == Key.F12 || e.Key == Key.F22) && this.isScanning)
            {
                if (this.barcodeReader.Length != 0)
                {
                    this.window.HandleBarcode(this.barcodeReader.ToString().Trim('L'));
                    this.barcodeReader.Clear();
                }

                this.isScanning = false;
                e.Handled = true;
            }
            else if (this.isScanning)
            {
                e.Handled = true;
                var xChar = this.keyConverter.ConvertToString(e.Key);
                this.barcodeReader.Append(xChar);
            }
        }
    }
}