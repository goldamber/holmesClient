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

        #region Constructors.
        //Initialization.
        public BookReader()
        {
            InitializeComponent();
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
            bool isAbsolute = _proxy.CheckAbsolute(book, EngServRef.ServerData.Book) == true ? true : false;
            string booksPath = _proxy.GetItemProperty(book, EngServRef.ServerData.Book, EngServRef.PropertyData.Path);

            List<string> _words = new List<string>();
            string path = isAbsolute ? booksPath : $@"Temp\Books\{booksPath}";
            if (!isAbsolute)
            {
                if (!Directory.Exists(@"Temp\Books"))
                    Directory.CreateDirectory(@"Temp\Books");
                byte[] res = _proxy.Download(booksPath, EngServRef.FilesType.Books);
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
                        data = sr.ReadToEnd();
                        sr.Dispose();
                    }
                }
            }

            Thread thd = new Thread(new ThreadStart(() =>
            {
                foreach (string item in data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    _words.Add(item);
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    foreach (string item in _words)
                    {
                        WrapPanel tmp = new WrapPanel { Margin = new Thickness(2) };

                        foreach (string word in item.Split(" \\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            TextBox lb = new TextBox { Text = word, Style = TryFindResource("txtSub") as Style };
                            lb.PreviewMouseRightButtonDown += Lb_MouseRightButtonDown;
                            lb.PreviewMouseDown += Label_MouseDown;
                            lb.MouseEnter += Lb_MouseEnter;
                            tmp.Children.Add(lb);
                        }

                        tmp.Background = Brushes.White;
                        tmp.MouseEnter += Label_MouseEnter;
                        tmp.MouseLeave += Label_MouseLeave;

                        stWords.Items.Add(tmp);
                    }

                    int? bm = _proxy.GetLastMark(book, user, EngServRef.ServerData.Book);
                    if (bm != null)
                    {
                        int pos = Convert.ToInt32(_proxy.GetItemProperty(Convert.ToInt32(bm), EngServRef.ServerData.Bookmark, EngServRef.PropertyData.Position));
                        stWords.SelectedItem = stWords.Items[pos];
                        stWords.ScrollIntoView(stWords.SelectedItem);
                    }
                }));
            }));
            thd.IsBackground = true;
            thd.Start();
        }
        #endregion

        #region Add action (word, bookmark).
        //'Add word' click.
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord("", _proxy);
            frm.ShowDialog();
        }
        //Add existing word.
        private void Lb_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord((sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0], _proxy);
            frm.ShowDialog();
        }

        //Add bookmark.
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int pos = stWords.Items.IndexOf((sender as FrameworkElement).Parent);
                _proxy.AddBookmark(pos, book, user);
            }
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
    }
}