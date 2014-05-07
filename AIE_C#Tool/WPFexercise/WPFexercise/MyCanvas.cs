using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

//namespace WPFexercise
//{
//    public class MyCanvas : Canvas
//    {
    //    public BitmapImage background = null;
    //    public List<Image> imageList = new List<Image>();

    //    public void LoadImages()
    //    {
    //        // run the Open File dialog and get the paths and names for all files
    //        string[] filePaths = FileIO.OpenDialog()[0];
    //        string[] fileNames = FileIO.OpenDialog()[1];

    //        if (filePaths[0] != null)
    //        {
    //            // initialize Image objects and poplulate imageList
    //            for (int i = 0; i < filePaths.Length; i++)
    //            {
    //                // using an Object Initializer, make a new Image object
    //                Image image = new Image
    //                {
    //                    FilePath = filePaths[i],
    //                    Name = fileNames[i],
    //                    ID = i,
    //                    Frame = BitmapDecoder.Create(new Uri(image.FilePath), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First(),
    //                    Width = image.Frame.PixelWidth,
    //                    Height = image.Frame.PixelHeight,
    //                };
    //            }
    //        }
    //        else
    //        {
    //            return;
    //        }

    //       // background = new BitmapImage(new Uri(filename));
    //        this.InvalidateVisual();


    //    }

    //    protected override void OnRender(DrawingContext dc)    //    {    //        if (background != null)    //        {    //            dc.DrawImage(background, new Rect(0, 0, background.PixelWidth, background.PixelHeight));    //        }    //    }
    //}
//}
