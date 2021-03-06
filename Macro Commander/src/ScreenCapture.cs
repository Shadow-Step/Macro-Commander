﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Macro_Commander.enu;

namespace Macro_Commander.src
{
    static class ScreenCapture
    {
        public static Bitmap CaptureFromScreen(int width, int height, int x, int y,CaptureMode mode = CaptureMode.DrawMiddle)
        {
            Bitmap image = new Bitmap(width, height);
            if (mode == CaptureMode.EmptyImage)
                return image;
            using (Graphics graph = Graphics.FromImage(image))
            {
                graph.CopyFromScreen(x - width / 2, y - height / 2, 0, 0, image.Size);
            }
            if(mode == CaptureMode.DrawMiddle)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int z = 0; z < 4; z++)
                    {
                        image.SetPixel(width / 2 - 2 + i, height / 2 - 2 + z, Color.Red);
                    }
                }
            }
            return image;
        }
    }
}
