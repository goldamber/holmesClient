using AppEnglish.EngServRef;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AppEnglish.View.Games
{
    enum GameMode { FillGaps, MultipleChoice };

    public partial class GamePlayer : Window
    {
        EngServiceClient _proxy;        //Host.
        DispatcherTimer dsTime;
        List<SubTitle> _subs = new List<SubTitle>();    //List of subtitles.
        List<string> _subWords = new List<string>();    //All words.
        int? user;              //Users id.
        int? _game = null;               //Id of game.
        int videoId;            //VideosId.
        bool _play = true;      //Play video.
        bool _scroll = true;    //Scroll to subs.
        int wordsCount = 0;       //The number of words for this video.
        int gapsCount = 0;       //The number of gaps for this game.
        int _hits = 0;          //The number of valid words.
        int _fails = 0;          //The number of mistakes.
        int _score = 0;         //Current score.
        int level;              //Users level.
        GameMode mode = GameMode.FillGaps;
        const int MAX_UNCHANGED_LEVEL = 3;     //If users level is greater than this value, then his score will be counted depending on his level.
        const int MAX_COMBOBOXITEMS_COUNT = 4;

        #region Initialization.
        #region Constructors.
        public GamePlayer()
        {
            InitializeComponent();
            
            dsTime = new DispatcherTimer();
            dsTime.Interval = TimeSpan.FromMilliseconds(700);
            dsTime.Tick += new EventHandler(TimerTick);
        }
        public GamePlayer(EngServiceClient tmp, int video, int? user) : this()
        {
            _proxy = tmp;
            videoId = video;
            this.user = user;
            txtName.Text = _proxy.GetItemProperty(video, ServerData.Video, PropertyData.Name);
            level = Convert.ToInt32(_proxy.GetItemProperty(Convert.ToInt32(user), ServerData.User, PropertyData.Level));
            _game = _proxy.GetItemsId("Video player", ServerData.Game);
        }
        #endregion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!GetWordsNumber())
                Close();
        }
        //Video player loaded.
        private void stVideo_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (stVideo.Visibility == Visibility.Visible)
            {
                LoadVideoSubs();
                Thread thd = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            bool isAbsolute = _proxy.CheckAbsolute(videoId, ServerData.Video) == true ? true : false;
                            string videoPath = _proxy.GetItemProperty(videoId, ServerData.Video, PropertyData.Path);
                            string path = isAbsolute ? videoPath : $@"Temp\Videos\{videoPath}";
                            if (!isAbsolute)
                            {
                                if (!Directory.Exists(@"Temp\Videos"))
                                    Directory.CreateDirectory(@"Temp\Videos");
                                byte[] res = _proxy.DownloadAsync(videoPath, FilesType.Videos).Result;
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
                                MessageBox.Show($"Video can not be found!\n({path})", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                                Close();
                                return;
                            }
                            if (_game == null)
                            {
                                MessageBox.Show("This game does not exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                Close();
                                return;
                            }

                            txtScore.Text = _score.ToString();
                            txtGaps.Text = gapsCount.ToString();
                            txtHits.Text = _hits.ToString();
                            txtFails.Text = _fails.ToString();
                            mainVideo.Source = new Uri(isAbsolute ? path : $"pack://siteoforigin:,,,/{path}");
                            mainVideo.Volume = slVolume.Value;
                            mainVideo.Play();
                        }));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }))
                { IsBackground = true };
                try
                {
                    if (_proxy.InnerChannel.State == CommunicationState.Faulted)
                        throw new EndpointNotFoundException();
                    thd.Start();
                }
                catch (EndpointNotFoundException)
                {
                    MessageBox.Show("There is no connection to service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }
        /// <summary>
        /// Load subtitles into listbox.
        /// </summary>
        /// <returns>FALSE - if fails.</returns>
        void LoadVideoSubs()
        {
            Thread thd = new Thread(new ThreadStart(() =>
                {
                    List<int> gaps = new List<int>();
                    if (gapsCount == wordsCount)
                    {
                        for (int i = 0; i < wordsCount; i++)
                        {
                            gaps.Add(i);
                        }
                    }
                    else
                        gaps = FillGaps(gapsCount);
                    int count = 0;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        foreach (SubTitle item in _subs)
                        {
                            WrapPanel tmp = new WrapPanel { Margin = new Thickness(2) };
                            
                            foreach (string word in item.Phrase.Split(" .,\"\\/=+-/)*^:;<>?!@$%({}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (gaps.Contains(count))
                                {
                                    switch (mode)
                                    {
                                        case GameMode.FillGaps:
                                            TextBox lb = new TextBox { Text = "", Style = TryFindResource("txtSub") as Style, Tag = word, IsReadOnly = false };
                                            lb.GotFocus += GapSubtitle_GotFocus;
                                            lb.LostFocus += GapSubtitle_LostFocus;
                                            lb.TextChanged += GapSubtitle_TextChanged;
                                            tmp.Children.Add(lb);
                                            break;
                                        case GameMode.MultipleChoice:
                                            ComboBox cmb = new ComboBox { Style = TryFindResource("cmbSub") as Style, Tag = word };
                                            List<string> cmbItems = FillComboBox(word);
                                            foreach (string val in cmbItems)
                                            {
                                                ComboBoxItem opt = new ComboBoxItem { Content = val, Foreground = Brushes.Black };
                                                opt.Selected += OptGame_Selected;
                                                cmb.Items.Add(opt);
                                            }
                                            tmp.Children.Add(cmb);
                                            break;
                                    }
                                }
                                else
                                    tmp.Children.Add(new TextBox { Text = word, Style = TryFindResource("txtSub") as Style });
                                count++;
                            }

                            tmp.Background = Brushes.White;
                            tmp.MouseEnter += Label_MouseEnter;
                            tmp.MouseLeave += Label_MouseLeave;
                            tmp.MouseDown += Label_MouseDown;

                            stWords.Items.Add(tmp);
                        }
                    }));
                }));
            thd.IsBackground = true;
            thd.Start();
        }
        /// <summary>
        /// Sets the number of words in the video.
        /// </summary>
        /// <returns>TRUE - if subtitles loaded.</returns>
        bool GetWordsNumber()
        {
            try
            {
                string subs = _proxy.GetItemProperty(videoId, ServerData.Video, PropertyData.SubPath);
                if (!Directory.Exists(@"Temp\Subtitles"))
                    Directory.CreateDirectory(@"Temp\Subtitles");
                byte[] res = _proxy.DownloadAsync(subs, FilesType.Subtitles).Result;
                subs = $@"Temp\Subtitles\{subs}";
                if (res != null)
                {
                    using (FileStream fs = File.OpenWrite(subs))
                    {
                        Task.WaitAll(fs.WriteAsync(res, 0, res.Length));
                        fs.Dispose();
                    }
                }
                if (!File.Exists(subs))
                    throw new ArgumentNullException("There are no subtitles for this video!");

                string str;
                using (FileStream fs = new FileStream(subs, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        str = sr.ReadToEndAsync().Result;
                    }
                }

                Thread thd = new Thread(new ThreadStart(() =>
                {
                    foreach (string item in str.Split(new string[] { "\n\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            SubTitle sub = new SubTitle { Num = Convert.ToInt32(item.Split('\n')[0]) };
                            string time = item.Split('\n')[1];
                            for (int i = 2; i < item.Split('\n').Length; i++)
                            {
                                sub.Phrase += item.Split('\n')[i];
                            }
                            sub.TimeStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(time.Split(':')[0]), Convert.ToInt32(time.Split(':')[1]), Convert.ToInt32(time.Split(':')[2].Split(',')[0]), Convert.ToInt32(time.Split(',')[1].Split(' ')[0]));
                            sub.TimeEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(time.Split(new string[] { "> " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(':')[0]), Convert.ToInt32(time.Split(new string[] { "> " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(':')[1]), Convert.ToInt32(time.Split(new string[] { "> " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(':')[2].Split(',')[0]), Convert.ToInt32(time.Split(new string[] { "> " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(':')[2].Split(',')[1]));
                            _subs.Add(sub);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    foreach (SubTitle item in _subs)
                    {
                        foreach (string word in item.Phrase.Split(" .,\"\\/=+-/)*^:;<>?!@$%({}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!_subWords.Contains(word))
                                _subWords.Add(word);
                            wordsCount++;
                        }
                    }
                }));
                thd.IsBackground = true;
                thd.Start();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        private void mainVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            slDurration.Maximum = mainVideo.NaturalDuration.TimeSpan.TotalSeconds;
            dsTime.Start();
        }
        #endregion

        #region Visualization.
        private void scvVideo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            stWords.MinWidth = scvVideo.ActualWidth - 50;
        }
        //Word enter.
        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.White;
        }
        //Word leave.
        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.Silver;
        }
        #endregion
        #region Timers.
        //Video playing.
        void TimerTick(object sender, EventArgs e)
        {
            if (_play)
            {
                slDurration.Value = mainVideo.Position.TotalSeconds;

                try
                {
                    foreach (Panel item in stWords.Items)
                    {
                        item.Opacity = 0.4;

                        foreach (FrameworkElement val in item.Children)
                        {
                            if (val is TextBox)
                                (val as TextBox).FontWeight = FontWeights.Normal;
                        }
                    }

                    SubTitle ind = _subs.Find(new Predicate<SubTitle>(i => i.TimeStart.TimeOfDay <= mainVideo.Position && i.TimeEnd.TimeOfDay >= mainVideo.Position));

                    if (ind != null)
                    {
                        Panel tmp = stWords.Items[ind.Num - 1] as Panel;

                        tmp.Opacity = 1;

                        foreach (FrameworkElement val in tmp.Children)
                        {
                            if (val is TextBox)
                                (val as TextBox).FontWeight = FontWeights.Bold;
                        }

                        stWords.SelectedItem = tmp;
                        if (_scroll)
                            stWords.ScrollIntoView(stWords.SelectedItem);
                    }

                    slDurration.ToolTip = (mainVideo.Position.Hours < 10 ? "0" : "") + $"{mainVideo.Position.Hours}:" + (mainVideo.Position.Minutes < 10 ? "0" : "") + $"{mainVideo.Position.Minutes}:" + (mainVideo.Position.Seconds < 10 ? "0" : "") + $"{mainVideo.Position.Seconds}/" + (mainVideo.NaturalDuration.TimeSpan.Hours < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Hours}:" + (mainVideo.NaturalDuration.TimeSpan.Minutes < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Minutes}:" + (mainVideo.NaturalDuration.TimeSpan.Seconds < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Seconds}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion
        #region Slider data.
        private void slDurration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mainVideo.Source != null)
                mainVideo.Position = TimeSpan.FromSeconds(slDurration.Value);
        }
        private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainVideo.Volume = slVolume.Value;
            imVol.Source = new BitmapImage(new Uri((slVolume.Value == 0) ? $"pack://application:,,,/Images/NoVol.png" : "pack://application:,,,/Images/Volume.png"));
            slVolume.ToolTip = $"{Convert.ToInt32(100 * slVolume.Value)}%";
        }
        #endregion
        #region Buttons, keys.
        //Choose mode.
        private void btnModeFill_Click(object sender, RoutedEventArgs e)
        {
            mode = GameMode.FillGaps;
            LoadDifficulty();
        }
        private void btnModeChoice_Click(object sender, RoutedEventArgs e)
        {
            mode = GameMode.MultipleChoice;
            LoadDifficulty();
        }
        void LoadDifficulty()
        {
            stOptions.Visibility = Visibility.Collapsed;
            stDiffculty.Visibility = Visibility.Visible;
            txtBeginner.Text = $"Fill {Math.Round((wordsCount * 25) / 100f)} of {wordsCount} words";
            txtIntermediate.Text = $"Fill {Math.Round((wordsCount * 50) / 100f)} of {wordsCount} words";
            txtAdvanced.Text = $"Fill {Math.Round((wordsCount * 75) / 100f)} of {wordsCount} words";
            txtExpert.Text = $"Fill {wordsCount} of {wordsCount} words";
        }
        //Choose difficulty.
        private void btnBeginner_Click(object sender, RoutedEventArgs e)
        {
            gapsCount = Convert.ToInt32(Math.Round((wordsCount * 25) / 100f));
            stDiffculty.Visibility = Visibility.Collapsed;
            stVideo.Visibility = Visibility.Visible;
        }
        private void btnIntermediate_Click(object sender, RoutedEventArgs e)
        {
            gapsCount = Convert.ToInt32(Math.Round((wordsCount * 50) / 100f));
            stDiffculty.Visibility = Visibility.Collapsed;
            stVideo.Visibility = Visibility.Visible;
        }
        private void btnAdvanced_Click(object sender, RoutedEventArgs e)
        {
            gapsCount = Convert.ToInt32(Math.Round((wordsCount * 75) / 100f));
            stDiffculty.Visibility = Visibility.Collapsed;
            stVideo.Visibility = Visibility.Visible;
        }
        private void btnExpert_Click(object sender, RoutedEventArgs e)
        {
            gapsCount = wordsCount;
            stDiffculty.Visibility = Visibility.Collapsed;
            stVideo.Visibility = Visibility.Visible;
        }
        //Move to a word.
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mainVideo.Source != null)
            {
                DateTime move = _subs.Find(new Predicate<SubTitle>(i => i.Num == Convert.ToInt32(stWords.Items.IndexOf(sender as Panel)) + 1)).TimeStart;
                mainVideo.Position = move.TimeOfDay;
            }
        }
        //Play a video (screen click).
        private void mainVideo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnPlay_Click(null, null);
        }
        //Play a video (button click).
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (mainVideo.Source != null)
            {
                if (_play)
                {
                    mainVideo.Pause();
                    lPlay.Content = ">";
                    _play = false;
                }
                else
                {
                    mainVideo.Play();
                    lPlay.Content = "||";
                    _play = true;
                }
            }
        }
        //Scroll the video.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int len = 2;

            switch (e.Key)
            {
                case Key.D:
                    if (slDurration.Value + len <= slDurration.Maximum)
                        slDurration.Value += len;
                    break;
                case Key.A:
                    if (slDurration.Value - len >= 0)
                        slDurration.Value -= len;
                    break;
            }
        }
        //Show results.
        private void btnEndGame_Click(object sender, RoutedEventArgs e)
        {
            if (gapsCount == 0)
            {
                EndVideo();
                stVideo.Visibility = Visibility.Collapsed;

                level++;
                _proxy.AddScore(_score, Convert.ToInt32(user), Convert.ToInt32(_game));
                _proxy.EditData(Convert.ToInt32(user), level.ToString(), ServerData.User, PropertyData.Level);
                Dictionary<int, int> lst = _proxy.GetHighScores(Convert.ToInt32(_game));
                int count = 1;
                foreach (int item in lst.Keys)
                {
                    StackPanel st = new StackPanel { Orientation = Orientation.Horizontal };
                    st.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Padding = new Thickness(5), Content = count });
                    string img = _proxy.GetItemProperty(item, ServerData.User, PropertyData.Imgpath) ?? "Wolf.png";
                    if (img == null)
                        return;
                    int size = 35;
                    if (img == "Wolf.png")
                        st.Children.Add(new Image { Source = new BitmapImage(new Uri($"pack://application:,,,/Images/Wolf.png")), Height = size, Width = size, Clip = new EllipseGeometry { RadiusX = size / 2, RadiusY = size / 2, Center = new Point(size / 2, size / 2) } });
                    else
                    {
                        if (!Directory.Exists($@"Temp\Avatars"))
                            Directory.CreateDirectory($@"Temp\Avatars");

                        byte[] res = _proxy.Download(img, FilesType.Avatars);
                        if (res != null)
                        {
                            try
                            {
                                using (FileStream fs = File.OpenWrite($@"Temp\Avatars\{img}"))
                                {
                                    Task.WaitAny(fs.WriteAsync(res, 0, res.Length));
                                    fs.Dispose();
                                }
                            }
                            catch (IOException) { }

                            Image tmp = new Image { Height = size, Width = size, Clip = new EllipseGeometry { RadiusX = size / 2, RadiusY = size / 2, Center = new Point(size / 2, size / 2) } };
                            FormData.SetImage($@"pack://siteoforigin:,,,/Temp\Avatars\{img}", tmp);
                            st.Children.Insert(0, tmp);
                        }
                    }

                    Label login = new Label { Style = TryFindResource("lbFormNormal") as Style, Margin = new Thickness(50, 5, 50, 5), Padding = new Thickness(5), Content = _proxy.GetItemProperty(Convert.ToInt32(item), ServerData.User, PropertyData.Login) };
                    if (item == user)
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
            else
            {
                _play = true;
                btnPlay_Click(null, null);
                if (MessageBox.Show("Are you sure you want to end this game?\nYour will loose your score.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    Close();
                else
                    btnPlay_Click(null, null);
            }
        }
        private void btnFinishGame_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        #region CheckBoxes.
        private void chScroll_Checked(object sender, RoutedEventArgs e)
        {
            _scroll = true;
        }
        private void chScroll_Unchecked(object sender, RoutedEventArgs e)
        {
            _scroll = false;
        }
        #endregion
        #region Gaps.
        /// <summary>
        /// Generates the indexes of empty boxes.
        /// </summary>
        /// <param name="capacity">The number of gaps.</param>
        /// <returns>Indexes of gaps.</returns>
        List<int> FillGaps(int capacity)
        {
            Random rnd = new Random();
            List<int> gaps = new List<int>();

            while (capacity > 0)
            {
                int num = rnd.Next(0, wordsCount);
                if (!gaps.Contains(num))
                {
                    gaps.Add(num);
                    capacity--;
                }
            }
            return gaps;
        }
        /// <summary>
        /// Generates items' captions for combo-box.
        /// </summary>
        /// <param name="word">The word to be added.</param>
        /// <returns>List of items.</returns>
        List<string> FillComboBox(string word)
        {
            Random rnd = new Random();
            List<string> str = new List<string>();
            List<int> indexes = new List<int>();
            int pos = rnd.Next(0, MAX_COMBOBOXITEMS_COUNT);
            int cap = 1;

            if (_subWords.Count > MAX_COMBOBOXITEMS_COUNT + 1)
            {
                indexes.Add(_subWords.IndexOf(word));
                while (cap < MAX_COMBOBOXITEMS_COUNT)
                {
                    int num = rnd.Next(0, _subWords.Count);
                    if (!indexes.Contains(num))
                    {
                        indexes.Add(num);
                        cap++;
                    }
                }
                int tmp = indexes[pos];
                indexes[pos] = indexes[0];
                indexes[0] = indexes[pos];
            }
            for (int i = 0; i < MAX_COMBOBOXITEMS_COUNT; i++)
            {
                if (i == pos)
                    str.Add(word.ToLower());
                else
                    str.Add((indexes.Count > 0) ? _subWords[rnd.Next(0, _subWords.Count)].ToLower() : _subWords[indexes[i]].ToLower());
            }
            return str;
        }
        //Visulization for 'Fill gaps' mode.
        private void GapSubtitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender as TextBox).IsReadOnly)
            {
                (sender as TextBox).Background = Brushes.WhiteSmoke;
                (sender as TextBox).BorderBrush = Brushes.CornflowerBlue;
                (sender as TextBox).Foreground = Brushes.Black;
                (sender as TextBox).BorderThickness = new Thickness(1);
            }
        }
        //Visulization for 'Fill gaps' mode.
        private void GapSubtitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text.ToLower() != (sender as TextBox).Tag.ToString().ToLower())
            {
                (sender as TextBox).Background = Brushes.Pink;
                (sender as TextBox).BorderBrush = Brushes.DarkRed;
                (sender as TextBox).Foreground = Brushes.DarkRed;
                (sender as TextBox).BorderThickness = new Thickness(2);
                SubtractScore();
            }
        }
        //Check the word.
        private void GapSubtitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.ToLower() == (sender as TextBox).Tag.ToString().ToLower())
            {
                (sender as TextBox).Background = Brushes.LightGreen;
                (sender as TextBox).BorderBrush = Brushes.ForestGreen;
                (sender as TextBox).Foreground = Brushes.ForestGreen;
                (sender as TextBox).FontWeight = FontWeights.Bold;
                (sender as TextBox).BorderThickness = new Thickness(2);
                (sender as TextBox).IsReadOnly = true;
                AddScore();
            }
        }
        //Choose option and check it.
        private void OptGame_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox parent = ((sender as ComboBoxItem).Parent as ComboBox);
            if (parent.Tag.ToString().ToLower() == (sender as ComboBoxItem).Content.ToString().ToLower())
            {
                parent.Background = Brushes.LightGreen;
                parent.BorderBrush = Brushes.ForestGreen;
                parent.Foreground = Brushes.ForestGreen;
                parent.FontWeight = FontWeights.Bold;
                parent.BorderThickness = new Thickness(2);
                parent.IsEnabled = false;
                AddScore();
            }
            else
            {
                parent.Background = Brushes.Pink;
                parent.BorderBrush = Brushes.DarkRed;
                parent.Foreground = Brushes.DarkRed;
                parent.BorderThickness = new Thickness(2);
                SubtractScore();
            }
        }
        //Confirm a mistake.
        void SubtractScore()
        {
            _fails++;
            txtFails.Text = _fails.ToString();
            int dif = level < MAX_UNCHANGED_LEVEL ? MAX_UNCHANGED_LEVEL : Convert.ToInt32(level / 2);
            _score -= dif;
            if (_score < 0)
                _score = 0;
            txtScore.Text = _score.ToString();
        }
        //Confirm a hit.
        void AddScore()
        {
            _hits++;
            txtHits.Text = _hits.ToString();
            gapsCount--;
            txtGaps.Text = gapsCount.ToString();
            int dif = level == 0 ? 1 : level;
            _score += dif;
            txtScore.Text = _score.ToString();
        }
        #endregion

        #region Dispose.
        void EndVideo()
        {
            mainVideo.Close();
            if (dsTime.IsEnabled)
                dsTime.Stop();
            stVideo.Visibility = Visibility.Collapsed;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (stVideo.Visibility == Visibility.Collapsed)
                EndVideo();
            else
            {
                _play = true;
                btnPlay_Click(null, null);
                if (MessageBox.Show("Are you sure you want to end this game?\nYour will loose your score.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EndVideo();
                else
                {
                    e.Cancel = true;
                    btnPlay_Click(null, null);
                }
            }
        }
        #endregion
    }
}