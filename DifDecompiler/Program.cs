using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dif.Net;

namespace DifDecompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var dif = InteriorResource.Load(args[0]);

            var csxexporter = new CSXExporter();

            var w= new StreamWriter(File.OpenWrite(args[0] + ".csx"));
            csxexporter.ExportCSX(dif.detailLevels[0], w);
        }
    }
}
