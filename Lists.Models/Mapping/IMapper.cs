using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lists.Models.Mapping
{
    public interface IMapper<in Tin, out Tout>
    {
        Tout Convert(Tin source);
    }
}
