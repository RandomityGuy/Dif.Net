using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Dif.Net.Builder
{
    public class DifBuilder
    {
        struct HullPoly
        {
            public List<int> points;
            public int planeIndex;
        };

        public bool FlipNormals { get; set; } = false;

        List<Polygon> polygons = new List<Polygon>();

        Interior interior;

        //For optimization/searching of these
        Dictionary<int, short> planehashes = new Dictionary<int, short>();
        Dictionary<int, int> pointhashes = new Dictionary<int, int>();
        Dictionary<int, int> emitstringhashes = new Dictionary<int, int>();

        List<string> materialList = new List<string>() { "NONE" };

        public void AddTriangle(Vector3 p1,Vector3 p2,Vector3 p3,Vector2 uv1,Vector2 uv2,Vector2 uv3)
        {
            var poly = new Polygon()
            {
                Vertices = FlipNormals ? new List<Vector3>() { p3, p2, p1 } : new List<Vector3>() { p1, p2, p3 },
                Indices = new List<int>() { 0, 1, 2},
                UV = FlipNormals ? new List<Vector2>() { uv3, uv2, uv1 } : new List<Vector2>() { uv1, uv2, uv3 },
                Material = "None",
                Normal = Vector3.Normalize(Vector3.Cross((p2 - p1), (p3 - p1))) * (FlipNormals ? -1 : 1)
            };
            polygons.Add(poly);
        }
        public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3,Vector3 normal, string material)
        {
            var poly = new Polygon()
            {
                Vertices = FlipNormals ? new List<Vector3>() { p3, p2, p1 } : new List<Vector3>() { p1, p2, p3 },
                Indices = new List<int>() { 0, 1, 2 },
                UV = FlipNormals ? new List<Vector2>() { uv3, uv2, uv1 } : new List<Vector2>() { uv1, uv2, uv3 },
                Material = material,
                Normal = FlipNormals ? new Plane().FromPtNormal(p3, -1 * normal).Normal : new Plane().FromPtNormal(p1, normal).Normal
            };
            if (!materialList.Contains(material))
                materialList.Add(material);
            polygons.Add(poly);
        }
        public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3,string material)
        {
            var poly = new Polygon()
            {
                Vertices = FlipNormals ? new List<Vector3>() { p3, p2, p1 } : new List<Vector3>() { p1, p2, p3 },
                Indices = new List<int>() { 0, 1, 2 },
                UV = FlipNormals ? new List<Vector2>() { uv3, uv2, uv1 } : new List<Vector2>() { uv1, uv2, uv3 },
                Material = material,
                Normal = Vector3.Normalize(Vector3.Cross((p2 - p1), (p3 - p1))) * (FlipNormals ? -1 : 1)
            };
            if (!materialList.Contains(material))
                materialList.Add(material);
            polygons.Add(poly);
        }
        public void AddPolygon(Polygon poly)
        {
            polygons.Add(poly);
            if (!materialList.Contains(poly.Material))
                materialList.Add(poly.Material);
        }

        /*
         * The order of exports for an interior is:
         * BSPNode
         * -SolidLeafSurfaces       
         * --Surface
         * ---Plane
         * ----Normal
         * ---Material
         * ---TexGen
         * ---Winding
         * ----Point
         * -BSPSolidLeaf
         * ConvexHull
         * -All the hull indexes
         * -Emit strings
         * Zones
         * -ZoneSurfaces
         * CoordBins
         * The rest unused stuff
        */

        short ExportPlane(Polygon poly)
        {
            if (interior.planes == null)
                interior.planes = new List<InteriorPlane>();

            if (planehashes == null)
                planehashes = new Dictionary<int, short>();

            var plane = new Plane();
            plane = plane.FromPtNormal(poly.Vertices[0], poly.Normal);
            var hash = plane.Normal.X.GetHashCode() ^ plane.Normal.Y.GetHashCode() ^ plane.Normal.Z.GetHashCode() ^ plane.D.GetHashCode();
            if (planehashes.ContainsKey(hash))
                return planehashes[hash];

            if (interior.planes.Count > ushort.MaxValue)
                throw new Exception("Plane Limits Reached");

            var index = (short)interior.planes.Count;

            var p = new InteriorPlane();
            if (interior.normals.Count > ushort.MaxValue)
                throw new Exception("Normal Limits Reached");
            p.normalIndex = (short)interior.normals.Count;
            p.planeDistance = plane.D;
            interior.planes.Add(p);
            interior.normals.Add(poly.Normal);
            planehashes.Add(hash, index);
            return index;
        }

        short ExportPlane(Plane plane)
        {
            if (interior.planes == null)
                interior.planes = new List<InteriorPlane>();

            if (planehashes == null)
                planehashes = new Dictionary<int, short>();

            if (interior.normals == null)
                interior.normals = new List<Vector3>();

            var hash = plane.Normal.X.GetHashCode() ^ plane.Normal.Y.GetHashCode() ^ plane.Normal.Z.GetHashCode() ^ plane.D.GetHashCode();
            if (planehashes.ContainsKey(hash))
                return planehashes[hash];

            if (interior.planes.Count > ushort.MaxValue)
                throw new Exception("Plane Limits Reached");

            var index = (short)interior.planes.Count;

            var p = new InteriorPlane();
            if (interior.normals.Count > ushort.MaxValue)
                throw new Exception("Normal Limits Reached");
            p.normalIndex = (short)interior.normals.Count;
            p.planeDistance = plane.D;
            interior.planes.Add(p);
            interior.normals.Add(plane.Normal);
            planehashes.Add(hash, index);
            return index;
        }

        short ExportTexture(string texture)
        {
            if (interior.materialList == null)
                interior.materialList = new List<string>();

            if (interior.materialList.Count > ushort.MaxValue)
                throw new Exception("Texture Limits Reached");

            if (interior.materialList.Contains(texture))
                return (short)interior.materialList.IndexOf(texture);

            var index = (short)interior.materialList.Count;
            interior.materialList.Add(texture);
            return index;
        }

        int ExportPoint(Vector3 p)
        {
            if (interior.points == null)
                interior.points = new List<Vector3>();

            if (interior.pointVisibilities == null)
                interior.pointVisibilities = new List<byte>();

            var hash = p.X.GetHashCode() ^ p.Y.GetHashCode() ^ p.Z.GetHashCode();
            if (pointhashes.ContainsKey(hash))
                return pointhashes[hash];

            var index = interior.points.Count;
            interior.points.Add(p);
            interior.pointVisibilities.Add(255);
            return index;
        }

        short ExportTexGen(Polygon poly)
        {
            if (interior.texGenEQs == null)
                interior.texGenEQs = new List<TexGenEQ>();

            var v1 = poly.Vertices[poly.Indices[0]];
            var v2 = poly.Vertices[poly.Indices[1]];
            var v3 = poly.Vertices[poly.Indices[2]];

            var uv1 = poly.UV[poly.Indices[0]];
            var uv2 = poly.UV[poly.Indices[1]];
            var uv3 = poly.UV[poly.Indices[2]];

            var texgen = SolveTexGen(v1, v2, v3, uv1, uv2, uv3);
            if (interior.texGenEQs.Count > ushort.MaxValue)
                throw new Exception("Maximum TexGens Reached");
            var index = (short)interior.texGenEQs.Count;
            interior.texGenEQs.Add(texgen);
            return index;
        }

        void ExportWinding(Polygon poly)
        {
            if (interior.windingIndices == null)
                interior.windingIndices = new List<WindingIndex>();

            if (interior.windings == null)
                interior.windings = new MultiSizeIntList<int, short, IndexListSizeComparer>();

            var finalwinding = new List<int>();
            foreach (var pt in poly.Vertices)
                finalwinding.Add(ExportPoint(pt));

            var windingindex = new WindingIndex();
            windingindex.windingCount = finalwinding.Count;
            windingindex.windingStart = interior.windings.Count;

            interior.windingIndices.Add(windingindex);
            interior.windings.AddRange(finalwinding);
        }

        int ExportSurface(Polygon poly)
        {
            if (interior.surfaces == null)
                interior.surfaces = new List<Surface>();

            if (interior.normalLMapIndices == null)
                interior.normalLMapIndices = new List<byte>();

            if (interior.alarmLMapIndices == null)
                interior.alarmLMapIndices = new List<byte>();

            var index = interior.surfaces.Count;
            var surf = new Surface();

            surf.planeIndex = ExportPlane(poly);
            surf.textureIndex = ExportTexture(poly.Material);
            surf.texGenIndex = ExportTexGen(poly);
            surf.surfaceFlags = 16;
            surf.fanMask = 15;
            ExportWinding(poly);
            var last = interior.windingIndices.Last();
            surf.windingStart = last.windingStart;
            if (last.windingCount > byte.MaxValue)
                throw new Exception("Max Windings for surface reached");
            surf.windingCount = (byte)last.windingCount;
            interior.windingIndices.RemoveAt(interior.windingIndices.Count - 1);
            surf.lightCount = 0;
            surf.lightStateInfoStart = 0;
            surf.mapSizeX = 0;
            surf.mapSizeY = 0;
            surf.mapOffsetX = 0;
            surf.mapOffsetY = 0;
            interior.surfaces.Add(surf);
            interior.normalLMapIndices.Add(0);
            interior.alarmLMapIndices.Add(0);

            return index;
        }

        short CreateLeafIndex(ushort baseIndex, bool isSolid)
        {
            ushort baseRet;
            if (isSolid)
            {
                baseRet = 0xC000;
            }
            else
            {
                baseRet = 0x8000;
            }
            return (short)(baseRet | baseIndex);
        }

        short ExportBSP(BSPBuilder.BSPNode n, ref List<Polygon> orderedPolys)
        {
            if (interior.bspNodes == null)
                interior.bspNodes = new List<BSPNode>();

            if (interior.bspSolidLeaves == null)
                interior.bspSolidLeaves = new List<BSPSolidLeaf>();

            if (interior.solidLeafSurfaces == null)
                interior.solidLeafSurfaces = new MultiSizeIntList<int, short>();

            if (n.IsLeaf)
            {
                var leafindex = interior.bspSolidLeaves.Count;

                if (leafindex > ushort.MaxValue)
                    throw new Exception("Max BSPSolidLeaves Reached");

                var leaf = new BSPSolidLeaf();
                leaf.surfaceIndex = interior.solidLeafSurfaces.Count;
                leaf.surfaceCount = 0;

                var leafPolyIndices = new List<int>();


                leafPolyIndices.Add(ExportSurface(n.Polygon));
                orderedPolys.Add(n.Polygon);

                for (int i = 0; i < leafPolyIndices.Count; i++)
                {
                    interior.solidLeafSurfaces.Add(leafPolyIndices[i]);
                    leaf.surfaceCount++;
                }

                interior.bspSolidLeaves.Add(leaf);
                return CreateLeafIndex((ushort)leafindex, true);
            }
            else
            {
                var bspnode = new BSPNode();
                int nodeindex = interior.bspNodes.Count;
                if (nodeindex > ushort.MaxValue)
                    throw new Exception("Max BSPNodes reached");
                interior.bspNodes.Add(bspnode);
                bspnode.planeIndex = ExportPlane(n.Plane);
                if (n.Front != null)
                    bspnode.frontIndex = ExportBSP(n.Front,ref orderedPolys);
                else
                    bspnode.frontIndex = CreateLeafIndex(0, false);
                if (n.Back != null)
                    bspnode.backIndex = ExportBSP(n.Back, ref orderedPolys);
                else
                    bspnode.backIndex = CreateLeafIndex(0, false);
                return (short)nodeindex;
            }
        }

        int ExportEmitString(List<byte> emitString)
        {
            if (interior.convexHullEmitStrings == null)
                interior.convexHullEmitStrings = new List<byte>();

            var hash = 0;
            emitString.Select(a => a.GetHashCode()).ToList().ForEach(a => hash ^= a);

            if (emitstringhashes == null)
                emitstringhashes = new Dictionary<int, int>();

            if (emitstringhashes.ContainsKey(hash))
                return emitstringhashes[hash];

            var index = interior.convexHullEmitStrings.Count;
            interior.convexHullEmitStrings.AddRange(emitString);
            emitstringhashes.Add(hash, index);

            return index;

        }

        void ExportConvexHulls(List<List<Polygon>> polys)
        {
            if (interior.convexHulls == null)
                interior.convexHulls = new List<ConvexHull>();
            if (interior.convexHullEmitStrings == null)
                interior.convexHullEmitStrings = new List<byte>();
            if (interior.hullIndices == null)
                interior.hullIndices = new MultiSizeIntList<int, short>();
            if (interior.hullPlaneIndices == null)
                interior.hullPlaneIndices = new MultiSizeIntList<short, short>();
            if (interior.hullSurfaceIndices == null)
                interior.hullSurfaceIndices = new MultiSizeIntList<int, short>();
            if (interior.polyListPlanes == null)
                interior.polyListPlanes = new MultiSizeIntList<short, short>();
            if (interior.polyListPoints == null)
                interior.polyListPoints = new MultiSizeIntList<int, short>();
            if (interior.polyListStrings == null)
                interior.polyListStrings = new List<byte>();
            if (interior.hullEmitStringIndices == null)
                interior.hullEmitStringIndices = new MultiSizeIntList<int, short>();
           
            for (var polyIndex = 0; polyIndex < polys.Count; polyIndex++)
            {
                var hull = new ConvexHull();
                hull.surfaceStart = interior.hullSurfaceIndices.Count;
                if (polys[polyIndex].Count > ushort.MaxValue)
                    throw new Exception("Max surfaces reached for convex hull");
                hull.surfaceCount = (short)polys[polyIndex].Count;

                for (int i = 0; i < hull.surfaceCount; i++)
                    interior.hullSurfaceIndices.Add(hull.surfaceStart + i);

                hull.hullStart = interior.hullIndices.Count;
                hull.hullCount = 0;

                var inthullcount = 0;
                for (int i = 0; i < polys[polyIndex].Count; i++)
                    inthullcount += polys[polyIndex][i].Vertices.Count;

                if (inthullcount > ushort.MaxValue)
                    throw new Exception("Max vertices reached for convex hull");

                hull.hullCount = (short)inthullcount;

                var hullPoints = new List<int>();
                var hullpolys = new List<HullPoly>();

                hull.polyListPointStart = interior.polyListPoints.Count;

                for (int i = 0; i < polys[polyIndex].Count; i++)
                {
                    var hp = new HullPoly();
                    hp.points = new List<int>();
                    for (int j = 0; j < polys[polyIndex][i].Indices.Count; j++)
                    {
                        int pt = ExportPoint(polys[polyIndex][i].Vertices[polys[polyIndex][i].Indices[j]]);
                        interior.hullIndices.Add(pt);
                        interior.polyListPoints.Add(pt);
                        hp.points.Add(pt);
                        hullPoints.Add(pt);
                    }
                    hp.planeIndex = ExportPlane(polys[polyIndex][i]);
                    hullpolys.Add(hp);
                }

                hull.polyListPlaneStart = interior.polyListPlanes.Count;
                hull.planeStart = interior.hullPlaneIndices.Count;

                for (int i = 0; i < polys[polyIndex].Count; i++)
                {
                    var planeindex = ExportPlane(polys[polyIndex][i]);
                    interior.polyListPlanes.Add(planeindex);
                    interior.hullPlaneIndices.Add(planeindex);
                }

               var minx = 1E+08f;
               var miny = 1E+08f;
               var minz = 1E+08f;
               var maxx = -1E+08f;
               var maxy = -1E+08f;
               var maxz = -1E+08f;

               for (int i = 0; i < polys[polyIndex].Count; i++)
               {
                   for (int j = 0; j < polys[polyIndex][i].Vertices.Count; j++)
                   {
                       var v = polys[polyIndex][i].Vertices[j];
                       if (v.X < minx)
                           minx = v.X;
                       if (v.Y < miny)
                           miny = v.Y;
                       if (v.Z < minz)
                           minz = v.Z;
                       if (v.X > maxx)
                           maxx = v.X;
                       if (v.Y > maxy)
                           maxy = v.Y;
                       if (v.Z > maxz)
                           maxz = v.Z;
                   }
               }

                hull.minX = minx;
                hull.minY = miny;
                hull.minZ = minz;
                hull.maxX = maxx;
                hull.maxY = maxy;
                hull.maxZ = maxz;


                //Prepare the emitStrings, copied from m2d
                for (int i = 0; i < hullPoints.Count; i++)
                {
                    var emitPoints = new List<int>(128);
                    var emitEdges = new List<short>(128);
                    var emitPolys = new List<int>(128);

                    var point = hullPoints[i];

                    for (int j = 0; j < hullpolys.Count; j++)
                    {
                        bool found = false;
                        for (int k = 0; k < hullpolys[j].points.Count ; k++)
                        {
                            if (hullpolys[j].points[k] == point)
                                found = true;
                        }
                        if (found)
                        {
                            for (int k = 0; k < hullpolys[j].points.Count; k++)
                                emitPoints.Add(hullpolys[j].points[k]);

                            for (int k = 0; k < hullpolys[j].points.Count; k++)
                            {
                                int first = hullpolys[j].points[k];
                                int next = hullpolys[j].points[(k + 1) % hullpolys[j].points.Count];

                                var edge = (((short)Math.Min(first, next) << 8) | ((short)Math.Max(first, next)) << 0);
                                emitEdges.Add((short)edge);
                            }

                            emitPolys.Add(j);
                        }
                    }

                    for (int j = 0; j < hullpolys.Count; j++)
                    {
                        for (int k = 0; k < emitPolys.Count; k++)
                        {
                            if (emitPolys[k] == j) continue;

                            if (hullpolys[emitPolys[k]].planeIndex == hullpolys[j].planeIndex)
                            {
                                bool found = false;

                                for (int l = 0; l < emitPolys.Count; l++)
                                    if (emitPolys[l] == j)
                                        found = true;

                                if (!found)
                                {
                                    for (int l = 0; l < hullpolys[j].points.Count; l++)
                                    {
                                        emitPoints.Add(hullpolys[j].points[l]);

                                        int first = hullpolys[j].points[l];
                                        int next = hullpolys[j].points[(l + 1) % hullpolys[j].points.Count];

                                        var edge = (((short)Math.Min(first, next) << 8) | ((short)Math.Max(first, next)) << 0);
                                        emitEdges.Add((short)edge);
                                    }
                                    emitPolys.Add(j);
                                }
                            }
                        }
                    }

                    emitPoints.Sort();
                    emitPoints = emitPoints.Distinct().ToList();
                    emitPoints.Sort();
                    emitEdges.Sort();
                    emitEdges = emitEdges.Distinct().ToList();
                    emitEdges.Sort();

                    for (int j = 0; j < emitEdges.Count; j++)
                    {
                        var firstIndex = emitEdges[j] >> 8;
                        var nextIndex = emitEdges[j] & 255;
                        var newFirst = 65535;
                        var newNext = 65535;
                        for (int l2 = 0; l2 < emitPoints.Count; l2++)
                        {
                            bool flag15 = emitPoints[l2] == firstIndex;
                            if (flag15)
                            {
                                newFirst = l2;
                            }
                            bool flag16 = emitPoints[l2] == nextIndex;
                            if (flag16)
                            {
                                newNext = l2;
                            }
                        }
                        var newSignature = Math.Min(newFirst, newNext) << 8 | Math.Max(newFirst, newNext);
                        emitEdges[j] = (short)newSignature;
                    }

                    var emitString = new List<byte>();

                    emitString.Add((byte)emitPoints.Count);
                    foreach (var p in emitPoints)
                        emitString.Add((byte)i);

                    emitString.Add((byte)emitEdges.Count);
                    foreach (var e in emitEdges)
                    {
                        emitString.Add((byte)(e >> 8));
                        emitString.Add((byte)(e & 0xFF));
                    }

                    emitString.Add((byte)emitPolys.Count);

                    for (int j = 0; j < emitPolys.Count; j++)
                    {
                        emitString.Add((byte)hullpolys[emitPolys[j]].points.Count);
                        emitString.Add((byte)emitPolys[j]);
                        for (int k = 0; k < hullpolys[emitPolys[j]].points.Count; k++)
                        {
                            for (int l = 0; l < emitPoints.Count; l++)
                            {
                                if (emitPoints[l] == hullpolys[emitPolys[j]].points[k])
                                {
                                    emitString.Add((byte)l);
                                    break;
                                }
                            }
                        }
                    };

                    if (interior.hullEmitStringIndices == null)
                        interior.hullEmitStringIndices = new MultiSizeIntList<int, short>();

                    interior.hullEmitStringIndices.Add(ExportEmitString(emitString));
                }

                interior.convexHulls.Add(hull);
            }
            interior.polyListStrings.Add(0);
        }

        void ExportCoordBins()
        {
            if (interior.coordBins == null)
                interior.coordBins = new CoordBinList();

            if (interior.coordBinIndices == null)
                interior.coordBinIndices = new MultiSizeIntList<short, short>();

            for (int i = 0; i < 256; i++)
                interior.coordBins.Add(new CoordBin());

            for (int i = 0; i < 16; i++)
            {
                float minX = interior.boundingBox.min.X;
                float maxX = interior.boundingBox.min.X;
                minX += i * ((interior.boundingBox.max.X - interior.boundingBox.min.X) / 16);
                maxX += (i + 1) * ((interior.boundingBox.max.X - interior.boundingBox.min.X) / 16);
                for (int j = 0; j < 16; j++)
                {
                    float minY = interior.boundingBox.min.Y;
                    float maxY = interior.boundingBox.min.Y;
                    minY += j * ((interior.boundingBox.max.Y - interior.boundingBox.min.Y) / 16);
                    maxY += (j + 1) * ((interior.boundingBox.max.Y - interior.boundingBox.min.Y) / 16);

                    int binIndex = (i * 16) + j;
                    var cb = interior.coordBins[binIndex];
                    cb.binStart = interior.coordBinIndices.Count;                   

                    if (interior.convexHulls.Count > ushort.MaxValue)
                        throw new Exception("Max convexhulls reached for coordbin");

                    for (int k = 0; k < interior.convexHulls.Count; k++)
                    {
                        var hull = interior.convexHulls[k];

                        if (!(minX > hull.maxX || maxX < hull.minX || maxY < hull.minY || minY > hull.maxY))
                        {
                            interior.coordBinIndices.Add((short)k);
                        }
                    }
                    cb.binCount = interior.coordBinIndices.Count - cb.binStart;

                    interior.coordBins[binIndex] = cb;
                }
            }
        }

        public void Build(ref InteriorResource ir)
        {
            var bspnodes = new List<BSPBuilder.BSPNode>();
            foreach (var poly in polygons)
            {
                var leafnode = new BSPBuilder.BSPNode();
                leafnode.IsLeaf = true;
                leafnode.Polygon = poly;
                var n = new BSPBuilder.BSPNode();
                n.Front = leafnode;
                n.Plane = new Plane().FromPtNormal(poly.Vertices[0], poly.Normal);
                bspnodes.Add(n);
            }

            var BSPBuilder = new BSPBuilder();
            var root = BSPBuilder.BuildBSPRecursive(bspnodes);

            polygons.Clear();
            root.GatherPolygons(ref polygons);

            interior = new Interior();

            var orderedpolys = new List<Polygon>();
            ExportBSP(root, ref orderedpolys);

            var groupedPolys = new List<List<Polygon>>();

            int fullpolycount = orderedpolys.Count / 8;
            int rem = orderedpolys.Count % 8;

            for (int i = 0; i < orderedpolys.Count - rem; i += 8)
            {
                var polysList = new List<Polygon>();
                for (int j = 0; j < 8; j++)
                    polysList.Add(orderedpolys[i + j]);

                groupedPolys.Add(polysList);
            }
            var lastPolys = new List<Polygon>();
            for (int i = orderedpolys.Count - rem; i < orderedpolys.Count; i++)
            {
                lastPolys.Add(orderedpolys[i]);
            }
            if (lastPolys.Count != 0)
                groupedPolys.Add(lastPolys);


            ExportConvexHulls(groupedPolys);

            if (interior.zones == null)
                interior.zones = new List<Zone>();
            var z = new Zone();
            z.portalStart = 0;
            z.portalCount = 0;
            z.surfaceStart = 0;
            z.surfaceCount = interior.surfaces.Count;

            interior.zones.Add(z);

            if (interior.zoneSurfaces == null)
                interior.zoneSurfaces = new MultiSizeIntList<short, short>();

            if (interior.surfaces.Count > ushort.MaxValue)
                throw new Exception("Max surfaces for zone reached");
            for (int i = 0; i < interior.surfaces.Count; i++)
                interior.zoneSurfaces.Add((short)i);


            ExportCoordBins();

            //Initialize all the null values to default values
            interior.interiorFileVersion = 0;
            interior.materialListVersion = 1;
            interior.coordBinMode = 0;
            interior.baseAmbientColor = new ColorF() { red = 1, green = 1, blue = 1, alpha = 1 };
            interior.alarmAmbientColor = new ColorF() { red = 1, green = 1, blue = 1, alpha = 1 };
            interior.detailLevel = 0;
            interior.minPixels = 250;
            interior.hasAlarmState = false;
            interior.numLightStateEntries = 0;
            interior.boundingBox = GetBoundingBox();
            interior.boundingSphere = GetBoundingSphere();
            interior.animatedLights = new List<AnimatedLight>();
            interior.lightMaps = new List<Lightmap>();
            interior.lightStates = new List<LightState>();
            interior.nameBuffer = new List<byte>();
            interior.nullSurfaces = new List<NullSurface>();
            interior.portals = new List<Portal>();
            interior.stateDataBuffer = new StateDataBuffer();
            interior.stateDatas = new List<StateData>();
            interior.texMatIndices = new List<int>();
            interior.texMatrices = new List<TexMatrix>();
            interior.texNormals = new List<Vector3>();
            interior.zonePortalList = new MultiSizeIntList<short, short>();
            interior.extendedLightMapData = new ExtendedLightmapData() { extendedData = 0 };

            ir.detailLevels.Add(interior);
        }

        Box3F GetBoundingBox()
        {
            var minx = 1E+08f;
            var miny = 1E+08f;
            var minz = 1E+08f;
            var maxx = -1E+08f;
            var maxy = -1E+08f;
            var maxz = -1E+08f;

            foreach (var poly in polygons)
            {
                foreach (var v in poly.Vertices)
                {
                    if (v.X > maxx)
                        maxx = v.X;
                    if (v.Y > maxy)
                        maxy = v.Y;
                    if (v.Z > maxz)
                        maxz = v.Z;

                    if (v.X < minx)
                        minx = v.X;
                    if (v.Y < miny)
                        miny = v.Y;
                    if (v.Z < minz)
                        minz = v.Z;
                }
            }

            var box = new Box3F();
            box.min = new Vector3(minx, miny, minz);
            box.max = new Vector3(maxx, maxy, maxz);

            return box;

        }

        SphereF GetBoundingSphere()
        {
            var box = interior.boundingBox;
            var center = box.max + box.min;
            center /= 2;

            var r = box.max - center;

            var sphere = new SphereF();
            sphere.point = center;
            sphere.radius = r.Length();

            return sphere;
        }

        //Solution by HiGuy
        bool closeEnough(Vector3 p1, Vector3 p2, float dist = 0.0001f)
        {
            return (p2-p1).Length() < dist;
        }

        bool closeEnough(float p1, float p2, float dist = 0.0001f)
        {
            return Math.Abs(p1 - p2) < dist;
        }

        Vector3 Solve3VarLinearEquation(Matrix3x4 coefficients)
        {
            if (closeEnough(coefficients.a1, 0)) coefficients.a1 = 0;
            if (closeEnough(coefficients.a2, 0)) coefficients.a2 = 0;
            if (closeEnough(coefficients.a3, 0)) coefficients.a3 = 0;
            if (closeEnough(coefficients.a4, 0)) coefficients.a4 = 0;
            if (closeEnough(coefficients.b1, 0)) coefficients.b1 = 0;
            if (closeEnough(coefficients.b2, 0)) coefficients.b2 = 0;
            if (closeEnough(coefficients.b3, 0)) coefficients.b3 = 0;
            if (closeEnough(coefficients.b4, 0)) coefficients.b4 = 0;
            if (closeEnough(coefficients.c1, 0)) coefficients.c1 = 0;
            if (closeEnough(coefficients.c2, 0)) coefficients.c2 = 0;
            if (closeEnough(coefficients.c3, 0)) coefficients.c3 = 0;
            if (closeEnough(coefficients.c4, 0)) coefficients.c4 = 0;

            if (closeEnough(coefficients.a1, 0))
            {
                if (closeEnough(coefficients.b1, 0))
                    coefficients.SwapRows(0, 2);
                else
                    if (closeEnough(coefficients.c1, 0))
                    coefficients.SwapRows(0, 1);
            }

            if (!closeEnough(coefficients.a1, 0))
            {
                if (!closeEnough(coefficients.b1, 0))
                {
                    coefficients.AddRows(0, 1, -coefficients.b1 / coefficients.a1);
                }
                if (!closeEnough(coefficients.c1, 0))
                {
                    coefficients.AddRows(0, 2, -coefficients.c1 / coefficients.a1);
                }
            }

            if (closeEnough(coefficients.b2, 0))
            {
                coefficients.SwapRows(1, 2);
            }

            if (!closeEnough(coefficients.b2, 0))
            {
                if (!closeEnough(coefficients.c2, 0))
                {
                    coefficients.AddRows(1, 2, -coefficients.c2 / coefficients.b2);
                }
            }

            if (!closeEnough(coefficients.a1, 0, 0.00001f))
            {
                coefficients.ScaleRow(0, 1 / coefficients.a1);
            }
            if (!closeEnough(coefficients.b2, 0, 0.00001f))
            {
                coefficients.ScaleRow(1, 1 / coefficients.b2);
            }
            if (!closeEnough(coefficients.c3, 0, 0.00001f))
            {
                coefficients.ScaleRow(2, 1 / coefficients.c3);
            }

            var res = new Vector3();

            res.Z = coefficients.c4;
            res.Y = coefficients.b4 - (res.Z * coefficients.b3);
            res.X = coefficients.a4 - (res.Y * coefficients.a2) - (res.Z * coefficients.a3);

            return res;
        }

        TexGenEQ SolveTexGen(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            var xmatrix = new Matrix3x4(p1.X, p1.Y, p1.Z, uv1.X, p2.X, p2.Y, p2.Z, uv2.X, p3.X, p3.Y, p3.Z, uv3.X);
            var ymatrix = new Matrix3x4(p1.X, p1.Y, p1.Z, uv1.Y, p2.X, p2.Y, p2.Z, uv2.Y, p3.X, p3.Y, p3.Z, uv3.Y);

            var xsolution = Solve3VarLinearEquation(xmatrix);
            var ysolution = Solve3VarLinearEquation(ymatrix);

            var texgen = new TexGenEQ();
            texgen.planeX = new Plane(xsolution.X, xsolution.Y, xsolution.Z,0);
            texgen.planeY = new Plane(ysolution.X, ysolution.Y, ysolution.Z, 0);
            return texgen;
        }
    }
}
