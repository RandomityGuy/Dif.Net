using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public interface IWritable
    {
        void Write(BinaryWriter wb);
    }
}
