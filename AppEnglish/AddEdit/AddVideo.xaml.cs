using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppEnglish.AddEdit
{
    public partial class AddVideo : Window
    {
        EngServRef.EngServiceClient _proxy;
        int? video = null;
        int? mark = null;
        int? user;

        #region Constructors, init.
        //Initialization.
        public AddVideo()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes '_proxy', fills the listboxes.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="userId">Id of user.</param>
        public AddVideo(EngServRef.EngServiceClient tmp, int? userId) : this()
        {
            _proxy = tmp;
            user = userId;
            FillCategories();
        }
        /// <summary>
        /// 'Edit' form. Hides rating, fills fields.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="id">Video id.</param>
        public AddVideo(EngServRef.EngServiceClient tmp, int id, EngServRef.ServerData type) : this()
        {
            _proxy = tmp;
            video = id;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (video != null)
            {
                txtName.Text = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Name);
                txtDesc.Text = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Description);
                txtPath.Text = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Path);
                txtSubs.Text = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.SubPath);
                if (_proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Year) != null)
                    txtYear.Text = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Year).ToString();
                chCopy.IsChecked = !_proxy.CheckAbsolute(Convert.ToInt32(video), EngServRef.ServerData.Video);
                string imgPath = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Imgpath);
                if (imgPath == "WolfV.png")
                    FormData.SetImage("pack://application:,,,/Images/WolfV.png", imDrag);
                else
                {
                    if (File.Exists($@"Temp\VideoImages\{imgPath}"))
                        FormData.SetImage($@"pack://siteoforigin:,,,/Temp\VideoImages\{imgPath}", imDrag);
                    else
                        MessageBox.Show("Image can not be found!", "Something went wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                lPath.Content = "...";

                stRating.Visibility = Visibility.Collapsed;
                FillCategories(new List<int>(_proxy.GetItemData(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.ServerData.VideoCategory)));
            }
        }
        #endregion

        #region Visualisation, validation.
        //Change the size of the inner fields.
        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in (sender as StackPanel).Children)
            {
                if (item is StackPanel)
                {
                    foreach (FrameworkElement val in (item as Panel).Children)
                    {
                        double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 40;
                        if (val is TextBox && len > 10)
                            val.Width = len;
                    }
                }
            }
        }
        //Check the title of video.
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "" || (((sender as TextBox) == txtName) && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Video).Result && video == null) || (((sender as TextBox) == txtName) && video != null && txtName.Text != _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Name) && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Video).Result))
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = (sender as TextBox).Text == "" ? "Empty strings are not allowed!" : "This name is already taken!";
                btnOK.IsEnabled = false;
            }
            else
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtNormal") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormNormal") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = "Input data.";
                btnOK.IsEnabled = true;
            }
        }
        //Forbids the input of letters.
        private void txtYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Fill 'Categories' list-box.
        void FillCategories()
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.VideoCategory).Result);
            foreach (int item in lst)
            {
                lstCategory.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.VideoCategory, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left });
            }
        }
        //Fill 'Categories' list-box with default values.
        void FillCategories(List<int> tmp)
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.VideoCategory).Result);
            foreach (int item in lst)
            {
                lstCategory.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.VideoCategory, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = tmp.Contains(item) });
            }
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
        //Sets image.
        private void Border_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            lPath.Content = files[0];
            FormData.SetImage(lPath.Content.ToString(), imDrag);
            brImage.Opacity = 0.4;
        }
        //Choose image (via OpenFileDialog).
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.tif)|*.png;*.jpg;*.jpeg;*.gif;*.tif|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                lPath.Content = openFileDialog.FileName;
                FormData.SetImage(lPath.Content.ToString(), imDrag);
            }
        }
        #endregion
        #region Rating.
        //Sets a rate.
        private void imgRating_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mark = ((sender as Image).Opacity == 1) ? 0 : Convert.ToInt32((sender as Image).Tag);
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = mark;
                item.Opacity = (mark != 0 && Convert.ToInt32(item.Tag) <= Convert.ToInt32((sender as Image).Tag)) ? 1 : 0.2;
            }
        }
        //Visualisation.
        private void imgRating_MouseEnter(object sender, MouseEventArgs e)
        {
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = (sender as Image).Tag;
                item.Opacity = (Convert.ToInt32(item.Tag) <= Convert.ToInt32((sender as Image).Tag)) ? 0.5 : 0.2;
            }
        }
        //Visualisation.
        private void imgRating_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = (mark == null) ? "N/A" : mark.ToString();
                item.Opacity = (mark != null && Convert.ToInt32(item.Tag) <= mark) ? 1 : 0.2;
            }
        }
        #endregion
        #region Buttons.
        //Choose the location of a file.
        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video files (*.mp4, *.mkv, *.flv, *.webm, *.avi, *.mov, *.wmv, *.mpg, *.mpeg)|*.mp4; *.mkv; *.flv; *.webm; *.avi; *.mov; *.wmv; *.mpg; *.mpeg|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
                txtPath.Text = ofd.FileName;
        }
        //Choose the location of subs.
        private void btnSubs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Subtitle files (*.srt, *.sub, *.sbv)|*.srt; *.sub; *.sbv|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
                txtSubs.Text = ofd.FileName;
        }
        #endregion

        #region Close form (OK, Cancel).
        //Add a new video.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int? year = null;
            if (txtYear.Text != "")
                year = Convert.ToInt32(txtYear.Text);
            int edit = 0;

            if (txtPath.Text == "")
            {
                MessageBox.Show("Choose file!", "No file chosen", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() =>
                {
                    stMain.Visibility = Visibility.Collapsed;
                    stPreloader.Visibility = Visibility.Visible;
                    stPreloader.Children.Clear();
                    stPreloader.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

                    if (video == null)
                    {
                        int? id = _proxy.AddVideo(txtName.Text, txtDesc.Text == "" ? null : txtDesc.Text, txtPath.Text, txtSubs.Text, "WolfV.png", chCopy.IsChecked == true ? false : true, mark, user, year, DateTime.Now);
                        if (id == null)
                        {
                            MessageBox.Show("Something went wrong.", "Operation denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                            stMain.Visibility = Visibility.Visible;
                            stPreloader.Visibility = Visibility.Collapsed;
                            return;
                        }
                        edit = Convert.ToInt32(id);

                        if (lPath.Content.ToString() != "...")
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), $"{edit}{Path.GetExtension(lPath.Content.ToString())}", EngServRef.FilesType.VideosImages))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                _proxy.RemoveItem(edit, EngServRef.ServerData.Video);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(lPath.Content.ToString())}", EngServRef.ServerData.Video, EngServRef.PropertyData.Imgpath);
                        }
                        if (chCopy.IsChecked == true)
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(txtPath.Text), $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.FilesType.Videos))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                _proxy.RemoveItem(edit, EngServRef.ServerData.Video);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.ServerData.Video, EngServRef.PropertyData.Path);
                        }
                        if (txtSubs.Text != "")
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(txtSubs.Text), $"{edit}{Path.GetExtension(txtSubs.Text)}", EngServRef.FilesType.Subtitles))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                _proxy.RemoveItem(edit, EngServRef.ServerData.Video);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtSubs.Text)}", EngServRef.ServerData.Video, EngServRef.PropertyData.SubPath);
                        }
                    }
                    else
                    {
                        edit = Convert.ToInt32(video);

                        string path = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.Path);
                        if (txtPath.Text != path && chCopy.IsChecked == true)
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(txtPath.Text), $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.FilesType.Videos))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.ServerData.Video, EngServRef.PropertyData.Path);
                        }
                        else if (txtPath.Text == path && txtPath.Text.Contains(":") && chCopy.IsChecked == true)
                        {
                            if (!File.Exists(txtPath.Text))
                            {
                                MessageBox.Show("This file does not exist!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            if (!_proxy.Upload(File.ReadAllBytes(txtPath.Text), $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.FilesType.Videos))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.ServerData.Video, EngServRef.PropertyData.Path);
                        }
                        else if (chCopy.IsChecked == false)
                            _proxy.EditData(edit, txtPath.Text, EngServRef.ServerData.Video, EngServRef.PropertyData.Path);
                        string subs = _proxy.GetItemProperty(Convert.ToInt32(video), EngServRef.ServerData.Video, EngServRef.PropertyData.SubPath);
                        if (txtSubs.Text != subs)
                        {
                            if (!File.Exists(txtSubs.Text))
                            {
                                MessageBox.Show("This file does not exist!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            if (!_proxy.Upload(File.ReadAllBytes(txtSubs.Text), $"{edit}{Path.GetExtension(txtSubs.Text)}", EngServRef.FilesType.Subtitles))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtSubs.Text)}", EngServRef.ServerData.Video, EngServRef.PropertyData.SubPath);
                        }
                        if (lPath.Content.ToString() != "...")
                        {
                            FormData.EditVideos.Add(edit);
                            string file = $"{edit}{Path.GetExtension(lPath.Content.ToString())}";
                            if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), file, EngServRef.FilesType.VideosImages))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                stPreloader.Visibility = Visibility.Collapsed;
                                return;
                            }
                            _proxy.EditData(edit, file, EngServRef.ServerData.Video, EngServRef.PropertyData.Imgpath);
                        }
                        _proxy.EditData(edit, chCopy.IsChecked != true ? "True" : null, EngServRef.ServerData.Video, EngServRef.PropertyData.IsAbsolute);
                        _proxy.EditData(edit, txtYear.Text == "" ? null : txtYear.Text, EngServRef.ServerData.Video, EngServRef.PropertyData.Year);
                        _proxy.RemoveFullItemData(edit, EngServRef.ServerData.VideoCategory);
                        _proxy.EditData(edit, txtName.Text, EngServRef.ServerData.Video, EngServRef.PropertyData.Name);
                        _proxy.EditData(edit, txtDesc.Text == "" ? null : txtDesc.Text, EngServRef.ServerData.Video, EngServRef.PropertyData.Description);
                    }

                    foreach (CheckBox item in lstCategory.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.VideoCategory);
                    }
                    Close();
                }));
            }));
        }
        //Close form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}