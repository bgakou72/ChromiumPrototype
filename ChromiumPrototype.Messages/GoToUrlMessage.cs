using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromiumPrototype.Messages
{
    public class GoToUrlMessage : MessageBase
    {
        public string Url { get; set; }
    }
}
