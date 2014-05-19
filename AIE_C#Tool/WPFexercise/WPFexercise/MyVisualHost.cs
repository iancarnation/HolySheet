using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace WPFexercise
{
    public class MyVisualHost : FrameworkElement
    {
        // Create a collection of child visual objects
        private VisualCollection _children;
        public List<Sprite> imageList;

        public double pixelArea = 0;   // the total area of all images (in pixels) ** should this be int?**
        //public double maxUnitWidth = 0;// greatest width of an image, helps determine how many can fit in a row
        public double tallestInRow = 0;
        public int canvasSize = 0;     // size of canvas side (square)

        public MyVisualHost()
        {
            _children = new VisualCollection(this);
            imageList = new List<Sprite>();
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

        public List<Sprite> GetImageList()
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
                // initialize Sprite objects and poplulate imageList
                for (int i = 0; i < filePaths.Length; i++)
                {
                    // using an Object Initializer, make a new Sprite object
                    Sprite image = new Sprite
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
                    //if (image.Height > tallestInRow)
                    //{
                    //    tallestInRow = image.Height;
                    //}
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

        public int CalculateCanvas(bool a_bIsTooSmall = false)
        {
            // find square root of pixelArea
            double sqrtPixArea = Math.Sqrt(pixelArea);
            // find the next greatest number that is a power of two = desiredCanvasWidth
            int desiredCanvasWidth = (int)sqrtPixArea;
            NextPower2(ref desiredCanvasWidth);
            if (a_bIsTooSmall)
            {
                // increase by 1 so NextPower2() will increment it
                desiredCanvasWidth += 1;
                NextPower2(ref desiredCanvasWidth);
            }
            // set the canvas properties to new desired dimensions
            return desiredCanvasWidth;
        }
        // Image Arrangement Logic
        public void ArrangeImages()
        {
            // there are separate X and Y cursor values for determining the unit's placement on the canvas (in pixels) = cursor.X/Y
            // create "cursor" for image insertion point
            System.Windows.Vector cursor = new Vector(0, 0);
            tallestInRow = 0;

            for (int i = 0; i < imageList.Count(); i++)
            {
                // check if tallest in row
                if (imageList[i].Height > tallestInRow)
                {
                    tallestInRow = imageList[i].Height;
                }

                if (i == 0)
                {
                    imageList[i].Position = cursor;
                }
                else
                {
                    // if the unit will not fit on the current row(pixels would go beyond canvas edge):
                    if (cursor.X + imageList[i - 1].Width + imageList[i].Width > canvasSize)
                    {
                        // move move cursor.Y down equal to the height of the tallest image in the row
                        cursor.Y += tallestInRow;
                        // reset tallestInRow value
                        tallestInRow = 0;
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
                // create a DrawingVisual for the image and add it to the collection
                _children.Add(CreateDrawingVisual(i));
            }

            // check if images have gone beneath edge of canvas...not sure why calculation isn't accurate
            // so this is a band-aid for now
            if (cursor.Y > canvasSize)
            {
                bool canvasTooSmall = true;
                canvasSize = CalculateCanvas(canvasTooSmall);
                // clear the Visual Collection to prepare for rearranging
                _children.Clear();
                ArrangeImages();
            }
        }

        public void NextPower2(ref int a_irValue)
        {
            bool b;

            b = (a_irValue & (a_irValue - 1)) == 0; // determines whether value is a power of 2

            if (!b)
            {
                a_irValue--;
                a_irValue |= a_irValue >> 1;
                a_irValue |= a_irValue >> 2;
                a_irValue |= a_irValue >> 4;
                a_irValue |= a_irValue >> 8;
                a_irValue |= a_irValue >> 16;
                a_irValue++;
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
