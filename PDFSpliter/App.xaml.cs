using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PDFSplitter.ViewModels;
using PDFSplitter.Views;

namespace PDFSplitter
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            bool startApp = true;

            var file = e.Args.FirstOrDefault(s => s.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && File.Exists(s));

            if (!string.IsNullOrEmpty(file))
            {
                startApp = false;

                ProgressWindow progress = new ProgressWindow();
                ProgressWindowViewModel viewModel = new ProgressWindowViewModel();
                progress.DataContext = viewModel;
                progress.Show();

                viewModel.Start(file);
            }

            if (startApp)
            {
                MainWindow window = new MainWindow();
                window.Show();
            }
        }
    }
}
