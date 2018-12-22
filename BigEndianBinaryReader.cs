﻿////////////////////////////////////////////////////////////////////////////////
//
// ACO converter for Paint.NET
//
// This software is provided under the MIT License:
//   Copyright (C) 2012-2017 Nicholas Hayes
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

// Portions of this file has been adapted from:
/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2011 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Text;

namespace SwatchConverter
{
    internal sealed class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream stream) : base(stream)
        {
        }

        public override double ReadDouble()
        {
            double val = base.ReadDouble();
            unsafe
            {
                SwapBytes((byte*)&val, 8);
            }
            return val;
        }

        public override short ReadInt16()
        {
            short val = base.ReadInt16();
            unsafe
            {
                SwapBytes((byte*)&val, 2);
            }
            return val;
        }

        public override int ReadInt32()
        {
            int val = base.ReadInt32();
            unsafe
            {
                SwapBytes((byte*)&val, 4);
            }
            return val;
        }

        public override long ReadInt64()
        {
            long val = base.ReadInt64();
            unsafe
            {
                SwapBytes((byte*)&val, 8);
            }
            return val;
        }

        public override ushort ReadUInt16()
        {
            ushort val = base.ReadUInt16();
            unsafe
            {
                SwapBytes((byte*)&val, 2);
            }
            return val;
        }

        public override uint ReadUInt32()
        {
            uint val = base.ReadUInt32();
            unsafe
            {
                SwapBytes((byte*)&val, 4);
            }
            return val;
        }

        public override ulong ReadUInt64()
        {
            ulong val = base.ReadUInt64();
            unsafe
            {
                SwapBytes((byte*)&val, 8);
            }
            return val;
        }

        //////////////////////////////////////////////////////////////////

        public string ReadUnicodeString()
        {
            int lengthInChars = ReadInt32();
            byte[] bytes = ReadBytes(lengthInChars * 2);

            return Encoding.BigEndianUnicode.GetString(bytes).TrimEnd('\0');
        }

        //////////////////////////////////////////////////////////////////

        private static unsafe void SwapBytes(byte* ptr, int nLength)
        {
            for (long i = 0; i < nLength / 2; ++i)
            {
                byte t = *(ptr + i);
                *(ptr + i) = *(ptr + nLength - i - 1);
                *(ptr + nLength - i - 1) = t;
            }
        }
    }
}