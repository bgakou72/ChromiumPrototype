using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Xilium.CefGlue;

namespace CefGlueScreenshot.Offscreen {
	internal class OffscreenRenderHandler : CefRenderHandler {
		private readonly OffscreenBrowser offscreenBrowser;

		public OffscreenRenderHandler(OffscreenBrowser offscreenBrowser) {
			this.offscreenBrowser = offscreenBrowser;
		}

		protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height) {
            
		}

		protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo) {
			return false;
		}

		protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect) {
			rect.X = 0;
			rect.Y = 0;
			rect.Width = offscreenBrowser.Width;
			rect.Height = offscreenBrowser.Height;
			return true;
		}

		protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY) {
			screenX = viewX;
			screenY = viewY;
			return true;
		}

		protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect) {
			rect = new CefRectangle(0, 0, offscreenBrowser.Width, offscreenBrowser.Height);
			return true;
		}

		protected override void OnPopupSize(CefBrowser browser, CefRectangle rect) { }

		protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo) { }

		protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y) { }
	}
}