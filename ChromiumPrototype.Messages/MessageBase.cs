using System;

namespace ChromiumPrototype.Messages
{
    public abstract class MessageBase
    {
        public Guid SessionId { get; set; }
    }
}