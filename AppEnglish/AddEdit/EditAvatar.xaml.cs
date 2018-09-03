using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AppEnglish.AddEdit
{
    public partial class EditAvatar : Window
    {
        EngServRef.EngServiceClient _proxy;
        int id;

        #region Constructors.
        //Initialization.
        public EditAvatar()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'. Requires the users id.
        public EditAvatar(EngServRef.EngServiceClient tmp, int id) : this()
        {
            _proxy = tmp;
            this.id = id;
        }
        #endregion
        #region Drag&drop.
        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            (sender as Border).Opacity = 1;
        }
        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            (sender as Border).Opacity = 0.4;
        }
        //Choose image.
        private void Border_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            lPath.Content = files[0];
            btnOK.IsEnabled = true;
            imDrag.Source = new BitmapImage(new Uri(lPath.Content.ToString()));
            brImage.Opacity = 0.4;
        }
        //Click.
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.tif)|*.png;*.jpg;*.jpeg;*.gif;*.tif|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                lPath.Content = openFileDialog.FileName;
                btnOK.IsEnabled = true;
                imDrag.Source = new BitmapImage(new Uri(lPath.Content.ToString()));
            }
        }
        #endregion
        #region Close form (OK, Cancel).
        //Change.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _proxy.EditData(id, lPath.Content.ToString(), EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath);
            Close();
        }
        //Close the form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}