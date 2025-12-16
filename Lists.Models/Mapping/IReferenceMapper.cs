using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Mapping
{
    public interface IReferenceMapper<Tin, Tout>
    {
        void Convert(Tin source, ref Tout result);
    }
}
