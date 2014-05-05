using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.IO;

using System.Text.RegularExpressions;

namespace WPFexercise
{
    static class FileIO
    {
        public static string OpenDialog()
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
                return dlg.FileName;
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

        public static void Save(BitmapImage a_oNewImage, string a_sFileName)
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

            // add to frame queue
            imgEncoder.Frames.Add(BitmapFrame.Create(a_oNewImage));

            // save to file stream
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            imgEncoder.Save(fileStream);
            fileStream.Close();

        }
        
    };
}
