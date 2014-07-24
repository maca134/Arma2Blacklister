using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arma2Blacklist
{
    class BeMessageIpConnect : BeMessageDefault
    {
        public short player_id { get; set; }

        public string ip { get; set; }
    }
}
