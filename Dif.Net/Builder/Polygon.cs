using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Dif.Net.Builder
{
    public class Polygon
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> UV = new List<Vector2>();
        public List<int> Indices = new List<int>();
        public Vector3 Normal = new Vector3();
        public string Material;
        public int surfaceIndex = -1;
    }
}
