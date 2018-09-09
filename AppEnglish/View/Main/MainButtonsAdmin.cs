using AppEnglish.AddEdit;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish
{
    //Clicks actions (admin).
    public partial class MainWindow : MetroWindow
    {
        #region Users actions.
        //Show a list of all users to the admin.
        private async void btnUsersAct_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.User);
            await Task.Run(() => LoadList(lst, DataType.User));
        }
        //Show a form for editting the role.
        private void btnEditRole_Click(object sender, RoutedEventArgs e)
        {
            int usId = Convert.ToInt32((sender as Button).Tag);
            EditRole form = new EditRole(_proxy, usId);
            form.ShowDialog();

            if (usId == _proxy.GetUserId(lUserName.Content.ToString()))
                lRole.Content = _proxy.GetItemProperty(usId, EngServRef.ServerData.User, EngServRef.PropertyData.RolesName);
            if (lRole.Content.ToString() != "admin")
                ButtonBack_Click(null, null);
            else
                btnUsersAct_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this user?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.Delete(_proxy.GetItemProperty(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath), EngServRef.FilesType.Avatar);
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.User);
                btnUsersAct_Click(null, null);
            }
        }
        #endregion
        #region Video actions.
        //Show a form for editting the video.
        private void btnEditVideo_Click(object sender, RoutedEventArgs e)
        {
            btnVideos_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveVideo_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this video?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.Delete(_proxy.GetItemProperty(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Video, EngServRef.PropertyData.Imgpath), EngServRef.FilesType.VideoImage);
                _proxy.Delete(_proxy.GetItemProperty(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Video, EngServRef.PropertyData.Path), EngServRef.FilesType.Video);
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Video);
                btnVideos_Click(null, null);
            }
        }
        #endregion
        #region Book actions.
        //Show a form for editting the book.
        private void btnEditBook_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            string name = _proxy.GetItemProperty(id, EngServRef.ServerData.Book, EngServRef.PropertyData.Name);
            string desc = _proxy.GetItemProperty(id, EngServRef.ServerData.Book, EngServRef.PropertyData.Description);
            string path = _proxy.GetItemProperty(id, EngServRef.ServerData.Book, EngServRef.PropertyData.Path);
            string img = _proxy.GetItemProperty(id, EngServRef.ServerData.Book, EngServRef.PropertyData.Imgpath);
            bool copy = _proxy.CheckAbsolute(id, EngServRef.ServerData.Book) == true? true:false;
            int? year = null;
            if (_proxy.GetItemProperty(id, EngServRef.ServerData.Book, EngServRef.PropertyData.Year) != null)
                year = Convert.ToInt32(_proxy.GetItemProperty(id, EngServRef.ServerData.Book, EngServRef.PropertyData.Year));
            List<int> lst = new List<int>(_proxy.GetItemData(id, EngServRef.ServerData.Book, EngServRef.ServerData.BookCategory));
            List<int> auth = new List<int>(_proxy.GetItemData(id, EngServRef.ServerData.Book, EngServRef.ServerData.Author));
            AddBook form = new AddBook(_proxy, id, name, desc, year, path, copy, lst, auth, img);
            form.ShowDialog();
            btnBooks_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this book?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.Delete(_proxy.GetItemProperty(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Book, EngServRef.PropertyData.Imgpath), EngServRef.FilesType.BookImage);
                _proxy.Delete(_proxy.GetItemProperty(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Book, EngServRef.PropertyData.Path), EngServRef.FilesType.Book);
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Book);
                btnBooks_Click(null, null);
            }
        }
        #endregion
        #region Word actions.
        //Show a form for editting the word.
        private void btnEditWord_Click(object sender, RoutedEventArgs e)
        {
            btnWords_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveWord_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this word?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.Delete(_proxy.GetItemProperty(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Word, EngServRef.PropertyData.Imgpath), EngServRef.FilesType.WordImage);
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Word);
                btnWords_Click(null, null);
            }
        }
        #endregion
    }
}