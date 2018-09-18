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
        EngServRef.EngServiceClient _proxy;
        int book;
        int user;
        #region Pagination.
        List<string> _words = new List<string>();   //All words.
        const int _maxWordsPerPage = 1000;      //The limit of words per page.
        int maxBook = 1;         //Index of the last word.
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
        public BookReader(EngServRef.EngServiceClient proxy, int id, int usersId) : this()
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
            bool isAbsolute = _proxy.CheckAbsolute(book, EngServRef.ServerData.Book) == true ? true : false;
            string booksPath = _proxy.GetItemProperty(book, EngServRef.ServerData.Book, EngServRef.PropertyData.Path);
            txtName.Text = _proxy.GetItemProperty(book, EngServRef.ServerData.Book, EngServRef.PropertyData.Name);
            
            string path = isAbsolute ? booksPath : $@"Temp\Books\{booksPath}";
            if (!isAbsolute)
            {
                if (!Directory.Exists(@"Temp\Books"))
                    Directory.CreateDirectory(@"Temp\Books");
                byte[] res = _proxy.DownloadAsync(booksPath, EngServRef.FilesType.Books).Result;
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
                    
                    int? bm = _proxy.GetLastMarkAsync(book, user, EngServRef.ServerData.Book).Result;
                    if (bm != null)
                    {
                        txtPage.Text = _proxy.GetItemProperty(Convert.ToInt32(bm), EngServRef.ServerData.Bookmark, EngServRef.PropertyData.Position);
                        maxBook = ChangePage(_maxWordsPerPage * Convert.ToInt32(txtPage.Text) - _maxWordsPerPage + 1, _maxWordsPerPage * Convert.ToInt32(txtPage.Text));
                    }
                    else
                        maxBook = ChangePage(1, _maxWordsPerPage);
                    if (Convert.ToInt32(txtPage.Text) == _max)
                        btnLast.IsEnabled = btnNext.IsEnabled = false;
                    prgPreloader.Visibility = Visibility.Collapsed;
                }));
            }));
            thd.IsBackground = true;
            thd.Start();
        }
        #endregion

        #region Add action (word, bookmark).
        //Add a new word.
        private void Lb_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord((sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0], _proxy);
            frm.ShowDialog();
        }
        //Add bookmark.
        private void txtPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            int pos = Convert.ToInt32((sender as TextBox).Text);
            if (btnFirst != null && btnPrev != null)
                btnFirst.IsEnabled = btnPrev.IsEnabled = (pos != _min);
            if (btnNext != null && btnLast != null)
                btnNext.IsEnabled = btnLast.IsEnabled = (pos != _max);
            _proxy?.AddBookmark(pos, book, user);
        }
        #endregion
        #region Visualization (tips).
        //Show word data.
        private void Lb_MouseEnter(object sender, MouseEventArgs e)
        {
            /*StringBuilder str = new StringBuilder();
            List<EngServRef.Definition> lst = new List<EngServRef.Definition>(_proxy.GetWordData((sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]));
            List<EngServRef.Translation> lstT = new List<EngServRef.Translation>(_proxy.GetWordTrans((sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]));
            EngServRef.WordForm frm = _proxy.GetWordExtra((sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            if (lst.Count > 0 || lstT.Count > 0)
                str.Append($"Word: {(sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]}.\n");

            if (frm != null)
            {
                str.Append("Forms: ");
                if (frm.PastForm != null)
                    str.Append(frm.PastForm + "\n");
                if (frm.Gerund != null)
                    str.Append(frm.Gerund + "\n");
                if (frm.PastThForm != null)
                    str.Append(frm.PastThForm + "\n");
                if (frm.PluralForm != null)
                    str.Append(frm.PluralForm + "\n");
                if (frm.PresentForm != null)
                    str.Append(frm.PresentForm + "\n");
            }
            if (lst.Count > 0)
            {
                str.Append($"\nDefinitions: ");
                foreach (EngServRef.Definition item in lst)
                {
                    str.Append(item.Name + "\n");
                }
            }
            if (lstT.Count > 0)
            {
                str.Append($"\nTranslations: ");
                foreach (EngServRef.Translation item in lstT)
                {
                    str.Append(item.Name + "\n");
                }
            }

            if (str.Length > 0)
                (sender as TextBox).ToolTip = str;*/
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
            maxBook = ChangePage(1, _maxWordsPerPage);
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > _min)
            {
                txtPage.Text = (Convert.ToInt32(txtPage.Text) - 1).ToString();
                int mw = maxBook - _maxWordsPerPage <= 0 ? 1 : maxBook - _maxWordsPerPage;
                ChangePage(mw, maxBook);
                maxBook = mw;
            }
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) < _max)
            {
                txtPage.Text = (Convert.ToInt32(txtPage.Text) + 1).ToString();
                maxBook = ChangePage(maxBook, _maxWordsPerPage * Convert.ToInt32(txtPage.Text));
            }
        }
        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            txtPage.Text = _max.ToString();
            maxBook = ChangePage(_totalWords - _maxWordsPerPage <= 0 ? 1 : _totalWords - _maxWordsPerPage, _totalWords + 1);
        }
        /// <summary>
        /// Flip the page.
        /// </summary>
        /// <param name="from">The first word.</param>
        /// <param name="to">The last word.</param>
        /// <returns>Index of the last word.</returns>
        int ChangePage(int from, int to)
        {
            int wordsCount = 1;
            stWords.Items.Clear();

            foreach (string item in _words)
            {
                if (wordsCount < to)
                {
                    wordsCount += item.Split(" \\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordsCount >= from)
                    {
                        WrapPanel tmp = new WrapPanel { Margin = new Thickness(2) };
                        foreach (string word in item.Split(" \\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            TextBox lb = new TextBox { Text = word, Style = TryFindResource("txtSub") as Style };
                            lb.PreviewMouseRightButtonDown += Lb_MouseButtonDown;
                            lb.MouseEnter += Lb_MouseEnter;
                            tmp.Children.Add(lb);
                        }

                        tmp.Background = Brushes.White;
                        tmp.MouseEnter += Label_MouseEnter;
                        tmp.MouseLeave += Label_MouseLeave;

                        stWords.Items.Add(tmp);
                    }
                }
            }
            return wordsCount;
        }
        #endregion
    }
}