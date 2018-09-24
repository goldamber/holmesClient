using AppEnglish.EngServRef;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish.View.Games
{
    enum WordsGameMode { Learning, Challenge }
    enum CardState { Answer, Question }

    public partial class WordsGamePlayer : Window
    {
        EngServiceClient _proxy;    //Host.
        int? _user;                 //Users id.
        int? _game = null;          //Id of game.
        ServerData _type;           //Id of words category.
        int _catId;                 //Type of words category.
        const int MAX_ITEMS_COUNT = 4;
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

        #region Buttons.
        private void btnGameFlashcards_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("flashcards", ServerData.Game);
            mode = WordsGameMode.Learning;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            Label word = new Label { Style = TryFindResource("lbCard") as Style, Tag = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name) };
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
            word.MouseDown += FlashCard_MouseDown;
            stGameContent.Children.Add(word);
            PlayGame();
        }
        private void btnGameQuizzes_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("quizzes", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            if (_words.Count < 4)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GenerateQuiezzesCard();
            PlayGame();
        }
        private void btnGamePuzzles_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("puzzles", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            if (_words.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GeneratePuzzlesCard();
            PlayGame();
        }
        private void btnGameVisualization_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("visualization", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetWordsWithImages(_catId, _type));
            if (_words.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            _puzzle = false;
            GeneratePuzzlesCard(_puzzle);
            PlayGame();
        }
        private void btnGameMatch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It is a challange game.\nJust choose appropriate answer.\nWhen you think you finished, click 'End game' button.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            mode = WordsGameMode.Challenge;
            PlayGame();
        }
        private void btnFindMistake_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It is a challange game.\nYou will be shown some words. If you think that this word is written with no mistake, push the button 'Valid', otherwise 'Wrong'.\nWhen you think you finished, click 'End game' button.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            mode = WordsGameMode.Challenge;
            PlayGame();
        }
        private void btnGameAgility_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It is a challange game.\nJust choose appropriate answer.\nWhen you think you finished, click 'End game' button.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            mode = WordsGameMode.Challenge;
            PlayGame();
        }
        private void btnGamePronunciation_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("pronunciation", ServerData.Game);
            MessageBox.Show("You will be shown the some tongue twisters. Just repeat them in order to improve your skills.\nGuess the word and than click on card in order to see the answer.\nThere is no challange mode.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            mode = WordsGameMode.Learning;

            stGame.HorizontalAlignment = HorizontalAlignment.Center;
            stGame.VerticalAlignment = VerticalAlignment.Center;
            stGameContent.Style = TryFindResource("stCard") as Style;

            if (_proxy.GetToungeTwisters() == null)
            {
                MessageBox.Show("There are no words in this category!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            _words = new List<int>(_proxy.GetToungeTwisters());
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            Label word = new Label { Style = TryFindResource("lbCard") as Style, Content = new TextBlock { Text = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name), Style = TryFindResource("txtCard") as Style } };
            word.MouseDown += Pronunciation_MouseDown;
            stGameContent.Children.Add(word);

            PlayGame();
        }

        private void Pronunciation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            (sender as Label).Content = new TextBlock { Text = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name), Style = TryFindResource("txtCard") as Style };
        }
        private void FlashCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (state == CardState.Question)
            {
                string word = (sender as Label).Tag.ToString();
                (sender as Label).Content = new Label { Content = word, Style = TryFindResource("lbFormNormal") as Style };
            }
            else
            {
                Random rnd = new Random();
                int wordsId = rnd.Next(0, _words.Count);
                stGameContent.Children.Clear();
                Label word = new Label { Style = TryFindResource("lbCard") as Style, Tag = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name) };
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
                word.MouseDown += FlashCard_MouseDown;
                stGameContent.Children.Add(word);
            }
            state = state == CardState.Question ? CardState.Answer : CardState.Question;
        }
        private void RbQuizzes_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Content.ToString() == (sender as RadioButton).Tag.ToString())
            {
                MessageBox.Show("Correct");
                AddScore();
            }
            else
            {
                MessageBox.Show("Wrong");
                SubtractScore();
            }

            GenerateQuiezzesCard();
        }

        private void BtnSetChar_Click(object sender, RoutedEventArgs e)
        {
            bool stop = true;
            bool win = true;
            Button button = new Button();
            foreach (Button item in wrWords.Children)
            {
                if (item.Content.ToString() == "" && item.IsEnabled)
                {
                    button = item;
                    break;
                }
            }

            button.Content = (sender as Button).Content;
            if (_hints)
            {
                if (button.Content.ToString() == button.Tag.ToString() && button.IsEnabled)
                {
                    button.Foreground = Brushes.ForestGreen;
                    button.Background = Brushes.LightGreen;
                    button.BorderBrush = Brushes.ForestGreen;
                }
                else
                {
                    button.Foreground = Brushes.Red;
                    button.Background = Brushes.Pink;
                    button.BorderBrush = Brushes.Red;
                }
            }
            foreach (Button item in wrWords.Children)
            {
                if (item.Content.ToString() == "" && item.IsEnabled)
                {
                    button = item;
                    stop = false;
                    break;
                }
                if (item.Content.ToString() != item.Tag.ToString())
                    win = false;
            }
            if (stop)
            {
                MessageBox.Show(win ? "Correct" : "Wrong");
                if (win)
                    AddScore();
                else
                    SubtractScore();
                GeneratePuzzlesCard(_puzzle);
            }
        }
        private void ClearPuzzle_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Content = "";
            CheckBoxPuzzle_Click(null, null);
        }

        private void btnEndGame_Click(object sender, RoutedEventArgs e)
        {
            if (mode == WordsGameMode.Challenge)
                ShowResults();
            else
                Close();
        }
        void ShowResults()
        {
            stGame.Visibility = Visibility.Collapsed;
            _level++;
            _proxy.AddScore(_score, Convert.ToInt32(_user), Convert.ToInt32(_game));
            _proxy.EditData(Convert.ToInt32(_user), _level.ToString(), ServerData.User, PropertyData.Level);
            Dictionary<int, int> lst = _proxy.GetHighScores(Convert.ToInt32(_game));
            int count = 1;
            foreach (int item in lst.Keys)
            {
                StackPanel st = new StackPanel { Orientation = Orientation.Horizontal };
                st.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Padding = new Thickness(5), Content = count });
                AddImage(item, "Wolf.png", "Avatars", 35, st, ServerData.User, false);

                Label login = new Label { Style = TryFindResource("lbFormNormal") as Style, Margin = new Thickness(50, 5, 50, 5), Padding = new Thickness(5), Content = _proxy.GetItemProperty(Convert.ToInt32(item), ServerData.User, PropertyData.Login) };
                if (item == _user)
                    st.Background = Brushes.LightGreen;
                st.Children.Add(login);

                StackPanel panel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                panel.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, Padding = new Thickness(0), Margin = new Thickness(5), FontSize = 14, FontWeight = FontWeights.Bold, Content = "Score:" });
                panel.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, Margin = new Thickness(0), Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 14, Content = lst[item] });
                st.Children.Add(panel);
                lstHighScores.Items.Add(st);
                count++;
                if (count - 1 != lst.Count)
                    lstHighScores.Items.Add(new Separator { Background = Brushes.PowderBlue });
            }
            stResuts.Visibility = Visibility.Visible;
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
            WrapPanel wr = new WrapPanel();
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
            WrapPanel wr = new WrapPanel();
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
            wrWords = new WrapPanel { Name = "wrWord" };
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