using MahApps.Metro.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        /// <param name="type">Categories type.</param>
        /// <param name="innerData">Type of related elements.</param>
        /// <param name="delete">'Delete' action.</param>
        /// <param name="edit">'Edit' action.</param>
        private void AddCategoryItem(int item, Expander exp, EngServRef.ServerData type, EngServRef.ServerData innerData, RoutedEventHandler delete, RoutedEventHandler edit)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddExpanderData($"{innerData}s", item, st, type, innerData);
                    AddButtons(item, st, delete, edit, null);
                    if (type == EngServRef.ServerData.Group && _proxy.GetItemData(item, EngServRef.ServerData.Group, EngServRef.ServerData.Word)?.Length > 0)
                    {
                        Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/PlayGame.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.WhiteSmoke, Tag = item, ToolTip = "Play" };
                        btn.Click += PlayWordsGroupGame_Click;
                        st.Children.Add(btn);
                    }
                    exp.Content = st;
                });
            });
        }
        /// <summary>
        /// Add a words categoty to template.
        /// </summary>
        /// <param name="item">Id of category.</param>
        /// <param name="exp">The element in which an item is supposed to appear.</param>
        private void AddWCategoryItem(int item, Expander exp)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddStaticContent(item, st, EngServRef.ServerData.WordCategory, EngServRef.PropertyData.Abbreviation);
                    AddExpanderData("Words", item, st, EngServRef.ServerData.WordCategory, EngServRef.ServerData.Word);
                    AddButtons(item, st, btnRemoveWCategory_Click, btnEditWCategory_Click, null);
                    if (_proxy.GetItemData(item, EngServRef.ServerData.WordCategory, EngServRef.ServerData.Word)?.Length > 0)
                    {
                        Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/PlayGame.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.WhiteSmoke, Tag = item, ToolTip = "Play" };
                        btn.Click += PlayWordsCatGame_Click;
                        st.Children.Add(btn);
                    }
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
                AddCategoryItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), EngServRef.ServerData.BookCategory, EngServRef.ServerData.Book, btnRemoveBCategory_Click, btnEditBCategory_Click);
        }
        private void expVideoCategory_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddCategoryItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), EngServRef.ServerData.VideoCategory, EngServRef.ServerData.Video, btnRemoveVCategory_Click, btnEditVCategory_Click);
        }
        private void expWordCategory_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddWCategoryItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander));
        }
        private void expWordGroup_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddCategoryItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), EngServRef.ServerData.Group, EngServRef.ServerData.Word, btnRemoveWGroup_Click, btnEditWGroup_Click);
        }
    }
}