using AppEnglish.EngServRef;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish.View.Games
{
    enum WordsGameMode { Learning, Challenge }
    enum CardState { Answer, Question }

    public partial class WordsGamePlayer : Window
    {
        #region Variables.
        EngServiceClient _proxy;    //Host.
        int? _user;                 //Users id.
        int? _game = null;          //Id of game.
        ServerData _type;           //Id of words category.
        int _catId;                 //Type of words category.
        const int MAX_ITEMS_COUNT = 4;
        const int MAX_MATCH_COUNT = 12;
        const int MAX_UNCHANGED_LEVEL = 3;     //If users level is greater than this value, then his score will be counted depending on his level.
        int _score = 0;             //Current score.
        int _level;                 //Users level.
        int _lives = 3;             //The number of lives.
        WordsGameMode mode;         //Game mode.
        List<int> _words = new List<int>();     //Game words.
        CardState state = CardState.Question;   //Is card open or not.
        bool _hints = false;                    //Show  hints ti the player.
        WrapPanel wrWords = new WrapPanel();
        bool _puzzle = true;
        string _wordMatch = null;
        bool valid = true;
        #endregion

        #region Contructors.
        public WordsGamePlayer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="user">Id of player.</param>
        /// <param name="cat">Id of category.</param>
        /// <param name="data">Type of category.</param>
        public WordsGamePlayer(EngServiceClient tmp, int user, int cat, ServerData data) : this()
        {
            _proxy = tmp;
            _user = user;
            _catId = cat;
            _type = data;
            _level = Convert.ToInt32(_proxy.GetItemProperty(user, ServerData.User, PropertyData.Level));
        }
        #endregion
        #region Visualization.
        void PlayGame()
        {
            stOptions.Visibility = Visibility.Collapsed;
            stGame.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Downloads an image from server and presents it.
        /// </summary>
        /// <param name="id">Images id.</param>
        /// <param name="defaultPath">Default path.</param>
        /// <param name="height">Images height.</param>
        /// <param name="tempPath">The location of temporary files.</param>
        /// <param name="parent">The panel where an image is supposed to be added.</param>
        /// <param name="type">Data type.</param>
        void AddImage(int id, string defaultPath, string tempPath, int height, Panel parent, ServerData type, bool edit)
        {
            Thread thd = new Thread(new ThreadStart(() =>
            {
                Dispatcher.InvokeAsync(new Action(() => {
                    string img = _proxy.GetItemPropertyAsync(id, type, PropertyData.Imgpath).Result ?? defaultPath;
                    if (img == null)
                        return;

                    if (img == defaultPath)
                        parent.Children.Add(new Image { Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{defaultPath}")), Height = height });
                    else
                    {
                        if (!Directory.Exists($@"Temp\{tempPath}"))
                            Directory.CreateDirectory($@"Temp\{tempPath}");

                        FilesType filesType = FilesType.Avatars;
                        switch (type)
                        {
                            case ServerData.Video:
                                filesType = FilesType.VideosImages;
                                break;
                            case ServerData.Book:
                                filesType = FilesType.BooksImages;
                                break;
                            case ServerData.User:
                                filesType = FilesType.Avatars;
                                break;
                            case ServerData.Word:
                                filesType = FilesType.WordsImages;
                                break;
                        }
                        byte[] res = _proxy.Download(img, filesType);
                        if (res != null)
                        {
                            try
                            {
                                using (FileStream fs = File.OpenWrite($@"Temp\{tempPath}\{img}"))
                                {
                                    Task.WaitAny(fs.WriteAsync(res, 0, res.Length));
                                    fs.Dispose();
                                }
                            }
                            catch (IOException) { }

                            Image tmp = new Image { Height = height };
                            FormData.SetImage($@"pack://siteoforigin:,,,/Temp\{tempPath}\{img}", tmp);
                            parent.Children.Insert(0, tmp);
                        }
                    }
                }));
            }))
            { IsBackground = true };
            thd.Start();
        }

        void AddScore()
        {
            int dif = _level == 0 ? 1 : _level;
            _score += dif;
        }
        void SubtractScore()
        {
            _lives--;
            int dif = _level < MAX_UNCHANGED_LEVEL ? MAX_UNCHANGED_LEVEL : Convert.ToInt32(_level / 2);
            _score -= dif;
            if (_score < 0)
                _score = 0;
            if (_lives == 0)
                ShowResults();
        }

        void GenerateQuiezzesCard()
        {
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            stGameContent.Children.Clear();
            WrapPanel wr = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Score:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _score.ToString() });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Lives:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _lives.ToString() });
            stGameContent.Children.Add(wr);

            Label word = new Label { Style = TryFindResource("lbCard") as Style };
            StackPanel st = new StackPanel();
            AddImage(_words[wordsId], null, "WordImages", 100, st, ServerData.Word, false);
            if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)?.Length > 0)
            {
                int def = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition).Length)];
                st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(def, ServerData.Definition, PropertyData.Name) });
            }
            else if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)?.Length > 0)
            {
                int trans = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation).Length)];
                st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(trans, ServerData.Translation, PropertyData.Name) });
            }
            word.Content = st;
            stGameContent.Children.Add(word);

            string givenWord = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name);
            int pos = rnd.Next(0, MAX_ITEMS_COUNT);
            for (int i = 0; i < MAX_ITEMS_COUNT; i++)
            {
                RadioButton rb = new RadioButton { Content = (i == pos)? givenWord : _proxy.GetItemProperty(_words[rnd.Next(0, _words.Count)], ServerData.Word, PropertyData.Name), Margin = new Thickness(10), Tag = givenWord };
                rb.Checked += RbQuizzes_Checked;
                stGameContent.Children.Add(rb);
            }
        }
        void GeneratePuzzlesCard(bool puzzle = true)
        {
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            stGameContent.Children.Clear();
            WrapPanel wr = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Score:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _score.ToString() });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Lives:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _lives.ToString() });
            CheckBox checkBox = new CheckBox { Style = TryFindResource("chNormal") as Style, Content = "Show hints" };
            checkBox.Click += CheckBoxPuzzle_Click;
            checkBox.Checked += CheckBoxPuzzle_Checked;
            checkBox.Unchecked += CheckBoxPuzzle_Unchecked;
            wr.Children.Add(checkBox);
            stGameContent.Children.Add(wr);

            Label word = new Label { Style = TryFindResource("lbCard") as Style };
            StackPanel st = new StackPanel();
            AddImage(_words[wordsId], null, "WordImages", 100, st, ServerData.Word, false);
            if (puzzle)
            {
                if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)?.Length > 0)
                {
                    int def = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition).Length)];
                    st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(def, ServerData.Definition, PropertyData.Name) });
                }
                else if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)?.Length > 0)
                {
                    int trans = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation).Length)];
                    st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(trans, ServerData.Translation, PropertyData.Name) });
                }
            }
            word.Content = st;
            stGameContent.Children.Add(word);

            string givenWord = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name);
            wrWords = new WrapPanel { Name = "wrWord", HorizontalAlignment = HorizontalAlignment.Center };
            List<char> lst = new List<char>();
            foreach (char item in givenWord)
            {
                Button btn = new Button { Style = TryFindResource("btnPlaySquare") as Style, Content = "", Tag = item.ToString().ToUpper(), Background = Brushes.WhiteSmoke, BorderBrush = Brushes.Gray, Foreground = Brushes.Gray };
                if (item == ' ')
                {
                    btn.IsEnabled = false;
                    btn.Content = " ";
                    btn.BorderThickness = new Thickness(0);
                }
                else
                    lst.Add(item);
                btn.Click += ClearPuzzle_Click;
                wrWords.Children.Add(btn);
            }
            stGameContent.Children.Add(wrWords);

            wr = new WrapPanel();
            string str = "ABCDEFGHIJKLMOPQRSTUVWXYZ";
            for (int i = 0; i < rnd.Next(5, 12); i++)
            {
                lst.Insert(rnd.Next(0, lst.Count), str[rnd.Next(0, str.Length)]);
                lst.Reverse();
            }
            foreach (char item in lst)
            {
                Button btn = new Button { Style = TryFindResource("btnPlaySquare") as Style, Content = item.ToString().ToUpper() };
                btn.Click += BtnSetChar_Click; ;
                wr.Children.Add(btn);
            }
            stGameContent.Children.Add(wr);
        }
        private void GenerateMatchCard()
        {
            Random rnd = new Random();
            stGameContent.Children.Clear();
            WrapPanel wr = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Score:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _score.ToString() });
            stGameContent.Children.Add(wr);

            int max = _words.Count < MAX_MATCH_COUNT ? _words.Count : MAX_MATCH_COUNT;
            List<int> imgWords = new List<int>();
            for (int i = 0; i < max;)
            {
                int index = rnd.Next(0, _words.Count);
                if (!imgWords.Contains(_words[index]))
                {
                    imgWords.Add(_words[index]);
                    i++;
                }
            }
            List<int> descWords = new List<int>();
            for (int i = 0; i < max;)
            {
                int index = rnd.Next(0, _words.Count);
                if (!descWords.Contains(_words[index]))
                {
                    descWords.Add(_words[index]);
                    i++;
                }
            }
            wrWords = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            bool desc = false;
            int img = 0;
            int nameInd = 0;
            for (int i = 0; i < max*2; i++)
            {
                if (desc)
                {
                    int index = imgWords[img];
                    Label container = new Label { Style = TryFindResource("lbCard") as Style, Height = 250, Width = 250, Tag = _proxy.GetItemProperty(index, ServerData.Word, PropertyData.Name) };
                    StackPanel st = new StackPanel();
                    AddImage(index, null, "WordImages", 50, st, ServerData.Word, false);
                    if (_proxy.GetItemData(index, ServerData.Word, ServerData.Definition)?.Length > 0)
                    {
                        int def = _proxy.GetItemData(index, ServerData.Word, ServerData.Definition)[rnd.Next(0, _proxy.GetItemData(index, ServerData.Word, ServerData.Definition).Length)];
                        st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(def, ServerData.Definition, PropertyData.Name) });
                    }
                    else if (_proxy.GetItemData(index, ServerData.Word, ServerData.Translation)?.Length > 0)
                    {
                        int trans = _proxy.GetItemData(index, ServerData.Word, ServerData.Translation)[rnd.Next(0, _proxy.GetItemData(index, ServerData.Word, ServerData.Translation).Length)];
                        st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(trans, ServerData.Translation, PropertyData.Name) });
                    }
                    container.Content = st;
                    container.MouseDown += ContainerMatch_MouseDown;
                    wrWords.Children.Add(container);
                    img++;
                }
                else
                {
                    int index = descWords[nameInd];
                    string wordsName = _proxy.GetItemProperty(index, ServerData.Word, PropertyData.Name);
                    Label container = new Label { Style = TryFindResource("lbCard") as Style, Height = 250, Width = 250, Tag = wordsName, Content = new Label { Style = TryFindResource("lbFormNormal") as Style, Content = wordsName } };
                    container.MouseDown += ContainerMatch_MouseDown;
                    wrWords.Children.Add(container);
                    nameInd++;
                }
                desc = !desc;
            }
            stGameContent.Children.Add(wrWords);
        }
        private void GenerateMistakeCard()
        {
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            stGameContent.Children.Clear();
            WrapPanel wr = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Score:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _score.ToString() });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Lives:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _lives.ToString() });
            stGameContent.Children.Add(wr);

            string givenWord = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name);
            string defaultWord = givenWord;
            string str = "ABCDEFGHIJKLMOPQRSTUVWXYZ";
            int pos = rnd.Next(0, givenWord.Length);
            valid = (rnd.Next(0, 20) % 2 == 0) ? true : false;
            givenWord = givenWord.ToUpper();
            if (!valid)
            {
                while (true)
                {
                    char wrong = str[rnd.Next(0, str.Length)];
                    if (wrong != givenWord.ToCharArray()[pos])
                    {
                        givenWord = givenWord.Replace(givenWord[pos], wrong);
                        break;
                    }
                }
            }
            Label word = new Label { Style = TryFindResource("lbCard") as Style, Content = new Label { Content = givenWord, Style = TryFindResource("lbFormNormal") as Style } };
            stGameContent.Children.Add(word);

            WrapPanel panel = new WrapPanel { Orientation = Orientation.Horizontal };
            Button btnValid = new Button { Style = TryFindResource("btnNormal") as Style, Background = Brushes.LightGreen, BorderBrush = Brushes.DarkGreen, Foreground = Brushes.Black };
            StackPanel st = new StackPanel { Orientation = Orientation.Horizontal };
            st.Children.Add(new Image { Height = 20, Source = new BitmapImage(new Uri("pack://application:,,,/Images/Tick.png")) });
            st.Children.Add(new TextBlock { Text = "Valid" });
            btnValid.Content = st;
            btnValid.Click += BtnValid_Click;
            panel.Children.Add(btnValid);

            Button btnWrong = new Button { Style = TryFindResource("btnNormal") as Style, Background = Brushes.Red, BorderBrush = Brushes.DarkRed, Foreground = Brushes.White, Tag = defaultWord };
            st = new StackPanel { Orientation = Orientation.Horizontal };
            st.Children.Add(new Image { Height = 20, Source = new BitmapImage(new Uri("pack://application:,,,/Images/Wrong.png")) });
            st.Children.Add(new TextBlock { Text = "Wrong" });
            btnWrong.Content = st;
            btnWrong.Click += BtnWrong_Click; ;
            panel.Children.Add(btnWrong);
            stGameContent.Children.Add(panel);
        }
        private void GenerateAgilityCard()
        {
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            stGameContent.Children.Clear();
            WrapPanel wr = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Score:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _score.ToString() });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, FontWeight = FontWeights.Bold, Text = "Lives:" });
            wr.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _lives.ToString() });
            stGameContent.Children.Add(wr);

            string givenWord = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name);
            Label word = new Label { Style = TryFindResource("lbCard") as Style, Content = new Label { Content = givenWord, Style = TryFindResource("lbFormNormal") as Style } };
            stGameContent.Children.Add(word);
            List<int> synonyms = new List<int>(_proxy.GetFItems(givenWord, ServerData.Word, PropertyData.Synonyms));
            string validOption = _proxy.GetItemProperty(synonyms[rnd.Next(0, synonyms.Count)], ServerData.Word, PropertyData.Name);
            List<int> allWords = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            string wrongOption = _proxy.GetItemProperty(allWords[rnd.Next(0, allWords.Count)], ServerData.Word, PropertyData.Name);
            while (wrongOption == validOption)
            {
                wrongOption = _proxy.GetItemProperty(allWords[rnd.Next(0, allWords.Count)], ServerData.Word, PropertyData.Name);
            }
            bool first = (rnd.Next(0, 20) % 2 == 0) ? true : false;

            WrapPanel panel = new WrapPanel { Orientation = Orientation.Horizontal };
            Button btnFirst = new Button { Style = TryFindResource("btnNormal") as Style, Content = first? validOption: wrongOption, Tag = first? givenWord: null };
            btnFirst.Click += BtnFirst_Click;
            panel.Children.Add(btnFirst);

            btnFirst = new Button { Style = TryFindResource("btnNormal") as Style, Content = !first ? validOption : wrongOption, Tag = !first ? givenWord : null };
            btnFirst.Click += BtnFirst_Click;
            panel.Children.Add(btnFirst);
            stGameContent.Children.Add(panel);
        }
        
        private void CheckBoxPuzzle_Unchecked(object sender, RoutedEventArgs e)
        {
            _hints = false;
        }
        private void CheckBoxPuzzle_Checked(object sender, RoutedEventArgs e)
        {
            _hints = true;
        }
        private void CheckBoxPuzzle_Click(object sender, RoutedEventArgs e)
        {
            if (_hints)
            {
                foreach (Button item in wrWords.Children)
                {
                    if (item.Content.ToString() == item.Tag.ToString() && item.IsEnabled)
                    {
                        item.Foreground = Brushes.ForestGreen;
                        item.Background = Brushes.LightGreen;
                        item.BorderBrush = Brushes.ForestGreen;
                    }
                    else
                    {
                        item.Foreground = Brushes.Red;
                        item.Background = Brushes.Pink;
                        item.BorderBrush = Brushes.Red;
                    }
                }
            }
            else
            {
                foreach (Button item in wrWords.Children)
                {
                    item.Background = Brushes.WhiteSmoke;
                    item.BorderBrush = Brushes.Gray;
                    item.Foreground = Brushes.Gray;
                }
            }
        }
        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (stGame.Visibility == Visibility.Visible && mode == WordsGameMode.Challenge)
            {
                if (MessageBox.Show("Are you sure you want to end this game?\nYour will lose your score.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }
    }
}