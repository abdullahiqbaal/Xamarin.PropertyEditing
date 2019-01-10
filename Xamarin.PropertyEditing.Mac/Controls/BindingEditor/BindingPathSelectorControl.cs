using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Xamarin.PropertyEditing.ViewModels;

namespace Xamarin.PropertyEditing.Mac
{
	internal class PathOutlineView : NSOutlineView
	{
		private IReadOnlyCollection<PropertyTreeElement> viewModel;
		public IReadOnlyCollection<PropertyTreeElement> ViewModel
		{
			get => this.viewModel;
			set {
				if (this.viewModel != value) {
					this.viewModel = value;
					var dataSource = new PathOutlineViewDataSource (this.viewModel);
					Delegate = new PathOutlineViewDelegate (dataSource);
					DataSource = dataSource;
				}

				if (this.viewModel != null) {
					ReloadData ();
				}
			}
		}

		public PathOutlineView ()
		{
			Initialize ();
		}

		// Called when created from unmanaged code
		public PathOutlineView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PathOutlineView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export ("validateProposedFirstResponder:forEvent:")]
		public bool ValidateProposedFirstResponder (NSResponder responder, NSEvent ev)
		{
			return true;
		}

		public void Initialize ()
		{
			AutoresizingMask = NSViewResizingMask.WidthSizable;
			HeaderView = null;
			TranslatesAutoresizingMaskIntoConstraints = false;
		}
	}

	internal class PathOutlineViewDelegate : NSOutlineViewDelegate
	{
		private PathOutlineViewDataSource dataSource;

		public PathOutlineViewDelegate (PathOutlineViewDataSource dataSource)
		{
			this.dataSource = dataSource;
		}

		public override nfloat GetRowHeight (NSOutlineView outlineView, NSObject item)
		{
			return PropertyEditorControl.DefaultControlHeight;
		}

		public override NSView GetView (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		{
			var labelContainer = (UnfocusableTextField)outlineView.MakeView ("path", this);
			if (labelContainer == null) {
				labelContainer = new UnfocusableTextField {
					Identifier = "path",
				};
			}
			var target = (item as NSObjectFacade).Target;

			switch (target) {
				case PropertyTreeElement propertyTreeElement:
					labelContainer.StringValue = string.Format("{0}:({1})", propertyTreeElement.Property.Name, propertyTreeElement.Property.RealType.Name);
					break;

				default:
					labelContainer.StringValue = "Type Not Supported";
					break;
			}

			return labelContainer;
		}

		public override bool ShouldSelectItem (NSOutlineView outlineView, NSObject item)
		{
			var target = (item as NSObjectFacade).Target;
			switch (target) {
				case PropertyTreeElement propertyTreeElement:
					var result = propertyTreeElement.Children.Task.Result;
					return result.Count == 0;

				default:
					return false;
			}
		}
	}

	internal class PathOutlineViewDataSource : NSOutlineViewDataSource
	{
		private readonly IReadOnlyCollection<PropertyTreeElement> viewModel;

		internal PathOutlineViewDataSource (IReadOnlyCollection<PropertyTreeElement> viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException (nameof (viewModel));

			this.viewModel = viewModel;
		}

		public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			var childCount = 0;
			if (item == null) {
				childCount = this.viewModel != null ? this.viewModel.Count () : 0;
			} else {
				var target = (item as NSObjectFacade).Target;
				switch (target) {
					case PropertyTreeElement propertyTreeElement:
						IReadOnlyCollection<PropertyTreeElement> result = propertyTreeElement.Children.Task.Result;
						childCount = result.Count;
						break;
					default:
						childCount = 0;
						break;
				}
			}

			return childCount;
		}

		public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
		{
			object element;

			if (item == null) {
				element = this.viewModel.ElementAt ((int)childIndex);
			} else {
				var target = (item as NSObjectFacade).Target;
				switch (target) {
					case PropertyTreeElement propertyTreeElement:
						IReadOnlyCollection<PropertyTreeElement> result = propertyTreeElement.Children.Task.Result;
						element = result.ElementAt ((int)childIndex);
						break;

					default:
						return null;
				}
			}

			return new NSObjectFacade (element);
		}

		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
			var target = (item as NSObjectFacade).Target;
			switch (target) {
				case PropertyTreeElement propertyTreeElement:
					IReadOnlyCollection<PropertyTreeElement> result = propertyTreeElement.Children.Task.Result;
					return result.Count > 0;

				default:
					return false;
			}
		}
	}

	internal class BindingPathSelectorControl : NSView
	{
		private PathOutlineView pathOutlineView;
		internal const string PathSelectorColumnColId = "PathSelectorColumn";

		private NSTextField customPath;
		public NSTextField CustomPath { get; private set; }

		public BindingPathSelectorControl (CreateBindingViewModel viewModel)
		{
			TranslatesAutoresizingMaskIntoConstraints = false;

			var customCheckBox = new NSButton {
				ControlSize = NSControlSize.Small,
				Font = NSFont.FromFontName (PropertyEditorControl.DefaultFontName, PropertyEditorControl.DefaultFontSize),
				Title = Properties.Resources.Custom,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			customCheckBox.SetButtonType (NSButtonType.Switch);

			AddSubview (customCheckBox);
			AddConstraints (new[] {
				NSLayoutConstraint.Create (customCheckBox, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, 8f),
				NSLayoutConstraint.Create (customCheckBox, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1f, -90f),
				NSLayoutConstraint.Create (customCheckBox, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			this.customPath = new NSTextField {
				ControlSize = NSControlSize.Mini,
				Enabled = false,
				Font = NSFont.FromFontName (PropertyEditorControl.DefaultFontName, PropertyEditorControl.DefaultFontSize),
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			AddSubview (this.customPath);
			AddConstraints (new[] {
				NSLayoutConstraint.Create (this.customPath, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, 45f),
				NSLayoutConstraint.Create (this.customPath, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1f, 5f),
				NSLayoutConstraint.Create (this.customPath, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1f, -10f),
				NSLayoutConstraint.Create (this.customPath, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1f, 24),
			});

			customCheckBox.Activated += (sender, e) => {
				this.customPath.Enabled = customCheckBox.State == NSCellStateValue.On;
			};

			this.pathOutlineView = new PathOutlineView {

			};

			this.pathOutlineView.Activated += (sender, e) => {
				if (sender is PathOutlineView pov) {
					if (pov.SelectedRow != -1) {
						if (pov.ItemAtRow (pov.SelectedRow) is NSObjectFacade item) {
							if (item.Target is ITypeInfo typeInfo) {
								viewModel.TypeSelector.SelectedType = typeInfo;
							}
						}
					}
				}
			};

			var pathColumn = new NSTableColumn (PathSelectorColumnColId);
			this.pathOutlineView.AddColumn (pathColumn);

			// Set OutlineTableColumn or the arrows showing children/expansion will not be drawn
			this.pathOutlineView.OutlineTableColumn = pathColumn;

			// create a table view and a scroll view
			var outlineViewContainer = new NSScrollView {
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			// add the panel to the window
			outlineViewContainer.DocumentView = this.pathOutlineView;
			AddSubview (outlineViewContainer);

			AddConstraints (new[] {
				NSLayoutConstraint.Create (outlineViewContainer, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, 45f),
				NSLayoutConstraint.Create (outlineViewContainer, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1f, 5f),
				NSLayoutConstraint.Create (outlineViewContainer, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this, NSLayoutAttribute.Width, 1f, -10f),
				NSLayoutConstraint.Create (outlineViewContainer, NSLayoutAttribute.Height, NSLayoutRelation.Equal,this, NSLayoutAttribute.Height, 1f, -50f),
			});

			viewModel.PropertyChanged += (sender, e) => {
				if (((viewModel.ShowObjectSelector && viewModel.SelectedObjects != null)
				|| (viewModel.ShowTypeSelector && viewModel.TypeSelector != null && viewModel.TypeSelector.SelectedType != null)
				|| (viewModel.ShowResourceSelector && viewModel.SelectedResource != null)) 
				&& viewModel.PropertyRoot != null 
				&& viewModel.PropertyRoot.Value != null) {
					this.pathOutlineView.ViewModel = viewModel.PropertyRoot.Value.Children;
				}
			};
		}
	}
}
