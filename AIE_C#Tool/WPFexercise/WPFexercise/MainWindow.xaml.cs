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

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            // run the Open File dialog and get the file path
            string filename = FileIO.OpenDialog();

            if (filename != null)
            {
                canvas1.LoadImage(filename);
            }
            else
            {
                return;
            }
            
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            // run the Save File dialog and create file path
            string filename = FileIO.SaveDialog();

            if (filename != null)
            {
                FileIO.Save(canvas1.background, filename);
            }
            else
            {
                return;
            }
        }
    }
}
