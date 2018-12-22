////////////////////////////////////////////////////////////////////////////////
//
// ACO converter for Paint.NET
//
// This software is provided under the MIT License:
//   Copyright (C) 2012-2017 Nicholas Hayes
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;
using System.Diagnostics;
using System.Globalization;

namespace SwatchConverter
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal sealed class ColorSwatch : IEquatable<ColorSwatch>
    {
        private readonly ColorBgra color;
        private readonly string name;

        public ColorSwatch(ColorBgra color, string name)
        {
            this.color = color;
            this.name = name;
        }

        public ColorBgra Color
        {
            get
            {
                return this.color;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, "Color: R: {0}, G: {1} B: {2} A: {3}, Name: {4}",
                                     this.color.R, this.color.G, this.color.B, this.color.A, this.name);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            ColorSwatch other = obj as ColorSwatch;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(ColorSwatch other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.color == other.color && string.Equals(this.name, other.name, StringComparison.Ordinal));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;

                hash = (hash * 127) + this.color.GetHashCode();
                hash = (hash * 127) + (this.name == null ? 0 : this.name.GetHashCode());

                return hash;
            }
        }

        public static bool operator ==(ColorSwatch left, ColorSwatch right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left) == null || ((object)right) == null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ColorSwatch left, ColorSwatch right)
        {
            return !(left == right);
        }
    }
}
