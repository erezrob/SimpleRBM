using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DeepLearn
{
    public static class DataParser
    {

        public static double[][] Parse(string filePath)
        {
            var x = File.ReadAllText(filePath);

            x = x.Replace("\r\n", "");

            var y = x.Split(" ".ToCharArray());

            var t =
                y.Select(
                    s =>
                    s.Substring(1).PadRight(1024, '0').Select(
                        n => double.Parse(n.ToString(CultureInfo.InvariantCulture))).ToArray()).ToArray();

            return t;
        }


    }
}
