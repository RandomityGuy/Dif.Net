using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dif.Net;
using Dif.Net.Builder;
using System.Numerics;
using System.IO;

namespace obj2difSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("obj2difSharp v1.0");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage obj2difSharp <filename> [-f] [-r]");
                Console.WriteLine("filename: the path to the file to convert to dif");
                Console.WriteLine("(optional) f: flip normals");
                Console.WriteLine("(optional) r: reverse vertex order");
            }
            var objreader = new ObjReader();
            objreader.Parse(args[0]);

            var flipnormal = false;
            var reverseverts = false;
            foreach (var arg in args)
            {
                if (arg == "-f")
                    flipnormal = true;

                if (arg == "-r")
                    reverseverts = true;
            }

            var builders = new List<DifBuilder>();
            var builder = new DifBuilder();
            builders.Add(builder);
            builder.FlipNormals = flipnormal;
            var tricount = 0;
            foreach (var group in objreader.Groups)
            {
                foreach (var face in group.Faces)
                {
                    if (tricount > 16384) //Max limits reached
                    {
                        builder = new DifBuilder();
                        builder.FlipNormals = flipnormal;
                        builders.Add(builder);
                        tricount = 0;
                    }
                    tricount++;
                    if (reverseverts)
                        builder.AddTriangle(group.Points[face.v3], group.Points[face.v2], group.Points[face.v1], group.UV[face.uv3], group.UV[face.uv2], group.UV[face.uv1],group.Normals[face.n3], Path.GetFileNameWithoutExtension(face.material));
                    else
                        builder.AddTriangle(group.Points[face.v1], group.Points[face.v2], group.Points[face.v3], group.UV[face.uv1], group.UV[face.uv2], group.UV[face.uv3], group.Normals[face.n1], Path.GetFileNameWithoutExtension(face.material));
                }
            }

            Console.WriteLine("Building Difs");

            for (int i = 0; i < builders.Count; i++)
            {
                var build = builders[i];
                var dif = new InteriorResource();
                build.Build(ref dif);
                InteriorResource.Save(dif, Path.ChangeExtension(args[0], null) + i + ".dif");
            }
        }
    }
}
