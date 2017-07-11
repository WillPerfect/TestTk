using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkHome
{
    public class Configure
    {
        public bool Reconnect { get; set; }
        public int ReconnectDelaySeconds { get; set; }

        public int CollectStartTime { get; set; }
        public int CollectEndTime { get; set; }
        public int CollectInterval { get; set; }

        public int QunfaStartTime { get; set; }
        public int QunfaEndTime { get; set; }
        public int QunfaInterval { get; set; }
    }
}
