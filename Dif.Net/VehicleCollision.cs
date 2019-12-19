using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct VehicleCollision
    {
        public int vehicleCollisionFileVersion;
        public List<ConvexHull> convexHulls;
        public List<byte> convexHullEmitStrings;
        public List<int> hullIndices;
        public List<short> hullPlaneIndices;
        public List<int> hullEmitStringIndices;
        public List<int> hullSurfaceIndices;
        public List<short> polyListPlanes;
        public List<int> polyListPoints;
        public List<byte> polyListStrings;
        public List<NullSurface> nullSurfaces;
        public List<Vector3> points;
        public List<Plane> planes;
        public List<int> windings;
        public List<WindingIndex> windingIndices;

    }
}
