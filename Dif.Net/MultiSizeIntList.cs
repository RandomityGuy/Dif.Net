using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public interface IMultiSizeIntListComparer
    {
        bool Compare(bool issigned, byte param);
    }

    public class IndexListSizeComparer : IMultiSizeIntListComparer
    {
        public bool Compare(bool issigned, byte param)
        {
            return (param > 0);
        }
    }

    public class NormalIndexListSizeComparer : IMultiSizeIntListComparer
    {
        public bool Compare(bool issigned,byte param)
        {
            return (issigned && param == 0);
        }
    }

    //Basically a list that can store either type1s or type2s depending on interior version
    public class MultiSizeIntList<type1,type2> : List<int>, IReadable, IWritable
    {
        byte param = 0;
        bool signed = false;


        public void Read(BinaryReader rb)
        {
            var size = rb.ReadInt32();

            param = 0;

            signed = false;

            if ((size & 0x80000000) == 0x80000000)
            {
                signed = true;
                unchecked
                {
                    size ^= (int)0x80000000;
                }

                param = rb.ReadByte();
            }

            for (var i = 0; i < size;i++)
            {
                if (signed)
                {
                    Add((int)Convert.ChangeType(IO.Read<type2>(rb),typeof(int)));
                }
                else
                    Add((int)Convert.ChangeType(IO.Read<type1>(rb), typeof(int)));
            }
        }

        public void Write(BinaryWriter wb)
        {
            if (signed)
            {
                wb.Write((int)(Count | 0x80000000));
                wb.Write(param);
            }
            else
                wb.Write(Count);
            foreach (var o in this)
            {
                if (signed)
                    IO.Write(typeof(type2), o, wb);
                else
                    IO.Write(typeof(type1), o, wb);
            }
        }
    }

    //Terrible hacks
    public class MultiSizeIntList<type1, type2,comparetype> : List<int>, IReadable, IWritable where comparetype : IMultiSizeIntListComparer,new()
    {
        byte param = 0;
        bool signed = false;

        public void Read(BinaryReader rb)
        {
            var size = rb.ReadInt32();           

            param = 0;

            signed = false;

            if ((size & 0x80000000) == 0x80000000)
            {
                signed = true;
                unchecked
                {
                    size ^= (int)0x80000000;
                }

                param = rb.ReadByte();
            }

            var comparer = new comparetype();

            for (var i = 0; i < size; i++)
            {
                if (comparer.Compare(signed,param))
                {
                    Add((int)Convert.ChangeType(IO.Read<type2>(rb), typeof(int)));
                }
                else
                    Add((int)Convert.ChangeType(IO.Read<type1>(rb), typeof(int)));
            }
        }

        public void Write(BinaryWriter wb)
        {
            if (signed)
            {
                wb.Write((int)(Count | 0x80000000));
                wb.Write(param);
            }
            else
                wb.Write(Count);

            var comparer = new comparetype();
            foreach (var o in this)
            {
                if (comparer.Compare(signed, param))
                    IO.Write(typeof(type2), o, wb);
                else
                    IO.Write(typeof(type1), o, wb);
            }
        }
    }
}
