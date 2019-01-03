using System;
using AppKit;
using Foundation;

namespace Xamarin.PropertyEditing.Mac
{
	public class HostResourceProvider
		: IHostResourceProvider
	{
		public virtual NSColor GetNamedColor (string name)
		{
			return NSColor.FromName (name);
		}

		public virtual NSImage GetNamedImage (string name)
		{
			return NSImage.ImageNamed (name);
		}

		public virtual NSFont GetNamedFont (string name, nfloat fontSize)
		{
			return NSFont.FromFontName (name, fontSize);
		}
	}

	public static class NamedResources
	{
		public const string Checkerboard0Color = "Checkerboard0";
		public const string Checkerboard1Color = "Checkerboard1";
		public const string ForegroundColor = "ForegroundColor";
		public const string PadBackgroundColor = "PadBackgroundColor";
		public const string DescriptionLabelColor = "DescriptionLabelColor";
	}
}
