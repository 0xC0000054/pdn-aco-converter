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

using PaintDotNet;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SwatchConverter
{
    internal sealed class ColorSwatchCollection : ReadOnlyCollection<ColorSwatch>
    {
        public ColorSwatchCollection(IEnumerable<ColorSwatch> items) : base(new List<ColorSwatch>(items))
        {
        }

        public ColorBgra[] GetSwatchColors()
        {
            IList<ColorSwatch> items = this.Items;

            ColorBgra[] colors = new ColorBgra[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                colors[i] = items[i].Color;
            }

            return colors;
        }
    }
}
