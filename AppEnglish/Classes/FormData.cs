using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    /// <summary>
    /// The metadata transfered between forms.
    /// </summary>
    static class FormData
    {
        public static string Author { get; set; } = "";
        public static int AuthorsID { get; set; }

        public static List<int> WordsToPrint { get; set; } = new List<int>();

        public static int FilterPosition { get; set; } = 0;     //The postion of 'Filter' comboBox.
        public static int SortPosition { get; set; } = 0;       //The postion of 'Sort' comboBox.

        #region Types that contains images. They can be updated only upon restart.
        public static List<int> EditBooks { get; set; } = new List<int>();
        public static List<int> EditVideos { get; set; } = new List<int>();
        public static List<int> EditWords { get; set; } = new List<int>();
        public static List<int> EditUsers { get; set; } = new List<int>();
        #endregion

        /// <summary>
        /// Set a source to the image.
        /// </summary>
        /// <param name="path">Path to the image file</param>
        /// <param name="img">Image.</param>
        public static void SetImage(string path, Image img)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path);
            bitmapImage.EndInit();
            img.Source = bitmapImage;
        }
    }
}