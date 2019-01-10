using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using CoreGraphics;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal class CreateValueConverterWindow : NSPanel
	{
		public AddValueConverterViewModel ViewModel { get; }

		private NSTextField valueConverterName;
		public string ValueConverterName { 
			get { return this.valueConverterName.Cell.Title; } 
		}

		public CreateValueConverterWindow (CreateBindingViewModel viewModel, AsyncValue<IReadOnlyDictionary<IAssemblyInfo, ILookup<string, ITypeInfo>>> typetasks )
		{
			if (viewModel == null)
				throw new ArgumentNullException (nameof (viewModel));

			ViewModel = new AddValueConverterViewModel (viewModel.TargetPlatform, viewModel.Target, typetasks);

			FloatingPanel = true;

			StyleMask |= NSWindowStyle.Resizable;

			Title = Properties.Resources.AddValueConverterTitle;

			MaxSize = new CGSize (500, 560); // TODO discuss what the Max/Min Size should be and if we should have one.
			MinSize = new CGSize (200, 320);

			var container = new NSView (new CGRect (CGPoint.Empty, new CGSize (400, 400))) {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			var valueConvertorLabel = new UnfocusableTextField {
				StringValue = Properties.Resources.ValueConverterName,
				TranslatesAutoresizingMaskIntoConstraints = false,
			 };
			container.AddSubview (valueConvertorLabel);

			container.AddConstraints (new[] {
				NSLayoutConstraint.Create (valueConvertorLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (valueConvertorLabel, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 5f),
				NSLayoutConstraint.Create (valueConvertorLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			this.valueConverterName = new NSTextField {
				ControlSize = NSControlSize.Small,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			container.AddSubview (this.valueConverterName);

			container.AddConstraints (new[] {
				NSLayoutConstraint.Create (this.valueConverterName, NSLayoutAttribute.Top, NSLayoutRelation.Equal, valueConvertorLabel, NSLayoutAttribute.Bottom, 1f, 1f),
				NSLayoutConstraint.Create (this.valueConverterName, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 5f),
				NSLayoutConstraint.Create (this.valueConverterName, NSLayoutAttribute.Width, NSLayoutRelation.Equal, container, NSLayoutAttribute.Width, 1f, -10f),
				NSLayoutConstraint.Create (this.valueConverterName, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			var typeSelectorControl = new BindingTypeSelectorControl (viewModel);

			container.AddSubview (typeSelectorControl);

			container.AddConstraints (new[] {
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1f, 10f),
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Left, NSLayoutRelation.Equal, container, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Width, NSLayoutRelation.Equal, container, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Height, NSLayoutRelation.Equal, container, NSLayoutAttribute.Height, 1f, -30f)
			});

			var buttonDone = new NSButton {
				BezelStyle = NSBezelStyle.Rounded,
				Highlighted = true,
				Title = Properties.Resources.DoneTitle,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			buttonDone.KeyEquivalent = "\r"; // Fire when enter pressed

			buttonDone.Activated += (sender, e) => {
				Close ();
			};

			container.AddSubview (buttonDone);

			container.AddConstraints (new[] {
				NSLayoutConstraint.Create (buttonDone, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Bottom, 1f, -32f),
				NSLayoutConstraint.Create (buttonDone, NSLayoutAttribute.Right, NSLayoutRelation.Equal, container, NSLayoutAttribute.Right, 1f, -16f),
				NSLayoutConstraint.Create (buttonDone, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			ContentViewController = new NSViewController (null, null) {
				View = container,
			};

			viewModel.PropertyChanged += (sender, e) => {
				if (e.PropertyName == nameof (CreateBindingViewModel.AddValueConverter)) {
					Console.WriteLine ("Test");
					/*typeSelectorControl.ViewModel = viewModel.TypeSelector;

					typeSelectorControl.showAllAssembliesCheckBox.State = viewModel.TypeSelector.ShowAllAssemblies ? NSCellStateValue.On : NSCellStateValue.Off;*/
				}
			};
		}
	}
}
