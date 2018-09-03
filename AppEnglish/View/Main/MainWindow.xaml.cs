using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    //Register, visual settings.
    public partial class MainWindow : MetroWindow
    {
        EngServRef.EngServiceClient _proxy;

        //Check connection.
        public MainWindow()
        {
            InitializeComponent();
            _proxy = new EngServRef.EngServiceClient();

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
            imDrag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ImageDrop.png"));

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
                imDrag.Source = new BitmapImage(new Uri(lPath.Content.ToString()));
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

            if (!Directory.Exists("Avatars"))
                Directory.CreateDirectory("Avatars");

            if (lPath.Content.ToString() != "...")
                File.Copy(lPath.Content.ToString(), $@"Avatars\{Path.GetFileName(lPath.Content.ToString())}", true);

            _proxy.AddUserAsync(txtRName.Text, txtRPswd.Password, lPath.Content.ToString() == "..." ? "Wolf.png" : $@"Avatars\{Path.GetFileName(lPath.Content.ToString())}", "user", 0);
            btnReturn_Click(null, null);
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
                string path = _proxy.GetItemPropertyAsync(Convert.ToInt32(txtUserName.Tag), EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath).Result ?? "Wolf.png";
                imUserAvatar.Source = new BitmapImage(new Uri(path != "Wolf.png" ? $"pack://siteoforigin:,,,/{path}" : "pack://application:,,,/Images/Wolf.png"));
                ButtonBack_Click(null, null);
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

        //Close connection.
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_proxy.State == CommunicationState.Opened)
                _proxy.Close();
        }
    }
}