using AppEnglish.AddEdit;
using AppEnglish.EngServRef;
using EpubSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppEnglish
{
    public partial class BookReader : Window
    {
        EngServiceClient _proxy;
        int book;
        int user;
        #region Pagination.
        List<string> _words = new List<string>();   //All words.
        const int _maxWordsPerPage = 250;      //The limit of words per page.
        int _min = 1;           //First page.
        int _max = 1;           //The total number of pages.
        int _totalWords = 1;    //The total number of words.
        #endregion

        #region Constructors.
        //Initialization.
        public BookReader()
        {
            InitializeComponent();

            btnFirst.Content = "<<";
            btnPrev.Content = "<";
            btnLast.Content = ">>";
            btnNext.Content = ">";
        }
        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="proxy">Server.</param>
        /// <param name="id">Books id.</param>
        /// <param name="usersId">Users id.</param>
        public BookReader(EngServiceClient proxy, int id, int usersId) : this()
        {
            _proxy = proxy;
            book = id;
            user = usersId;
        }
        #endregion
        #region Book initialization.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            prgPreloader.Visibility = Visibility.Visible;
            bool isAbsolute = _proxy.CheckAbsolute(book, ServerData.Book) == true ? true : false;
            string booksPath = _proxy.GetItemProperty(book, ServerData.Book, PropertyData.Path);
            txtName.Text = _proxy.GetItemProperty(book, ServerData.Book, PropertyData.Name);
            
            string path = isAbsolute ? booksPath : $@"Temp\Books\{booksPath}";
            if (!isAbsolute)
            {
                if (!Directory.Exists(@"Temp\Books"))
                    Directory.CreateDirectory(@"Temp\Books");
                byte[] res = _proxy.DownloadAsync(booksPath, FilesType.Books).Result;
                if (res != null)
                {
                    using (FileStream fs = File.OpenWrite(path))
                    {
                        Task.WaitAll(fs.WriteAsync(res, 0, res.Length));
                        fs.Dispose();
                    }
                }
            }
            if (!File.Exists(path))
            {
                MessageBox.Show($"This file does not exist!\n({path})", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            string data = "";
            if (System.IO.Path.GetExtension(path) == ".epub")
            {
                EpubBook book = EpubReader.Read(path);
                data = book.ToPlainText();
            }
            else if (System.IO.Path.GetExtension(path) == ".pdf")
            {
                PdfReader reader = new PdfReader(path);
                string text = string.Empty;
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    text += PdfTextExtractor.GetTextFromPage(reader, page);
                }
                reader.Close();
                data = text;
            }
            else
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        data = sr.ReadToEndAsync().Result;
                        sr.Dispose();
                    }
                }
            }

            Thread thd = new Thread(new ThreadStart(() =>
            {
                _words.AddRange(data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));

                Dispatcher.InvokeAsync(new Action(() =>
                {
                    foreach (string item in _words)
                    {
                        _totalWords += item.Split(" \\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
                    }
                    _max = (_totalWords / _maxWordsPerPage) + 1;
                    txtMax.Text = $"of {_max}";
                    slPages.Maximum = _max;

                    int? bm = _proxy.GetLastMarkAsync(book, user, ServerData.Book).Result;
                    if (bm != null)
                    {
                        txtPage.Text = _proxy.GetItemProperty(Convert.ToInt32(bm), ServerData.Bookmark, PropertyData.Position);
                        ChangePage(Convert.ToInt32(txtPage.Text));
                    }
                    else
                        ChangePage(1);
                    if (Convert.ToInt32(txtPage.Text) == _max)
                    {
                        btnLast.IsEnabled = btnNext.IsEnabled = false;
                        slPages.IsEnabled = false;
                    }
                    prgPreloader.Visibility = Visibility.Collapsed;
                }));
            }))
            {
                IsBackground = true
            };
            thd.Start();
        }
        #endregion

        #region Add action (word, bookmark).
        //Add a new word.
        private void Lb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string data = (sender as TextBox).Text;
            try
            {
                data = (sender as TextBox).Text.Split(" 1234567890.,!?()-_<>;:\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            }
            catch
            {
                return;
            }
            int? word = _proxy.GetWord(data);
            if (word == null)
            {
                AddWord form = new AddWord(_proxy, data, user, book, ServerData.Book);
                form.ShowDialog();
            }
            else
            {
                if (MessageBox.Show("Do you want to add this word to your list?", "Save word", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _proxy.AddItemsWord(Convert.ToInt32(word), book, ServerData.Book);
                    _proxy.AddItemsWord(Convert.ToInt32(word), user, ServerData.User);
                }
            }
        }
        //Add bookmark, change current page.
        private void txtPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            int pos = Convert.ToInt32((sender as TextBox).Text);
            if (btnFirst != null && btnPrev != null)
                btnFirst.IsEnabled = btnPrev.IsEnabled = (pos != _min);
            if (btnNext != null && btnLast != null)
                btnNext.IsEnabled = btnLast.IsEnabled = (pos != _max);
            if (slPages != null)
                slPages.Value = pos;
            
            if (_proxy != null)
            {
                ChangePage(pos);
                _proxy.AddBookmark(pos, book, user);
            }
        }
        #endregion
        #region Visualization (tips).
        //Show word data.
        private void Lb_MouseEnter(object sender, MouseEventArgs e)
        {
            string data = (sender as TextBox).Text;
            try
            {
                data = (sender as TextBox).Text.Split(" 1234567890.,!?()-_<>;:\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            }
            catch
            {
                return;
            }
            int? word = _proxy.GetWord(data);
            if (word != null)
            {
                int item = Convert.ToInt32(word);
                StackPanel st = new StackPanel();
                AddImage(item, st);
                int trans = Convert.ToInt32(_proxy.GetItemProperty(item, ServerData.Word, PropertyData.Transcription));
                if (trans != 0)
                {
                    StackPanel hor = new StackPanel();
                    hor.Children.Add(new Label { Content = $"Transcription:", FontSize = 14, FontWeight = FontWeights.Bold });
                    hor.Children.Add(new TextBlock { Text = _proxy.GetItemPropertyAsync(trans, ServerData.Transcription, PropertyData.British).Result, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Justify, Margin = new Thickness(5) });
                    st.Children.Add(hor);
                }
                AddStaticContent(item, st, PropertyData.PluralForm);
                AddStaticContent(item, st, PropertyData.PastForm);
                AddStaticContent(item, st, PropertyData.PastThForm);
                AddExpanderData("Examples", item, st, ServerData.Example);
                AddExpanderData("Translations", item, st, ServerData.Translation);
                AddExpanderData("Definitions", item, st, ServerData.Definition);
                (sender as TextBox).ToolTip = st;
            }
        }
        /// <summary>
        /// Downloads an image from server and presents it.
        /// </summary>
        /// <param name="id">Images id.</param>
        /// <param name="parent">The panel where an image is supposed to be added.</param>
        void AddImage(int id, Panel parent)
        {
            Thread thd = new Thread(new ThreadStart(() =>
            {
                Dispatcher.InvokeAsync(new Action(() =>
                {
                    string img = _proxy.GetItemPropertyAsync(id, ServerData.Word, PropertyData.Imgpath).Result;
                    if (img == null)
                        return;

                    if (!Directory.Exists($@"Temp\WordImages"))
                        Directory.CreateDirectory($@"Temp\WordImages");
                    
                    byte[] res = _proxy.Download(img, FilesType.WordsImages);
                    if (res != null)
                    {
                        try
                        {
                            using (FileStream fs = File.OpenWrite($@"Temp\WordImages\{img}"))
                            {
                                Task.WaitAny(fs.WriteAsync(res, 0, res.Length));
                                fs.Dispose();
                            }
                        }
                        catch (IOException) { }
                        Image tmp = new Image { Height = 110 };
                        FormData.SetImage($@"pack://siteoforigin:,,,/Temp\WordImages\{img}", tmp);
                        parent.Children.Insert(0, tmp);
                    }
                }));
            }))
            { IsBackground = true };
            thd.Start();
        }
        /// <summary>
        /// Describes static one-line property.
        /// </summary>
        /// <param name="content">The data to insert.</param>
        /// <param name="item">Id of the item to which this property belongs.</param>
        /// <param name="st">The panel where to insert.</param>
        /// <param name="property">The type of property.</param>
        void AddStaticContent(int item, Panel st, PropertyData property)
        {
            Thread thd = new Thread(new ThreadStart(() =>
            {
                Dispatcher.InvokeAsync(new Action(() => {
                    if (_proxy.GetItemPropertyAsync(item, ServerData.Word, property).Result != null)
                    {
                        StackPanel hor = new StackPanel();
                        string header = property.ToString();
                        switch (property)
                        {
                            case PropertyData.PastForm:
                                header = "Past form";
                                break;
                            case PropertyData.PastThForm:
                                header = "Past participle";
                                break;
                            case PropertyData.PluralForm:
                                header = "Plural";
                                break;
                        }
                        hor.Children.Add(new Label { Content = $"{header}:", FontSize = 14, FontWeight = FontWeights.Bold });
                        hor.Children.Add(new TextBlock { Text = _proxy.GetItemPropertyAsync(item, ServerData.Word, property).Result, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Justify, Margin = new Thickness(5) });
                        st.Children.Add(hor);
                    }
                }));
            }))
            { IsBackground = true };
            thd.Start();
        }
        /// <summary>
        /// Insters extra data to an item (categories, words, ...).
        /// </summary>
        /// <param name="header">The title of expander.</param>
        /// <param name="item">Id of item to be decorated.</param>
        /// <param name="st">A panel where the data are supposed to be added.</param>
        /// <param name="res">A type of the inserted data.</param>
        void AddExpanderData(string header, int item, Panel st, ServerData res)
        {
            Thread thd = new Thread(new ThreadStart(() =>
            {
                Dispatcher.InvokeAsync(new Action(() => {
                    if (_proxy.GetItemData(item, ServerData.Word, res) == null || _proxy.GetItemData(item, ServerData.Word, res).Length == 0)
                        return;

                    GroupBox hor = new GroupBox { Header = header, Background = Brushes.Azure };
                    StackPanel ver = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    int count = 1;
                    foreach (int val in _proxy.GetItemData(item, ServerData.Word, res))
                    {
                        object obj = new object();
                        Task.Run(() => {
                            lock (obj)
                            {
                                Dispatcher.InvokeAsync(() => {
                                    StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Background = Brushes.Azure };
                                    panel.Children.Add(new Label { Content = count, FontSize = 9, FontWeight = FontWeights.Bold, Margin = new Thickness(5) });
                                    TextBlock label = new TextBlock { Padding = new Thickness(5), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Tag = header, Text = _proxy.GetItemProperty(val, res, PropertyData.Name), FontSize = 12, FontWeight = FontWeights.Normal };
                                    if (res == ServerData.Author)
                                        label.Text = _proxy.GetItemPropertyAsync(val, res, PropertyData.Name).Result + " " + _proxy.GetItemProperty(val, res, PropertyData.Surname);
                                    panel.Children.Add(label);
                                    ver.Children.Add(panel);
                                    count++;
                                });
                            }
                        });
                    }
                    hor.Content = ver;
                    st.Children.Add(hor);
                }));
            }))
            { IsBackground = true };
            thd.Start();
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.White;
        }
        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.Silver;
        }
        #endregion
        #region Pagination.
        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            txtPage.Text = _min.ToString();
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > _min)
                txtPage.Text = (Convert.ToInt32(txtPage.Text) - 1).ToString();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) < _max)
                txtPage.Text = (Convert.ToInt32(txtPage.Text) + 1).ToString();
        }
        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            txtPage.Text = _max.ToString();
        }
        private void slPages_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtPage.Text = Convert.ToInt32((sender as Slider).Value).ToString();
        }
        /// <summary>
        /// Flip the page.
        /// </summary>
        /// <param name="page">The number of page.</param>
        void ChangePage(int page)
        {
            int wordsCount = 1;
            int from = (page - 1) * _maxWordsPerPage;
            int to = page * _maxWordsPerPage;

            prgPreloader.Visibility = Visibility.Visible;
            stWords.Items.Clear();
            foreach (string item in _words)
            {
                WrapPanel tmp = new WrapPanel { Margin = new Thickness(2) };
                foreach (string word in item.Split(" \\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (wordsCount >= from && wordsCount < to)
                    {
                        TextBox lb = new TextBox { Text = word, Style = TryFindResource("txtSub") as Style };
                        lb.PreviewMouseDown += Lb_PreviewMouseDown;
                        lb.MouseEnter += Lb_MouseEnter;
                        tmp.Children.Add(lb);
                    }
                    wordsCount++;
                    if (wordsCount > to)
                        break;
                }

                if (tmp.Children.Count > 0)
                {
                    tmp.Background = Brushes.White;
                    tmp.MouseEnter += Label_MouseEnter;
                    tmp.MouseLeave += Label_MouseLeave;
                    stWords.Items.Add(tmp);
                }
            }
            prgPreloader.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}