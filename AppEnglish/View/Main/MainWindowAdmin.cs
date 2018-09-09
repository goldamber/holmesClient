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
            AddImage(item, "Wolf.png", "Avatars", st, EngServRef.ServerData.User);

            AddStaticContent(item, st, EngServRef.ServerData.User, EngServRef.PropertyData.Name);
            AddHoverableData(item, EngServRef.ServerData.User, EngServRef.PropertyData.RolesName, st);
            AddHoverableData(item, EngServRef.ServerData.User, EngServRef.PropertyData.Level, st);
            AddButtons(item, st, btnRemoveUser_Click, btnEditRole_Click, null);

            tmp.Content = st;
            parent.Children.Add(tmp);
        }
    }
}