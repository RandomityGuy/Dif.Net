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

        public static VehicleCollision Default
        {
            get
            {
                var vh = new VehicleCollision();
                vh.vehicleCollisionFileVersion = 0;
                vh.convexHulls = new List<ConvexHull>();
                vh.convexHullEmitStrings = new List<byte>();
                vh.hullEmitStringIndices = new List<int>();
                vh.hullIndices = new List<int>();
                vh.hullPlaneIndices = new List<short>();
                vh.hullSurfaceIndices = new List<int>();
                vh.points = new List<Vector3>();
                vh.polyListPlanes = new List<short>();
                vh.polyListPoints = new List<int>();
                vh.polyListStrings = new List<byte>();
                vh.nullSurfaces = new List<NullSurface>();
                vh.planes = new List<Plane>();
                vh.windingIndices = new List<WindingIndex>();
                vh.windings = new List<int>();
                return vh;
            }
        }

    }
}
