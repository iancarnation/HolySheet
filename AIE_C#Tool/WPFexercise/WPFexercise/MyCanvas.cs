using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFexercise
{
    public class MyCanvas : Canvas
    {
        BitmapImage background = null;

        public void LoadImage(string filename)
        {
            background = new BitmapImage(new Uri(filename));
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)        {            if (background != null)            {                dc.DrawImage(background, new Rect(0, 0, background.PixelWidth, background.PixelHeight));            }        }
    }
}
