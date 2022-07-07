using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct NullSurface
    {
        public int windingStart;
        public short planeIndex;
        public byte surfaceFlags;
        [Version(0,12)]
        public byte windingCount;
        [Version(13)]
        public int windingCount_v13;

    }
}
