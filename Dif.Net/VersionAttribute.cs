using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false,Inherited = false)]
    public class VersionAttribute : Attribute
    {
        public int MinVersion = 0;
        public int MaxVersion = 14;
        public int[] Exclusions;

        public VersionAttribute(int min = 0,int max = 14,int[] exclusions = null)
        {
            MinVersion = min;
            MaxVersion = max;
            Exclusions = exclusions;
        }
    }
}
