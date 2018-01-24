using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace SpliteThisFuckingPDF
{
    // TODO: Check memory resource
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Path to select PDF file
        /// </summary>
        private string _path = null;

        /// <summary>
        /// Base object for splite select pdf
        /// </summary>
        private Splite _splite;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event for open file dialog and Assignment select file path to _path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Return name button is normal
            SpliteButton.Content = "Splite PDF";

            // Create new Dialog for select file
            // and add filter for select only pdf files
            var dialog = new OpenFileDialog()
            {
                Filter = "Pdf document (.pdf)|*.pdf",
            };

            if (dialog.ShowDialog() == true)
                _path = dialog.FileName;
            else 
                return;

            if (string.IsNullOrEmpty(_path))
                MessageBox.Show("Not select PDF file!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

            if (Path.GetExtension(_path) != ".pdf")
            {
                MessageBox.Show("Select incorrect file! Please, select only PDF File", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                CalculateButton.IsEnabled = true;
            }
            
            PathDirectory.Text = _path;
        }

        private void SpliteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _splite.SpliteDocument();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
            CalculateButton.Content = "Calculated";
            SpliteButton.Content = "OK!";
            SpliteButton.IsEnabled = false;
            
            // Delete link on object
            _splite = null;
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _splite = new Splite(_path);
                _splite.Generate();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (_splite.IsGenerate)
            {
                CalculateButton.Content = "OK!";
                CalculateButton.IsEnabled = false;
                SpliteButton.IsEnabled = true;
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void PathDirectory_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop, true) is string[] dropPath)
                {
                    foreach (string s in dropPath)
                    {
                        if (Path.GetExtension(s) == ".pdf" && File.Exists(s))
                        {
                            PathDirectory.Text = s;
                            PathDirectory.IsEnabled = true;
                            _path = s;
                            CalculateButton.IsEnabled = true;
                            DragAndDropZone.Visibility = Visibility.Hidden;
                            SpliteButton.Content = "Splite PDF";
                            return;
                        }
                    }
                }   
            }

            MessageBox.Show("Select incorrect file! Please, select only PDF File", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            DropZone.Visibility = Visibility.Hidden;
            DropZone.IsEnabled = false;
            SpliteButton.Content = "Splite PDF";
        }

        // Event if file drag to main window
        // Show big label
        private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
        {
            // Show Label
            DragAndDropZone.Visibility = Visibility.Visible;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                //PathDirectory.IsEnabled = false;
                
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        // Event to leave file on main window
        // Hidden big label
        private void MainWindow_OnDragLeave(object sender, DragEventArgs e)
        {
            DragAndDropZone.Visibility = Visibility.Hidden;
        }
    }
}
