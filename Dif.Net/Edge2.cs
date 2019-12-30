using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct Edge2
    {
        public int vertex0;
        public int vertex1;
        public int normal0;
        public int normal1;
        [Version(3)]
        public int face0;
        [Version(3)]
        public int face1;
    }
}
