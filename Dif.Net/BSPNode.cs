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
        public int frontIndex;
        public int backIndex;

        public bool isFrontLeaf;
        public bool isFrontSolid;

        public bool isBackLeaf;
        public bool isBackSolid;

        public void Read(BinaryReader rb)
        {
            planeIndex = rb.ReadInt16();

            if (IO.interiorFileVersion >= 14)
            {
                frontIndex = rb.ReadInt32();

                if ((frontIndex & 0x80000) == 0x80000)
                {
                    isFrontLeaf = true;
                    frontIndex &= ~0x80000;
                    frontIndex |= 0x8000;
                }

                if ((frontIndex & 0x40000) == 0x40000)
                {
                    isFrontSolid = true;
                    frontIndex &= ~0x40000;
                    frontIndex |= 0x4000;
                }
            
                backIndex = rb.ReadInt32();

                if ((backIndex & 0x80000) == 0x80000)
                {
                    isBackLeaf = true;
                    backIndex &= ~0x80000;
                    backIndex |= 0x8000;
                }

                if ((backIndex & 0x40000) == 0x40000)
                {
                    isBackSolid = true;
                    backIndex &= ~0x40000;
                    backIndex |= 0x4000;
                }
            }
            else
            {
                frontIndex = rb.ReadInt16();

                if ((frontIndex & 0x8000) == 0x8000)
                    isFrontLeaf = true;

                if ((frontIndex & 0x4000) == 0x4000)
                    isFrontSolid = true;

                backIndex = rb.ReadInt16();

                if ((backIndex & 0x8000) == 0x8000)
                    isBackLeaf = true;

                if ((backIndex & 0x4000) == 0x4000)
                    isBackSolid = true;
            }
        }

        public void Write(BinaryWriter wb)
        {
            wb.Write(planeIndex);

            if (IO.interiorFileVersion >= 14)
            {
                var frontwrite = frontIndex;
                if (isFrontLeaf)
                {
                    frontwrite &= ~0x8000;
                    frontwrite |= 0x80000;
                }
                if (isFrontSolid)
                {
                    frontwrite &= ~0x4000;
                    frontwrite |= 0x40000;
                }

                wb.Write(frontwrite);

                var backwrite = backIndex;
                if (isBackLeaf)
                {
                    backwrite &= ~0x8000;
                    backwrite |= 0x80000;
                }
                if (isBackSolid)
                {
                    backwrite &= ~0x4000;
                    backwrite |= 0x40000;
                }

                wb.Write(backwrite);
            }
            else
            {
                wb.Write((short)frontIndex);
                wb.Write((short)backIndex);
            }
        }
    }
}
