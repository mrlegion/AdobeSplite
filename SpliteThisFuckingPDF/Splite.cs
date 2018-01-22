using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using iText.Kernel.Pdf;

namespace SpliteThisFuckingPDF
{
    class Splite
    {
        /// <summary>
        /// Reader for read pdf file
        /// </summary>
        private readonly PdfReader _reader;

        /// <summary>
        /// PDF document
        /// </summary>
        private readonly PdfDocument _pdf;

        /// <summary>
        /// Collection for splite group
        /// [ format name, group numbers pages ]
        /// </summary>
        private Dictionary<string, List<int>> _splite = new Dictionary<string, List<int>>();

        /// <summary>
        /// Base variable for new directory where need save files
        /// </summary>
        private string _newDirectory;

        // Formats [ "Name" , int [ width, height, area, minArea, maxArea ] ]
        private readonly Dictionary<string, int[]> _formats = new Dictionary<string, int[]>() 
        {
            { "a5", new []{148, 210, 31080, 29725, 32465}},
            { "a4", new []{210, 297, 62370, 59450, 64930}},
            { "a3", new []{297, 420, 124740, 120350, 128350}},
            // { "a2", new []{420, 594, 249480, 244435, 255000}}, // Enabled if need
        };
        
        /// <summary>
        /// Number of page in select document
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Check is generation function
        /// </summary>
        public bool IsGenerate
        {
            get { return (_splite.Count != 0); }
        }

        /// <summary>
        /// Create new object of Splite class
        /// </summary>
        /// <param name="path">Path to PDF file</param>
        public Splite(string path)
        {
            try
            {
                _reader = new PdfReader(path);
                _pdf = new PdfDocument(_reader);
                Count = _pdf.GetNumberOfPages();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            // Get name file for new folder 
            string folder = Path.GetFileNameWithoutExtension(path) ?? "Splite Folder";
            // Get parent folder where founding file
            string basePath = Path.GetDirectoryName(path) ?? throw new InvalidOperationException();
            // Create new directory where need save new files
            _newDirectory = Path.Combine(basePath, folder);
        }

        /// <summary>
        /// Create collection split files
        /// </summary>
        public void Generate()
        {
            for (int pages = 0; pages < Count; pages++)
            {
                var page = _pdf.GetPage(pages + 1);
                string name = GetName(page);

                if (_splite.ContainsKey(name))
                    _splite[name].Add(pages + 1);
                else
                {
                    _splite.Add(name, new List<int>() { pages + 1 });
                }
            }
        }


        /// <summary>
        /// Splite document
        /// </summary>
        public void SpliteDocument()
        {
            // Create new directory
            if (!Directory.Exists(_newDirectory))
                try
                {
                    Directory.CreateDirectory(_newDirectory);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    throw;
                }
            else
            {
                var folderExist = MessageBox.Show("Directory already exist! You won owerride this folder?", "Directroy exist", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (folderExist == MessageBoxResult.No)
                {
                    _newDirectory += "__" + Path.GetRandomFileName();
                    Directory.CreateDirectory(_newDirectory);
                }
            }

            PdfWriter writer;
            PdfDocument document;
            
            // Create new file
            foreach (var format in _splite)
            {
                writer = new PdfWriter(new FileStream(Path.Combine(_newDirectory, $"{format.Key}.pdf"), FileMode.Create));
                document = new PdfDocument(writer);
                _pdf.CopyPagesTo(format.Value, document);

                document?.Close();
            }
        }

        private string GetName(PdfPage page)
        {
            var size = page.GetPageSize();
            int width = (int)Math.Floor(size.GetWidth() / 2.834);
            int heigth = (int)Math.Floor(size.GetHeight() / 2.834);
            var area = width * heigth;

            string name = null;

            foreach (var format in _formats)
            {
                if (area >= format.Value[3] && area <= format.Value[4])
                {
                    name = format.Key;
                }
            }

            if (name == null)
                name = $"{width} x {heigth}";

            name += $"_{GetOrientation(width, heigth)}";

            return name;
        }

        private string GetOrientation(int width, int heigth)
        {
            return (width < heigth) ? "vertical" : "horizontal";
        }
    }
}
