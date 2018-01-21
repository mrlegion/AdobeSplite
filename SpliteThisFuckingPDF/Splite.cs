using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;

namespace SpliteThisFuckingPDF
{
    class Splite
    {
        private readonly PdfReader _reader;
        private readonly PdfDocument _pdf;
        private Dictionary<string, List<int>> _splite = new Dictionary<string, List<int>>();
        private readonly string _newDirectory = "";

        private readonly Dictionary<string, int[]> _formats = new Dictionary<string, int[]>() // Formats [ "Name" , int [ width, height, area ] ]
        {
            { "a5", new []{148, 210, 31080}},
            { "a4", new []{210, 297, 62370}},
            { "a3", new []{297, 420, 124740}},
            { "a2", new []{420, 594, 249480}},
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
                Generate();
                _newDirectory = Path.Combine(Path.GetDirectoryName(path), "splite");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Generate()
        {
            for (int pages = 0; pages < Count; pages++)
            {
                var page = _pdf.GetPage(pages + 1);
                var pageArea = GetArea(page);

                if (pageArea == -1)
                    throw new ArgumentException();

                string fName = GetFormatName(pageArea) ?? GetFormatNameWithSize(page);

                if (_splite.ContainsKey(fName))
                    _splite[fName].Add(pages + 1);
                else
                {
                    _splite.Add(fName, new List<int>() { pages + 1 });
                }
            }
        }

        public void SpliteDocument()
        {
            // Create new directory
            if (!Directory.Exists(_newDirectory))
                Directory.CreateDirectory(_newDirectory);

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

        private string GetFormatName(int area)
        {
            foreach (var format in _formats)
                if (area == format.Value[2])
                {
                    return format.Key;
                }

            return null;
        }

        private int GetArea(PdfPage page)
        {
            int result = -1;
            
            var pageSize = page.GetPageSize();
            int width = (int)Math.Floor(pageSize.GetWidth() / 2.834);
            int heigth = (int)Math.Floor(pageSize.GetHeight() / 2.834);
            
            result = width * heigth;

            return result;
        }

        private string GetFormatNameWithSize(PdfPage page)
        {
            var pageSize = page.GetPageSize();
            int width = (int)Math.Floor(pageSize.GetWidth() / 2.834);
            int heigth = (int)Math.Floor(pageSize.GetHeight() / 2.834);

            return $"{width} x {heigth}";
        }
    }
}
