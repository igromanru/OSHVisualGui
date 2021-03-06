﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using System.ComponentModel;
using System.Drawing.Design;

namespace OSHVisualGui.GuiControls
{
	public class PictureBox : ScalableControl
	{
		#region Properties

		internal override string DefaultName => "pictureBox";

		private Bitmap image;

		private string path;
		[Editor(typeof(FilenameEditor), typeof(UITypeEditor)), FileDialogFilter("Image files (*.jpg, *.bmp, *.gif, *.png)|*.jpg;*.bmp;*.gif;*.png|All files (*.*)|*.*")]
		public string Path
		{
			get => path;
			set
			{
				path = value;
				if (string.IsNullOrEmpty(path))
				{
					image = null;
					return;
				}
				try
				{
					image = (Bitmap)Image.FromFile(path);
				}
				catch
				{
					image = Properties.Resources.imagenotfound;
				}
			}
		}

		private readonly bool DefaultStretch;
		public bool Stretch { get; set; }

		#endregion

		public PictureBox()
		{
			Type = ControlType.PictureBox;

			Path = string.Empty;

			Size = DefaultSize = new Size(100, 100);

			ForeColor = DefaultForeColor = Color.Empty;
			BackColor = DefaultBackColor = Color.Empty;

			Stretch = DefaultStretch = false;
		}

		public override IEnumerable<KeyValuePair<string, ChangedProperty>> GetChangedProperties()
		{
			foreach (var pair in base.GetChangedProperties())
			{
				yield return pair;
			}
			if (Stretch != DefaultStretch)
			{
				yield return new KeyValuePair<string, ChangedProperty>("stretch", new ChangedProperty(Stretch));
			}
			if (!string.IsNullOrEmpty(Path))
			{
				yield return new KeyValuePair<string, ChangedProperty>("image", new ChangedProperty(new System.IO.FileInfo(path)));
			}
		}

		public override void Render(Graphics graphics)
		{
			if (BackColor.A > 0)
			{
				graphics.FillRectangle(backBrush, new Rectangle(AbsoluteLocation, Size));
			}

			using (var pen = new Pen(Color.Black, 1))
			{
				pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
				graphics.DrawRectangle(pen, AbsoluteLocation.X, AbsoluteLocation.Y, Size.Width, Size.Height);
			}

			if (image != null)
			{
				if (Stretch == false)
				{
					var size = new Size(Math.Min(image.Size.Width, Size.Width), Math.Min(image.Size.Height, Size.Height));
					graphics.DrawImageUnscaledAndClipped(image, new Rectangle(AbsoluteLocation, size));
				}
				else
				{
					graphics.DrawImage(image, new Rectangle(AbsoluteLocation, Size));
				}
			}
		}

		public override Control Copy()
		{
			var copy = new PictureBox();
			CopyTo(copy);
			return copy;
		}

		public override string ToString()
		{
			return Name + " - PictureBox";
		}

		public override void ReadPropertiesFromXml(XElement element)
		{
			base.ReadPropertiesFromXml(element);

			if (element.HasAttribute("image"))
				Path = Path.FromXMLString(element.Attribute("image").Value.Trim());
		}
	}
}
