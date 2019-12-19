using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dif.Net;

namespace DifNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var interior = InteriorResource.Load("beginner_finish.dif");
            InteriorResource.Save(interior, "savetest.dif");
        }
    }
}
