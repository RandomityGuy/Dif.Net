using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public class Lightmap : IReadable, IWritable
    {
        public List<byte> PNG;
        public bool keepLightMap;

        public void Read(BinaryReader rb)
        {
            byte[] match = new byte[] { 0x45, 0x4E, 0x44, (byte)0xAE, 0x42, 0x60, (byte)0x82 };
            List<byte> buffer = new List<byte>();
            while (true)
            {
                var c = rb.ReadByte();
                buffer.Add(c);

                if (c == 0x49)
                {
                    var curpos = rb.BaseStream.Position;
                    var bytes = rb.ReadBytes(7);
                    if (Enumerable.SequenceEqual(bytes, match))
                    {
                        break;
                    }
                    else
                    {
                        rb.BaseStream.Seek(curpos, SeekOrigin.Begin);
                    }
                }
            }
            buffer.AddRange(match);
            PNG = buffer;
            keepLightMap = rb.ReadBoolean();
        }

        public void Write(BinaryWriter wb)
        {
            foreach (var b in PNG)
                IO.Write(b, wb);
            IO.Write(keepLightMap, wb);
        }
    }
}
