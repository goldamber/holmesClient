using AppEnglish.EngServRef;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish
{
    //Register, visual settings.
    public partial class MainWindow : MetroWindow
    {
        EngServiceClient _proxy;

        //Check connection.
        public MainWindow()
        {
            InitializeComponent();
            DeleteTemporary();
            _proxy = new EngServiceClient();

            try
            {
                _proxy.Open();
                if (_proxy.InnerChannel.State != CommunicationState.Opened)
                    throw new EndpointNotFoundException();
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("There is no connection to a service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        #region Visual settings (visibility, size).
        //Size changed.
        private void stRegister_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in ((sender as ScrollViewer).Content as StackPanel).Children)
            {
                if (item is StackPanel)
                {
                    foreach (var val in (item as StackPanel).Children)
                    {
                        double len = (sender as ScrollViewer).ActualWidth - ((item as StackPanel).Children[0] as Label).ActualWidth - 25;

                        if (val is TextBox && len > 10)
                            (val as TextBox).Width = len;
                        if (val is PasswordBox && len > 10)
                            (val as PasswordBox).Width = len;
                    }
                }
            }
        }
        //Return (visibility).
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            stRegister.Visibility = Visibility.Collapsed;
            stLogin.Visibility = Visibility.Collapsed;
            stFirst.Visibility = Visibility.Visible;         
        }
        //Login (visibility).
        private void Login_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtUserName.Text = "";
            txtPswd.Password = "";

            stFirst.Visibility = Visibility.Collapsed;
            stLogin.Visibility = Visibility.Visible;
        }
        //Register (visibility).
        private void Register_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtRName.Text = "";
            txtRPswd.Password = "";
            txtRCfrPswd.Password = "";
            lPath.Content = "...";
            FormData.SetImage("pack://application:,,,/Images/ImageDrop.png", imDrag);

            stFirst.Visibility = Visibility.Collapsed;
            stRegister.Visibility = Visibility.Visible;
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
            FormData.SetImage(lPath.Content.ToString(), imDrag);
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
                FormData.SetImage(lPath.Content.ToString(), imDrag);
            }
        }
        #endregion

        #region Register.
        //Check the existence of login.
        private void txtREmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isUser = _proxy.CheckExistenceAsync(txtRName.Text, EngServRef.ServerData.User).Result;

            foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
            {
                if (item is TextBox)
                    item.Style = TryFindResource(isUser ? "txtWrong" : "txtNormal") as Style;
                else if (item is Label)
                    item.Style = TryFindResource(isUser ? "lbFormWrong" : "lbFormNormal") as Style;
            }
            ((sender as TextBox).Parent as Panel).ToolTip = isUser ? "This username already exists!" : "Input data.";
            btnReg.IsEnabled = !isUser && txtRName.Text != "" && txtRPswd.Password.Length >= 6;

            if ((sender as TextBox).Text == "")
                ((sender as TextBox).Parent as Panel).ToolTip = "Empty strings are not allowed!";
        }
        //Check the length of password.
        private void txtRPswd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool wrongPswd = (sender as PasswordBox).Password.Length < 6;

            foreach (FrameworkElement item in ((sender as PasswordBox).Parent as Panel).Children)
            {
                if (item is PasswordBox)
                    item.Style = TryFindResource(wrongPswd ? "pswdWrong" : "pswdNormal") as Style;
                else if (item is Label)
                    item.Style = TryFindResource(wrongPswd ? "lbFormWrong" : "lbFormNormal") as Style;
            }
            ((sender as PasswordBox).Parent as Panel).ToolTip = wrongPswd ? "Password must contain more than 6 symbols!" : "Input password.";

            btnReg.IsEnabled = !wrongPswd && txtRName.Text != "" && !_proxy.CheckExistenceAsync(txtRName.Text, EngServRef.ServerData.User).Result;
        }
        //Add user to a database and return to the first window.
        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            if (txtRPswd.Password != txtRCfrPswd.Password)
            {
                MessageBox.Show("Passwords do not match! Try again.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Task.Run(new Action(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    int id = _proxy.GetLastId(EngServRef.ServerData.User);

                    if (lPath.Content.ToString() != "...")
                    {
                        string ava = $"{id}{Path.GetExtension(lPath.Content.ToString())}";
                        if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), ava, EngServRef.FilesType.Avatars))
                        {
                            MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    _proxy.AddUserAsync(txtRName.Text, txtRPswd.Password, lPath.Content.ToString() == "..."? "Wolf.png": $"{id}{Path.GetExtension(lPath.Content.ToString())}", "user", 0);

                    btnReturn_Click(null, null);
                }));
            }));
        }
        #endregion
        #region Login, logout.
        //Chech the length password.
        private void txtPswd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool wrongPswd = (sender as PasswordBox).Password == "";

            foreach (FrameworkElement item in ((sender as PasswordBox).Parent as Panel).Children)
            {
                if (item is PasswordBox)
                    item.Style = TryFindResource(wrongPswd ? "pswdWrong" : "pswdNormal") as Style;
                else if (item is Label)
                    item.Style = TryFindResource(wrongPswd ? "lbFormWrong" : "lbFormNormal") as Style;
            }
            ((sender as PasswordBox).Parent as Panel).ToolTip = wrongPswd ? "Empty strings are not allowed!" : "Input password.";

            btnLogin.IsEnabled = !wrongPswd && txtUserName.Text != "";
        }
        //Chech the existence of login.
        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isUser = _proxy.CheckExistenceAsync(txtUserName.Text, EngServRef.ServerData.User).Result;

            foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
            {
                if (item is TextBox)
                    item.Style = TryFindResource(!isUser ? "txtWrong" : "txtNormal") as Style;
                else if (item is Label)
                    item.Style = TryFindResource(!isUser ? "lbFormWrong" : "lbFormNormal") as Style;
            }
            ((sender as TextBox).Parent as Panel).ToolTip = !isUser ? "This username does not exist!" : "Input data.";
            btnLogin.IsEnabled = isUser && txtUserName.Text != "" && txtPswd.Password != "";

            if ((sender as TextBox).Text == "")
                ((sender as TextBox).Parent as Panel).ToolTip = "Empty strings are not allowed!";
        }

        //Login and show the actions to user.
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!_proxy.CheckUserPswdAsync(txtUserName.Text, txtPswd.Password).Result)
                MessageBox.Show("Wrong password!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                stLogin.Visibility = Visibility.Collapsed;
                grCab.Visibility = Visibility.Visible;

                lUserName.Content = txtUserName.Text.ToUpper();
                if (_proxy.GetUserIdAsync(txtUserName.Text).Result == null)
                {
                    MessageBox.Show("This user does not exist!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                txtUserName.Tag = _proxy.GetUserIdAsync(txtUserName.Text).Result;
                string roleId = _proxy.GetItemPropertyAsync(Convert.ToInt32(txtUserName.Tag), EngServRef.ServerData.User, EngServRef.PropertyData.Role).Result;
                lRole.Content = roleId == null? "": _proxy.GetItemProperty(Convert.ToInt32(roleId), EngServRef.ServerData.Role, EngServRef.PropertyData.Name);
                SetAvatar(Convert.ToInt32(txtUserName.Tag), false);
                ButtonBack_Click(null, null);
            }
        }
        void SetAvatar(int id, bool edit)
        {
            string path = _proxy.GetItemPropertyAsync(id, EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath).Result ?? "Wolf.png";
            if (path == "Wolf.png")
                FormData.SetImage("pack://application:,,,/Images/Wolf.png", imUserAvatar);
            else
            {
                Task.Run(new Action(() =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (!Directory.Exists(@"Temp\Avatars"))
                            Directory.CreateDirectory(@"Temp\Avatars");
                        byte[] res = _proxy.Download(path, EngServRef.FilesType.Avatars);
                        if (res != null)
                        {
                            try
                            {
                                FileInfo file = new FileInfo($@"Temp\Avatars\{path}");
                                file.Refresh();
                                using (FileStream fs = file.OpenWrite())
                                {
                                    fs.Write(res, 0, res.Length);
                                    fs.Dispose();
                                }
                            }
                            catch (IOException)
                            {
                                if (edit)
                                {
                                    if (MessageBox.Show("The data have been uploaded to the server. It will be updated the next time you come.\nDo you want to restart now?", "Check next time", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                        Close();
                                }
                            }

                            FormData.SetImage($@"pack://siteoforigin:,,,/Temp\Avatars\{path}", imUserAvatar);
                        }
                    }));
                }));
            }
        }
        //Go to the main menu and logout.
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            imUserAvatar.Source = null;
            lUserName.Content = "";
            lRole.Content = "";

            grCab.Visibility = Visibility.Collapsed;
            stFirst.Visibility = Visibility.Visible;
        }
        #endregion

        //Deletes all temporary files.
        void DeleteTemporary()
        {
            if (Directory.Exists("Temp"))
            {
                string[] arr = Directory.GetDirectories("Temp");
                foreach (string item in arr)
                {
                    string[] files = Directory.GetFiles(item);
                    foreach (string val in files)
                    {
                        try
                        {
                            File.Delete(val);
                        }
                        catch { }
                    }
                }
            }
        }
        //Close connection.
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_proxy.State == CommunicationState.Opened)
                _proxy.Close();
            DeleteTemporary();
        }
    }
}