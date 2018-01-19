using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout.Element;

namespace AdobeSplite
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = @"F:\!Work\!MyProjects\CSharp\Adobe\Splite\pdf\ДЕТСКИЙ КАЛЕНДАРЬ Январь.pdf";
            
            PdfReader reader = new PdfReader(dir);
            PdfDocument pdf = new PdfDocument(reader);

            int count = pdf.GetNumberOfPages();

            Console.WriteLine(count);

            // Collection of page size
            var pagesSize = new double[count][]; // [ width, height, area ]

            // Fill collection on page size
            for (int i = 0; i < count; i++)
            {
                var page = pdf.GetPage(i + 1);
                var size = page.GetPageSize();
                var width = Math.Floor(size.GetWidth() / 2.834);
                var height = Math.Floor(size.GetHeight() / 2.834);
                var area = width * height;
                pagesSize[i] = new[] { width, height, area };
            }

            Dictionary<string, List<int>> formatDocument = new Dictionary<string, List<int>>();
            string formatName = null;

            for (int p = 0; p < pagesSize.Length; ++p)
            {
                if (formatName == null) formatName = $"{pagesSize[p][0]} x {pagesSize[p][1]}";

                if (formatDocument.ContainsKey(formatName))
                {
                    formatDocument[formatName].Add(p + 1);
                }
                else
                {
                    formatName = $"{pagesSize[p][0]} x {pagesSize[p][1]}";
                    formatDocument.Add(formatName, new List<int>() { p + 1 });
                }
            }

            foreach (var intse in formatDocument)
            {
                Console.WriteLine($"Format name: {intse.Key}");
                foreach (var ints in intse.Value)
                {
                    Console.Write($"{ints} ");
                }

                Console.WriteLine("\n" + new string('-', 40));
            }

            string newDirectory = @"F:\!Work\!MyProjects\CSharp\Adobe\Splite\pdf\splite";
            if (!Directory.Exists(newDirectory)) Directory.CreateDirectory(newDirectory);

            PdfWriter writer = null;
            PdfDocument newDoc = null;

            foreach (var keys in formatDocument)
            {
                writer = new PdfWriter(new FileStream(Path.Combine(newDirectory, $"{keys.Key}.pdf"), FileMode.Create));
                newDoc = new PdfDocument(writer);
                pdf.CopyPagesTo(keys.Value, newDoc);

                newDoc?.Close();

            }

            

        }

        static void ArrayAdd (ref int[] array, int value)
        {
            var temp = new int[array.Length + 1];
            Array.Copy(array, 0, temp, 0, array.Length);
            temp[temp.Length - 1] = value;
            array = temp;
        }
    }
}
