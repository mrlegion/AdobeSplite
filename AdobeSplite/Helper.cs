using System.Collections.Generic;

namespace AdobeSplite
{
    public static class Helper
    {
        public static readonly Dictionary<string, int[]> Formats = new Dictionary<string, int[]>() // Formats [ "Name" , int [ width, height, area ] ]
        {
            { "a5", new []{148, 210, 31080}},
            { "a4", new []{210, 297, 62370}},
            { "a3", new []{297, 420, 124740}},
            { "a2", new []{420, 594, 249480}},
        };
    }
}
