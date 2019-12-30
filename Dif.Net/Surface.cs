using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct Surface
    {
        public struct LightMapTexGen
        {
            public short finalWord;
            public float texGenXDistance;
            public float texGenYDistance;
        }

        public int windingStart;
        [Version(0,12)]
        public byte windingCount;
        [Version(13)]
        public int windingCount_v13;
        public short planeIndex;
        public short textureIndex;
        public int texGenIndex;
        public byte surfaceFlags;
        public int fanMask;
        public LightMapTexGen lightMapTexGen;
        public short lightCount;
        public int lightStateInfoStart;

        [Version(0, 12)]
        public byte mapOffsetX;
        [Version(0, 12)]
        public byte mapOffsetY;
        [Version(0, 12)]
        public byte mapSizeX;
        [Version(0, 12)]
        public byte mapSizeY;

        [Version(13)]
        public int mapOffsetX_v13;
        [Version(13)]
        public int mapOffsetY_v13;
        [Version(13)]
        public int mapSizeX_v13;
        [Version(13)]
        public int mapSizeY_v13;

        [Version(1,14)] //ehh, needs more testing
        public byte unused;

        [Version(2, 5)]
        public int brushId;
    }
}
