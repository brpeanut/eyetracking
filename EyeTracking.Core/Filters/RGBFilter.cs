﻿using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracking.Core.Filters
{
    public class RGBFilter : IFilter
    {


        public RGBFilter()
        {

        }

        public System.Drawing.Bitmap Apply(System.Drawing.Bitmap image)
        {
            unsafe
            {
                //create an empty bitmap the same size as original
                Bitmap newBitmap = new Bitmap(image.Width, image.Height);

                //lock the original bitmap in memory
                BitmapData originalData = image.LockBits(
                   new Rectangle(0, 0, image.Width, image.Height),
                   ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                //lock the new bitmap in memory
                BitmapData newData = newBitmap.LockBits(
                   new Rectangle(0, 0, image.Width, image.Height),
                   ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                //set the number of bytes per pixel
                int pixelSize = 3;
                int mingray = 0;
                int maxgray = 20;
                byte ar = 34;
                byte ag = 32;
                byte ab = 36;
                byte threshold = 10;
                for (int y = 0; y < image.Height; y++)
                {
                    //get the data from the original image
                    byte* oRow = (byte*)originalData.Scan0 + (y * originalData.Stride);

                    //get the data from the new image
                    byte* nRow = (byte*)newData.Scan0 + (y * newData.Stride);

                    for (int x = 0; x < image.Width; x++)
                    {
                        //create the grayscale version
                        byte blue = oRow[x * pixelSize];
                        byte green = oRow[x * pixelSize + 1];
                        byte red = oRow[x * pixelSize + 2];
                        byte intensity = (byte)((red + green + blue) / 3);

                        // Apply functions
                        if (red >= ar - threshold && red <= ar + threshold
                            && green >= ag - threshold && green <= ag + threshold
                            && blue >= ab - threshold && blue <= ab + threshold)
                            intensity = 255;
                        else
                            intensity /= 2;
                        // Set new color.

                        nRow[x * pixelSize] = intensity; //B
                        nRow[x * pixelSize + 1] = intensity; //G
                        nRow[x * pixelSize + 2] = intensity; //R
                    }
                }

                //unlock the bitmaps
                newBitmap.UnlockBits(newData);
                image.UnlockBits(originalData);

                image.Dispose();
                return newBitmap;
            }
        }

        public object Clone()
        {
            return new RGBFilter()
            {
            };
        }

        public override string ToString()
        {
            return "RGBFilter";
        }
    }
}
