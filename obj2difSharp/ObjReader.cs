using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace obj2difSharp
{
    public class ObjReader
    {
        public List<Vector3> Points;
        public List<Vector2> UV;
        public List<Vector3> Normals;
        public List<Obj> Groups;
        public Dictionary<string, string> MtlLib;

        public void LoadMtlLib(string path)
        {
            if (MtlLib == null)
                MtlLib = new Dictionary<string, string>();

            var mtl = File.ReadLines(path);
            var currentmaterial = "";

            foreach (var line in mtl)
            {
                var type = line.Split(' ')[0];
                switch (type)
                {
                    case "newmtl":
                        currentmaterial = line.Split(' ').Last();
                        break;

                    case "map_d":
                    case "map_Kd":
                        MtlLib[currentmaterial] = line.Split(' ').Last();
                        break;
                }
            }

        }

        public void Parse(string path)
        {
            var obj = File.ReadLines(path);

            Points = new List<Vector3>();
            UV = new List<Vector2>();
            Normals = new List<Vector3>();
            Groups = new List<Obj>();

            var currentgroup = new Obj();
            currentgroup.Faces = new List<Face>();
            currentgroup.Normals = new List<Vector3>();
            currentgroup.Points = new List<Vector3>();
            currentgroup.UV = new List<Vector2>();

            var currentmaterial = "";

            foreach (var line in obj)
            {
                var type = line.Split(' ')[0];

                try
                {
                    switch (type)
                    {
                        case "v":
                            Points.Add(Extensions.FromString(line.GetWords(1, 3)));
                            break;

                        case "vt":
                            UV.Add(Extensions.FromStringV2(line.GetWords(1, 2)));
                            break;

                        case "vn":
                            Normals.Add(Extensions.FromString(line.GetWords(1, 3)));
                            break;

                        case "g":
                            Groups.Add(currentgroup);
                            currentgroup = new Obj() { name = line.Split(' ')[1] };
                            currentgroup.Faces = new List<Face>();
                            currentgroup.Normals = new List<Vector3>();
                            currentgroup.Points = new List<Vector3>();
                            currentgroup.UV = new List<Vector2>();
                            break;

                        case "f":
                            var indicelist = line.GetWords(1).Split(' ');

                            var triangulatedIndiceList = new List<string>();

                            for (var j = 0; j < indicelist.Length - 2;j++)
                            {
                                triangulatedIndiceList.Add(indicelist[0]);
                                triangulatedIndiceList.Add(indicelist[j + 1]);
                                triangulatedIndiceList.Add(indicelist[j + 2]);
                            }

                            for (var j = 0; j < triangulatedIndiceList.Count;j+=3)
                            {
                                var face = new Face();
                                face.material = currentmaterial;
                                var i1 = triangulatedIndiceList[j];
                                var i2 = triangulatedIndiceList[j+1];
                                var i3 = triangulatedIndiceList[j+2];

                                if (i1.Contains("/"))
                                {
                                    var s1 = i1.Split('/');
                                    var s2 = i2.Split('/');
                                    var s3 = i3.Split('/');

                                    if (s1.Length == 2)
                                    {
                                        var p1 = Points[Convert.ToInt32(s1[0])-1];
                                        var p2 = Points[Convert.ToInt32(s2[0]) - 1];
                                        var p3 = Points[Convert.ToInt32(s3[0]) - 1];
                                        currentgroup.Points.Add(p1);
                                        currentgroup.Points.Add(p2);
                                        currentgroup.Points.Add(p3);

                                        var uv1 = UV[Convert.ToInt32(s1[1]) - 1];
                                        var uv2 = UV[Convert.ToInt32(s2[1]) - 1];
                                        var uv3 = UV[Convert.ToInt32(s3[1]) - 1];
                                        currentgroup.UV.Add(uv1);
                                        currentgroup.UV.Add(uv2);
                                        currentgroup.UV.Add(uv3);

                                        face.v1 = currentgroup.Points.IndexOf(p1);
                                        face.v2 = currentgroup.Points.IndexOf(p2);
                                        face.v3 = currentgroup.Points.IndexOf(p3);
                                        face.uv1 = currentgroup.UV.IndexOf(uv1);
                                        face.uv2 = currentgroup.UV.IndexOf(uv2);
                                        face.uv3 = currentgroup.UV.IndexOf(uv3);
                                    }
                                    if (s1.Length == 3)
                                    {
                                        var p1 = Points[Convert.ToInt32(s1[0]) - 1];
                                        var p2 = Points[Convert.ToInt32(s2[0]) - 1];
                                        var p3 = Points[Convert.ToInt32(s3[0]) - 1];
                                        currentgroup.Points.Add(p1);
                                        currentgroup.Points.Add(p2);
                                        currentgroup.Points.Add(p3);
                                        face.v1 = currentgroup.Points.IndexOf(p1);
                                        face.v2 = currentgroup.Points.IndexOf(p2);
                                        face.v3 = currentgroup.Points.IndexOf(p3);

                                        if (s1[1] != "")
                                        {
                                            var uv1 = UV[Convert.ToInt32(s1[1]) - 1];
                                            var uv2 = UV[Convert.ToInt32(s2[1]) - 1];
                                            var uv3 = UV[Convert.ToInt32(s3[1]) - 1];
                                            currentgroup.UV.Add(uv1);
                                            currentgroup.UV.Add(uv2);
                                            currentgroup.UV.Add(uv3);
                                            face.uv1 = currentgroup.UV.IndexOf(uv1);
                                            face.uv2 = currentgroup.UV.IndexOf(uv2);
                                            face.uv3 = currentgroup.UV.IndexOf(uv3);
                                        }
                                        if (s1[2] != "")
                                        {
                                            var n1 = Normals[Convert.ToInt32(s1[2]) - 1];
                                            var n2 = Normals[Convert.ToInt32(s2[2]) - 1];
                                            var n3 = Normals[Convert.ToInt32(s3[2]) - 1];
                                            currentgroup.Normals.Add(n1);
                                            currentgroup.Normals.Add(n2);
                                            currentgroup.Normals.Add(n3);
                                            face.n1 = currentgroup.Normals.IndexOf(n1);
                                            face.n2 = currentgroup.Normals.IndexOf(n2);
                                            face.n3 = currentgroup.Normals.IndexOf(n3);
                                        }
                                    }
                                }
                                else
                                {
                                    var p1 = Points[Convert.ToInt32(i1) - 1];
                                    var p2 = Points[Convert.ToInt32(i2) - 1];
                                    var p3 = Points[Convert.ToInt32(i3) - 1];

                                    currentgroup.Points.Add(p1);
                                    currentgroup.Points.Add(p2);
                                    currentgroup.Points.Add(p3);

                                    face.v1 = currentgroup.Points.IndexOf(p1);
                                    face.v2 = currentgroup.Points.IndexOf(p2);
                                    face.v3 = currentgroup.Points.IndexOf(p3);
                                }

                                currentgroup.Faces.Add(face);
                            }
                            break;
                        case "usemtl":
                            currentmaterial = line.GetWord(1);
                            break;

                        case "mtllib":
                            LoadMtlLib(line.GetWord(1));
                            break;


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION when reading line " + line);
                    Console.WriteLine(e.GetType().ToString());
                    Console.WriteLine(e.Message);
                    continue;
                }

            }
            Groups.Add(currentgroup);

            foreach (var group in Groups)
            {
                foreach (var face in group.Faces)
                {
                    if (MtlLib.ContainsKey(face.material))
                        face.material = MtlLib[face.material];
                    else
                        face.material = null;
                }
            }
        }

    }
}
