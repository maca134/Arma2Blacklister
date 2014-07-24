using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arma2Blacklist
{
    class BeMessageDisconnect : BeMessageDefault
    {
        public short player_id { get; set; }
    }
}
