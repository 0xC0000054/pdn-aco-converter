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

// Portions of this file has been adapted from:
////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2011 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

// Portions of this file has been adapted from:
/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;

namespace SwatchConverter
{
    internal static class ColorSpaceConverter
    {
        /// <summary>
        /// Converts HSB to RGB.
        /// </summary>
        /// <param name="hue">The hue value in the range of [0, 360].</param>
        /// <param name="saturation">The saturation value in the range of [0, 1].</param>
        /// <param name="brightness">The brightness value in the range of [0, 1].</param>
        /// <returns>The HSB color converted to RGB.</returns>
        public static ColorBgra HSBToRGB(double hue, double saturation, double brightness)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (saturation == 0)
            {
                // If saturation is 0, all colors are the same.
                // This is some flavor of gray.
                r = brightness;
                g = brightness;
                b = brightness;
            }
            else
            {
                double p;
                double q;
                double t;

                double fractionalSector;
                int sectorNumber;
                double sectorPos;

                // The color wheel consists of 6 sectors.
                // Figure out which sector you're in.
                sectorPos = hue / 60;
                sectorNumber = (int)(Math.Floor(sectorPos));

                // get the fractional part of the sector.
                // That is, how many degrees into the sector
                // are you?
                fractionalSector = sectorPos - sectorNumber;

                // Calculate values for the three axes
                // of the color.
                p = brightness * (1 - saturation);
                q = brightness * (1 - (saturation * fractionalSector));
                t = brightness * (1 - (saturation * (1 - fractionalSector)));

                // Assign the fractional colors to r, g, and b
                // based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = brightness;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = brightness;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = brightness;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = brightness;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = brightness;
                        break;

                    case 5:
                        r = brightness;
                        g = p;
                        b = q;
                        break;
                }
            }
            // return an ColorBgra structure, with values scaled
            // to be between 0 and 255.

            return ColorBgra.FromBgra((byte)Math.Round(b * 255.0), (byte)Math.Round(g * 255.0), (byte)Math.Round(r * 255.0), 255);
        }

        /// <summary>
        /// Converts Lab to RGB.
        /// </summary>
        /// <param name="exL">The L component in the range of [0, 100].</param>
        /// <param name="exA">The a component in the range of [-128, 127].</param>
        /// <param name="exB">The b component in the range of [-128, 127].</param>
        /// <returns>The Lab color converted to RGB.</returns>
        public static ColorBgra LabToRGB(double exL, double exA, double exB)
        {
            int L = (int)exL;
            int a = (int)exA;
            int b = (int)exB;

            // For the conversion we first convert values to XYZ and then to RGB
            // Standards used Observer = 2, Illuminant = D65

            const double ref_X = 95.047;
            const double ref_Y = 100.000;
            const double ref_Z = 108.883;

            double var_Y = (L + 16.0) / 116.0;
            double var_X = a / 500.0 + var_Y;
            double var_Z = var_Y - b / 200.0;

            double var_X3 = var_X * var_X * var_X;
            double var_Y3 = var_Y * var_Y * var_Y;
            double var_Z3 = var_Z * var_Z * var_Z;

            if (var_Y3 > 0.008856)
            {
                var_Y = var_Y3;
            }
            else
            {
                var_Y = (var_Y - 16 / 116) / 7.787;
            }

            if (var_X3 > 0.008856)
            {
                var_X = var_X3;
            }
            else
            {
                var_X = (var_X - 16 / 116) / 7.787;
            }

            if (var_Z3 > 0.008856)
            {
                var_Z = var_Z3;
            }
            else
            {
                var_Z = (var_Z - 16 / 116) / 7.787;
            }

            double X = ref_X * var_X;
            double Y = ref_Y * var_Y;
            double Z = ref_Z * var_Z;

            return XYZToRGB(X, Y, Z);
        }

        /// <summary>
        /// Converts XYZ to RGB.
        /// </summary>
        /// <param name="X">The X component in the range of [0, 100].</param>
        /// <param name="Y">The Y component in the range of [0, 100].</param>
        /// <param name="Z">The Z component in the range of [0, 100].</param>
        /// <returns></returns>
        private static ColorBgra XYZToRGB(double X, double Y, double Z)
        {
            // Standards used Observer = 2, Illuminant = D65
            // ref_X = 95.047, ref_Y = 100.000, ref_Z = 108.883

            double var_X = X / 100.0;
            double var_Y = Y / 100.0;
            double var_Z = Z / 100.0;

            double var_R = var_X * 3.2406 + var_Y * (-1.5372) + var_Z * (-0.4986);
            double var_G = var_X * (-0.9689) + var_Y * 1.8758 + var_Z * 0.0415;
            double var_B = var_X * 0.0557 + var_Y * (-0.2040) + var_Z * 1.0570;

            if (var_R > 0.0031308)
            {
                var_R = 1.055 * (Math.Pow(var_R, 1 / 2.4)) - 0.055;
            }
            else
            {
                var_R = 12.92 * var_R;
            }

            if (var_G > 0.0031308)
            {
                var_G = 1.055 * (Math.Pow(var_G, 1 / 2.4)) - 0.055;
            }
            else
            {
                var_G = 12.92 * var_G;
            }

            if (var_B > 0.0031308)
            {
                var_B = 1.055 * (Math.Pow(var_B, 1 / 2.4)) - 0.055;
            }
            else
            {
                var_B = 12.92 * var_B;
            }

            int nRed = (int)(var_R * 256.0);
            int nGreen = (int)(var_G * 256.0);
            int nBlue = (int)(var_B * 256.0);

            if (nRed < 0)
            {
                nRed = 0;
            }
            else if (nRed > 255)
            {
                nRed = 255;
            }

            if (nGreen < 0)
            {
                nGreen = 0;
            }
            else if (nGreen > 255)
            {
                nGreen = 255;
            }

            if (nBlue < 0)
            {
                nBlue = 0;
            }
            else if (nBlue > 255)
            {
                nBlue = 255;
            }

            return ColorBgra.FromBgra((byte)nBlue, (byte)nGreen, (byte)nRed, 255);
        }

        ///////////////////////////////////////////////////////////////////////////////
        //
        // The algorithms for these routines were taken from:
        //     http://www.neuro.sfc.keio.ac.jp/~aly/polygon/info/color-space-faq.html
        //
        // RGB --> CMYK                              CMYK --> RGB
        // ---------------------------------------   --------------------------------------------
        // Black   = minimum(1-Red,1-Green,1-Blue)   Red   = 1-minimum(1,Cyan*(1-Black)+Black)
        // Cyan    = (1-Red-Black)/(1-Black)         Green = 1-minimum(1,Magenta*(1-Black)+Black)
        // Magenta = (1-Green-Black)/(1-Black)       Blue  = 1-minimum(1,Yellow*(1-Black)+Black)
        // Yellow  = (1-Blue-Black)/(1-Black)
        //

        /// <summary>
        /// Converts CMYK to RGB.
        /// </summary>
        /// <param name="cyan">The cyan value in the range of [0, 1].</param>
        /// <param name="magenta">The magenta value in the range of [0, 1].</param>
        /// <param name="yellow">The yellow value in the range of [0, 1].</param>
        /// <param name="black">The black value in the range of [0, 1].</param>
        /// <returns>The CMYK color converted to RGB.</returns>
        public static ColorBgra CMYKToRGB(double cyan, double magenta, double yellow, double black)
        {
            double C, M, Y, K;

            C = (1.0 - cyan);
            M = (1.0 - magenta);
            Y = (1.0 - yellow);
            K = (1.0 - black);

            int nRed = (int)((1.0 - (C * (1 - K) + K)) * 255);
            int nGreen = (int)((1.0 - (M * (1 - K) + K)) * 255);
            int nBlue = (int)((1.0 - (Y * (1 - K) + K)) * 255);

            if (nRed < 0)
            {
                nRed = 0;
            }
            else if (nRed > 255)
            {
                nRed = 255;
            }

            if (nGreen < 0)
            {
                nGreen = 0;
            }
            else if (nGreen > 255)
            {
                nGreen = 255;
            }

            if (nBlue < 0)
            {
                nBlue = 0;
            }
            else if (nBlue > 255)
            {
                nBlue = 255;
            }

            return ColorBgra.FromBgra((byte)nBlue, (byte)nGreen, (byte)nRed, 255);
        }
    }
}
