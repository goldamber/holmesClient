using AppEnglish.AddEdit;
using AppEnglish.EngServRef;
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
        private void btnUsersAct_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.User, DataType.User);
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
            int id = Convert.ToInt32((sender as Button).Tag);
            if (RemoveTemplate(id, "Are you sure you want to remove this user?", ServerData.User))
            {
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.User, PropertyData.Imgpath), FilesType.Avatar);
                btnUsersAct_Click(null, null);
            }
        }
        #endregion
        #region Authors actions.
        //Show a list of all authors to the admin.
        private void btnAuthorsAct_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.Author, DataType.Author);
        }
        //Show a form for adding a new author.
        private void btnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AddAuthor form = new AddAuthor(_proxy);
            form.ShowDialog();
            btnAuthorsAct_Click(null, null);
        }
        //Show a form for editting authors data.
        private void btnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            AddAuthor form = new AddAuthor(_proxy, id);
            form.ShowDialog();
            btnAuthorsAct_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveTemplate(Convert.ToInt32((sender as Button).Tag), "Are you sure you want to remove this author?", ServerData.Author))
                btnAuthorsAct_Click(null, null);
        }
        #endregion
        #region Books Categories actions.
        //Show a list of all categories to the admin.
        private void btnBooksCategoriesAct_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.BookCategory, DataType.BookCategory);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveBCategory_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveTemplate(Convert.ToInt32((sender as Button).Tag), "Are you sure you want to remove this category?", ServerData.BookCategory))
                btnBooksCategoriesAct_Click(null, null);
        }
        //Show a form for adding a new category.
        private void btnAddBCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategory form = new AddCategory(_proxy, ServerData.BookCategory);
            form.ShowDialog();
            btnBooksCategoriesAct_Click(null, null);
        }
        //Show a form for editting category.
        private void btnEditBCategory_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            AddCategory form = new AddCategory(_proxy, ServerData.BookCategory, id);
            form.ShowDialog();
            btnBooksCategoriesAct_Click(null, null);
        }
        #endregion

        #region Video actions.
        //Show a form for editting the video.
        private async void btnEditVideo_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Video);
            await Task.Run(() => LoadList(lst, DataType.Video, true));
        }
        //Remove item and refresh the canvas.
        private void btnRemoveVideo_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            if (RemoveTemplate(id, "Are you sure you want to remove this video?", ServerData.Video))
            {
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.Video, PropertyData.Imgpath), FilesType.VideoImage);
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.Video, PropertyData.SubPath), FilesType.Subtitles);
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.Video, PropertyData.Path), FilesType.Video);
                btnVideos_Click(null, null);
            }
        }
        #endregion
        #region Book actions.
        //Show a form for editting the book.
        private async void btnEditBook_Click(object sender, RoutedEventArgs e)
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
            List<int> cat = new List<int>(_proxy.GetItemData(id, EngServRef.ServerData.Book, EngServRef.ServerData.BookCategory));
            List<int> auth = new List<int>(_proxy.GetItemData(id, EngServRef.ServerData.Book, EngServRef.ServerData.Author));
            AddBook form = new AddBook(_proxy, id, name, desc, year, path, copy, cat, auth, img);
            form.ShowDialog();
            
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Book);
            await Task.Run(() => LoadList(lst, DataType.Book, true));
        }
        //Remove item and refresh the canvas.
        private void btnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            if (RemoveTemplate(id, "Are you sure you want to remove this book?", ServerData.Book))
            {
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.Book, PropertyData.Imgpath), FilesType.BookImage);
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.Book, PropertyData.Path), FilesType.Book);
                btnBooks_Click(null, null);
            }
        }
        #endregion
        #region Word actions.
        //Show a form for editting the word.
        private async void btnEditWord_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Word);
            await Task.Run(() => LoadList(lst, DataType.Word, true));
        }
        //Remove item and refresh the canvas.
        private void btnRemoveWord_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            if (RemoveTemplate(id, "Are you sure you want to remove this word?", ServerData.Word))
            {
                _proxy.Delete(_proxy.GetItemProperty(id, ServerData.Word, PropertyData.Imgpath), FilesType.WordImage);
                btnWords_Click(null, null);
            }
        }
        #endregion

        /// <summary>
        /// Generate a list of items (admin).
        /// </summary>
        /// <param name="type">Items type.</param>
        /// <param name="clientType">Items type.</param>
        async void GenerateListTemplate(ServerData type, DataType clientType)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });
            
            int[] lst = await _proxy.GetItemsAsync(type);
            await Task.Run(() => LoadList(lst, clientType, false));
        }
        /// <summary>
        /// Deletes an item.
        /// </summary>
        /// <param name="id">Items id.</param>
        /// <param name="text">Question to the user.</param>
        /// <param name="type">Items type.</param>
        /// <returns>
        /// True - if user is sure about deleting this item.
        /// True - if user is not sure about deleting this item.
        /// </returns>
        bool RemoveTemplate(int id, string text, ServerData type)
        {
            if (MessageBox.Show(text, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemAsync(id, type);
                return true;
            }
            return false;
        }
    }
}