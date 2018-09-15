using MahApps.Metro.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish
{
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Add user to template.
        /// </summary>
        /// <param name="item">Users Id.</param>
        /// <param name="exp">The element in which an item is supposed to appear.</param>
        private void AddUserItem(int item, Expander exp)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddImage(item, "Wolf.png", "Avatars", st, EngServRef.ServerData.User, false);

                    AddStaticContent(item, st, EngServRef.ServerData.User, EngServRef.PropertyData.Name);
                    AddHoverableData(item, EngServRef.ServerData.User, EngServRef.PropertyData.RolesName, st);
                    AddHoverableData(item, EngServRef.ServerData.User, EngServRef.PropertyData.Level, st);
                    AddButtons(item, st, btnRemoveUser_Click, btnEditRole_Click, null);

                    exp.Content = st;
                });
            });
        }
        /// <summary>
        /// Add an author to template.
        /// </summary>
        /// <param name="item">Author Id.</param>
        /// <param name="exp">The element in which an item is supposed to appear.</param>
        private void AddAuthorItem(int item, Expander exp)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddExpanderData("Books", item, st, EngServRef.ServerData.Author, EngServRef.ServerData.Book);
                    AddButtons(item, st, btnRemoveAuthor_Click, btnEditAuthor_Click, null);
                    exp.Content = st;
                });
            });
        }
        /// <summary>
        /// Add a categoty to template.
        /// </summary>
        /// <param name="item">Id of category.</param>
        /// <param name="exp">The element in which an item is supposed to appear.</param>
        private void AddBCategoryItem(int item, Expander exp)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddExpanderData("Books", item, st, EngServRef.ServerData.BookCategory, EngServRef.ServerData.Book);
                    AddButtons(item, st, btnRemoveBCategory_Click, btnEditBCategory_Click, null);
                    exp.Content = st;
                });
            });
        }

        private void expUser_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddUserItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander));
        }
        private void expAuthor_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddAuthorItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander));
        }
        private void expBookCategory_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddBCategoryItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander));
        }
    }
}