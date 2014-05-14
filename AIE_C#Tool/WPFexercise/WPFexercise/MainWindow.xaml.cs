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
using System.Xml.Linq;
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
            // set canvas size here?
            int canvasSize = visualHost.CalculateCanvas();
            canvas1.Width = canvasSize;
            canvas1.Height = canvasSize;

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
        public List<Image> imageList;

        public double pixelArea = 0;   // the total area of all images (in pixels) ** should this be int?**
        public double maxUnitWidth = 0;// greatest width of an image, helps determine how many can fit in a row
        public int canvasSize = 0;     // size of canvas side (square)

        public MyVisualHost()
        {
            _children = new VisualCollection(this);
            imageList = new List<Image>();
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

        public List<Image> GetImageList()
        {
            return imageList;
        }

        public void LoadImages()
        {
            // run the Open File dialog and get the paths and names for all files
            string[][] tempArray = FileIO.OpenDialog();
            string[] filePaths = tempArray[0];
            string[] fileNames = tempArray[1];

            if (filePaths[0] != null)
            {
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
                    // add image's area to total pixel area, and width to total pixel width
                    double imageArea = image.Width * image.Height;
                    pixelArea += imageArea;
                    if (image.Width > maxUnitWidth)
                    {
                        maxUnitWidth = image.Width;
                    }
                    // add image to list now to allow for checking previous image for dimensions (to determine position)
                    imageList.Add(image);
                }
                // - determine total area in pixels = pixelArea
                // - determine the total number of images (area in 'units') = unitArea
                int unitArea = imageList.Count();
                // Determine canvas size
                canvasSize = CalculateCanvas();
                // Arrange Images
                ArrangeImages();                
            }
            else
            {
                return;
            }
        }
        // Determine Canvas Size..let's go with nearest power of two for now
        public int CalculateCanvas()
        {
            // find square root of pixelArea
            double sqrtPixArea = Math.Sqrt(pixelArea);
            // find the next greatest number that is a power of two = desiredCanvasWidth
            int desiredCanvasWidth = (int)sqrtPixArea;
            NextPower2(ref desiredCanvasWidth);
            // set the canvas properties to new desired dimensions
            return desiredCanvasWidth;
        }
        // Image Arrangement Logic
        public void ArrangeImages()
        {
            // determine how many units can wholly fit within the desiredCanvasWidth
            int maxUnitInRow = (int)(canvasSize / maxUnitWidth);
            // there are separate X and Y cursor values for determining the unit's placement on the canvas (in pixels) = cursor.X/Y
            // create "cursor" for image insertion point
            System.Windows.Vector cursor = new Vector(0, 0);
            
            for (int i = 0; i < imageList.Count(); i++)
            {
                if (i == 0)
                {
                    imageList[i].Position = cursor;
                }
                else
                {
                    // if the unit ID is a multiple of maxUnitInRow:
                    if (imageList[i].ID % maxUnitInRow == 0)
                    {
                        // move cursor.Y down to the bottom of unit with ID = this ID - maxUnitInRow
                        cursor.Y += (imageList[(imageList[i].ID - maxUnitInRow)].Height);
                        // move cursor.X back to 0
                        cursor.X = 0;
                        // set unit's position
                        imageList[i].Position = cursor;
                    }
                    else
                    {
                        // add previous unit's width to cursor.X for proper offset amount
                        cursor.X += imageList[i - 1].Width;
                        // set unit's position
                        imageList[i].Position = cursor;
                    }
                }
                // create a DrawingVisual for the image
                // add it to the collection ** should the DrawingVisual be a member of the Image?? **
                _children.Add(CreateDrawingVisual(i));
            }
        }

        public void NextPower2(ref int a_irValue)
        {
            bool b;

            b = (a_irValue & (a_irValue - 1)) == 0; // determines whether value is a power of 2

            if (!b)
            {
                a_irValue --;
                a_irValue |= a_irValue >> 1;
                a_irValue |= a_irValue >> 2;
                a_irValue |= a_irValue >> 4;
                a_irValue |= a_irValue >> 8;
                a_irValue |= a_irValue >> 16;
                a_irValue ++;
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
