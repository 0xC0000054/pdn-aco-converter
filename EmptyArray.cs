////////////////////////////////////////////////////////////////////////////////
//
// ACO converter for Paint.NET
//
// This software is provided under the MIT License:
//   Copyright (C) 2012-2019 Nicholas Hayes
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

namespace SwatchConverter
{
    internal sealed class EmptyArray<T>
    {
        private EmptyArray()
        {
        }

        public static readonly T[] Value = new T[0];
    }
}
