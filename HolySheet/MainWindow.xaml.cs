using System;
using System.Windows;

namespace HolySheet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void WindowLoaded(object sender, EventArgs e)
        {
            InitializeComponent();
            
        }
        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            MyVisualHost visualHost = new MyVisualHost();
            // retrieve the calculated canvas size from visualHost
            int desiredCanvasSize = visualHost.canvasSize;
            canvas1.Width = desiredCanvasSize;
            canvas1.Height = desiredCanvasSize;

            canvas1.Children.Add(visualHost);

        }

        private void MenuItemExport_Click(object sender, RoutedEventArgs e)
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
}
