using System.Collections.Generic;

namespace PDFSplitter.Model
{
    /// <summary>
    /// Includes all helpers functions and values
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Description of main formats
        /// </summary>
        public static readonly Dictionary<string, int[]> Formats = new Dictionary<string, int[]>()
        {
            { "a5", new []{148, 210, 31080, 29725, 32465}},
            { "a4", new []{210, 297, 62370, 59450, 64930}},
            { "a3", new []{297, 420, 124740, 120350, 128350}},
            // { "a2", new []{420, 594, 249480, 244435, 255000}}, // Enabled if need
        };
    }
}