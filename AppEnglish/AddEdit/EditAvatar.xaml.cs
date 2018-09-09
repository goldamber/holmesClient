using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
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

            string path = _proxy.GetItemProperty(id, EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath)?? "Wolf.png";
            if (path == "Wolf.png")
                imDrag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Wolf.png"));
            else if (!File.Exists($@"Temp\Avatars\{path}"))
            {
                Task.Run(new Action(() => {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (!Directory.Exists(@"Temp\Avatars"))
                            Directory.CreateDirectory(@"Temp\Avatars");
                        if (_proxy.Download(path, EngServRef.FilesType.Avatar) != null)
                        {
                            File.WriteAllBytes($@"Temp\Avatars\{path}", _proxy.Download(path, EngServRef.FilesType.Avatar));
                            imDrag.Source = new BitmapImage(new Uri($@"pack://siteoforigin:,,,/Temp\Avatars\{path}"));
                        }
                    }));
                }));
            }
            else
                imDrag.Source = new BitmapImage(new Uri($@"pack://siteoforigin:,,,/Temp\Avatars\{path}"));
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
            Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() =>
                {
                    string file = $"{_proxy.GetItemProperty(id, EngServRef.ServerData.User, EngServRef.PropertyData.Login)}{Path.GetExtension(lPath.Content.ToString())}";
                    if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), file, EngServRef.FilesType.Avatar))
                    {
                        MessageBox.Show($"The file is too large!", "Choose another file", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _proxy.EditData(id, file, EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath);
                    Close();
                }));
            }));
        }
        //Close the form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}