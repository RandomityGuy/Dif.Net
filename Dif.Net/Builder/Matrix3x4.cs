using System;
using System.Collections.Generic;
using System.Text;

namespace Dif.Net.Builder
{
    public class Matrix3x4
    {
        public float a1 = 1;
        public float a2 = 0;
        public float a3 = 0;
        public float a4 = 0;
        public float b1 = 0;
        public float b2 = 1;
        public float b3 = 0;
        public float b4 = 0;
        public float c1 = 0;
        public float c2 = 0;
        public float c3 = 1;
        public float c4 = 0;

        public Matrix3x4(float a1, float a2, float a3, float a4, float b1, float b2, float b3, float b4, float c1, float c2, float c3, float c4)
        {
            this.a1 = a1;
            this.a2 = a2;
            this.a3 = a3;
            this.a4 = a4;
            this.b1 = b1;
            this.b2 = b2;
            this.b3 = b3;
            this.b4 = b4;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
            this.c4 = c4;
        }

        public Matrix3x4()
        {
            
        }

        public float Determinant3x3()
        {
            return (a1 * b2 * c3) + (a2 * b3 * c1) + (a3 * b1 * c2) - (a3 * b2 * c1) - (a2 * b1 * c3) - (a1 * b3 * c2);
        }

        public void SwapRows(int row1,int row2)
        {
            float r1, r2, r3, r4;
            if (row1 == 0)
            {
                if (row2 == 1)
                {
                    r1 = a1;
                    r2 = a2;
                    r3 = a3;
                    r4 = a4;
                    a1 = b1;
                    a2 = b2;
                    a3 = b3;
                    a4 = b4;
                    b1 = r1;
                    b2 = r2;
                    b3 = r3;
                    b4 = r4;
                }
                if (row2 == 2)
                {
                    r1 = a1;
                    r2 = a2;
                    r3 = a3;
                    r4 = a4;
                    a1 = c1;
                    a2 = c2;
                    a3 = c3;
                    a4 = c4;
                    c1 = r1;
                    c2 = r2;
                    c3 = r3;
                    c4 = r4;
                }
            }
            if (row1 == 1)
            {
                if (row2 == 0)
                {
                    r1 = b1;
                    r2 = b2;
                    r3 = b3;
                    r4 = b4;
                    b1 = a1;
                    b2 = a2;
                    b3 = a3;
                    b4 = a4;
                    a1 = r1;
                    a2 = r2;
                    a3 = r3;
                    a4 = r4;
                }
                if (row2 == 2)
                {
                    r1 = b1;
                    r2 = b2;
                    r3 = b3;
                    r4 = b4;
                    b1 = c1;
                    b2 = c2;
                    b3 = c3;
                    b4 = c4;
                    c1 = r1;
                    c2 = r2;
                    c3 = r3;
                    c4 = r4;
                }
            }
            if (row1 == 2)
            {
                if (row2 == 0)
                {
                    r1 = c1;
                    r2 = c2;
                    r3 = c3;
                    r4 = c4;
                    c1 = a1;
                    c2 = a2;
                    c3 = a3;
                    c4 = a4;
                    a1 = r1;
                    a2 = r2;
                    a3 = r3;
                    a4 = r4;
                }
                if (row2 == 1)
                {
                    r1 = c1;
                    r2 = c2;
                    r3 = c3;
                    r4 = c4;
                    c1 = b1;
                    c2 = b2;
                    c3 = b3;
                    c4 = b4;
                    b1 = r1;
                    b2 = r2;
                    b3 = r3;
                    b4 = r4;
                }
            }
        }

        public void ScaleRow(int row,float scale)
        {
            if (row == 0)
            {
                a1 *= scale;
                a2 *= scale;
                a3 *= scale;
                a4 *= scale;
            }
            if (row == 1)
            {
                b1 *= scale;
                b2 *= scale;
                b3 *= scale;
                b4 *= scale;
            }
            if (row == 2)
            {
                c1 *= scale;
                c2 *= scale;
                c3 *= scale;
                c4 *= scale;
            }
        }

        public void AddRows(int srcrow,int destrow,float scale)
        {
            float r1 = 0, r2 = 0, r3 = 0, r4 = 0;
            if (srcrow == 0)
            {
                r1 = a1 * scale;
                r2 = a2 * scale;
                r3 = a3 * scale;
                r4 = a4 * scale;
            }
            if (srcrow == 1)
            {
                r1 = b1 * scale;
                r2 = b2 * scale;
                r3 = b3 * scale;
                r4 = b4 * scale;
            }
            if (srcrow == 2)
            {
                r1 = c1 * scale;
                r2 = c2 * scale;
                r3 = c3 * scale;
                r4 = c4 * scale;
            }

            if (destrow == 0)
            {
                a1 += r1;
                a2 += r2;
                a3 += r3;
                a4 += r4;
            }
            if (destrow == 1)
            {
                b1 += r1;
                b2 += r2;
                b3 += r3;
                b4 += r4;
            }
            if (destrow == 2)
            {
                c1 += r1;
                c2 += r2;
                c3 += r3;
                c4 += r4;
            }
        }



        //public static Matrix3x4 operator *(float lhs,Matrix3x3 rhs)
        //{
        //    return new Matrix3x3(rhs.a1 * lhs, rhs.a2 * lhs, rhs.a3 * lhs, rhs.b1 * lhs, rhs.b2 * lhs, rhs.b3 * lhs, rhs.c1 * lhs, rhs.c2 * lhs, rhs.c3 * lhs);
        //}

    }
}
