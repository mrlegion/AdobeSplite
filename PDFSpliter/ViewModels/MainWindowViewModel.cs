using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Microsoft.WindowsAPICodePack.Dialogs;
using PDFSplitter.Model;

namespace PDFSplitter.ViewModels
{
    /// <summary>
    /// View Model for logic MainWindow view
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private PdfSplitModel model;

        private BackgroundWorker worker;

        private bool isCalculated;

        private bool isSeparation;

        private string parent = null;

        private string file;

        /// <summary>
        /// Create MainWindowViewModel element
        /// </summary>
        public MainWindowViewModel()
        {
            if (parent == null)
            {
                parent = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }

            File = "Click Browse button or drop file in program";

            model = new PdfSplitModel();

            model.PropertyChanged += (sender, args) => { RaisePropertyChanged(args.PropertyName); };

            BrowseCommand = new DelegateCommand(this.GetBrowseDialog);

            CalculatedCommand = new DelegateCommand(() => {});

            SeparationCommand = new DelegateCommand(() => {});

            CloseCommand = new DelegateCommand<Window>(window =>
            {
                window?.Close();
            });
        }

        /// <summary>
        /// Gets command for browse dialog
        /// </summary>
        public DelegateCommand BrowseCommand { get; }

        /// <summary>
        /// Gets command for calculated selected pdf file
        /// </summary>
        public DelegateCommand CalculatedCommand { get; }


        /// <summary>
        /// Gets command for separation pdf file
        /// </summary>
        public DelegateCommand SeparationCommand { get; }

        /// <summary>
        /// Gets command for close select window
        /// </summary>
        public DelegateCommand<Window> CloseCommand { get; }

        /// <summary>
        /// Gets or sets value for calculated state
        /// </summary>
        public bool IsCalculated
        {
            get => isCalculated;
            set => SetProperty(ref isCalculated, value);
        }

        /// <summary>
        /// Gets or sets value for separation state
        /// </summary>
        public bool IsSeparation
        {
            get => isSeparation;
            set => SetProperty(ref isSeparation, value);
        }

        /// <summary>
        /// Gets state of processing calculated or separated
        /// </summary>
        public bool InProcessing { get; }

        /// <summary>
        /// Gets or sets path to pdf file
        /// </summary>
        public string File
        {
            get => file;
            set => SetProperty(ref file, value);
        }

        /// <summary>
        /// Gets progress work
        /// </summary>
        public int Progress => model.Progress;

        private void GetBrowseDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory = parent,
                AllowNonFileSystemItems = true,
                IsFolderPicker = false,
                Title = "Select PDF file",
                Filters = { new CommonFileDialogFilter("PDF File", "pdf") }
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            File = dialog.FileName;
            parent = Path.GetDirectoryName(dialog.FileName);

            IsCalculated = true;
        }
    }
}