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

    public class BSPBuilder
    {
        public class BSPNode
        {
            public bool IsLeaf = false;
            public Plane Plane = new Plane();
            public BSPNode Front;
            public BSPNode Back;
            public Polygon Polygon;
            public Vector3? Center = null;

            public int FrontDepth
            {
                get
                {
                    if (Front == null)
                        return 0;
                    else
                        return 1 + Front.FrontDepth;
                }
            }

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
                        if (!Back.Center.HasValue)
                            Back.CalculateCenter();

                        avgcenter += Back.Center.Value;
                        c++;
                    }
                    if (Front != null)
                    {
                        if (!Front.Center.HasValue)
                            Front.CalculateCenter();

                        avgcenter += Front.Center.Value;
                        c++;
                    }
                    avgcenter /= c;
                    Center = avgcenter;
                }
            }

            public void GatherPolygons(ref List<Polygon> polys)
            {
                if (IsLeaf)
                    polys.Add(Polygon);
                else
                {
                    if (Front != null)
                        Front.GatherPolygons(ref polys);
                    if (Back != null)
                        Back.GatherPolygons(ref polys);
                }
                        
            }

            public int Count
            {
                get
                {
                    int count = 0;
                    if (Front != null)
                    {
                        if (!Front.IsLeaf)
                        {
                            count += Front.Count;
                            count++;
                        }
                    }
                    if (Back != null)
                    {
                        if (!Back.IsLeaf)
                        {
                            count += Back.Count;
                            count++;
                        }
                    }

                    return count;
                }
            }

            public string Serialize()
            {
                return $"{{ IsLeaf : \"{IsLeaf.ToString()}\" , Plane : \"{Plane.Normal.X.ToString() + " " + Plane.Normal.Y + " " + Plane.Normal.Z + " " + Plane.D}\", Front : {(Front == null ? "null" : Front.Serialize())}, Back : {(Back == null ? "null" : Back.Serialize())}, Center : \"{Center.Value.ToString()}\" }}";
            }
        }

        int hashPoint(Vector3 p)
        {
            return p.GetHashCode();//p.X.GetHashCode() ^ p.Y.GetHashCode() ^ p.Z.GetHashCode();
        }

        //Basically, the algorithm is recursively group up 2 closest bsp nodes into a single node till only 1 remains
        List<BSPNode> BuildBSP(List<BSPNode> nodes)
        {
            var kdtree = new KdTree<float, BSPNode>(3, new KdTree.Math.FloatMath(),AddDuplicateBehavior.Skip);

            //Calculate all the centers of nodes
            foreach (var node in nodes)
            {
                node.CalculateCenter();
                kdtree.Add(new float[] { node.Center.Value.X, node.Center.Value.Y, node.Center.Value.Z }, node);
            }

            var newnodes = new List<BSPNode>();

            var containednodelist = new HashSet<BSPNode>();

            BSPNode previousnode = null;

            //Now to pair up nearest nodes
            for (var i = 0; i < nodes.Count; i++)
            {
                //We already used this node up
                if (containednodelist.Contains(nodes[i]))
                    continue;


                var node = nodes[i];
                var pt = node.Center.Value;
                //We dont want to find this very node
                kdtree.RemoveAt(new float[] { pt.X, pt.Y, pt.Z });
                //Find the nearest node
                var nn = kdtree.GetNearestNeighbours(new float[] { pt.X, pt.Y, pt.Z }, 1);

                BSPNode nb;

                //No nodes found
                if (nn.Length == 0)
                {
                    //Well we just use the previously searched node again
                    //The reason why this is necessary is not known but apparently, doing this
                    //actually fixes the whole bsp generation and the dif generated is proper
                    //nb = previousnode;
                    newnodes.Add(nodes[i]);
                    break;
                    //newnodes.Add(previousnode);
                }
                else
                {
                    previousnode = nn[0].Value;

                    nb = nn[0].Value;

                    containednodelist.Add(nb);
                }

                //The centre of the new node is the mean of both nodes
                var center = (pt + nb.Center.Value) * 0.5f;

                //Construct the plane
                var p = new Plane(center, nb.Center.Value - pt);

                //Construct the node
                var newnode = new BSPNode();
                newnode.Center = center;
                newnode.Front = nb;
                newnode.Back = node;
                newnode.Plane = p;

                //Remove the node center from the search tree
                kdtree.RemoveAt(new float[] { nb.Center.Value.X, nb.Center.Value.Y, nb.Center.Value.Z });

                //Add the node in
                newnodes.Add(newnode);
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
