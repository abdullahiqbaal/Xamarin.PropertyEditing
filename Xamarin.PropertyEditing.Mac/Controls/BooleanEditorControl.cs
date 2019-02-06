using System;
using System.Collections;
using AppKit;
using Xamarin.PropertyEditing.Mac.Resources;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal class BooleanEditorControl
		: PropertyEditorControl<PropertyViewModel<bool?>>
	{
		const string setBezelColorSelector = "setBezelColor:";

		public BooleanEditorControl (IHostResourceProvider hostResource)
			: base (hostResource)
		{
			BooleanEditor = new NSButton {
				AllowsMixedState = true,
				ControlSize = NSControlSize.Small,
				Font = NSFont.FromFontName (DefaultFontName, DefaultFontSize),
				Title = string.Empty,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			BooleanEditor.SetButtonType (NSButtonType.Switch);

			// update the value on 'enter'
			BooleanEditor.Activated += (sender, e) => {
				switch (BooleanEditor.State) {
					case NSCellStateValue.Off:
						ViewModel.Value = false;
						break;
					case NSCellStateValue.On:
						ViewModel.Value = true;
						break;
				}
			};

			AddSubview (BooleanEditor);

			this.AddConstraints (new[] {
				NSLayoutConstraint.Create (BooleanEditor, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this,  NSLayoutAttribute.Top, 1f, 6f),
				NSLayoutConstraint.Create (BooleanEditor, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this,  NSLayoutAttribute.Width, 1f, -50f),
			});
		}

		internal NSButton BooleanEditor { get; set; }

		public override NSView FirstKeyView => BooleanEditor;
		public override NSView LastKeyView => BooleanEditor;

		public string Title { 
			get { return BooleanEditor.Title; } 
			set { BooleanEditor.Title = value; } 
		}

		protected override void UpdateValue ()
		{
			if (ViewModel.Value.HasValue) {
				BooleanEditor.State = ViewModel.Value.Value ? NSCellStateValue.On : NSCellStateValue.Off;
				BooleanEditor.Title = ViewModel.Value.ToString ();
				BooleanEditor.AllowsMixedState = false;
			} else {
				BooleanEditor.AllowsMixedState = true;
				BooleanEditor.State = NSCellStateValue.Mixed;
				BooleanEditor.Title = string.Empty;
			}
		}

		protected override void SetEnabled ()
		{
			BooleanEditor.Enabled = ViewModel.Property.CanWrite;
		}

		protected override void UpdateAccessibilityValues ()
		{
			BooleanEditor.AccessibilityEnabled = BooleanEditor.Enabled;
			BooleanEditor.AccessibilityTitle = string.Format (LocalizationResources.AccessibilityBoolean, ViewModel.Property.Name);
		}
	}
}
