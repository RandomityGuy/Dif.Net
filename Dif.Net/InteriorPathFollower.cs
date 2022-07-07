using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct InteriorPathFollower
    {
        public struct Waypoint
        {
            public Vector3 pos;
            public Quaternion rot;
            public int msToNext;
            public int smoothingType;

        }

        public string name;
        public string datablock;
        public int interiorResIndex;
        public Vector3 offset;
        public Dictionary<string, string> properties;
        public List<int> triggerIds;
        public List<Waypoint> waypoints;
        public int totalMs;

    }
}
