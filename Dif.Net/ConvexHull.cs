using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct ConvexHull
    {
        public int hullStart;
        public short hullCount;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;
        public int surfaceStart;
        public short surfaceCount;
        public int planeStart;
        public int polyListPlaneStart;
        public int polyListPointStart;
        public int polyListStringStart;
        [Version(12)]
        public byte staticMesh;
    }
}
