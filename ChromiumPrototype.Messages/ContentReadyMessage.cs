using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromiumPrototype.Messages
{
    public class ContentReadyMessage
    {
        public Guid SessionId { get; set; }
        public string FileName { get; set; }
    }
}
