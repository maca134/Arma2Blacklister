using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arma2Blacklist
{
    class BeMessageKick : BeMessageDefault
    {
        public short player_id { get; set; }

        public string player_guid { get; set; }

        public string message { get; set; }
    }
}
