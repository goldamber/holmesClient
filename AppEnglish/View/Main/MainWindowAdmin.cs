using MahApps.Metro.Controls;
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
        /// <param name="parent">The element in which an item is supposed to appear.</param>
        private void AddUserItem(int item, Panel parent)
        {
            Expander tmp = new Expander { Header = _proxy.GetItemProperty(item, EngServRef.ServerData.User, EngServRef.PropertyData.Name) };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            AddImage(item, "Wolf.png", "Avatars", st, EngServRef.ServerData.User, false);

            AddStaticContent(item, st, EngServRef.ServerData.User, EngServRef.PropertyData.Name);
            AddHoverableData(item, EngServRef.ServerData.User, EngServRef.PropertyData.RolesName, st);
            AddHoverableData(item, EngServRef.ServerData.User, EngServRef.PropertyData.Level, st);
            AddButtons(item, st, btnRemoveUser_Click, btnEditRole_Click, null);

            tmp.Content = st;
            parent.Children.Add(tmp);
        }
        /// <summary>
        /// Add an author to template.
        /// </summary>
        /// <param name="item">Author Id.</param>
        /// <param name="parent">The element in which an item is supposed to appear.</param>
        private void AddAuthorItem(int item, Panel parent)
        {
            Expander tmp = new Expander { Header = _proxy.GetItemProperty(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Name) + " " + _proxy.GetItemProperty(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Surname) };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

            AddExpanderData("Books", item, st, EngServRef.ServerData.Author, EngServRef.ServerData.Book);
            AddButtons(item, st, btnRemoveAuthor_Click, btnEditAuthor_Click, null);

            tmp.Content = st;
            parent.Children.Add(tmp);
        }
        /// <summary>
        /// Add a categoty to template.
        /// </summary>
        /// <param name="item">Id of category.</param>
        /// <param name="parent">The element in which an item is supposed to appear.</param>
        private void AddBCategoryItem(int item, Panel parent)
        {
            Expander tmp = new Expander { Header = _proxy.GetItemProperty(item, EngServRef.ServerData.BookCategory, EngServRef.PropertyData.Name) };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            AddExpanderData("Books", item, st, EngServRef.ServerData.BookCategory, EngServRef.ServerData.Book);
            AddButtons(item, st, btnRemoveBCategory_Click, btnEditBCategory_Click, null);

            tmp.Content = st;
            parent.Children.Add(tmp);
        }
    }
}