using System;
using System.Linq;
using AppKit;
using CoreGraphics;
using Foundation;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal class BasePanelWindow : NSPanel
	{
		protected NSButton ButtonDone;
		private NSButton buttonCancel;
		private NSButton buttonHelp;

		private PropertyViewModel propertyViewModel;

		protected NSPopUpButton BindingTypePopup;

		protected NSPopUpButton ValueConverterPopup;

		protected NSButton AddConverterButton;

		protected NSView AncestorTypeBox;
		protected NSView PathBox;

		internal BasePanelWindow (PropertyViewModel propertyViewModel)
		{
			this.propertyViewModel = propertyViewModel;

			FloatingPanel = true;

			StyleMask |= NSWindowStyle.Resizable;

			MaxSize = new CGSize (960, 720); // TODO discuss what the Max/Min Size should be and if we should have one.
			MinSize = new CGSize (320, 240);

			var container = new NSView (new CGRect (CGPoint.Empty, new CGSize (728, 440))) {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			this.ButtonDone = new NSButton {
				BezelStyle = NSBezelStyle.Rounded,
				Highlighted = true,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			this.ButtonDone.KeyEquivalent = "\r"; // Fire when enter pressed

			container.AddSubview (this.ButtonDone);

			this.buttonCancel = new NSButton {
				BezelStyle = NSBezelStyle.Rounded,
				Title = Properties.Resources.Cancel,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			this.buttonCancel.Activated += (sender, e) => {
				Close ();
			};

			container.AddSubview (this.buttonCancel);

			this.buttonHelp = new NSButton {
				BezelStyle = NSBezelStyle.HelpButton,
				Title = string.Empty,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			container.AddSubview (this.buttonHelp);

			var bindingTypeLabel = new UnfocusableTextField {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Alignment = NSTextAlignment.Right,
			};

			bindingTypeLabel.StringValue = Properties.Resources.BindingType;
			container.AddSubview (bindingTypeLabel);

			this.BindingTypePopup = new NSPopUpButton {
				TranslatesAutoresizingMaskIntoConstraints = false,
				StringValue = String.Empty,
				ControlSize = NSControlSize.Small,
				Font = NSFont.FromFontName (PropertyEditorControl.DefaultFontName, PropertyEditorControl.DefaultFontSize),
			};

			var bindingTypeMenuList = new NSMenu ();
			this.BindingTypePopup.Menu = bindingTypeMenuList;
			container.AddSubview (this.BindingTypePopup);

			var valueConverterLabel = new UnfocusableTextField {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Alignment = NSTextAlignment.Right,
			};

			this.AncestorTypeBox = new NSView {
				TranslatesAutoresizingMaskIntoConstraints = false, 
				WantsLayer = true,

				// Layer out of alphabetical order so that WantsLayer creates the layer first
				Layer = {
					CornerRadius = 1.0f,
					BorderColor = new CGColor (.5f, .5f, .5f, .5f),
					BorderWidth = 1,
				},
			};

			container.AddSubview (this.AncestorTypeBox);

			this.PathBox = new NSView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				WantsLayer = true,

				// Layer out of alphabetical order so that WantsLayer creates the layer first
				Layer = {
					CornerRadius = 1.0f,
					BorderColor = new CGColor (.5f, .5f, .5f, .5f),
					BorderWidth = 1,
				},
			};

			container.AddSubview (this.PathBox);

			valueConverterLabel.StringValue = Properties.Resources.Converter;
			container.AddSubview (valueConverterLabel);

			this.ValueConverterPopup = new NSPopUpButton {
				TranslatesAutoresizingMaskIntoConstraints = false,
				StringValue = String.Empty,
				ControlSize = NSControlSize.Small,
				Font = NSFont.FromFontName (PropertyEditorControl.DefaultFontName, PropertyEditorControl.DefaultFontSize),
			};

			var valueConverterMenuList = new NSMenu ();
			this.ValueConverterPopup.Menu = valueConverterMenuList;
			container.AddSubview (this.ValueConverterPopup);

			this.AddConverterButton = new NSButton {
				BezelStyle = NSBezelStyle.Rounded,
				Image = NSImage.ImageNamed (NSImageName.AddTemplate),
				Title = string.Empty,
				ToolTip = Properties.Resources.AddValueConverterEllipsis,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			container.AddSubview (this.AddConverterButton);

			var buttonMoreSettings = new NSButton {
				BezelStyle = NSBezelStyle.Disclosure,
				Title = string.Empty,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			buttonMoreSettings.SetButtonType (NSButtonType.PushOnPushOff);

			container.AddSubview (buttonMoreSettings);

			var labelOtherSettings = new UnfocusableTextField {
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			container.AddSubview (labelOtherSettings);

			var toggleAbleView = new NSView (new CGRect (CGPoint.Empty, new CGSize (320, 240)));

			//Work out the titlebar height
			var titleBarHeight = Frame.Size.Height - ContentRectFor (Frame).Size.Height;

			buttonMoreSettings.Activated += (sender, e) => {
				if (sender is NSButton moreButton) {
					ToggleSettingsLabel (moreButton.State == NSCellStateValue.Off, labelOtherSettings);

					toggleAbleView.Hidden = moreButton.State == NSCellStateValue.Off;
					container.SetFrameSize (new CGSize (container.Frame.Width, toggleAbleView.Hidden ? container.Frame.Height - toggleAbleView.Frame.Height : container.Frame.Height + toggleAbleView.Frame.Height));
					SetFrame (new CGRect (new CGPoint (Frame.X, Frame.Y), new CGSize (Frame.Width, container.Frame.Height + titleBarHeight)), false, true);
				}
			};

			ToggleSettingsLabel (buttonMoreSettings.State == NSCellStateValue.Off, labelOtherSettings);

			container.AddConstraints (new[] {
				NSLayoutConstraint.Create (bindingTypeLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1f, 20f),
				NSLayoutConstraint.Create (bindingTypeLabel, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 20f),
				NSLayoutConstraint.Create (bindingTypeLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.BindingTypePopup, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bindingTypeLabel, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (this.BindingTypePopup, NSLayoutAttribute.Left, NSLayoutRelation.Equal, bindingTypeLabel, NSLayoutAttribute.Right, 1f, 5f),
				NSLayoutConstraint.Create (this.BindingTypePopup, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.AncestorTypeBox, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1f, 50f),
				NSLayoutConstraint.Create (this.AncestorTypeBox, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 20f),
				NSLayoutConstraint.Create (this.AncestorTypeBox, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 263),
				NSLayoutConstraint.Create (this.AncestorTypeBox, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1f, 340),

				NSLayoutConstraint.Create (this.PathBox, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (this.PathBox, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Right, 1f, 10f),
				NSLayoutConstraint.Create (this.PathBox, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 263),
				NSLayoutConstraint.Create (this.PathBox, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1f, 340),

				NSLayoutConstraint.Create (valueConverterLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1f, 340f),
				NSLayoutConstraint.Create (valueConverterLabel, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 20f),
				NSLayoutConstraint.Create (valueConverterLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.ValueConverterPopup, NSLayoutAttribute.Top, NSLayoutRelation.Equal, valueConverterLabel, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (this.ValueConverterPopup, NSLayoutAttribute.Left, NSLayoutRelation.Equal, valueConverterLabel, NSLayoutAttribute.Right, 1f, 5f),
				NSLayoutConstraint.Create (this.ValueConverterPopup, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.AddConverterButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.ValueConverterPopup, NSLayoutAttribute.Top, 1f, 3f),
				NSLayoutConstraint.Create (this.AddConverterButton, NSLayoutAttribute.Left, NSLayoutRelation.Equal,this.ValueConverterPopup, NSLayoutAttribute.Right, 1f, 5f),
				NSLayoutConstraint.Create (this.AddConverterButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1f, 14),
				NSLayoutConstraint.Create (this.AddConverterButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 14),

				NSLayoutConstraint.Create (buttonMoreSettings, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1f, 370f),
				NSLayoutConstraint.Create (buttonMoreSettings, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 16f),
				NSLayoutConstraint.Create (buttonMoreSettings, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
				NSLayoutConstraint.Create (buttonMoreSettings, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (labelOtherSettings, NSLayoutAttribute.Top, NSLayoutRelation.Equal, buttonMoreSettings, NSLayoutAttribute.Top, 1f,0f),
				NSLayoutConstraint.Create (labelOtherSettings, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 35f),
				NSLayoutConstraint.Create (labelOtherSettings, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.buttonHelp, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Bottom, 1f, -32f),
				NSLayoutConstraint.Create (this.buttonHelp, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 16f),
				NSLayoutConstraint.Create (this.buttonHelp, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
				NSLayoutConstraint.Create (this.buttonHelp, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.ButtonDone, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Bottom, 1f, -32f),
				NSLayoutConstraint.Create (this.ButtonDone, NSLayoutAttribute.Right, NSLayoutRelation.Equal, container, NSLayoutAttribute.Right, 1f, -16f),
				NSLayoutConstraint.Create (this.ButtonDone, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),

				NSLayoutConstraint.Create (this.buttonCancel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.ButtonDone, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (this.buttonCancel, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this.ButtonDone, NSLayoutAttribute.Left, 1f, -10f),
				NSLayoutConstraint.Create (this.buttonCancel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			ContentViewController = new NSViewController (null, null) {
				View = container,
			};
		}

		private static void ToggleSettingsLabel (bool show, UnfocusableTextField labelOtherSettings)
		{
			labelOtherSettings.StringValue = show ? Properties.Resources.ShowSettings : Properties.Resources.HideSettings;
		}
	}
}
