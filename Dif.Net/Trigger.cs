using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Dif.Net
{
    public struct Trigger
    {
        public struct PolyHedronEdge
        {
            public int face0;
            public int face1;
            public int vertex0;
            public int vertex1;
        }

        public string name;
        public string datablock;
        public Dictionary<string, string> properties;
        public List<Vector3> polyHedronPoints;
        public List<Plane> polyHedronPlanes;
        public List<PolyHedronEdge> polyHedronEdges;
        public Vector3 offset;

    }
}
