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
                Button btnGrid = new Button { Name = "btnAdd", Background = Brushes.LightGreen, Content = "Add " + data.ToString(), Foreground = Brushes.White, Margin = new Thickness(20) };
                switch (data)
                {
                    case DataType.Video:
                        btnGrid.Click += btnAddVideo;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        btnSearch.Tag = "Video";
                        break;
                    case DataType.Book:
                        btnGrid.Click += btnAddBook;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Author", Foreground = Brushes.Black });
                        btnSearch.Tag = "Book";
                        break;
                    case DataType.Word:
                        btnGrid.Click += btnAddWord;
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Translation", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Definition", Foreground = Brushes.Black });
                        cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
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
            Expander tmp = new Expander { Header = item };
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

            Button btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Foreground = Brushes.WhiteSmoke, Tag = item, Content = "Remove" };
            btn.Click += btnRemoveVideo_Click;
            st.Children.Add(btn);

            btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Foreground = Brushes.Black, Tag = item, Content = "Edit" };
            btn.Click += btnEditVideo_Click;
            st.Children.Add(btn);

            btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Tag = item, Content = "View" };
            btn.Click += btnViewVideo_Click;
            st.Children.Add(btn);

            tmp.Content = st;
            stActions.Children.Add(tmp);
        }
        /// <summary>
        /// Add a book item to the template.
        /// </summary>
        /// <param name="item">Id of book.</param>
        private void AddBookItem(int item)
        {
            Expander tmp = new Expander { Header = item };
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

            Button btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Foreground = Brushes.WhiteSmoke, Tag = item, Content = "Remove" };
            btn.Click += btnRemoveBook_Click;
            st.Children.Add(btn);

            btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Foreground = Brushes.Black, Tag = item, Content = "Edit" };
            btn.Click += btnEditBook_Click;
            st.Children.Add(btn);

            btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Tag = item, Content = "View" };
            btn.Click += btnViewBook_Click;
            st.Children.Add(btn);

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
            Expander tmp = new Expander { Header = item };
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            string img = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Word, EngServRef.PropertyData.Imgpath).Result;
            if (img != null && File.Exists(img))
                st.Children.Add(new Image { Source = new BitmapImage(new Uri(img)) });
            
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.WordCategory).Result != null)
                AddExpanderData("Categories", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.WordCategory);
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.Translation).Result != null)
                AddExpanderData("Translations", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Translation);
            if (_proxy.GetItemDataAsync(item, EngServRef.ServerData.Word, EngServRef.ServerData.Definition).Result != null)
                AddExpanderData("Definitions", item, st, EngServRef.ServerData.Word, EngServRef.ServerData.Definition);

            Button btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Foreground = Brushes.WhiteSmoke, Tag = item, Content = "Remove" };
            if (parent == stActions)
                btn.Click += btnRemoveWord_Click;
            else
                btn.Click += btnRemoveFromUser_Click;
            st.Children.Add(btn);

            if (parent == stActions)
            {
                btn = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Foreground = Brushes.Black, Tag = item, Content = "Edit" };
                btn.Click += btnEditWord_Click;
                st.Children.Add(btn);
            }

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

                panel.Children.Add(new Border { CornerRadius = new CornerRadius(50), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(2), Child = new Label { Content = count, Padding = new Thickness(2), FontSize = 10, FontWeight = FontWeights.Bold }, Padding = new Thickness(7, 5, 7, 5), Margin = new Thickness(5) });
                panel.Children.Add(new Label { Padding = new Thickness(5), VerticalContentAlignment = VerticalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left, Tag = val, Content = _proxy.GetItemPropertyAsync(val, res, EngServRef.PropertyData.Name).Result, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.Normal });
                ver.Children.Add(panel);
                count++;
            }
            hor.Content = ver;
            st.Children.Add(hor);
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