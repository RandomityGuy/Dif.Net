using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace obj2difSharp
{
    public class Face
    {
        public int v1;
        public int v2;
        public int v3;
        public int n1;
        public int n2;
        public int n3;
        public int uv1;
        public int uv2;
        public int uv3;
        public string material = "";
    }

    public class Obj
    {
        public List<Vector3> Points;
        public List<Vector3> Normals;
        public List<Vector2> UV;
        public List<Face> Faces;
        public string name;
    }
}
