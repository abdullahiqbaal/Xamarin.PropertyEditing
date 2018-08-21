﻿using System.Collections.Generic;
using Xamarin.PropertyEditing.Drawing;

namespace Xamarin.PropertyEditing.ViewModels
{
	internal class PointPropertyViewModel
		: PropertyViewModel<CommonPoint>
	{
		public PointPropertyViewModel (TargetPlatform platform, IPropertyInfo property, IEnumerable<IObjectEditor> editors, PropertyVariationSet variant = null)
			: base (platform, property, editors, variant)
		{
		}

		public double X
		{
			get { return Value.X; }
			set
			{
				if (Value.X == value)
					return;

				Value = new CommonPoint (value, Value.X);
			}
		}

		public double Y
		{
			get { return Value.Y; }
			set
			{
				if (Value.Y == value)
					return;

				Value = new CommonPoint (Value.Y, value);
			}
		}

		protected override void OnValueChanged ()
		{
			base.OnValueChanged ();
			OnPropertyChanged (nameof(X));
			OnPropertyChanged (nameof(Y));
		}
	}
}