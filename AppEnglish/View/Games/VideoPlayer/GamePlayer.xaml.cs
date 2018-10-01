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
using System.Windows.Threading;

namespace AppEnglish.View.Games
{
    enum GameMode { FillGaps, MultipleChoice };

    public partial class GamePlayer : Window
    {
        #region Variables.
        EngServiceClient _proxy;        //Host.
        DispatcherTimer dsTime;
        List<SubTitle> _subs = new List<SubTitle>();    //List of subtitles.
        List<string> _subWords = new List<string>();    //All words.
        int? user;              //Users id.
        int? _game = null;      //Id of game.
        int videoId;            //VideosId.
        bool _play = true;      //Play video.
        bool _scroll = true;    //Scroll to subs.
        int wordsCount = 0;     //The number of words for this video.
        int gapsCount = 0;      //The number of gaps for this game.
        int _hits = 0;          //The number of valid words.
        int _fails = 0;         //The number of mistakes.
        int _score = 0;         //Current score.
        bool _focus = false;    //Is textboxes focused?
        int level;              //Users level.
        GameMode mode = GameMode.FillGaps;
        const int MAX_UNCHANGED_LEVEL = 3;     //If users level is greater than this value, then his score will be counted depending on his level.
        const int MAX_COMBOBOXITEMS_COUNT = 4;
        #endregion

        #region Initialization.
        #region Constructors.
        public GamePlayer()
        {
            InitializeComponent();
            
            dsTime = new DispatcherTimer();
            dsTime.Interval = TimeSpan.FromMilliseconds(700);
            dsTime.Tick += new EventHandler(TimerTick);
        }
        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="video">Id of video to be payed.</param>
        /// <param name="user">Id of player.</param>
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
                _focus = true;
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
                _focus = false;
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
                if (MessageBox.Show("Are you sure you want to end this game?\nYour will lose your score.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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