using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
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
        private readonly PdfSplitModel model;

        private string parent = null;

        private string file;

        private bool isDrop;

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

            // TODO: Can delete this code
            //CalculatedCommand = new DelegateCommand(() =>
            //{
            //    model.Generate();
            //});

            SeparationCommand = new DelegateCommand(() =>
            {
                model.Separate();
                File = "Click Browse button or drop file in program";
            });

            CloseCommand = new DelegateCommand<Window>(window =>
            {
                window?.Close();
            });
        }

        /// <summary>
        /// Gets command for browse dialog
        /// </summary>
        public DelegateCommand BrowseCommand { get; }

        // TODO: Delete this code later
        /// <summary>
        /// Gets command for calculated selected pdf file
        /// </summary>
        //public DelegateCommand CalculatedCommand { get; }


        /// <summary>
        /// Gets command for separation pdf file
        /// </summary>
        public DelegateCommand SeparationCommand { get; }

        /// <summary>
        /// Gets command for close select window
        /// </summary>
        public DelegateCommand<Window> CloseCommand { get; }

        /// <summary>
        /// Gets or sets state of drop file on window 
        /// </summary>
        public bool IsDrop
        {
            get => isDrop;
            set => SetProperty(ref isDrop, value);
        }

        /// <summary>
        /// Gets or sets load file state in model
        /// </summary>
        public bool IsLoaded => model.IsLoaded;

        /// <summary>
        /// Gets state of processing calculated or separated
        /// </summary>
        public bool InProcessing => model.InProcessing;

        /// <summary>
        /// Gets visibility state on root grid on window frame
        /// </summary>
        public bool IsVisibility => !IsDrop;

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

            BaseSettings(dialog.FileName);
        }

        /// <summary>
        /// Base settings on window load file
        /// </summary>
        /// <param name="file">Path to file</param>
        public void BaseSettings(string file)
        {
            File = file;
            parent = Path.GetDirectoryName(file);

            IsDrop = false;

            model.SetFile(File);

            
        }
    }
}