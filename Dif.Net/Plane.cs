using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Dif.Net
{
    public class Plane
    {
        public Vector3 Point = Vector3.Zero;

        public Vector3 Normal = Vector3.Zero;

        public float D = 0;

        public Plane()
        {

        }

        public Plane(Vector3 a,Vector3 b,Vector3 c)
        {
            var v1 = a - b;
            var v2 = c - b;
            var res = Vector3.Cross(v1, v2);

            Normal = res;
            float w = (float)Math.Sqrt(Normal.X * Normal.X + Normal.Y * Normal.Y + Normal.Z * Normal.Z);
            Normal.X /= w;
            Normal.Y /= w;
            Normal.Z /= w;

            //normal = glm::normalize(normal);

            D = -(Vector3.Dot(b, Normal));
            Point = b;
        }

        public Plane(Vector3 norm,float D)
        {
            Normal = norm;
            this.D = D;
        }

        public Plane(Vector3 pt, Vector3 n)
        {
            Normal = n;
            float w = (float)Math.Sqrt(Normal.X * Normal.X + Normal.Y * Normal.Y + Normal.Z * Normal.Z);
            Normal.X /= w;
            Normal.Y /= w;
            Normal.Z /= w;
            //normal = glm::normalize(normal);

            D = -(Vector3.Dot(pt, Normal));
            Point = pt;
        }

        public Plane(float x,float y, float z, float d)
        {
            Normal = new Vector3(x, y, z);
            D = d;
        }
    }
}
