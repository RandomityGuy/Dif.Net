using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct ForceField
    {
        public struct Surface
        {
            public int windingStart;
            public byte windingCount;
            public short planeIndex;
            public byte surfaceFlags;
            public int fanMask;
        }

        public int forceFieldFileVersion;
        public string name;
        public List<string> triggers;
        public Box3F boundingBox;
        public SphereF boundingSphere;
        public List<Vector3> normals;
        public List<InteriorPlane> planes;
        public List<BSPNode> bspNodes;
        public List<BSPSolidLeaf> bspSolidLeaves;
        public List<int> windings;
        public List<Surface> surfaces;
        public List<int> solidLeafSurfaces;
        public ColorF color;
    }
}
