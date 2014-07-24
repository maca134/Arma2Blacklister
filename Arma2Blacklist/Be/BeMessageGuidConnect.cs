using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arma2Blacklist
{
    class BeMessageGuidConnect : BeMessageDefault
    {
        public short player_id { get; set; }

        public string guid { get; set; }
    }
}
