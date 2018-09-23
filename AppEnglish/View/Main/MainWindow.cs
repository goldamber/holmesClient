using AppEnglish.EngServRef;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    //Renders views (user).
    public partial class MainWindow : MetroWindow
    {
        //Types of data to be presented.
        enum DataType { Video, Book, Word, User, Author, BookCategory, WordCategory, VideoCategory, Group, Grammar }
        
        #region Render a template for list.
        /// <summary>
        /// Render a list-view (template).
        /// </summary>
        /// <param name="lst">List of items to be presented (videos, books, ...)</param>
        /// <param name="data">Type of items to be presented</param>
        void LoadList(IEnumerable<int> lst, DataType data, bool edit)
        {
            Thread thd = new Thread(new ThreadStart(() => {
                Dispatcher.InvokeAsync(() => {
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

                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Date", Foreground = Brushes.Black });
                            break;
                        case DataType.Book:
                            btnGrid.Click += btnAddBook;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Description", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Author", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });

                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Year", Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Date", Foreground = Brushes.Black });
                            break;
                        case DataType.Word:
                            btnGrid.Click += btnAddWord;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Synonyms", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Homophones", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Translation", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Definition", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Category", Foreground = Brushes.Black });

                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            break;
                        case DataType.User:
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Login", IsSelected = true, Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Role", Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Level", Foreground = Brushes.Black });

                            cmbSort.Items.Add(new ComboBoxItem { Content = "Login", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Role", Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Level", Foreground = Brushes.Black });
                            break;
                        case DataType.BookCategory:
                            btnGrid.Click += btnAddBCategory_Click;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            btnGrid.ToolTip = "Add books category";
                            break;
                        case DataType.VideoCategory:
                            btnGrid.Click += btnAddVCategory_Click;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            btnGrid.ToolTip = "Add video category";
                            break;
                        case DataType.Author:
                            btnGrid.Click += btnAddAuthor_Click;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Surname", Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Surname", Foreground = Brushes.Black });
                            break;
                        case DataType.WordCategory:
                            btnGrid.Click += btnAddWCategory_Click;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Abbreviation", Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Abbreviation", Foreground = Brushes.Black });
                            btnGrid.ToolTip = "Add words category";
                            break;
                        case DataType.Group:
                            btnGrid.Click += btnAddWGroup_Click;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            btnGrid.ToolTip = "Add words group";
                            break;
                        case DataType.Grammar:
                            btnGrid.Click += btnAddGrammar_Click;
                            cmbFilter.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            cmbSort.Items.Add(new ComboBoxItem { Content = "Name", IsSelected = true, Foreground = Brushes.Black });
                            btnGrid.ToolTip = "Add rule";
                            break;
                    }
                    btnSort.Tag = btnSearch.Tag = data.ToString();
                    if (data != DataType.User)
                        stActions.Children.Add(btnGrid);
                    if (cmbFilter.Items.Count > FormData.FilterPosition)
                        cmbFilter.SelectedIndex = FormData.FilterPosition;
                    if (cmbSort.Items.Count > FormData.SortPosition)
                        cmbSort.SelectedIndex = FormData.SortPosition;
                    if (lst != null)
                    {
                        foreach (int item in lst)
                        {
                            switch (data)
                            {
                                case DataType.Video:
                                    Expander expVideo = new Expander { Header = _proxy.GetItemProperty(item, ServerData.Video, PropertyData.Name), Tag = item, IsEnabled = !FormData.EditVideos.Contains(item) };
                                    expVideo.Expanded += expVideo_Expanded;
                                    stActions.Children.Add(expVideo);
                                    break;
                                case DataType.Book:
                                    Expander expBook = new Expander { Header = _proxy.GetItemProperty(item, ServerData.Book, PropertyData.Name), Tag = item, IsEnabled = !FormData.EditBooks.Contains(item) };
                                    expBook.Expanded += expBook_Expanded;
                                    stActions.Children.Add(expBook);
                                    break;
                                case DataType.Word:
                                    Expander expWord = new Expander { Header = _proxy.GetItemProperty(item, ServerData.Word, PropertyData.Name), Tag = item, IsEnabled = !FormData.EditWords.Contains(item) };
                                    expWord.Expanded += expWord_Expanded;
                                    stActions.Children.Add(expWord);
                                    break;
                                case DataType.User:
                                    Expander expUser = new Expander { Header = _proxy.GetItemProperty(item, ServerData.User, PropertyData.Login), Tag = item, IsEnabled = !FormData.EditUsers.Contains(item) };
                                    expUser.Expanded += expUser_Expanded;
                                    stActions.Children.Add(expUser);
                                    break;
                                case DataType.Author:
                                    Expander expAuthor = new Expander { Header = _proxy.GetItemProperty(item, ServerData.Author, PropertyData.Name) + " " + _proxy.GetItemProperty(item, ServerData.Author, PropertyData.Surname), Tag = item };
                                    expAuthor.Expanded += expAuthor_Expanded;
                                    stActions.Children.Add(expAuthor);
                                    break;
                                case DataType.BookCategory:
                                    Expander expBC = new Expander { Header = _proxy.GetItemProperty(item, ServerData.BookCategory, PropertyData.Name), Tag = item };
                                    expBC.Expanded += expBookCategory_Expanded;
                                    stActions.Children.Add(expBC);
                                    break;
                                case DataType.VideoCategory:
                                    Expander expVC = new Expander { Header = _proxy.GetItemProperty(item, ServerData.VideoCategory, PropertyData.Name), Tag = item };
                                    expVC.Expanded += expVideoCategory_Expanded;
                                    stActions.Children.Add(expVC);
                                    break;
                                case DataType.WordCategory:
                                    Expander expWC = new Expander { Header = _proxy.GetItemProperty(item, ServerData.WordCategory, PropertyData.Name), Tag = item };
                                    expWC.Expanded += expWordCategory_Expanded;
                                    stActions.Children.Add(expWC);
                                    break;
                                case DataType.Group:
                                    Expander expWG = new Expander { Header = _proxy.GetItemProperty(item, ServerData.Group, PropertyData.Name), Tag = item };
                                    expWG.Expanded += expWordGroup_Expanded;
                                    stActions.Children.Add(expWG);
                                    break;
                                case DataType.Grammar:
                                    Expander expGrammar = new Expander { Header = _proxy.GetItemProperty(item, ServerData.Grammar, PropertyData.Name), Tag = item };
                                    expGrammar.Expanded += expGrammar_Expanded;
                                    stActions.Children.Add(expGrammar);
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
            }));
            thd.IsBackground = true;
            thd.Start();
        }
        
        private void expBook_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddBookItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), false);
        }
        private void expVideo_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddVideoItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), false);
        }
        private void expWord_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddWordItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), false);
        }
        private void expGrammar_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Content == null)
                AddGrammarItem(Convert.ToInt32((sender as Expander).Tag), (sender as Expander), false);
        }

        /// <summary>
        /// Add a video item to the template.
        /// </summary>
        /// <param name="item">Id of video.</param>
        /// <param name="exp">The expander where the data are supposed to be added.</param>
        /// <param name="edit">If this item is editable.</param>
        private void AddVideoItem(int item, Expander exp, bool edit)
        {
            Thread thd = new Thread(new ThreadStart(() =>
            {
                Dispatcher.InvokeAsync(new Action(() =>
                {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddImage(item, "WolfV.png", "VideoImages", st, ServerData.Video, edit);
                    AddStaticContent(item, st, ServerData.Video, PropertyData.Description);
                    AddStaticContent(item, st, ServerData.Video, PropertyData.Created);
                    AddMarkingStars(item, _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User), st, ServerData.Video);
                    AddHoverableData(item, ServerData.Video, PropertyData.Year, st);

                    AddExpanderData("Categories", item, st, ServerData.Video, ServerData.VideoCategory);
                    int id = _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User) ?? 0;
                    if (_proxy.GetUserItemWordsAsync(id, item, ServerData.Video).Result != null)
                    {
                        Expander words = new Expander { Header = "Videos words", Background = Brushes.Azure };
                        StackPanel stack = new StackPanel();
                        foreach (int val in _proxy.GetUserItemWordsAsync(id, item, ServerData.Video).Result)
                        {
                            Expander tmp = new Expander { Header = _proxy.GetItemProperty(val, ServerData.Word, PropertyData.Name), Tag = val, IsEnabled = !FormData.EditWords.Contains(val) };
                            tmp.Expanded += expWord_Expanded;
                            stack.Children.Add(tmp);
                        }
                        Button print = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right, Background = Brushes.Blue, Foreground = Brushes.White, Tag = $"Video:{item}", Content = "Print" };
                        print.Click += btnPrintWords_Click;
                        stack.Children.Add(print);

                        words.Content = stack;
                        st.Children.Add(words);
                    }

                    AddButtons(item, st, btnRemoveVideo_Click, btnEditVideo_Click, btnViewVideo_Click);
                    Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Sub.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.WhiteSmoke, Tag = item, ToolTip = "Add subtitles" };
                    btn.Click += BtnAddSubs_Click;
                    st.Children.Add(btn);
                    btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/PlayGame.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.WhiteSmoke, Tag = item, ToolTip = "Play" };
                    btn.Click += PlayLyricGame_Click;
                    st.Children.Add(btn);

                    exp.Content = st;
                }));
            }));
            thd.IsBackground = true;
            thd.Start();
        }
        /// <summary>
        /// Add a book item to the template.
        /// </summary>
        /// <param name="item">Id of book.</param>
        /// <param name="exp">The expander where the data are supposed to be added.</param>
        /// <param name="edit">If this item is editable.</param>
        private void AddBookItem(int item, Expander exp, bool edit)
        {
            Thread thd = new Thread(new ThreadStart(() => {
                Dispatcher.InvokeAsync(new Action(() => {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddImage(item, "WolfB.png", "BookImages", st, ServerData.Book, edit);
                    AddStaticContent(item, st, ServerData.Book, PropertyData.Description);
                    AddStaticContent(item, st, ServerData.Book, PropertyData.Created);
                    AddMarkingStars(item, _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User), st, ServerData.Book);
                    AddHoverableData(item, ServerData.Book, PropertyData.Year, st);

                    AddExpanderData("Categories", item, st, ServerData.Book, ServerData.BookCategory);
                    AddExpanderData("Authors", item, st, ServerData.Book, ServerData.Author);

                    int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
                    if (_proxy.GetUserItemWordsAsync(user, item, ServerData.Book).Result != null)
                    {
                        Expander words = new Expander { Header = "Books words", Background = Brushes.Azure };
                        StackPanel stack = new StackPanel();
                        foreach (int val in _proxy.GetUserItemWordsAsync(user, item, ServerData.Book).Result)
                        {
                            Expander tmp = new Expander { Header = _proxy.GetItemProperty(val, ServerData.Word, PropertyData.Name), Tag = val, IsEnabled = !FormData.EditWords.Contains(val) };
                            tmp.Expanded += expWord_Expanded;
                            stack.Children.Add(tmp);
                        }
                        Button print = new Button { Margin = new Thickness(5), MinWidth = 100, FontSize = 10, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right, Background = Brushes.Blue, Foreground = Brushes.White, Tag = $"Book:{item}", Content = "Print" };
                        print.Click += btnPrintWords_Click;
                        stack.Children.Add(print);

                        words.Content = stack;
                        st.Children.Add(words);
                    }

                    AddButtons(item, st, btnRemoveBook_Click, btnEditBook_Click, btnViewBook_Click);

                    exp.Content = st;
                }));
            }));
            thd.IsBackground = true;
            thd.Start();
        }
        /// <summary>
        /// Add a word to template.
        /// </summary>
        /// <param name="item">Id of a word.</param>
        /// <param name="exp">The element in which a word is supposed to appear.</param>
        /// <param name="edit">Is this item editable?</param>
        private void AddWordItem(int item, Expander exp, bool edit)
        {
            StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            AddImage(item, null, "WordImages", st, ServerData.Word, edit);
            bool hover = (exp.Parent == stActions);
            int trans = Convert.ToInt32(_proxy.GetItemProperty(item, ServerData.Word, PropertyData.Transcription));
            if (trans != 0)
            {
                StackPanel hor = new StackPanel();
                hor.Children.Add(new Label { Content = $"British:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new TextBlock { Text = _proxy.GetItemPropertyAsync(trans, ServerData.Transcription, PropertyData.British).Result, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Justify, Margin = new Thickness(5) });
                hor.Children.Add(new Label { Content = $"Canadian:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new TextBlock { Text = _proxy.GetItemPropertyAsync(trans, ServerData.Transcription, PropertyData.Canadian).Result, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Justify, Margin = new Thickness(5) });
                hor.Children.Add(new Label { Content = $"Australian:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new TextBlock { Text = _proxy.GetItemPropertyAsync(trans, ServerData.Transcription, PropertyData.Australian).Result, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Justify, Margin = new Thickness(5) });
                hor.Children.Add(new Label { Content = $"American:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new TextBlock { Text = _proxy.GetItemPropertyAsync(trans, ServerData.Transcription, PropertyData.American).Result, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Justify, Margin = new Thickness(5) });
                st.Children.Add(hor);
            }
            AddStaticContent(item, st, ServerData.Word, PropertyData.PluralForm);
            AddStaticContent(item, st, ServerData.Word, PropertyData.PastForm);
            AddStaticContent(item, st, ServerData.Word, PropertyData.PastThForm);

            AddExpanderData("Examples", item, st, ServerData.Word, ServerData.Example, hover);
            AddExpanderData("Categories", item, st, ServerData.Word, ServerData.WordCategory, hover);
            AddExpanderData("Groups", item, st, ServerData.Word, ServerData.Group, hover);
            AddExpanderData("Translations", item, st, ServerData.Word, ServerData.Translation, hover);
            AddExpanderData("Definitions", item, st, ServerData.Word, ServerData.Definition, hover);
            
            RoutedEventHandler editEvent = null;
            if (exp.Parent == stActions)
            {
                editEvent = btnEditWord_Click;
                AddButtons(item, st, btnRemoveWord_Click, editEvent, null);
            }
            else
            {
                Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Tag = item, ToolTip = "Delete" };
                btn.Click += btnRemoveUsersItemsWord_Click;
                st.Children.Add(btn);
            }
            
            exp.Content = st;
        }
        /// <summary>
        /// Add a grammar item to the template.
        /// </summary>
        /// <param name="item">Id of grammars item.</param>
        /// <param name="exp">The expander where the data are supposed to be added.</param>
        /// <param name="edit">If this item is editable.</param>
        private void AddGrammarItem(int item, Expander exp, bool edit)
        {
            Thread thd = new Thread(new ThreadStart(() => {
                Dispatcher.InvokeAsync(new Action(() => {
                    StackPanel st = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    AddStaticContent(item, st, ServerData.Grammar, PropertyData.Description);

                    AddExpanderData("Rules", item, st, ServerData.Grammar, ServerData.Rule, false);
                    AddExpanderData("Exceptions", item, st, ServerData.Grammar, ServerData.GrammarException, false);
                    AddExpanderData("Examples", item, st, ServerData.Grammar, ServerData.GrammarExample, false);

                    AddButtons(item, st, btnRemoveGrammar_Click, btnEditGrammar_Click, null);
                    exp.Content = st;
                }));
            }));
            thd.IsBackground = true;
            thd.Start();
        }
        #endregion
    }
}