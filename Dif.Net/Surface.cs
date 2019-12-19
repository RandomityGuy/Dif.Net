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
        public byte windingCount;
        public short planeIndex;
        public short textureIndex;
        public int texGenIndex;
        public byte surfaceFlags;
        public int fanMask;
        public LightMapTexGen lightMapTexGen;
        public short lightCount;
        public int lightStateInfoStart;
        public byte mapOffsetX;
        public byte mapOffsetY;
        public byte mapSizeX;
        public byte mapSizeY;
    }
}
