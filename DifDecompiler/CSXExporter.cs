using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Dif.Net;
using System.Numerics;
using Dif.Net.Builder;

namespace DifDecompiler
{
    public class CSXExporter
    {
        struct CSXDocNodes
        {
            internal XElement document;
            internal XElement brushes;
        }

        Interior interior;

        int currentFaceID = 0;
        int currentBrushID = 0;

        CSXDocNodes WriteCSXHeaderAndFooter()
        {
            var cscene = XElement.Parse("<ConstructorScene version=\"4\" creator=\"Torque Constructor\" date=\"2013 / 09 / 13 18:31:22\"/>");
            //cscene.Add(new XDeclaration("1.0", "utf-8", "yes"));
            cscene.Add(new XComment("Torque Constructor Scene document http://www.garagegames.com"));
            cscene.Add(XElement.Parse("<Sunlight azimuth=\"180\" elevation=\"35\" color=\"255 255 255\" ambient=\"64 64 64\" />"));
            cscene.Add(XElement.Parse(@"<LightingOptions lightingSystem="""" ineditor_defaultLightmapSize=""256"" ineditor_maxLightmapSize=""256"" ineditor_lightingPerformanceHint=""0"" ineditor_shadowPerformanceHint=""1"" ineditor_TAPCompatibility=""0"" ineditor_useSunlight=""0"" export_defaultLightmapSize=""256"" export_maxLightmapSize=""256"" export_lightingPerformanceHint=""0"" export_shadowPerformanceHint=""1"" export_TAPCompatibility=""0"" export_useSunlight=""0"" />"));
            cscene.Add(new XElement("GameTypes",
                new XElement("GameType", new XAttribute("name", "Constructor")),
                new XElement("GameType", new XAttribute("name", "Torque"))
                ));
            var details = new XElement("DetailLevels", new XAttribute("current", "0"));
            cscene.Add(details);
            var detail = new XElement("DetailLevel", new XAttribute("minPixelSize", "0"), new XAttribute("actionCenter", "0 0 0"));
            details.Add(detail);

            var imap = XElement.Parse("<InteriorMap brushScale=\"32\" lightScale=\"32\" ambientColor=\"0 0 0\" ambientColorEmerg=\"0 0 0\"/>");
            detail.Add(imap);

            var entities = new XElement("Entities", new XAttribute("nextEntityID", "1"),
                new XElement("Entity", new XAttribute("id", "0"), new XAttribute("classname", "worldspawn"), new XAttribute("gametype", "Torque"), new XAttribute("isPointEntity", "0"),
                XElement.Parse(@"<Properties detail_number=""0"" min_pixels=""250"" geometry_scale=""32.0"" light_geometry_scale=""8.0"" light_smoothing_scale=""4.0"" light_mesh_scale=""1.0"" ambient_color=""0 0 0"" emergency_ambient_color=""0 0 0"" mapversion=""220"" />")
                ));

            imap.Add(entities);

            var brushes = new XElement("Brushes");
            imap.Add(brushes);

            return new CSXDocNodes() { document = cscene, brushes = brushes };

        }

        public void WriteBrush(XElement brushes,ConvexHull hull)
        {
            var brush = new XElement("Brush");
            brush.Add(new XAttribute("id", currentBrushID.ToString()));
            currentBrushID++;
            brush.Add(new XAttribute("owner", "0"));
            brush.Add(new XAttribute("type", "0"));

            var hullpointsdoublearray = interior.hullSurfaceIndices.Skip(hull.surfaceStart).Take(hull.surfaceCount).Select(c =>
            {
                return (c & 0x8000) == 0x8000 ? interior.surfaces[c & ~0x8000] : interior.surfaces[c];
            }).Select(a => interior.windings.Skip(a.windingStart).Take(a.windingCount).Select(b => interior.points[b]));
            var hullpoints = new List<Vector3>();
            hullpointsdoublearray.ToList().ForEach(a => a.ToList().ForEach(b => hullpoints.Add(b)));
           
            var centre = Vector3.Zero;

            hullpoints.ForEach(a => centre += a);
            centre /= hullpoints.Count;

            brush.Add(new XAttribute("pos", centre.X.ToString() + " " + centre.Y + " " + centre.Z));
            brush.Add(new XAttribute("rot", "1 0 0 0"));
            brush.Add(new XAttribute("scale", ""));
            brush.Add(new XAttribute("transform", "1 0 0 " + centre.X + " 0 1 0 " + centre.Y + " 0 0 1" + centre.Z + " 0 0 0 1"));
            brush.Add(new XAttribute("group", "-1"));
            brush.Add(new XAttribute("locked", "0"));
            brush.Add(new XAttribute("nextFaceID", interior.surfaces.Count.ToString()));
            brush.Add(new XAttribute("nextVertexID", hullpoints.Count.ToString()));

            var vertices = new XElement("Vertices");
            foreach (var vertex in hullpoints)
            {
                var relpos = vertex - centre;
                vertices.Add(new XElement("Vertex", new XAttribute("pos", relpos.X.ToString() + " " + relpos.Y + " " + relpos.Z)));
            }

            brush.Add(vertices);

            var surfaceindices = interior.hullSurfaceIndices.Skip(hull.surfaceStart).Take(hull.surfaceCount);

            var surfaces = new List<Surface>();

            foreach (var index in surfaceindices)
                if ((index & 0x80000000) != 0x80000000)
                    surfaces.Add(interior.surfaces[index]);

            foreach (var surface in surfaces)
            {
                var indices = interior.windings.Skip(surface.windingStart).Take(surface.windingCount).ToList();

                for (var i = 1; i < surface.windingCount - 1; i ++)
                {
                    var face = new XElement("Face");
                    face.Add(new XAttribute("id", currentFaceID.ToString()));
                    currentFaceID++;
                    var isplaneflipped = (surface.planeIndex & 0x8000) == 0x8000;
                    var plane = isplaneflipped ? interior.planes[(short)unchecked(surface.planeIndex & (short)~0x8000)] : interior.planes[surface.planeIndex];
                    var norm = isplaneflipped ? interior.normals[plane.normalIndex] * -1 : interior.normals[plane.normalIndex];

                    face.Add(new XAttribute("plane", norm.X.ToString() + " " + norm.Y + " " + norm.Z + " " + plane.planeDistance));
                    face.Add(new XAttribute("material", interior.materialList[surface.textureIndex]));

                    var texgen = interior.texGenEQs[surface.texGenIndex];
                    face.Add(new XAttribute("texgens", texgen.planeX.Normal.X.ToString() + " " + texgen.planeX.Normal.Y + " " + texgen.planeX.Normal.Z + " " + texgen.planeX.D + " " + texgen.planeY.Normal.X + " " + texgen.planeY.Normal.Y + " " + texgen.planeY.Normal.Z + " " + texgen.planeY.D + " 0 1 1"));
                    face.Add(new XAttribute("texRot", "0"));
                    face.Add(new XAttribute("texScale", "1 1"));
                    face.Add(new XAttribute("texDiv", "128 128"));

                    var indicestr = hullpoints.IndexOf(interior.points[indices[0]]).ToString() + " " + hullpoints.IndexOf(interior.points[indices[i]]) + " " + hullpoints.IndexOf(interior.points[indices[i + 1]]);


                    face.Add(new XElement("Indices", new XAttribute("indices", indicestr)));

                    brush.Add(face);
                }
            }

            brushes.Add(brush);

        }

        public void ExportCSX(Interior interior,TextWriter w)
        {
            this.interior = interior;

            var docnodes = WriteCSXHeaderAndFooter();

            for (int i = 0; i < interior.convexHulls.Count; i++)
            {
                ConvexHull hull = interior.convexHulls[i];
                WriteBrush(docnodes.brushes, hull);
            }

            docnodes.document.Save(w);
         
        }

        //Solution by HiGuy
        bool closeEnough(Vector3 p1, Vector3 p2, float dist = 0.0001f)
        {
            return (p2 - p1).Length() < dist;
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
            texgen.planeX = new Dif.Net.Plane(xsolution.X, xsolution.Y, xsolution.Z, 0);
            texgen.planeY = new Dif.Net.Plane(ysolution.X, ysolution.Y, ysolution.Z, 0);
            return texgen;
        }
    }
}
