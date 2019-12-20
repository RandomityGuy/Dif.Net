using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public class BSPNode : IReadable, IWritable
    {
        public short planeIndex;
        public short frontIndex;
        public short backIndex;

        public void Read(BinaryReader rb)
        {
            planeIndex = rb.ReadInt16();
            frontIndex = rb.ReadInt16();
            backIndex = rb.ReadInt16();
        }

        public void Write(BinaryWriter wb)
        {
            wb.Write(planeIndex);
            wb.Write(frontIndex);
            wb.Write(backIndex);
        }
    }
}
