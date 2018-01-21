using System.Windows;
using Microsoft.Win32;

namespace SpliteThisFuckingPDF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _path = null;
        private Splite _splite;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            SpliteButton.Content = "Splite PDF";

            var dialog = new OpenFileDialog()
            {
                Filter = "Pdf document (.pdf)|*.pdf",
            };
            if (dialog.ShowDialog() == true)
                _path = dialog.FileName;

            PathDirectory.Text = _path ?? "Invalid file";

            if (string.IsNullOrEmpty(_path))
                MessageBox.Show("Not select PDF file!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

            _splite = new Splite(_path);

            if (_splite.IsGenerate)
                BrowseButton.Content = "OK!";
        }

        private void SpliteButton_Click(object sender, RoutedEventArgs e)
        {
            _splite.SpliteDocument();
            BrowseButton.Content = "Browse";
            SpliteButton.Content = "OK!";
        }
    }
}
