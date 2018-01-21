using System;
using System.Collections.Generic;
using System.IO;

// iTextSharp library
using iText.Kernel.Pdf;

namespace AdobeSplite
{
    public class Splite
    {
        private readonly PdfReader _reader;
        private readonly PdfDocument _pdf;
        private Dictionary<string, List<int>> _splite = new Dictionary<string, List<int>>();
        private string _newDirectory = "";
        
        public int Count { get; private set; }

        public Splite(string path)
        {
            try
            {
                _reader = new PdfReader(path);
                _pdf = new PdfDocument(_reader);
                Count = _pdf.GetNumberOfPages();
                IsGenerate();
                _newDirectory = Path.Combine(Path.GetDirectoryName(path), "splite");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void IsGenerate()
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
            foreach (var format in Helper.Formats)
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
