using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.IO;

using System.Text.RegularExpressions;

using System.Xml.Linq;

namespace WPFexercise
{
    static class FileIO
    {
        public static string[][] OpenDialog()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true; // Allows for multiple file selection
            dlg.FileName = "image"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Image Files(.png;.jpg;.gif;.bmp)|*.png;*.jpg;*.gif;*.bmp"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // returns filepath string
                string[][] nameArray = new string[2][];
                nameArray[0] = dlg.FileNames;
                nameArray[1] = dlg.SafeFileNames;
                
                return nameArray;
            }
            else
            {
                return null;
            }
        }

        public static string SaveDialog()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "image"; // default
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image Files (.png;.jpg;.gif;.bmp)|*.png;*.jpg;*.gif;*.bmp"; // Filter files by extension

            // show dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // process dialog results
            if (result == true)
            {
                return dlg.FileName;
            }
            else
            {
                return null;
            }

        }

        public static void Save(Canvas a_oSurface, string a_sFileName)
        {
            string fileName = a_sFileName;
            string fileExt = System.IO.Path.GetExtension(fileName);

            BitmapEncoder imgEncoder;

            // decide which encoder to use
            switch (fileExt.ToLower())
            {
                case (".png"):
                    {
                        imgEncoder = new PngBitmapEncoder();
                        break;
                    }
                case (".jpg"):
                    {
                        imgEncoder = new JpegBitmapEncoder();
                        break;
                    }
                case (".gif"):
                    {
                        imgEncoder = new GifBitmapEncoder();
                        break;
                    }
                case (".bmp"):
                    {
                        imgEncoder = new BmpBitmapEncoder();
                        break;
                    }
                default:
                    {
                        throw new System.Exception("Invalid file type :(");
                    }
            
            }
            // Save XML File
            SaveXML(a_oSurface, fileName);


            // Export Canvas
            // reference: http://denisvuyka.wordpress.com/2007/12/03/wpf-diagramming-saving-you-canvas-to-image-xps-document-or-raw-xaml/
            // Save current canvas 
            Transform transform = a_oSurface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            a_oSurface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(a_oSurface.Width, a_oSurface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            a_oSurface.Measure(size);
            a_oSurface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBmp =
                new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d, 96d,
                    PixelFormats.Pbgra32);
            renderBmp.Render(a_oSurface);

            // make a cropped bitmap ** not working.. has to do with getting int values from canvas **
            //var croppedBmp = new CroppedBitmap(renderBmp, new Int32Rect(0, (int)a_oSurface.Margin.Top, (int)a_oSurface.Width, (int)a_oSurface.Height));

            // create a file stream for saving image
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            // push rendered bitmap to encoder
            imgEncoder.Frames.Add(BitmapFrame.Create(renderBmp));
            // save data to the stream
            imgEncoder.Save(fileStream);
            fileStream.Close();
            
            /*
            // Export Visual
            // from: http://blogs.msdn.com/b/kirillosenkov/archive/2009/10/12/saving-images-bmp-png-etc-in-wpf-silverlight.aspx
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)a_oVisual.ActualWidth,
                (int)a_oVisual.ActualHeight,
                96d, 96d,
                PixelFormats.Pbgra32);
            bitmap.Render(a_oVisual);

            BitmapFrame frame = BitmapFrame.Create(bitmap);

            // add to frame queue
            imgEncoder.Frames.Add(frame);

            // save to file stream
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            imgEncoder.Save(fileStream);
            fileStream.Close();
             * */

        }
        public static void SaveXML(Canvas a_oCanvas, string a_sFileName)
        {
            
            XDocument d = new XDocument(
                new XComment("This is a comment."),
                new XElement("TextureAtlas",
                    new XAttribute("imagePath", a_sFileName),
                    new XAttribute("width", a_oCanvas.Width),
                    new XAttribute("height", a_oCanvas.Height),
                    // for each image in list
                    //for (int i=0; i < a_oCanvas.
                    new XElement("Book",
                        new XElement("Title", "Artifacts of Roman Civilization"),
                        new XElement("Author", "Moreno, Jordao")
                    ),
                    new XElement("Book",
                        new XElement("Title", "Medieval Tools and Implements"),
                        new XElement("Author", "Gazit, Inbar")
                    )
                ),
                new XComment("This is another comment.")
            );
            d.Declaration = new XDeclaration("1.0", "utf-8", "true");
            Console.WriteLine(d);

            // change file extension
            string fileName = System.IO.Path.ChangeExtension(a_sFileName, ".xml");
            

            // create a file stream for saving file
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            // save data to the stream
            d.Save(fileStream);
            fileStream.Close();
        }

        
    };
}
