////////////////////////////////////////////////////////////////////////////////
//
// ACO converter for Paint.NET
//
// This software is provided under the MIT License:
//   Copyright (C) 2012-2018 Nicholas Hayes
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using PaintDotNet;

namespace SwatchConverter
{
	// The Color Swatch file format specification is available at
	// https://www.adobe.com/devnet-apps/photoshop/fileformatashtml/PhotoshopFileFormats.htm#50577411_pgfId-1055819

	/// <summary>
	/// Decodes Adobe® Photoshop® Color Swatches.
	/// </summary>
	internal sealed class ColorSwatchDecoder
	{
		private readonly ColorSwatchCollection colors;

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorSwatchDecoder"/> class.
		/// </summary>
		/// <param name="path">The path of the swatch file.</param>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
		public ColorSwatchDecoder(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				this.colors = Load(stream);
			}
		}

		private enum ColorMode : short
		{
			RGB = 0,
			HSB = 1,
			CMYK = 2,
			Pantone = 3,
			Focoltone = 4,
			Trumatch = 5,
			Toyo88 = 6,
			Lab = 7,
			Gray = 8,
			HKS = 10
		}

		public ColorSwatchCollection Colors
		{
			get
			{
				return this.colors;
			}
		}

		private static ColorSwatchCollection Load(Stream stream)
		{
			ColorSwatchCollection colors = null;

			using (BigEndianBinaryReader reader = new BigEndianBinaryReader(stream))
			{
				short fileVersion = reader.ReadInt16();

				// According to http://cyotek.com/blog/reading-photoshop-color-swatch-aco-files-using-csharp
				// some ACO files only include the version 2 data.

				if (fileVersion != 1 && fileVersion != 2)
				{
					throw new FormatException(Properties.Resources.UnsupportedSwatchFileVersion);
				}

				ushort count = reader.ReadUInt16();

				if (count == 0)
				{
					throw new FormatException(Properties.Resources.EmptySwatchFile);
				}

				bool version2 = IsVersion2File(reader, fileVersion, count);

				if (!CheckSupportedColorModes(reader, count, version2))
				{
					throw new FormatException(Properties.Resources.UnsupportedColorMode);
				}

				colors = ReadColors(reader, count, version2);
			}

			return colors;
		}

		private static bool IsVersion2File(BigEndianBinaryReader reader, short fileVersion, ushort count)
		{
			if (fileVersion == 2)
			{
				return true;
			}
			else
			{
				long v2Offset = reader.Position + (count * 10);

				// Add 4 bytes to account for the file header length, some version 1 files may have a padding byte without a version 2 header.
				if ((v2Offset + 4L) < reader.Length)
				{
					long startOffset = reader.Position;

					reader.Position = v2Offset;

					short newVersion = reader.ReadInt16();
					ushort newCount = reader.ReadUInt16();

					if (newVersion == 2 && newCount == count && reader.Position < reader.Length)
					{
						return true;
					}
					else
					{
						reader.Position = startOffset;
					}
				}
			}

			return false;
		}

		private static bool CheckSupportedColorModes(BigEndianBinaryReader reader, ushort count, bool version2)
		{
			long startOffset = reader.Position;

			bool foundSupportedColor = false;

			for (int i = 0; i < count; i++)
			{
				ColorMode mode = (ColorMode)reader.ReadInt16();

				if (mode == ColorMode.RGB || mode == ColorMode.HSB || mode == ColorMode.CMYK || mode == ColorMode.Gray || mode == ColorMode.Lab)
				{
					foundSupportedColor = true;
					reader.Position = startOffset;
					break;
				}

				reader.Position += 8L;
				if (version2)
				{
					int nameLengthInBytes = reader.ReadInt32() * 2;
					reader.Position += nameLengthInBytes;
				}
			}

			return foundSupportedColor;
		}

		private static ColorSwatchCollection ReadColors(BigEndianBinaryReader reader, ushort count, bool version2)
		{
			HashSet<ColorSwatch> uniqueColors = new HashSet<ColorSwatch>();

			for (int i = 0; i < count; i++)
			{
				ColorMode mode = (ColorMode)reader.ReadInt16();

#if DEBUG
				System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "swatch {0} mode: {1}", i, mode));
#endif
				ColorBgra color;

				if (TryGetColor(reader, mode, out color))
				{
					string name = version2 ? reader.ReadUnicodeString() : null;
					uniqueColors.Add(new ColorSwatch(color, name));
				}
				else
				{
					// Skip any unknown color modes.
					reader.Position += 8L;
					if (version2)
					{
						int nameLengthInBytes = reader.ReadInt32() * 2;
						reader.Position += nameLengthInBytes;
					}
				}
			}

			return new ColorSwatchCollection(uniqueColors);
		}

		private static bool TryGetColor(BigEndianBinaryReader reader, ColorMode mode, out ColorBgra color)
		{
			if (mode == ColorMode.RGB)
			{
				ushort r = reader.ReadUInt16();
				ushort g = reader.ReadUInt16();
				ushort b = reader.ReadUInt16();
				reader.Position += 2L;

				byte red = (byte)(r / 257);
				byte green = (byte)(g / 257);
				byte blue = (byte)(b / 257);

				color = ColorBgra.FromBgra(blue, green, red, 255);

				return true;
			}
			else if (mode == ColorMode.HSB)
			{
				ushort h = reader.ReadUInt16();
				ushort s = reader.ReadUInt16();
				ushort b = reader.ReadUInt16();
				reader.Position += 2L;

				double hue = (h / 65535.0) * 360.0;
				double saturation = (s / 65535.0) * 100.0;
				double brightness = (b / 65535.0) * 100.0;

				color = ColorSpaceConverter.HSBToRGB(hue, saturation, brightness);

				return true;
			}
			else if (mode == ColorMode.Gray)
			{
				byte gray = (byte)((reader.ReadUInt16() / 10000.0) * 255);
				reader.Position += 6L;

				color = ColorBgra.FromBgra(gray, gray, gray, 255);

				return true;
			}
			else if (mode == ColorMode.CMYK)
			{
				ushort c = reader.ReadUInt16();
				ushort m = reader.ReadUInt16();
				ushort y = reader.ReadUInt16();
				ushort k = reader.ReadUInt16();

				color = ColorSpaceConverter.CMYKToRGB(c / 65535.0, m / 65535.0, y / 65535.0, k / 65535.0);

				return true;
			}
			else if (mode == ColorMode.Lab)
			{
				short l = reader.ReadInt16();
				short a = reader.ReadInt16();
				short b = reader.ReadInt16();
				reader.Position += 2L;

				color = ColorSpaceConverter.LabToRGB(l / 100.0, a / 100.0, b / 100.0);

				return true;
			}

			color = new ColorBgra();
			return false;
		}
	}
}
