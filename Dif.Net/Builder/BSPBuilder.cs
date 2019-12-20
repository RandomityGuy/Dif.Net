using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using KdTree;

namespace Dif.Net.Builder
{
    public static class PlaneExtensions
    {
        public static void FromPtNormal(this Plane p,Vector3 point, Vector3 normal)
        {
            p.Normal = normal;
            float w = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            p.Normal.X /= w;
            p.Normal.Y /= w;
            p.Normal.Z /= w;
            //normal = glm::normalize(normal);

            p.D = -(Vector3.Dot(point, normal));
        }
    }

    public class BSPBuilder
    {
        public class BSPNode
        {
            public bool IsLeaf = false;
            public Plane Plane;
            public BSPNode Front;
            public BSPNode Back;
            public Polygon Polygon;
            public Vector3 Center;

            public void CalculateCenter()
            {
                if (IsLeaf)
                {
                    var centroid = new Vector3();
                    Polygon.Vertices.ForEach(a => centroid += a);
                    centroid /= Polygon.Vertices.Count;

                    Center = centroid;
                }
                else
                {
                    int c = 0;
                    var avgcenter = new Vector3();
                    if (Back != null)
                    {
                        if (Back.Center == null)
                            Back.CalculateCenter();

                        avgcenter += Back.Center;
                        c++;
                    }
                    if (Front != null)
                    {
                        if (Front.Center == null)
                            Front.CalculateCenter();

                        avgcenter += Front.Center;
                        c++;
                    }
                    avgcenter /= c;
                    Center = avgcenter;
                }
            }

            public List<Polygon> GatherPolygons()
            {
                var retlist = new List<Polygon>();
                if (IsLeaf)
                    retlist.Add(Polygon);
                else
                {
                    if (Front != null)
                        retlist.AddRange(Front.GatherPolygons());
                    if (Back != null)
                        retlist.AddRange(Back.GatherPolygons());
                }
                return retlist;
                        
            }
        }

        int hashPoint(Vector3 p)
        {
            return p.X.GetHashCode() ^ p.Y.GetHashCode() ^ p.Z.GetHashCode();
        }

        List<BSPNode> BuildBSP(List<BSPNode> nodes)
        {
            var pts = new List<Vector3>();
            var centertobspmap = new Dictionary<int,BSPNode>();
            foreach (var node in nodes)
            {
                node.CalculateCenter();
                centertobspmap.Add(hashPoint(node.Center), node);
                pts.Add(node.Center);
            }

            var kdtree = new KdTree<float, Vector3>(3, new KdTree.Math.FloatMath());

            var newnodes = new List<BSPNode>();

            var containedptlist = new HashSet<int>();
            foreach (var pt in pts)
            {
                kdtree.Add(new float[] { pt.X, pt.Y, pt.Z }, pt);
            }

            for (var i = 0; i < pts.Count;i++)
            {
                if (containedptlist.Contains(hashPoint(pts[i])))
                    continue;

                if (i == pts.Count - 1)
                {
                    newnodes.Add(centertobspmap[hashPoint(pts[i])]);
                    break;
                }

                var pt = pts[i];
                kdtree.RemoveAt(new float[] { pt.X, pt.Y, pt.Z });

                var nn = kdtree.GetNearestNeighbours(new float[] { pt.X, pt.Y, pt.Z }, 1)[0];

                var nb = nn.Value;

                containedptlist.Add(hashPoint(nb));

                var center = (pt + nb) * 0.5f;

                var p = new Plane();
                p.FromPtNormal(center, nb - pt);

                var node = new BSPNode();
                node.Center = center;
                node.Front = centertobspmap[hashPoint(nb)];
                node.Back = centertobspmap[hashPoint(pt)];
                node.Plane = p;

                newnodes.Add(node);
            }

            return newnodes;

        }

        public BSPNode BuildBSPRecursive(List<BSPNode> nodes)
        {
            while (nodes.Count > 1)
                nodes = BuildBSP(nodes);
            return nodes[0];
        }
    }
}
