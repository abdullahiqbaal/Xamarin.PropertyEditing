using System;
using System.Drawing;
using AppKit;
using CoreGraphics;
using Xamarin.PropertyEditing.Drawing;
using Xamarin.PropertyEditing.Mac.Resources;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal abstract class PointEditorControl<T> : BasePointEditorControl<T>
	{
		public PointEditorControl (IHostResourceProvider hostResources)
			: base (hostResources)
		{
			XLabel.StringValue = "X"; // TODO Localise

			XEditor.Frame = new CGRect (0, 13, 90, 20);
			
			YLabel.StringValue = "Y"; // TODO Localise

			YEditor.Frame = new CGRect (132, 13, 90, 20);

			this.AddConstraints (new[] {
				NSLayoutConstraint.Create (XLabel, NSLayoutAttribute.Left, NSLayoutRelation.Equal, XEditor, NSLayoutAttribute.CenterX, 1f, -10f),
				NSLayoutConstraint.Create (YLabel, NSLayoutAttribute.Left, NSLayoutRelation.Equal, YEditor, NSLayoutAttribute.CenterX, 1f, -10f),
			});
		}

		public override nint GetHeight (EditorViewModel vm)
		{
			return 33;
		}
	}

	internal class SystemPointEditorControl
		: PointEditorControl<Point>
	{
		public SystemPointEditorControl (IHostResourceProvider hostResources)
			: base (hostResources)
		{
		}

		protected override void UpdateValue ()
		{
			XEditor.Value = ViewModel.Value.X;
			YEditor.Value = ViewModel.Value.Y;
		}
	}

	internal class CommonPointEditorControl : PointEditorControl<CommonPoint>
	{
		public CommonPointEditorControl (IHostResourceProvider hostResource)
			: base (hostResource)
		{
		}

		protected override void UpdateValue ()
		{
			XEditor.Value = ViewModel.Value.X;
			YEditor.Value = ViewModel.Value.Y;
		}
	}
}
