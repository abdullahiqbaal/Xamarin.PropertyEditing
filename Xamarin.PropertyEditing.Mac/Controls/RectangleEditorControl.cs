﻿using System;
using System.Drawing;
using AppKit;
using CoreGraphics;
using Xamarin.PropertyEditing.Drawing;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal abstract class RectangleEditorControl<T>
		: BaseRectangleEditorControl<T>
	{
		protected RectangleEditorControl (IHostResourceProvider hostResources)
			: base (hostResources)
		{
			// TODO localize
			XLabel.Frame = new CGRect (34, 28, 25, 22);
			XLabel.Font = NSFont.FromFontName (DefaultFontName, DefaultDescriptionLabelFontSize); // TODO: Washed-out color following specs
			XLabel.StringValue = "X";

			XEditor.Frame = new CGRect (0, 46, 90, 20);

			YLabel.Frame = new CGRect (166, 28, 25, 22);
			YLabel.Font = NSFont.FromFontName (DefaultFontName, DefaultDescriptionLabelFontSize); // TODO: Washed-out color following specs
			YLabel.StringValue = "Y";

			YEditor.Frame = new CGRect (132, 46, 90, 20);

			WidthLabel.Frame = new CGRect (20, -5, 50, 22);
			WidthLabel.Font = NSFont.FromFontName (DefaultFontName, DefaultDescriptionLabelFontSize); // TODO: Washed-out color following specs
			WidthLabel.StringValue = "WIDTH";

			WidthEditor.Frame = new CGRect (0, 13, 90, 20);

			HeightLabel.Frame = new CGRect (150, -5, 50, 22);
			HeightLabel.Font = NSFont.FromFontName (DefaultFontName, DefaultDescriptionLabelFontSize); // TODO: Washed-out color following specs
			HeightLabel.StringValue = "HEIGHT";

			HeightEditor.Frame = new CGRect (132, 13, 90, 20);
		}

		public override nint GetHeight (EditorViewModel vm)
		{
			return 66;
		}
	}

	internal class SystemRectangleEditorControl
		: RectangleEditorControl<Rectangle>
	{
		public SystemRectangleEditorControl (IHostResourceProvider hostResources)
			: base (hostResources)
		{
		}

		protected override void UpdateValue ()
		{
			XEditor.Value = ViewModel.Value.X;
			YEditor.Value = ViewModel.Value.Y;
			WidthEditor.Value = ViewModel.Value.Width;
			HeightEditor.Value = ViewModel.Value.Height;
		}
	}

	internal class CommonRectangleEditorControl
		: RectangleEditorControl<CommonRectangle>
	{
		public CommonRectangleEditorControl (IHostResourceProvider hostResources)
			: base (hostResources)
		{
		}

		protected override void UpdateValue ()
		{
			XEditor.Value = ViewModel.Value.X;
			YEditor.Value = ViewModel.Value.Y;
			WidthEditor.Value = ViewModel.Value.Width;
			HeightEditor.Value = ViewModel.Value.Height;
		}
	}
}