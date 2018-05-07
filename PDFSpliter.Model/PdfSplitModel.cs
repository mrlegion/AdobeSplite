
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;
using iText.Kernel.Pdf;

namespace PDFSplitter.Model
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Prism.Mvvm;

    #endregion

    enum WorkType
    {
        Fill,
        Separate,
        All
    }

    /// <summary>
    /// Business model for project PDFSplitter
    /// </summary>
    public class PdfSplitModel : BindableBase, IDisposable
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private bool inProcessing;

        private bool isLoaded;

        private int progress;

        private PdfReader reader;

        private PdfDocument document;

        private string newDirectory;

        private int count;

        private string filepath = null;

        private Dictionary<string, List<int>> splite = null;

        public bool IsCancelled { get; set; }

        /// <summary>
        /// Create new PdfSplitModel constructor
        /// </summary>
        public PdfSplitModel()
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += WorkerStartWork;
            worker.ProgressChanged += (sender, args) => Progress += args.ProgressPercentage;
            worker.RunWorkerCompleted += WorkerEndWork;
        }

        /// <summary>
        /// Sets base file to separate
        /// </summary>
        /// <param name="file">File path</param>
        public void SetFile(string file)
        {
            filepath = file;
            string folder = Path.GetFileNameWithoutExtension(filepath) ?? "Separator folder";
            string parent = Path.GetDirectoryName(filepath) ?? throw new InvalidOperationException();
            newDirectory = Path.Combine(parent, folder);
            worker.RunWorkerAsync(WorkType.Fill);
        }

        private void WorkerEndWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                IsCancelled = true;
                return;
            }

            InProcessing = false;
            Progress = 0;
        }

        private void WorkerStartWork(object sender, DoWorkEventArgs e)
        {
            InProcessing = true;
            int step = 0;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (e.Argument is WorkType type)
            {
                switch (type)
                {
                    case WorkType.Fill:
                        worker.ReportProgress(10);
                        reader = new PdfReader(filepath);

                        worker.ReportProgress(50);
                        document = new PdfDocument(reader);

                        worker.ReportProgress(75);
                        count = document.GetNumberOfPages();

                        FillPageCollection();

                        break;
                    case WorkType.Separate:
                        CreateDirectory();

                        step = 100 / splite.Count;

                        foreach (var formats in splite)
                        {
                            CreateFiles(formats.Key, formats.Value);
                            worker.ReportProgress(step);
                        }

                        reader?.Close();
                        document?.Close();

                        IsLoaded = false;
                        break;
                    case WorkType.All:
                        worker.ReportProgress(2);
                        reader = new PdfReader(filepath);

                        worker.ReportProgress(5);
                        document = new PdfDocument(reader);

                        worker.ReportProgress(10);
                        count = document.GetNumberOfPages();

                        FillPageCollection();
                        CreateDirectory();

                        step = 90 / splite.Count;

                        foreach (var formats in splite)
                        {
                            CreateFiles(formats.Key, formats.Value);
                            worker.ReportProgress(step);
                        }

                        reader?.Close();
                        document?.Close();

                        IsLoaded = false;

                        Environment.Exit(0);
                        break;
                }
            }
        }

        private void FillPageCollection()
        {
            splite = new Dictionary<string, List<int>>();

            for (int pages = 0; pages < count; pages++)
            {
                var page = document.GetPage(pages + 1);
                string name = GetFileName(page);

                if (splite.ContainsKey(name))
                    splite[name].Add(pages + 1);
                else
                {
                    splite.Add(name, new List<int>() { pages + 1 });
                }

                worker.ReportProgress(pages * (100 / count));
            }

            IsLoaded = true;
        }

        /// <summary>
        /// Gets or sets progress work
        /// </summary>
        public int Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }

        /// <summary>
        /// Gets or sets state work model
        /// </summary>
        public bool InProcessing
        {
            get => inProcessing;
            set => SetProperty(ref inProcessing, value);
        }

        /// <summary>
        /// Gets or sets load file state
        /// </summary>
        public bool IsLoaded
        {
            get => isLoaded;
            set => SetProperty(ref isLoaded, value);
        }

        /// <summary>
        /// Fill collection separate files
        /// </summary>
        public void Generate()
        {
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Separate select document
        /// </summary>
        public void Separate()
        {
            worker.RunWorkerAsync(WorkType.Separate);
        }

        /// <summary>
        /// Create new folder or replace exist folder
        /// </summary>
        private void CreateDirectory()
        {
            try
            {
                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                }
                else
                {
                    var folderExist = MessageBox.Show("Directory already exist! You won owerride this folder?", "Directroy exist", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (folderExist == MessageBoxResult.No)
                    {
                        newDirectory += "__" + Path.GetRandomFileName();
                        Directory.CreateDirectory(newDirectory);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Create new file in new directory
        /// </summary>
        /// <param name="name">New name for file</param>
        /// <param name="pages">Collection pages</param>
        private void CreateFiles(string name, List<int> pages)
        {
            
            PdfWriter writer = null;
            PdfDocument doc = null;
            
            // TODO: бить файл
            // назначаем количество по которому требуется бить файл
            int size = (name.Contains("a3")) ? 1000 : 2000;
            // определяем сколько частей
            var stack = Math.Floor((double)(pages.Count / size));
            // если частей больше одной, то начинаем деление
            if (stack > 1)
            {
                var temp = new List<int>(pages); 
                for (int i = 0; i <= stack; i++)
                {
                    // Create new file in new directory
                    string filename = Path.Combine(newDirectory, $"{name}_{i + 1}.pdf");
                    FileStream file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);

                    try
                    {
                        writer = new PdfWriter(file);
                        doc = new PdfDocument(writer);

                        if (temp.Count - size > 0)
                        {
                            var range = temp.GetRange(0, size);
                            document.CopyPagesTo(range, doc);
                            temp.RemoveRange(0, size);
                        }
                        else document.CopyPagesTo(temp, doc);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                    doc.Close();
                    writer.Close();

                    doc = null;
                    writer = null;
                }
            }
            else
            {
                // Create new file in new directory
                string filename = Path.Combine(newDirectory, $"{name}.pdf");
                FileStream file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);

                try
                {
                    writer = new PdfWriter(file);
                    doc = new PdfDocument(writer);
                    document.CopyPagesTo(pages, doc);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            // Close new document
            doc?.Close();
            writer?.Close();
        }

        /// <summary>
        /// Gets new file formats name
        /// </summary>
        /// <param name="page">Page a PDF file</param>
        /// <returns></returns>
        private string GetFileName(PdfPage page)
        {
            // Gets page size
            var size = page.GetPageSize();
            int width = (int)Math.Floor(size.GetWidth() / 2.834);
            int heigth = (int)Math.Floor(size.GetHeight() / 2.834);
            var area = width * heigth;

            // Create base name file
            string name = null;

            // Check page size with list formats
            foreach (var format in Helpers.Formats)
            {
                if (area >= format.Value[3] && area <= format.Value[4])
                {
                    name = format.Key;
                }
            }

            // If format not found
            if (name == null)
            {
                name = $"{width} x {heigth}";
            }

            name += $"_{GetNameOrientation(width, heigth)}";

            return name;
        }

        /// <summary>
        /// Gets name orientation file
        /// </summary>
        /// <param name="width">Width file</param>
        /// <param name="height">Height file</param>
        /// <returns></returns>
        private string GetNameOrientation(int width, int height)
        {
            return width < height ? "vertical" : "horizontal";
        }

        public void Canceled()
        {
            if (worker.WorkerSupportsCancellation)
                worker.CancelAsync();
        }

        public void StartProccess(string file)
        {
            filepath = file;
            string folder = Path.GetFileNameWithoutExtension(filepath) ?? "Separator folder";
            string parent = Path.GetDirectoryName(filepath) ?? throw new InvalidOperationException();
            newDirectory = Path.Combine(parent, folder);
            InProcessing = true;
            worker.RunWorkerAsync(WorkType.All);
        }

        public void Dispose()
        {
            worker?.Dispose();
            ((IDisposable)reader)?.Dispose();
            ((IDisposable)document)?.Dispose();
        }
    }
}
