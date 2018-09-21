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

            if (usId == _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User))
                lRole.Content = _proxy.GetItemProperty(usId, ServerData.User, PropertyData.RolesName);
            if (lRole.Content.ToString() != "admin")
                ButtonBack_Click(null, null);
            else
                btnUsersAct_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            string img = _proxy.GetItemProperty(id, ServerData.User, PropertyData.Imgpath);
            if (RemoveTemplate(id, "Are you sure you want to remove this user?", ServerData.User))
            {
                _proxy.Delete(img, FilesType.Avatars);
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
        #region Video Categories actions.
        //Show a list of all categories to the admin.
        private void btnVideoCategoriesAct_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.VideoCategory, DataType.VideoCategory);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveVCategory_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveTemplate(Convert.ToInt32((sender as Button).Tag), "Are you sure you want to remove this category?", ServerData.VideoCategory))
                btnVideoCategoriesAct_Click(null, null);
        }
        //Show a form for adding a new category.
        private void btnAddVCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategory form = new AddCategory(_proxy, ServerData.VideoCategory);
            form.ShowDialog();
            btnVideoCategoriesAct_Click(null, null);
        }
        //Show a form for editting category.
        private void btnEditVCategory_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            AddCategory form = new AddCategory(_proxy, ServerData.VideoCategory, id);
            form.ShowDialog();
            btnVideoCategoriesAct_Click(null, null);
        }
        #endregion
        #region Words Categories actions.
        //Show a list of all categories to the admin.
        private void btnWordsCategoriesAct_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.WordCategory, DataType.WordCategory);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveWCategory_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveTemplate(Convert.ToInt32((sender as Button).Tag), "Are you sure you want to remove this category?", ServerData.WordCategory))
                btnWordsCategoriesAct_Click(null, null);
        }
        //Show a form for adding a new category.
        private void btnAddWCategory_Click(object sender, RoutedEventArgs e)
        {
            AddWordsCategory form = new AddWordsCategory(_proxy);
            form.ShowDialog();
            btnWordsCategoriesAct_Click(null, null);
        }
        //Show a form for editting category.
        private void btnEditWCategory_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            AddWordsCategory form = new AddWordsCategory(_proxy, id);
            form.ShowDialog();
            btnWordsCategoriesAct_Click(null, null);
        }
        #endregion
        #region Words Groups actions.
        //Show a list of all categories to the admin.
        private void btnWordsGroupsAct_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.Group, DataType.Group);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveWGroup_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveTemplate(Convert.ToInt32((sender as Button).Tag), "Are you sure you want to remove this group?", ServerData.Group))
                btnWordsGroupsAct_Click(null, null);
        }
        //Show a form for adding a new category.
        private void btnAddWGroup_Click(object sender, RoutedEventArgs e)
        {
            AddCategory form = new AddCategory(_proxy, ServerData.Group);
            form.ShowDialog();
            btnWordsGroupsAct_Click(null, null);
        }
        //Show a form for editting category.
        private void btnEditWGroup_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            AddCategory form = new AddCategory(_proxy, ServerData.Group, id);
            form.ShowDialog();
            btnWordsGroupsAct_Click(null, null);
        }
        #endregion

        #region Video actions.
        //Show a form for editting the video.
        private async void btnEditVideo_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            AddVideo form = new AddVideo(_proxy, Convert.ToInt32((sender as Button).Tag), ServerData.Video);
            form.ShowDialog();

            int[] lst = await _proxy.GetItemsAsync(ServerData.Video);
            await Task.Run(() => LoadList(lst, DataType.Video, true));
        }
        //Remove item and refresh the canvas.
        private void btnRemoveVideo_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            string img = _proxy.GetItemProperty(id, ServerData.Video, PropertyData.Imgpath);
            string path = _proxy.GetItemProperty(id, ServerData.Video, PropertyData.Path);
            string sub = _proxy.GetItemProperty(id, ServerData.Video, PropertyData.SubPath);
            if (RemoveTemplate(id, "Are you sure you want to remove this video?", ServerData.Video))
            {
                _proxy.Delete(img, FilesType.VideosImages);
                _proxy.Delete(sub, FilesType.Subtitles);
                _proxy.Delete(path, FilesType.Videos);
                btnVideos_Click(null, null);
            }
        }
        #endregion
        #region Book actions.
        //Show a form for editting the book.
        private async void btnEditBook_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            string name = _proxy.GetItemProperty(id, ServerData.Book, PropertyData.Name);
            string desc = _proxy.GetItemProperty(id, ServerData.Book, PropertyData.Description);
            string path = _proxy.GetItemProperty(id, ServerData.Book, PropertyData.Path);
            string img = _proxy.GetItemProperty(id, ServerData.Book, PropertyData.Imgpath);
            bool copy = _proxy.CheckAbsolute(id, ServerData.Book) == true? true:false;
            int? year = null;
            if (_proxy.GetItemProperty(id, ServerData.Book, PropertyData.Year) != null)
                year = Convert.ToInt32(_proxy.GetItemProperty(id, ServerData.Book, PropertyData.Year));
            List<int> cat = new List<int>(_proxy.GetItemData(id, ServerData.Book, ServerData.BookCategory));
            List<int> auth = new List<int>(_proxy.GetItemData(id, ServerData.Book, ServerData.Author));
            AddBook form = new AddBook(_proxy, id, name, desc, year, path, copy, cat, auth, img);
            form.ShowDialog();
            
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(ServerData.Book);
            await Task.Run(() => LoadList(lst, DataType.Book, true));
        }
        //Remove item and refresh the canvas.
        private void btnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            string img = _proxy.GetItemProperty(id, ServerData.Book, PropertyData.Imgpath);
            string path = _proxy.GetItemProperty(id, ServerData.Book, PropertyData.Path);
            if (RemoveTemplate(id, "Are you sure you want to remove this book?", ServerData.Book))
            {
                _proxy.Delete(img, FilesType.BooksImages);
                _proxy.Delete(path, FilesType.Books);
                btnBooks_Click(null, null);
            }
        }
        #endregion
        #region Word actions.
        //Show a form for editting the word.
        private void btnEditWord_Click(object sender, RoutedEventArgs e)
        {
            AddWord form = new AddWord(_proxy, Convert.ToInt32((sender as Button).Tag), Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User)));
            form.ShowDialog();
            btnWords_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveWord_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            string img = _proxy.GetItemProperty(id, ServerData.Word, PropertyData.Imgpath);
            if (RemoveTemplate(id, "Are you sure you want to remove this word?", ServerData.Word))
            {
                _proxy.Delete(img, FilesType.WordsImages);
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