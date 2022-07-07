using System;
using System.Collections.Generic;
using System.Text;

namespace Dif.Net.Builder
{
    public class PointList
    {
        public struct Point
        {
            public double x;
            public double y;
            public double z;
        }

        public List<Point> pts = new List<Point>();

		public int kdtree_get_point_count() 
		{
			return pts.Count; 
		}

		public double kdtree_get_pt(int idx, int dim)
		{
			if (dim == 0) return pts[idx].x;
			else if (dim == 1) return pts[idx].y;
			else return pts[idx].z;
		}

		public bool kdtree_get_bbox() => false;

		public void copyfrom(List<Point> pts)
		{
			this.pts.AddRange(pts);
		}

    }

    public class L2SimpleAdaptor
    {
        PointList data_source = new PointList();

        public L2SimpleAdaptor(PointList data_source)
        {
            this.data_source = data_source;
        }

        public double evalMetric(List<double> a, int b_idx, int size)
        {
            var result = 0d;
            for (var i = 0; i < size; ++i)
            {
                var diff = a[i] - data_source.kdtree_get_pt(b_idx, i);
                result += diff * diff;
            }
            return result;
        }

    }

  }

