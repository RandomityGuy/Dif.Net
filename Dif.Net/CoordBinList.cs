using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public class CoordBinList : List<CoordBin>, IReadable, IWritable
    {
        public void Read(System.IO.BinaryReader rb)
        {
            for (var i = 0; i < 256; i ++)
            {
                this.Add(IO.Read<CoordBin>(rb));
            }
        }
        public void Write(System.IO.BinaryWriter wb)
        {
            foreach (var coordbin in this)
            {
                IO.Write(coordbin,wb);
            }
        }
    }
}
