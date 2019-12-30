using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct Zone
    {
        public short portalStart;
        public short portalCount;
        public int surfaceStart;
        public int surfaceCount;
        [Version(12)]
        public int staticMeshStart;
        [Version(12)]
        public int staticMeshCount;
    }
}
