using System;
using System.Collections;
using System.Diagnostics;
using AppKit;
using Xamarin.PropertyEditing.Mac.Resources;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal abstract class BasePointEditorControl<T> : PropertyEditorControl<PropertyViewModel<T>>
	{
		internal UnfocusableTextField XLabel { get; set; }
		internal NumericSpinEditor<T> XEditor { get; set; }
		internal UnfocusableTextField YLabel { get; set; }
		internal NumericSpinEditor<T> YEditor { get; set; }

		public override NSView FirstKeyView => XEditor;
		public override NSView LastKeyView => YEditor.DecrementButton;

		protected BasePointEditorControl (IHostResourceProvider hostResources)
			: base (hostResources)
		{
			XLabel = new UnfocusableTextField {
				Font = NSFont.FromFontName (DefaultFontName, DefaultDescriptionLabelFontSize),
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			XEditor = new NumericSpinEditor<T> (hostResources) {
				BackgroundColor = NSColor.Clear,
				Value = 0.0f
			};
			XEditor.ValueChanged += OnInputUpdated;

			YLabel = new UnfocusableTextField {
				Font = NSFont.FromFontName (DefaultFontName, DefaultDescriptionLabelFontSize),
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			YEditor = new NumericSpinEditor<T> (hostResources) {
				BackgroundColor = NSColor.Clear,
				Value = 0.0f
			};
			YEditor.ValueChanged += OnInputUpdated;

			AddSubview (XLabel);
			AddSubview (XEditor);
			AddSubview (YLabel);
			AddSubview (YEditor);

			this.AddConstraints (new[] {
				NSLayoutConstraint.Create (XLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this,  NSLayoutAttribute.Top, 1f, 16f),
				NSLayoutConstraint.Create (XLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, DefaultControlHeight),

				NSLayoutConstraint.Create (XEditor, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this,  NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (XEditor, NSLayoutAttribute.Right, NSLayoutRelation.Equal, YEditor,  NSLayoutAttribute.Left, 1f, -10f),
				NSLayoutConstraint.Create (XEditor, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this,  NSLayoutAttribute.Width, 1f, -280f),
				NSLayoutConstraint.Create (XEditor, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, DefaultControlHeight),

				NSLayoutConstraint.Create (YLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, XLabel,  NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (YLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, DefaultControlHeight),

				NSLayoutConstraint.Create (YEditor, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this,  NSLayoutAttribute.Right, 1f, -32f),
				NSLayoutConstraint.Create (YEditor, NSLayoutAttribute.Width, NSLayoutRelation.Equal, XEditor,  NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (YEditor, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, DefaultControlHeight),
			});

			ViewDidChangeEffectiveAppearance ();
		}

		public override void ViewDidChangeEffectiveAppearance ()
		{
			XLabel.TextColor = HostResources.GetNamedColor (NamedResources.DescriptionLabelColor);
			YLabel.TextColor = HostResources.GetNamedColor (NamedResources.DescriptionLabelColor);
		}

		protected override void HandleErrorsChanged (object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
		{
			UpdateErrorsDisplayed (ViewModel.GetErrors (ViewModel.Property.Name));
		}

		protected override void UpdateErrorsDisplayed (IEnumerable errors)
		{
			if (ViewModel.HasErrors) {
				SetErrors (errors);
			} else {
				SetErrors (null);
				SetEnabled ();
			}
		}

		protected override void SetEnabled ()
		{
			XEditor.Editable = ViewModel.Property.CanWrite;
			YEditor.Editable = ViewModel.Property.CanWrite;
		}

		protected override void UpdateAccessibilityValues ()
		{
			XEditor.AccessibilityEnabled = XEditor.Enabled;
			XEditor.AccessibilityTitle = string.Format (LocalizationResources.AccessibilityXEditor, ViewModel.Property.Name);

			YEditor.AccessibilityEnabled = YEditor.Enabled;
			YEditor.AccessibilityTitle = string.Format (LocalizationResources.AccessibilityYEditor, ViewModel.Property.Name);
		}

		protected virtual void OnInputUpdated (object sender, EventArgs e)
		{
			ViewModel.Value = (T)Activator.CreateInstance (typeof (T), XEditor.Value, YEditor.Value);
		}
	}
}
