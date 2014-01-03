// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace NicCage.Tests
{
	[Register ("TestController")]
	partial class TestController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton Search { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Text { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField TextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Search != null) {
				Search.Dispose ();
				Search = null;
			}

			if (Text != null) {
				Text.Dispose ();
				Text = null;
			}

			if (TextField != null) {
				TextField.Dispose ();
				TextField = null;
			}
		}
	}
}
