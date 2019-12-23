using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct StateData
    {
        public int surfaceIndex;
        public int mapIndex;
        public short lightStateIndex;
    }

    public class StateDataBuffer : IReadable, IWritable
    {
        public int flags;
        public List<byte> stateDataBuffers = new List<byte>();        

        public void Read(BinaryReader rb)
        {
            var count = rb.ReadInt32();
            flags = rb.ReadInt32();
            stateDataBuffers = new List<byte>();
            for (var i = 0; i < count; i++)
            {
                stateDataBuffers.Add(rb.ReadByte());
            }
        }

        public void Write(BinaryWriter wb)
        {
            IO.Write(stateDataBuffers.Count,wb);
            IO.Write(flags,wb);
            foreach (var b in stateDataBuffers)
                IO.Write(b, wb);
        }
    }
}
