using System;
using Xilium.CefGlue;

namespace CefGlueScreenshot.Offscreen
{
    public sealed class SourceVisitor : CefStringVisitor
    {
        private readonly Action<string> _callback;

        public SourceVisitor(Action<string> callback)
        {
            _callback = callback;
        }

        protected override void Visit(string value)
        {
            _callback(value);
        }

    }
}