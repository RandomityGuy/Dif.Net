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
            var interior = InteriorResource.Load("test.dif");
            InteriorResource.Save(interior, "test3.dif");
            var interior2 = InteriorResource.Load("test3.dif");
        }
    }
}
