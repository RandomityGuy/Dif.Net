using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dif.Net;
using Dif.Net.Builder;
using System.Numerics;

namespace DifTools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }
            var dif = InteriorResource.Load(args[0]);

            var outputloc = "";
            var outputver = 0;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = (string)args[i];
                if (arg == "-info")
                    ShowDifInfo(dif);

                if (arg == "-material")
                    ShowUsedMaterials(dif);

                if (arg == "-replacematerial")
                    if (i + 2 < args.Length)
                        ReplaceMaterial(ref dif, args[i + 1], args[i + 2]);

                if (arg == "-flipnormals")
                    FlipNormals(ref dif);

                if (arg == "-regenerate")
                    RegenerateDif(ref dif);

                if (arg == "-v")
                    if (i + 1 < args.Length)
                        outputver = Convert.ToInt32(args[i + 1]);

                if (arg == "-o")
                    if (i + 1 < args.Length)
                        outputloc = args[i + 1];

                if (arg == "-d")
                    AddDetail(ref dif, args[i + 1]);

                if (arg == "-lowercasemat")
                    LowercaseMaterial(ref dif);

                if (arg == "-removemp")
                    RemoveMP(ref dif);

                if (arg == "-csx")
                    ToCSX(ref dif, args[0]);

            }

            IO.interiorFileVersion = outputver;
            if (outputloc != "")
                InteriorResource.Save(dif, outputloc);


        }

        static void ShowHelp()
        {
            Console.WriteLine("DifTools");
            Console.WriteLine("Usage:");
            Console.WriteLine("DifTools <input.dif> [args]");
            Console.WriteLine("Args:");
            Console.WriteLine("-info: Show info about a dif");
            Console.WriteLine("-material: Show all used textures");
            Console.WriteLine("-replacematerial <from> <to>: Replace texture <from> to <to>");
            Console.WriteLine("-flipnormals: Flip normals");
            Console.WriteLine("-regenerate: Generate the dif again using obj2dif");
            Console.WriteLine("-v <version>: Change dif version to <version>");
            Console.WriteLine("-o <output.dif>: Set output file");
            Console.WriteLine("-d <detail.dif>: Add detail.dif as detail level of current dif");
        }

        static void ShowDifInfo(InteriorResource dif)
        {
            Console.WriteLine($"DetailLevels: {dif.detailLevels.Count}");
            for (int i = 0; i < dif.detailLevels.Count; i++)
            {
                Interior detail = (Interior)dif.detailLevels[i];
                Console.WriteLine($"Detail {i}:");
                Console.WriteLine($"-Version: {detail.interiorFileVersion.version}");
                Console.WriteLine($"-Points: {detail.points.Count}");
                Console.WriteLine($"-Convex Hulls: {detail.convexHulls.Count}");
                Console.WriteLine($"-BSP Nodes: {detail.bspNodes.Count}");
                Console.WriteLine($"-BSP Solid Leaves: {detail.bspSolidLeaves.Count}");
                Console.WriteLine($"-Surfaces: {detail.surfaces.Count}");
                Console.WriteLine($"-Zones: {detail.zones.Count}");
                Console.WriteLine($"-Portals: {detail.portals.Count}");
            }
            Console.WriteLine($"SubObjects: {dif.subObjects.Count}");
            for (int i = 0; i < dif.subObjects.Count; i++)
            {
                Interior detail = (Interior)dif.subObjects[i];
                Console.WriteLine($"SubObject {i}:");
                Console.WriteLine($"-Version: {detail.interiorFileVersion.version}");
                Console.WriteLine($"-Points: {detail.points.Count}");
                Console.WriteLine($"-Convex Hulls: {detail.convexHulls.Count}");
                Console.WriteLine($"-BSP Nodes: {detail.bspNodes.Count}");
                Console.WriteLine($"-BSP Solid Leaves: {detail.bspSolidLeaves.Count}");
                Console.WriteLine($"-Surfaces: {detail.surfaces.Count}");
                Console.WriteLine($"-Zones: {detail.zones.Count}");
                Console.WriteLine($"-Portals: {detail.portals.Count}");
            }
            Console.WriteLine($"Triggers: {dif.triggers.Count}");
        }

        static void ShowUsedMaterials(InteriorResource dif)
        {
            var mats = dif.detailLevels.Select(a => a.materialList).Aggregate((p, q) =>
            {
                var x = new List<string>(p);
                x.AddRange(q);
                return x;
            }
            ).Distinct().Union(dif.subObjects.Select(a => a.materialList).Aggregate((p, q) =>
            {
                var y = new List<string>(p);
                y.AddRange(q);
                return y;
            }));

            Console.WriteLine($"{mats.Count()} materials used:");
            foreach (var mat in mats)
                Console.WriteLine(mat);
        }

        static void ReplaceMaterial(ref InteriorResource dif,string from,string to)
        {
            for (int i = 0; i < dif.detailLevels.Count; i++)
            {
                Interior detail = dif.detailLevels[i];
                for (int j = 0; j < detail.materialList.Count; j++)
                {
                    string mat = detail.materialList[j];
                    if (mat == from)
                        dif.detailLevels[i].materialList[j] = to;
                }
            }
            for (int i = 0; i < dif.subObjects.Count; i++)
            {
                Interior detail = dif.subObjects[i];
                for (int j = 0; j < detail.materialList.Count; j++)
                {
                    string mat = detail.materialList[j];
                    if (mat == from)
                        dif.subObjects[i].materialList[j] = to;
                }
            }
        }

        static void LowercaseMaterial(ref InteriorResource dif)
        {
            for (int i = 0; i < dif.detailLevels.Count; i++)
            {
                Interior detail = dif.detailLevels[i];
                for (int j = 0; j < detail.materialList.Count; j++)
                {
                    detail.materialList[j] = detail.materialList[j].ToLower();
                }
            }
            for (int i = 0; i < dif.subObjects.Count; i++)
            {
                Interior detail = dif.subObjects[i];
                for (int j = 0; j < detail.materialList.Count; j++)
                {
                    detail.materialList[j] = detail.materialList[j].ToLower();
                }
            }
        }

        static void RemoveMP(ref InteriorResource dif)
        {
            dif.subObjects.Clear();
            dif.interiorPathFollowers.Clear();
            dif.gameEntities.Clear();
            dif.triggers.Clear();
            dif.detailLevels[0].materialList.Insert(0, "NULL");
            dif.detailLevels[0].materialList.ForEach(x => x = "NULL");
        }

        static void FlipNormals(ref InteriorResource dif)
        {
            dif.detailLevels.ForEach(a => a.normals.ForEach(b => b *= -1));
        }

        static void ToCSX(ref InteriorResource dif, string name)
        {
            var csxexporter = new CSXExporter();

            var w = new StreamWriter(File.OpenWrite(name + ".csx"));
            csxexporter.ExportCSX(dif.detailLevels[0], w);
        }

        static void RegenerateDif(ref InteriorResource dif)
        {
            dif.detailLevels = dif.detailLevels.Select(a => RegenerateInterior(a)).ToList();
        }

        static Interior RegenerateInterior(Interior interior)
        {
            //Basically just build the dif OUR way
            var builder = new DifBuilder();

            foreach (var hull in interior.convexHulls)
            {
                for (var i = hull.surfaceStart; i < hull.surfaceStart + hull.surfaceCount;i++)
                {
                    var surfindex = interior.hullSurfaceIndices[i];
                    if ((surfindex & 0x80000000) == 0x80000000)
                    {
                        continue; //Uhh we arent regenerating NullSurfaces, TODO
                    }
                    var surface = interior.surfaces[interior.hullSurfaceIndices[i]];

                    bool v = (surface.planeIndex & 0x8000) == 0x8000;
                    var normal = v ? interior.normals[interior.planes[unchecked(surface.planeIndex & ~(short)0x8000)].normalIndex] *= -1 : interior.normals[interior.planes[surface.planeIndex].normalIndex];

                    var texture = interior.materialList[surface.textureIndex];

                    var poly = new Polygon();
                    poly.Vertices = interior.windings.Skip(surface.windingStart).Take(surface.windingCount).Select(a => interior.points[a]).ToList();
                    poly.Material = texture;
                    poly.Normal = normal;
                    poly.UV = poly.Vertices.Select(a => new Vector2(Vector3.Dot(interior.texGenEQs[surface.texGenIndex].planeX.Normal,a) + interior.texGenEQs[surface.texGenIndex].planeX.D, Vector3.Dot(interior.texGenEQs[surface.texGenIndex].planeY.Normal, a) + interior.texGenEQs[surface.texGenIndex].planeY.D)).ToList();
                    poly.Indices = Enumerable.Range(0, poly.Vertices.Count).ToList();
                    if (v)
                        poly.Indices.Reverse();

                    builder.AddPolygon(poly);
                }
            }

            var intres = new InteriorResource();
            builder.Build(ref intres);

            return intres.detailLevels[0];
        }

        static void AddDetail(ref InteriorResource dif, string otherdifpath)
        {
            var other = InteriorResource.Load(otherdifpath);
            dif.detailLevels.Add(other.detailLevels[0]);
        }

    }
}
