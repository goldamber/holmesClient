using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    //Renders views.
    public partial class MainWindow : MetroWindow
    {
        System.Windows.Forms.WebBrowser web = new System.Windows.Forms.WebBrowser();

        #region User actions (choose an option).
        /// <summary>
        /// Show a list of all books to the user.
        /// </summary>
        private async void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Book);
            await Task.Run(() => LoadList(lst, DataType.Book));
        }
        /// <summary>
        /// Show a list of all videos to the user.
        /// </summary>
        private async void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Video);
            await Task.Run(() => LoadList(lst, DataType.Video));
        }
        /// <summary>
        /// Show a list of all words to the user.
        /// </summary>
        private async void btnWords_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Word);
            await Task.Run(() => LoadList(lst, DataType.Word));
        }
        #endregion

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

                Button btnGrid = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Add.png")), Height = 20 }, Width = 50, Height = 50, Name = "btnAdd", Background = Brushes.LightGreen, ToolTip = "Add " + data.ToString(), Margin = new Thickness(20) };
                switch (data)
                {
                    case DataType.Video:
                        btnGrid.Click += btnAddVideo;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Mark", Foreground = Brushes.Black });
                        btnSearch.Tag = "Video";
                        break;
                    case DataType.Book:
                        btnGrid.Click += btnAddBook;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Author", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Mark", Foreground = Brushes.Black });
                        btnSearch.Tag = "Book";
                        break;
                    case DataType.Word:
                        btnGrid.Click += btnAddWord;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Translation", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Definition", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Group", Foreground = Brushes.Black });
                        btnSearch.Tag = "Word";
                        break;
                    case DataType.Game:
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        btnSearch.Tag = "Game";
                        break;
                }
                grSearch.Children.Add(btnGrid);

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
                            case DataType.Game:
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
            string img = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Imgpath).Result;
            st.Children.Add(new Image { Source = new BitmapImage(new Uri(img != null && img != "WolfV.png" && File.Exists(img) ? img : $"pack://application:,,,/Images/{img}")), MaxHeight = 100, HorizontalAlignment = HorizontalAlignment.Center });

            if (_proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Description).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = "Description:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new Label { Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Description).Result });
                st.Children.Add(hor);
            }
            if (_proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Created).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = "Created:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new Label { Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Created).Result });
                st.Children.Add(hor);
            }
            if (_proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Year).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = "Year:", FontSize = 14, FontWeight = FontWeights.Bold });

                TextBlock label = new TextBlock { Padding = new Thickness(5), Foreground = Brushes.DarkBlue, TextDecorations = TextDecorations.Underline, Tag = "Year", Text = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Video, EngServRef.PropertyData.Year).Result };
                label.MouseDown += ItemData_MouseDown;
                label.MouseEnter += ItemData_MouseEnter;
                label.MouseLeave += ItemData_MouseLeave;
                hor.Children.Add(label);
                st.Children.Add(hor);
            }
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Video, EngServRef.ServerData.VideoCategory).Result != null)
                AddExpanderData("Categories", item, st, EngServRef.ServerData.Video, EngServRef.ServerData.VideoCategory);
            if (_proxy.GetUserItemWordsAsync(Convert.ToInt32(lUserName.Tag), item, EngServRef.ServerData.Book).Result != null && _proxy.GetUserItemWordsAsync(Convert.ToInt32(lUserName.Tag), item, EngServRef.ServerData.Book).Result.Length > 0)
            {
                Expander words = new Expander { Header = "Words", Background = Brushes.Azure };
                StackPanel stack = new StackPanel();
                foreach (int val in _proxy.GetUserItemWordsAsync(Convert.ToInt32(lUserName.Tag.ToString()), item, EngServRef.ServerData.Video).Result)
                {
                    AddWordItem(val, stack);
                }
                Button print = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right, Background = Brushes.Blue, Foreground = Brushes.White, Tag = item, Content = "Print" };
                print.Click += btnPrintWords_Click;
                stack.Children.Add(print);

                words.Content = stack;
                st.Children.Add(words);
            }

            #region Buttons.
            StackPanel stButtons = new StackPanel { Orientation = Orientation.Horizontal };
            Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Tag = item, ToolTip = "Delete" };
            btn.Click += btnRemoveVideo_Click;
            stButtons.Children.Add(btn);

            btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Tag = item, ToolTip = "Edit" };
            btn.Click += btnEditVideo_Click;
            stButtons.Children.Add(btn);

            btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/View.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Tag = item, ToolTip = "View" };
            btn.Click += btnViewVideo_Click;
            stButtons.Children.Add(btn);
            st.Children.Add(stButtons);
            #endregion

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
            string img = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Imgpath).Result;
            st.Children.Add(new Image { Source = new BitmapImage(new Uri(img != null && img != "WolfB.png" && File.Exists(img) ? img : $"pack://application:,,,/Images/{img}")), MaxHeight = 100, HorizontalAlignment = HorizontalAlignment.Center });

            if (_proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Description).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = "Description:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new Label { Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Description).Result });
                st.Children.Add(hor);
            }
            if (_proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Created).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = "Created:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new Label { Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Created).Result });
                st.Children.Add(hor);
            }
            if (_proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Year).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = "Year:", FontSize = 14, FontWeight = FontWeights.Bold });

                TextBlock label = new TextBlock { Padding = new Thickness(5), Foreground = Brushes.DarkBlue, TextDecorations = TextDecorations.Underline, Tag = "Year", Text = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Book, EngServRef.PropertyData.Year).Result };
                label.MouseDown += ItemData_MouseDown;
                label.MouseEnter += ItemData_MouseEnter;
                label.MouseLeave += ItemData_MouseLeave;
                hor.Children.Add(label);
                st.Children.Add(hor);
            }
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Book, EngServRef.ServerData.BookCategory).Result != null)
                AddExpanderData("Categories", item, st, EngServRef.ServerData.Book, EngServRef.ServerData.BookCategory);
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Book, EngServRef.ServerData.Author).Result != null)
                AddExpanderData("Authors", item, st, EngServRef.ServerData.Book, EngServRef.ServerData.Author);
            if (_proxy.GetUserItemWordsAsync(Convert.ToInt32(lUserName.Tag), item, EngServRef.ServerData.Book).Result != null && _proxy.GetUserItemWordsAsync(Convert.ToInt32(lUserName.Tag), item, EngServRef.ServerData.Book).Result.Length > 0)
            {
                Expander words = new Expander { Header = "Words", Background = Brushes.Azure };
                StackPanel stack = new StackPanel();
                foreach (int val in _proxy.GetUserItemWordsAsync(Convert.ToInt32(lUserName.Tag), item, EngServRef.ServerData.Book).Result)
                {
                    AddWordItem(val, stack);
                }
                Button print = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right, Background = Brushes.Blue, Foreground = Brushes.White, Tag = item, Content = "Print" };
                print.Click += btnPrintWords_Click;
                stack.Children.Add(print);

                words.Content = stack;
                st.Children.Add(words);
            }

            #region Buttons.
            StackPanel stButtons = new StackPanel { Orientation = Orientation.Horizontal };
            Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Tag = item, ToolTip = "Delete" };
            btn.Click += btnRemoveBook_Click;
            stButtons.Children.Add(btn);

            btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Tag = item, ToolTip = "Edit" };
            btn.Click += btnEditBook_Click;
            stButtons.Children.Add(btn);

            btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/View.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Tag = item, ToolTip = "View" };
            btn.Click += btnViewBook_Click;
            stButtons.Children.Add(btn);
            st.Children.Add(stButtons);
            #endregion

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
            string img = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Word, EngServRef.PropertyData.Imgpath).Result;
            if (img != null && File.Exists(img))
                st.Children.Add(new Image { Source = new BitmapImage(new Uri(img)) });
            
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.WordCategory).Result != null)
                AddExpanderData("Categories", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.WordCategory);
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.Translation).Result != null)
                AddExpanderData("Translations", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Translation);
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.Group).Result != null)
                AddExpanderData("Groups", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Group);
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.Definition).Result != null)
                AddExpanderData("Definitions", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Definition);

            #region Buttons.
            StackPanel stButtons = new StackPanel { Orientation = Orientation.Horizontal };
            Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Tag = item, ToolTip = "Delete" };
            if (parent == stActions)
                btn.Click += btnRemoveWord_Click;
            else
                btn.Click += btnRemoveFromUser_Click;
            stButtons.Children.Add(btn);

            if (parent == stActions)
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Tag = item, ToolTip = "Edit" };
                btn.Click += btnEditWord_Click;
                stButtons.Children.Add(btn);
            }
            st.Children.Add(stButtons);
            #endregion

            tmp.Content = st;
            parent.Children.Add(tmp);
        }
        /// <summary>
        /// Insters extra data to an item (categories, words, ...).
        /// </summary>
        /// <param name="header">The title of expander.</param>
        /// <param name="item">Id of item to be decorated.</param>
        /// <param name="st">A panel where the data are supposed to add.</param>
        /// <param name="data">A type of item.</param>
        /// <param name="res">A type of the inserted data.</param>
        void AddExpanderData(string header, int item, Panel st, EngServRef.ServerData data, EngServRef.ServerData res)
        {
            if (_proxy.GetItemDataAsync(item, data, res).Result.Length == 0)
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

        //Customize expanders label (hover).
        private void ItemData_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = TextDecorations.Underline;
            (sender as TextBlock).Foreground = Brushes.DarkBlue;
        }
        //Customize expanders label (hover).
        private void ItemData_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = null;
            (sender as TextBlock).Foreground = Brushes.DeepSkyBlue;
        }
        
        //Customize expander (hover).
        private void ExpanderItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.Azure;
        }
        //Customize expander (hover).
        private void ExpanderItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.LightBlue;
        }
        #endregion
    }
}