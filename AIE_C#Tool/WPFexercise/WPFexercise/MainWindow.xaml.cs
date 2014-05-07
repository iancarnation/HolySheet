using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFexercise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void WindowLoaded(object sender, EventArgs e)
        {
            InitializeComponent(); // still need this?
            MyVisualHost visualHost = new MyVisualHost();
            canvas1.Children.Add(visualHost);
        }
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            //LoadImages();

            //// run the Open File dialog and get the paths and names for all files
            //string filename = FileIO.OpenDialog();

            //if (filename != null)
            //{
            //    canvas1.LoadImage(filename);
            //}
            //else
            //{
            //    return;
            //}

        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            // run the Save File dialog and create file path
            string filename = FileIO.SaveDialog();

            if (filename != null)
            {
                 FileIO.Save(canvas1, filename);
            }
            else
            {
                return;
            }
        }
    }

    public class MyVisualHost : FrameworkElement
    {
        // Create a collection of child visual objects
        private VisualCollection _children;
        public List<Image> imageList = new List<Image>();

        public MyVisualHost()
        {
            _children = new VisualCollection(this);
            // Open File Dialog and load images
            this.LoadImages();
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
        }

        // Capture the mouse event and hit test the coordinate point value against
        // the child visual objects.
        void MyVisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event
            System.Windows.Point pt = e.GetPosition((UIElement)sender);

            // Initiate the hit test by setting up a hit test result callback method
            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(myCallback), new PointHitTestParameters(pt));
        }

        // If a child visual object is hit, toggle its opacity to visually indicate a hit.
        public HitTestResultBehavior myCallback(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof(DrawingVisual))
            {
                if (((DrawingVisual)result.VisualHit).Opacity == 1.0)
                {
                    ((DrawingVisual)result.VisualHit).Opacity = 0.4;
                }
                else
                {
                    ((DrawingVisual)result.VisualHit).Opacity = 1.0;
                }
            }

            // Stop the hit test enumeration of objects in the visual tree
            return HitTestResultBehavior.Stop;
        }

        public void LoadImages()
        {
            // run the Open File dialog and get the paths and names for all files
            string[][] tempArray = FileIO.OpenDialog();
            string[] filePaths = tempArray[0];
            string[] fileNames = tempArray[1];

            if (filePaths[0] != null)
            {
                // create "cursor" for image insertion point
                System.Windows.Vector cursor = new Vector(0, 0);

                // initialize Image objects and poplulate imageList
                for (int i = 0; i < filePaths.Length; i++)
                {
                    // using an Object Initializer, make a new Image object
                    Image image = new Image
                    {
                        FilePath = filePaths[i],
                        Name = fileNames[i],
                        ID = i,
                    };
                    // set more properties after initialization
                    image.Frame = BitmapDecoder.Create(new Uri(image.FilePath), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
                    image.Width = image.Frame.PixelWidth;
                    image.Height = image.Frame.PixelHeight;
                    // add image to list now to allow for checking previous image for dimensions (to determine position)
                    imageList.Add(image);
                    // determine proper position ** currently only making a strip, later determine proper sheet size **
                    if (i == 0)
                    {
                        image.Position = cursor;
                    }
                    else
                    {
                        // add previous images width to X coordinate for proper offset amount
                        cursor.X += imageList[i - 1].Width;
                        image.Position = cursor;
                    }
                    // create a DrawingVisual for the image
                    // add it to the collection ** should the DrawingVisual be a member of the Image?? **
                    _children.Add(CreateDrawingVisual(i));
                }
            }
            else
            {
                return;
            }
        }

        private DrawingVisual CreateDrawingVisual(int index)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw an image in the rectangle into the context
            Rect rect = new Rect((Point)imageList[index].Position, imageList[index].Size);
            drawingContext.DrawImage(imageList[index].Frame, rect);

            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }

        // Provide a required override for the VisualChildrenCount property ** haven't researched **
        protected override int VisualChildrenCount
        {
            get
            { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method. ** haven't researched **
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
