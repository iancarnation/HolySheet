using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFexercise
{
    public class Image
    {
        string m_sFilePath, m_sSafeFileName;
        int m_iID;
        System.Windows.Vector m_Position;
        System.Windows.Size m_Size;
        BitmapFrame m_BmpFrame;
        // the full file path
        public string FilePath
        {
            get { return m_sFilePath; }
            set { m_sFilePath = value; }
        }
        // file name and extension only
        public string Name
        {
            get { return m_sSafeFileName; }
            set { m_sSafeFileName = value; }
        }
        public int ID
        {
            get { return m_iID; }
            set { m_iID = value; }
        }
        public BitmapFrame Frame
        {
            get { return m_BmpFrame; }
            set { m_BmpFrame = value; }
        }
        public System.Windows.Vector Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        public double X
        {
            get { return m_Position.X; }
            set { m_Position.X = value; }
        }
        public double Y
        {
            get { return m_Position.Y; }
            set { m_Position.Y = value; }
        }
        public System.Windows.Size Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }
        public double Width
        {
            get { return m_Size.Width; }
            set { m_Size.Width = value; }
        }
        public double Height
        {
            get { return m_Size.Height; }
            set { m_Size.Height = value; }
        }
    }
}
