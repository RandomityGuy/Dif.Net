using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public struct GameEntity
    {
        public string datablock;
        public string name;
        public Vector3 position;
        public Dictionary<string, string> properties;
    }
}
