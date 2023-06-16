using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelWorldsServer2.ItemsData.Door
{
    public interface IDoor
    {
        void SetIsLocked(bool newValue);

        bool GetIsLocked();
    }
}
