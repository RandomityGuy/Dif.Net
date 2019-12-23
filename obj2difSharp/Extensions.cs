using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace obj2difSharp
{
    public static class Extensions
    {
        public static string GetWord(this string str,int n)
        {
            return str.Split(' ')[n];
        }

        public static string GetWords(this string str, int start, int end)
        {
            var words = str.Split(' ').ToList();
            return words.GetRange(start, end - start + 1).Aggregate((a, b) => a + " " + b);
        }

        public static string GetWords(this string str,int start)
        {
            var words = str.Split(' ').ToList();
            return words.GetRange(start, words.Count - start).Aggregate((a, b) => a + " " + b);
        }

        public static Vector3 FromString(string vs)
        {
            return new Vector3(float.Parse(vs.GetWord(0)), float.Parse(vs.GetWord(1)), float.Parse(vs.GetWord(2)));
        }

        public static Vector2 FromStringV2(string vs)
        {
            return new Vector2(float.Parse(vs.GetWord(0)), float.Parse(vs.GetWord(1)));
        }

    }
}
