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
        public List<byte> lightmap;
        public List<byte> lightdirmap;
        public byte keepLightMap;

        public void Read(BinaryReader rb)
        {
            byte[] match = new byte[] { 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
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
            lightmap = buffer;

            if (IO.interiorFileVersion >= 1 && IO.interiorFileVersion <= 14)
            {
                match = new byte[] { 0x45, 0x4E, 0x44, (byte)0xAE, 0x42, 0x60, (byte)0x82 };
                buffer = new List<byte>();
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
                lightdirmap = buffer;
            }

            keepLightMap = rb.ReadByte();
        }

        public void Write(BinaryWriter wb)
        {

            foreach (var b in lightmap)
                IO.Write(b, wb);
            if (IO.interiorFileVersion >= 1 && IO.interiorFileVersion <= 14)
                foreach (var b in lightdirmap)
                    IO.Write(b, wb);
            IO.Write(keepLightMap, wb);
        }
    }
}
