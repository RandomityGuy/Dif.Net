using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public class ExtendedLightmapData : IReadable,IWritable
    {
        public int lightMapBorderSize = 0;

        public int extendedData = 0;

        public void Read(BinaryReader rb)
        {
            extendedData = rb.ReadInt32();
            if (extendedData > 0)
            {
                lightMapBorderSize = rb.ReadInt32();
                rb.ReadInt32(); //dummy
            }
        }

        public void Write(BinaryWriter wb)
        {
            wb.Write(extendedData);
            if (extendedData > 0)
            {
                wb.Write(lightMapBorderSize);
                wb.Write(0);
            }
        }
    }
}
