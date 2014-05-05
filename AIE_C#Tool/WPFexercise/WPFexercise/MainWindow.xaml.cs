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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "image"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Image FIles(*.png;*.jpg;*.gif;*.bmp)|*.png;*.jpg;*.gif;*.bmp"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // grab the file name of the image
                string filename = dlg.FileName;

                // and load the image

                canvas1.LoadImage(filename);
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
