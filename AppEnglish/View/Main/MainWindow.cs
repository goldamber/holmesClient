using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    //Renders views (user).
    public partial class MainWindow : MetroWindow
    {
        //Types of data to be presented.
        enum DataType { Video, Book, Word, User }
        
        #region Render a template for list.
        /// <summary>
        /// Render a list-view (template).
        /// </summary>
        /// <param name="lst">List of items to be presented (videos, books, ...)</param>
        /// <param name="data">Type of items to be presented</param>
        void LoadList(IEnumerable<int> lst, DataType data)
        {
            Dispatcher.Invoke(() => {
                grSearch.Visibility = Visibility.Visible;

                grSearch.Children.Remove(FindName("btnAdd") as Button);
                cmbFilter.Items.Clear();
                cmbSort.Items.Clear();

                Button btnGrid = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Add.png")), Height = 20 }, Width = 50, Height = 50, Name = "btnAdd", Background = Brushes.LightGreen, ToolTip = "Add " + data.ToString(), HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(20) };
                switch (data)
                {
                    case DataType.Video:
                        btnGrid.Click += btnAddVideo;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Mark", Foreground = Brushes.Black });

                        cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbSort.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                        cmbSort.Items.Add(new ComboBoxItem { Content = "Date", Foreground = Brushes.Black });

                        btnSearch.Tag = "Video";
                        btnSort.Tag = "Video";
                        break;
                    case DataType.Book:
                        btnGrid.Click += btnAddBook;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Author", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Mark", Foreground = Brushes.Black });

                        cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbSort.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                        cmbSort.Items.Add(new ComboBoxItem { Content = "Date", Foreground = Brushes.Black });

                        btnSearch.Tag = "Book";
                        btnSort.Tag = "Book";
                        break;
                    case DataType.Word:
                        btnGrid.Click += btnAddWord;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Translation", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Definition", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Group", Foreground = Brushes.Black });

                        cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });

                        btnSearch.Tag = "Word";
                        btnSort.Tag = "Word";
                        break;
                    case DataType.User:
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Login", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Role", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Level", Foreground = Brushes.Black });

                        cmbSort.Items.Add(new ComboBoxItem { Content = "Login", IsSelected = true, Foreground = Brushes.Black });
                        cmbSort.Items.Add(new ComboBoxItem { Content = "Role", Foreground = Brushes.Black });
                        cmbSort.Items.Add(new ComboBoxItem { Content = "Level", Foreground = Brushes.Black });

                        btnSearch.Tag = "User";
                        btnSort.Tag = "User";
                        break;
                }
                if (data != DataType.User)
                    stActions.Children.Add(btnGrid);

                if (lst != null)
                {
                    foreach (int item in lst)
                    {
                        switch (data)
                        {
                            case DataType.Video:
                                AddVideoItem(item);
                                break;
                            case DataType.Book:
                                AddBookItem(item);
                                break;
                            case DataType.Word:
                                AddWordItem(item, stActions);
                                break;
                            case DataType.User:
                                AddUserItem(item, stActions);
                                break;
                        }
                    }
                }

                foreach (var item in stActions.Children)
                {
                    if (item is ProgressBar)
                    {
                        stActions.Children.Remove(item as UIElement);
                        break;
                    }
                }

                Button ret = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Width = 50, Height = 50, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(20, 0, 20, 0) };
                ret.Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/ArrowBack.png")), Height = 35 };
                ret.Click += ButtonBack_Click;
                stActions.Children.Add(ret);
            });            
        }
        /// <summary>
        /// Add a video item to the template.
        /// </summary>
        /// <param name="item">Id of video.</param>
        private void AddVideoItem(int item)
        {
            Expander tmp = new Expander { Header = _proxy.GetItemProperty(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Name) };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            AddImage(item, "WolfV.png", "VideoImages", st, EngServRef.ServerData.Video);

            AddStaticContent(item, st, EngServRef.ServerData.Video, EngServRef.PropertyData.Description);
            AddStaticContent(item, st, EngServRef.ServerData.Video, EngServRef.PropertyData.Created);
            AddMarkingStars(item, _proxy.GetUserId(lUserName.Content.ToString()), st, EngServRef.ServerData.Video);
            AddHoverableData(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Year, st);

            AddExpanderData("Categories", item, st, EngServRef.ServerData.Video, EngServRef.ServerData.VideoCategory);
            int id = _proxy.GetUserId(lUserName.Content.ToString()) ?? 0;
            if (_proxy.GetUserItemWordsAsync(id, item, EngServRef.ServerData.Video).Result != null && _proxy.GetUserItemWordsAsync(id, item, EngServRef.ServerData.Video).Result.Length > 0)
            {
                Expander words = new Expander { Header = "Words", Background = Brushes.Azure };
                StackPanel stack = new StackPanel();
                foreach (int val in _proxy.GetUserItemWordsAsync(id, item, EngServRef.ServerData.Video).Result)
                {
                    AddWordItem(val, stack);
                }
                Button print = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right, Background = Brushes.Blue, Foreground = Brushes.White, Tag = item, Content = "Print" };
                print.Click += btnPrintWords_Click;
                stack.Children.Add(print);

                words.Content = stack;
                st.Children.Add(words);
            }

            AddButtons(item, st, btnRemoveVideo_Click, btnEditVideo_Click, btnViewVideo_Click);

            tmp.Content = st;
            stActions.Children.Add(tmp);
        }
        /// <summary>
        /// Add a book item to the template.
        /// </summary>
        /// <param name="item">Id of book.</param>
        private void AddBookItem(int item)
        {
            Expander tmp = new Expander { Header = _proxy.GetItemProperty(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Name) };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            AddImage(item, "WolfB.png", "BookImages", st, EngServRef.ServerData.Book);

            AddStaticContent(item, st, EngServRef.ServerData.Book, EngServRef.PropertyData.Description);
            AddStaticContent(item, st, EngServRef.ServerData.Book, EngServRef.PropertyData.Created);
            AddMarkingStars(item, _proxy.GetUserId(lUserName.Content.ToString()), st, EngServRef.ServerData.Book);
            AddHoverableData(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Year, st);

            AddExpanderData("Categories", item, st, EngServRef.ServerData.Book, EngServRef.ServerData.BookCategory);
            AddExpanderData("Authors", item, st, EngServRef.ServerData.Book, EngServRef.ServerData.Author);

            int id = _proxy.GetUserId(lUserName.Content.ToString())?? 0;
            if (_proxy.GetUserItemWordsAsync(id, item, EngServRef.ServerData.Book).Result != null && _proxy.GetUserItemWordsAsync(id, item, EngServRef.ServerData.Book).Result.Length > 0)
            {
                Expander words = new Expander { Header = "Words", Background = Brushes.Azure };
                StackPanel stack = new StackPanel();
                foreach (int val in _proxy.GetUserItemWordsAsync(id, item, EngServRef.ServerData.Book).Result)
                {
                    AddWordItem(val, stack);
                }
                Button print = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right, Background = Brushes.Blue, Foreground = Brushes.White, Tag = item, Content = "Print" };
                print.Click += btnPrintWords_Click;
                stack.Children.Add(print);

                words.Content = stack;
                st.Children.Add(words);
            }
            
            AddButtons(item, st, btnRemoveBook_Click, btnEditBook_Click, btnViewBook_Click);

            tmp.Content = st;
            stActions.Children.Add(tmp);
        }
        /// <summary>
        /// Add a word to template.
        /// </summary>
        /// <param name="item">Id of a word.</param>
        /// <param name="parent">The element in which a word is supposed to appear.</param>
        private void AddWordItem(int item, Panel parent)
        {
            Expander tmp = new Expander { Header = _proxy.GetItemProperty(item, EngServRef.ServerData.Word, EngServRef.PropertyData.Name) };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            AddImage(item, null, "WordImages", st, EngServRef.ServerData.Word);

            AddExpanderData("Categories", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.WordCategory);
            AddExpanderData("Translations", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Translation);
            AddExpanderData("Groups", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Group);
            AddExpanderData("Definitions", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Definition);

            RoutedEventHandler delete;
            RoutedEventHandler edit = null;
            if (parent == stActions)
            {
                delete = btnRemoveWord_Click;
                edit = btnEditWord_Click;
            }
            else
                delete = btnRemoveFromUser_Click;
            AddButtons(item, st, delete, edit, null);

            tmp.Content = st;
            parent.Children.Add(tmp);
        }

        /// <summary>
        /// Insters extra data to an item (categories, words, ...).
        /// </summary>
        /// <param name="header">The title of expander.</param>
        /// <param name="item">Id of item to be decorated.</param>
        /// <param name="st">A panel where the data are supposed to be added.</param>
        /// <param name="data">A type of item.</param>
        /// <param name="res">A type of the inserted data.</param>
        void AddExpanderData(string header, int item, Panel st, EngServRef.ServerData data, EngServRef.ServerData res)
        {
            if (_proxy.GetItemDataAsync(item, data, res).Result == null || _proxy.GetItemDataAsync(item, data, res).Result.Length == 0)
                return;

            Expander hor = new Expander { Header = header, Background = Brushes.Azure };
            StackPanel ver = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            int count = 1;
            foreach (int val in _proxy.GetItemDataAsync(item, data, res).Result)
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Background = Brushes.Azure };
                panel.MouseEnter += ExpanderItem_MouseEnter;
                panel.MouseLeave += ExpanderItem_MouseLeave;

                panel.Children.Add(new Border { CornerRadius = new CornerRadius(22, 22, 20, 20), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(2), Child = new Label { Content = count, Padding = new Thickness(2), FontSize = 9, FontWeight = FontWeights.Bold }, Padding = new Thickness(7, 5, 7, 5), Margin = new Thickness(5) });
                TextBlock label = new TextBlock { Padding = new Thickness(5), Foreground = Brushes.DarkBlue, TextDecorations = TextDecorations.Underline, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Tag = header, Text = _proxy.GetItemPropertyAsync(val, res, EngServRef.PropertyData.Name).Result, FontSize = 12, FontWeight = FontWeights.Normal };
                label.MouseDown += ItemData_MouseDown;
                label.MouseEnter += ItemData_MouseEnter;
                label.MouseLeave += ItemData_MouseLeave;
                panel.Children.Add(label);
                ver.Children.Add(panel);
                count++;
            }
            hor.Content = ver;
            st.Children.Add(hor);
        }
        /// <summary>
        /// Describes static one-line property.
        /// </summary>
        /// <param name="content">The data to insert.</param>
        /// <param name="item">Id of the item to which this property belongs.</param>
        /// <param name="st">The panel where to insert.</param>
        /// <param name="dataType">The type of an item.</param>
        /// <param name="property">The type of property.</param>
        void AddStaticContent(int item, Panel st, EngServRef.ServerData dataType, EngServRef.PropertyData property)
        {
            if (_proxy.GetItemPropertyAsync(item, dataType, property).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = $"{property.ToString()}:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new Label { Content = _proxy.GetItemPropertyAsync(item, dataType, property).Result });
                st.Children.Add(hor);
            }
        }
        /// <summary>
        /// Inserts the data for sorting elements.
        /// </summary>
        /// <param name="item">Id of item to be decorated.</param>
        /// <param name="dataType">A type of item.</param>
        /// <param name="property">A type of the inserted data.</param>
        /// <param name="st">A panel where the data are supposed to be added.</param>
        void AddHoverableData(int item, EngServRef.ServerData dataType, EngServRef.PropertyData property, Panel st)
        {
            if (_proxy.GetItemPropertyAsync(item, dataType, property).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = $"{property.ToString()}:", FontSize = 14, FontWeight = FontWeights.Bold });

                TextBlock label = new TextBlock { Padding = new Thickness(5), Foreground = Brushes.DarkBlue, TextDecorations = TextDecorations.Underline, Tag = property.ToString(), Text = _proxy.GetItemPropertyAsync(item, dataType, property).Result };
                label.MouseDown += ItemData_MouseDown;
                label.MouseEnter += ItemData_MouseEnter;
                label.MouseLeave += ItemData_MouseLeave;
                hor.Children.Add(label);
                st.Children.Add(hor);
            }
        }
        /// <summary>
        /// Inserts default actions.
        /// </summary>
        /// <param name="item">Id of item.</param>
        /// <param name="st">A panel where the buttons are supposed to be inserted.</param>
        /// <param name="delete">'Delete' action.</param>
        /// <param name="edit">'Edit' action.</param>
        /// <param name="view">'View' action.</param>
        void AddButtons(int item, Panel st, RoutedEventHandler delete, RoutedEventHandler edit, RoutedEventHandler view)
        {
            StackPanel stButtons = new StackPanel { Orientation = Orientation.Horizontal };
            Button btn;
            if (delete != null && lRole.Content.ToString().Equals("admin"))
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Tag = item, ToolTip = "Delete" };
                btn.Click += delete;
                stButtons.Children.Add(btn);
            }
            if (edit != null && lRole.Content.ToString().Equals("admin"))
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Tag = item, ToolTip = "Edit" };
                btn.Click += edit;
                stButtons.Children.Add(btn);
            }
            if (view != null)
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/View.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Tag = item, ToolTip = "View" };
                btn.Click += btnViewBook_Click;
                stButtons.Children.Add(btn);
            }
            st.Children.Add(stButtons);
        }
        /// <summary>
        /// Downloads an image from server and presents it.
        /// </summary>
        /// <param name="id">Images id.</param>
        /// <param name="defaultPath">Default path.</param>
        /// <param name="tempPath">The location of temporary files.</param>
        /// <param name="parent">The panel where an image is supposed to be added.</param>
        /// <param name="type">Data type.</param>
        void AddImage(int id, string defaultPath, string tempPath, Panel parent, EngServRef.ServerData type)
        {
            string img = _proxy.GetItemPropertyAsync(id, type, EngServRef.PropertyData.Imgpath).Result ?? defaultPath;
            if (img == null)
                return;

            string source = "";
            if (img == defaultPath)
                source = $"pack://application:,,,/Images/{defaultPath}";
            else if (!File.Exists($@"Temp\{tempPath}\{img}"))
            {
                Task.Run(new Action(() => {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (!Directory.Exists($@"Temp\{tempPath}"))
                            Directory.CreateDirectory($@"Temp\{tempPath}");
                        
                        EngServRef.FilesType filesType = EngServRef.FilesType.Avatar;
                        switch (type)
                        {
                            case EngServRef.ServerData.Video:
                                filesType = EngServRef.FilesType.VideoImage;
                                break;
                            case EngServRef.ServerData.Book:
                                filesType = EngServRef.FilesType.BookImage;
                                break;
                            case EngServRef.ServerData.User:
                                filesType = EngServRef.FilesType.Avatar;
                                break;
                            case EngServRef.ServerData.Word:
                                filesType = EngServRef.FilesType.WordImage;
                                break;
                        }
                        if (_proxy.Download(img, filesType) != null)
                        {
                            File.WriteAllBytes($@"Temp\{tempPath}\{img}", _proxy.Download(img, filesType));
                            source = $@"pack://siteoforigin:,,,/Temp\{tempPath}\{img}";
                        }
                    }));
                }));
            }
            else
                source = $@"pack://siteoforigin:,,,/Temp\{tempPath}\{img}";
            parent.Children.Add(new Image { Source = new BitmapImage(new Uri(source)), Height = 110 });
        }
        /// <summary>
        /// Inserts rating.
        /// </summary>
        /// <param name="id">Id of item.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="parent">The panel, where items are supposed to be added.</param>
        /// <param name="type">Type of item.</param>
        void AddMarkingStars(int id, int? userId, Panel parent, EngServRef.ServerData type)
        {
            int? stars = null;
            switch (type)
            {
                case EngServRef.ServerData.Video:
                    stars = _proxy.GetMark(id, Convert.ToInt32(userId), EngServRef.ServerData.Video);
                    break;
                case EngServRef.ServerData.Book:
                    stars = _proxy.GetMark(id, Convert.ToInt32(userId), EngServRef.ServerData.Book);
                    break;
            }

            int mark = stars == null ? 0 : Convert.ToInt32(stars);
            StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal, ToolTip = mark == 0? "N/G": mark.ToString() };
            stack.Children.Add(new Label { Content = $"Rating:", FontSize = 14, FontWeight = FontWeights.Bold });
            for (int i = 0; i < 5; i++)
            {
                stack.Children.Add(new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Rating.png")), Height = 40, Margin = new Thickness(2), Opacity = i < mark? 1: 0.2 });
            }
            Button edit = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 7 }, Width = 32, Height = 30, VerticalAlignment = VerticalAlignment.Top, Background = Brushes.Yellow, Tag = $"{type.ToString()}:{id}:{userId}:{mark}", ToolTip = "Edit" };
            edit.Click += btnEditRating_Click;
            stack.Children.Add(edit);
            parent.Children.Add(stack);
        }

        //Customize expanders label (hover).
        private void ItemData_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = TextDecorations.Underline;
            (sender as TextBlock).Foreground = Brushes.DarkBlue;
        }
        //Customize expanders label (hover).
        private void ItemData_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = null;
            (sender as TextBlock).Foreground = Brushes.DeepSkyBlue;
        }
        
        //Customize expander (hover).
        private void ExpanderItem_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.Azure;
        }
        //Customize expander (hover).
        private void ExpanderItem_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.LightBlue;
        }
        #endregion
    }
}