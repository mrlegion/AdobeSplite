using System.IO;
using System.Linq;
using System.Windows;
using PDFSplitter.ViewModels;

namespace PDFSplitter.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void DragEnterHandler(object sender, DragEventArgs e)
        {
            vm.IsDrop = true;
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void DragLeaveHandler(object sender, DragEventArgs e)
        {
            vm.IsDrop = false;
        }

        private void DropEventHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var dragFileList = ((DataObject)e.Data).GetFileDropList().Cast<string>().ToList();

                if (dragFileList.Any(s => s.EndsWith(".pdf") && File.Exists(s)))
                {
                    vm.BaseSettings(dragFileList.First());
                    return;
                }
            }

            MessageBox.Show("Select incorrect file! Please, select only PDF File", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            vm.IsDrop = false;
        }
    }
}
