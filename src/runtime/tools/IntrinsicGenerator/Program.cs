using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace IntrinsicGenerator
{
    public static class Program
    {

        public static void Main()
        {
            var dir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var input = Path.Combine(dir, @"../../../../../../../doc/intel-intrinsics-data-latest.xml");

            var doc = XDocument.Load(input);

            using (var writer = new StreamWriter(Path.Combine(dir, "../../../../../system/intrinsics/x86.gen.sk")))
            {
                var intrinsicWriter = new IntrinsicWriter(doc, writer);
                intrinsicWriter.Generate();
            }
        }
    }
}
