using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using iText.Kernel.Pdf;

namespace SpliteThisFuckingPDF
{
    class Splite
    {
        private readonly PdfReader _reader;
        private readonly PdfDocument _pdf;
        private Dictionary<string, List<int>> _splite = new Dictionary<string, List<int>>();
        private readonly string _newDirectory = "";

        // Formats [ "Name" , int [ width, height, area, minArea, maxArea ] ]
        private readonly Dictionary<string, int[]> _formats = new Dictionary<string, int[]>() 
        {
            { "a5", new []{148, 210, 31080, 29725, 32465}},
            { "a4", new []{210, 297, 62370, 59450, 64930}},
            { "a3", new []{297, 420, 124740, 120350, 128350}},
            // { "a2", new []{420, 594, 249480, 244435, 255000}}, // Enabled if need
        };
        
        public int Count { get; private set; }

        public bool IsGenerate
        {
            get { return (_splite.Count != 0); }
        }

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

            string folder = Path.GetFileNameWithoutExtension(path) ?? "Splite Folder";
            string basePath = Path.GetDirectoryName(path) ?? throw new InvalidOperationException();

            _newDirectory = Path.Combine(basePath, folder);
        }

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
