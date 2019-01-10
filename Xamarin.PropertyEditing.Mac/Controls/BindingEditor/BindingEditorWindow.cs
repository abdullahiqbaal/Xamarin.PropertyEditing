using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;
using Xamarin.PropertyEditing.Common;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal class BindingEditorWindow : BasePanelWindow
	{
		private CreateBindingViewModel viewModel;
		internal BindingEditorWindow (PropertyViewModel propertyViewModel) : base (propertyViewModel)
		{
			this.viewModel = new CreateBindingViewModel (propertyViewModel.TargetPlatform, propertyViewModel.Editors.Single (), propertyViewModel.Property);

			Title = this.viewModel.PropertyDisplay;
			this.ButtonDone.Title = Properties.Resources.CreatBindingTitle;

			foreach (BindingSource item in this.viewModel.BindingSources.Value) {
				this.BindingTypePopup.Menu.AddItem (new NSMenuItem (item.Name) {
					RepresentedObject = new NSObjectFacade (item)
				});
			}

			this.BindingTypePopup.Activated += (o, e) => {
				if (this.BindingTypePopup.Menu.HighlightedItem.RepresentedObject is NSObjectFacade facade) {
					this.viewModel.SelectedBindingSource = (BindingSource)facade.Target;
				}
			};

			this.ValueConverterPopup.Activated += (o, e) => {
				if (this.ValueConverterPopup.Menu.HighlightedItem.RepresentedObject is NSObjectFacade facade) {
					this.viewModel.SelectedValueConverter = (Resource)facade.Target;
				}
			};

			foreach (Resource item in this.viewModel.ValueConverters.Value) {
				this.ValueConverterPopup.Menu.AddItem (new NSMenuItem (item.Name) {
					RepresentedObject = new NSObjectFacade (item)
				});
			}

			this.AddConverterButton.Activated += (sender, e) => {
				this.viewModel.SelectedValueConverter = CreateBindingViewModel.AddValueConverter;
			};

			var typeHeader = new HeaderView {
				Title = Properties.Resources.Type,
			};

			this.AncestorTypeBox.AddSubview (typeHeader);

			this.AncestorTypeBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (typeHeader, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (typeHeader, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (typeHeader, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (typeHeader, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 40),
			});

			var typeSelectorControl = new BindingTypeSelectorControl (this.viewModel);

			this.AncestorTypeBox.AddSubview (typeSelectorControl);

			this.AncestorTypeBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (typeSelectorControl, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Height, 1f, 0f)
			});

			var resourceSelectorControl = new BindingResourceSelectorControl (this.viewModel);

			this.AncestorTypeBox.AddSubview (resourceSelectorControl);

			this.AncestorTypeBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (resourceSelectorControl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (resourceSelectorControl, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (resourceSelectorControl, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (resourceSelectorControl, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Height, 1f, 0f)
			});

			var objectSelectorControl = new BindingObjectSelectorControl (this.viewModel);

			this.AncestorTypeBox.AddSubview (objectSelectorControl);

			this.AncestorTypeBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (objectSelectorControl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (objectSelectorControl, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (objectSelectorControl, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (objectSelectorControl, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Height, 1f, 0f)
			});

			var longDescription = new UnfocusableTextField {
				Alignment = NSTextAlignment.Left,
				TranslatesAutoresizingMaskIntoConstraints = false,
				StringValue = string.Empty,
			};

			this.AncestorTypeBox.AddSubview (longDescription);

			this.AncestorTypeBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (longDescription, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Top, 1f, 10f),
				NSLayoutConstraint.Create (longDescription, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Left, 1f, 10f),
				NSLayoutConstraint.Create (longDescription, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.AncestorTypeBox, NSLayoutAttribute.Width, 1f, -10f),
				NSLayoutConstraint.Create (longDescription, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			var pathHeader = new HeaderView {
				Title = Properties.Resources.Path,
			};

			this.PathBox.AddSubview (pathHeader);

			this.PathBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (pathHeader, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (pathHeader, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (pathHeader, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (pathHeader, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 40),
			});

			var pathSelectorControl = new BindingPathSelectorControl (this.viewModel);

			this.PathBox.AddSubview (pathSelectorControl);

			this.PathBox.AddConstraints (new[] {
				NSLayoutConstraint.Create (pathSelectorControl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (pathSelectorControl, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (pathSelectorControl, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Width, 1f, 0f),
				NSLayoutConstraint.Create (pathSelectorControl, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this.PathBox, NSLayoutAttribute.Height, 1f, 0f)
			});

			this.viewModel.PropertyChanged += (sender, e) => {
				if (e.PropertyName == nameof (CreateBindingViewModel.ShowLongDescription)) {
					longDescription.Hidden = !this.viewModel.ShowLongDescription;
					longDescription.StringValue = this.viewModel.ShowLongDescription ? this.viewModel.SelectedBindingSource.Description : string.Empty;
					typeHeader.Hidden = this.viewModel.ShowLongDescription;
				}

				if (e.PropertyName == nameof (CreateBindingViewModel.ShowObjectSelector)) {
					if (this.viewModel.ShowObjectSelector) {
						typeHeader.Title = Properties.Resources.SelectObjectTitle;
					}
				}

				if (e.PropertyName == nameof (CreateBindingViewModel.ShowTypeSelector)) {
					if (this.viewModel.ShowTypeSelector) {
						typeHeader.Title = Properties.Resources.SelectTypeTitle;
					}
				}

				if (e.PropertyName == nameof (CreateBindingViewModel.ShowResourceSelector)) {
					if (this.viewModel.ShowResourceSelector) {
						typeHeader.Title = Properties.Resources.SelectResourceTitle;
					}
				}
			};

			this.viewModel.CreateValueConverterRequested += OnCreateValueConverterRequested;

			this.ButtonDone.Activated += (sender, e) => {
				if (pathSelectorControl.CustomPath.Enabled && !string.IsNullOrEmpty (pathSelectorControl.CustomPath.Cell.Title)) {
					this.viewModel.Path = pathSelectorControl.CustomPath.Cell.Title;
				}

				Close ();
			};
		}

		private void OnCreateValueConverterRequested (object sender, CreateValueConverterEventArgs e)
		{
			ITypeInfo valueConverter = this.viewModel.TargetPlatform.EditorProvider.KnownTypes[typeof (CommonValueConverter)];

			var typesTask = this.viewModel.TargetPlatform.EditorProvider.GetAssignableTypesAsync (valueConverter, childTypes: false)
				.ContinueWith (t => t.Result.GetTypeTree (), TaskScheduler.Default);

			var createValueConverterWindow = new CreateValueConverterWindow (this.viewModel, new AsyncValue<IReadOnlyDictionary<IAssemblyInfo, ILookup<string, ITypeInfo>>> (typesTask));
			createValueConverterWindow.MakeKeyAndOrderFront (this);
			createValueConverterWindow.WillClose += (sende1, e1) => {
				e.Name = createValueConverterWindow.ValueConverterName;
				e.ConverterType = createValueConverterWindow.ViewModel.SelectedType;
				e.Source = createValueConverterWindow.ViewModel.Source;
			};
		}
	}
}
