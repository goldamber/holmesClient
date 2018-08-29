using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppEnglish
{
    public partial class BookReader : Window
    {
        EngServRef.EngServiceClient _proxy;

        #region Constructors.
        public BookReader()
        {
            InitializeComponent();
        }
        public BookReader(string str, EngServRef.EngServiceClient proxy) : this()
        {
            txtName.Text = str;
            _proxy = proxy;
        }
        #endregion

        #region Book initialization.
        private void FillListBox()
        {
            /*List<string> _words = new List<string>();
            string str = "";

            using (FileStream fs = new FileStream(!_book.IsAbsolulute ? $"Books/{_book.Path}" : _book.Path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    str = sr.ReadToEnd();
                }
            }

            Thread thd = new Thread(new ThreadStart(() =>
            {
                foreach (string item in str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
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

                    EngServRef.Bookmark _mark = _proxy.GetBookmark(txtName.Text, _user);
                    if (_mark != null)
                    {
                        stWords.SelectedItem = stWords.Items[_mark.Parent];
                        stWords.ScrollIntoView(stWords.SelectedItem);
                    }
                }));
            }));
            thd.IsBackground = true;
            thd.Start();*/
        }
        #endregion

        #region Add action (word, bookmark).
        //'Add word' click.
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord("", _proxy);
            frm.ShowDialog();
        }
        //Add bookmark.
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    _proxy.AddBookmark(txtName.Text, _user, ((sender as FrameworkElement).Parent as Panel).Children.IndexOf(sender as UIElement), stWords.Items.IndexOf((sender as FrameworkElement).Parent));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }*/
        }
        //Add existing word.
        private void Lb_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord((sender as TextBox).Text.Split(" .,!?()8-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0], _proxy);
            frm.ShowDialog();
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