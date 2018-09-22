using AppEnglish.AddEdit;
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

namespace AppEnglish
{
    public partial class VideoPlayer : Window
    {
        EngServiceClient _proxy;
        DispatcherTimer dsTime;
        Timer _time;
        List<SubTitle> _subs = new List<SubTitle>();
        int? user;              //Users id.
        int videoId;            //VideosId.
        bool _play = true;      //Play video.
        bool _sub = false;      //Show subs.
        bool _scroll = true;    //Scroll to subs.
        bool _justWords = false;    //Show only words (without video).
        bool _isOver = false;       //Is a pointer over the screen.
        bool _fullScreen = false;   //Fullscreen mode.
        DateTime _start = DateTime.Now;
        bool _prev = true;          //Previous postion of scroll.

        #region Constructors.
        //Initialize video.
        public VideoPlayer()
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
        /// <param name="video">Videos id</param>
        /// <param name="user">Users id</param>
        /// <param name="words">Show only words (without video).</param>
        public VideoPlayer(EngServiceClient tmp, int video, int? user, bool words) : this()
        {
            _proxy = tmp;
            _justWords = words;
            videoId = video;
            this.user = user;
            txtName.Text = _proxy.GetItemProperty(video, ServerData.Video, PropertyData.Name);
        }
        #endregion
        #region Initialization.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadVideo())
            {
                Thread thd = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            if (!_justWords)
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

                                mainVideo.Source = new Uri(isAbsolute ? path : $"pack://siteoforigin:,,,/{path}");
                                mainVideo.Volume = slVolume.Value;
                                int? vm = _proxy.GetLastMarkAsync(videoId, Convert.ToInt32(user), ServerData.Video).Result;
                                if (vm != null)
                                {
                                    mainVideo.Position = TimeSpan.Parse(_proxy.GetItemProperty(Convert.ToInt32(vm), ServerData.VideoBookmark, PropertyData.Position));
                                    slDurration.Value = mainVideo.Position.TotalSeconds;
                                }
                                mainVideo.Play();
                            }
                            else
                            {
                                mainVideo.Source = null;
                                grControl.Visibility = Visibility.Collapsed;
                                mainVideo.Visibility = Visibility.Collapsed;
                                chScroll.Visibility = Visibility.Collapsed;
                                exSettings.Visibility = Visibility.Collapsed;
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));
                thd.IsBackground = true;
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
            else
                Close();
        }
        /// <summary>
        /// Load subtitles into listbox.
        /// </summary>
        /// <returns>FALSE - if fails.</returns>
        bool LoadVideo()
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

                    Dispatcher.Invoke(new Action(() =>
                    {
                        foreach (SubTitle item in _subs)
                        {
                            WrapPanel tmp = new WrapPanel { Margin = new Thickness(2) };

                            foreach (string word in item.Phrase.Split(" .,\"\\/=+-/)*^:;<>?!@$%({}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            {
                                Label lb = new Label { Content = word, Style = TryFindResource("lbSub") as Style };
                                lb.PreviewMouseDown += Lb_PreviewMouseDown;
                                lb.MouseEnter += Lb_MouseEnter;
                                tmp.Children.Add(lb);
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

        //Show word data.
        private void Lb_MouseEnter(object sender, MouseEventArgs e)
        {
            string data = (sender as Label).Content.ToString();
            try
            {
                data = (sender as Label).Content.ToString().Split(" 1234567890.,!?()-_<>;:'\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
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
                (sender as Label).ToolTip = st;
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

        //Player actions.
        private void mainVideo_MouseEnter(object sender, MouseEventArgs e)
        {
            _isOver = true;
        }
        private void mainVideo_MouseLeave(object sender, MouseEventArgs e)
        {
            _isOver = false;
        }
        private void mainVideo_MouseMove(object sender, MouseEventArgs e)
        {
            if (_fullScreen)
            {
                if (_time != null)
                    _time.Dispose();
                Mouse.OverrideCursor = Cursors.Arrow;
                grControl.Visibility = Visibility.Visible;
                _time = new Timer(TimerWait, null, 0, 1000);
                _start = DateTime.Now;
            }
        }

        //Subtitle size tooltip.
        private void slSubSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slSubSize.ToolTip = $"{Convert.ToInt32(slSubSize.Value)}pt";
            lSub.FontSize = slSubSize.Value;
        }
        //Subtitle opacity tooltip.
        private void slSubOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slSubOpacity.ToolTip = Math.Round(slSubOpacity.Value, 2);
            lSub.Opacity = slSubOpacity.Value;
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

                        foreach (Label val in item.Children)
                        {
                            val.FontWeight = FontWeights.Normal;
                        }
                    }

                    SubTitle ind = _subs.Find(new Predicate<SubTitle>(i => i.TimeStart.TimeOfDay <= mainVideo.Position && i.TimeEnd.TimeOfDay >= mainVideo.Position));

                    if (ind != null)
                    {
                        Panel tmp = stWords.Items[ind.Num - 1] as Panel;

                        tmp.Opacity = 1;

                        foreach (Label val in tmp.Children)
                        {
                            val.FontWeight = FontWeights.Bold;
                        }

                        stWords.SelectedItem = tmp;
                        if (_scroll)
                            stWords.ScrollIntoView(stWords.SelectedItem);
                    }

                    if (_sub)
                    {
                        lSub.Text = ind != null ? ind.Phrase : "";
                        lSub.Visibility = ind != null && _fullScreen ? Visibility.Visible : Visibility.Collapsed;
                    }

                    slDurration.ToolTip = (mainVideo.Position.Hours < 10 ? "0" : "") + $"{mainVideo.Position.Hours}:" + (mainVideo.Position.Minutes < 10 ? "0" : "") + $"{mainVideo.Position.Minutes}:" + (mainVideo.Position.Seconds < 10 ? "0" : "") + $"{mainVideo.Position.Seconds}/" + (mainVideo.NaturalDuration.TimeSpan.Hours < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Hours}:" + (mainVideo.NaturalDuration.TimeSpan.Minutes < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Minutes}:" + (mainVideo.NaturalDuration.TimeSpan.Seconds < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Seconds}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        //Hide controls.
        void TimerWait(object state)
        {
            if ((DateTime.Now - _start).Seconds >= 3)
            {
                Dispatcher.Invoke(new Action(() => {
                    grControl.Visibility = Visibility.Hidden;
                    Mouse.OverrideCursor = Cursors.None;
                }));
            }
        }
        #endregion

        #region Slider data.
        private void slDurration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mainVideo.Source != null)
            {
                mainVideo.Position = TimeSpan.FromSeconds(slDurration.Value);
                _proxy.AddVideoBookmark(TimeSpan.FromSeconds(slDurration.Value), videoId, Convert.ToInt32(user));
            }
        }
        private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainVideo.Volume = slVolume.Value;
            imVol.Source = new BitmapImage(new Uri((slVolume.Value == 0) ? $"pack://application:,,,/Images/NoVol.png" : "pack://application:,,,/Images/Volume.png"));
            slVolume.ToolTip = $"{Convert.ToInt32(100 * slVolume.Value)}%";
        }
        #endregion
        #region Clicks.
        //Maximize/minimize player.
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_isOver)
            {
                if (_fullScreen)
                {
                    foreach (var item in stVideo.Children)
                    {
                        if (item is Panel)
                            (item as Panel).Visibility = Visibility.Visible;
                    }
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;

                    lSub.Visibility = Visibility.Collapsed;
                    _time.Dispose();
                    grControl.Visibility = Visibility.Visible;
                    Mouse.OverrideCursor = Cursors.Arrow;
                    scvVideo.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    Background = Brushes.White;
                    _fullScreen = false;
                    _scroll = _prev;

                    grMedia.MinHeight = 400;
                    grMedia.Height = 400;
                    grMedia.Width = stVideo.ActualWidth;
                }
                else
                {
                    foreach (var item in stVideo.Children)
                    {
                        if (item is Panel && item != grControl && item != grMedia)
                            (item as Panel).Visibility = Visibility.Collapsed;
                    }
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;

                    if (_sub)
                        lSub.Visibility = Visibility.Visible;

                    scvVideo.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Background = Brushes.Black;
                    //_prev = _scroll;
                    _scroll = false;
                    _fullScreen = true;

                    grMedia.MinHeight = stVideo.ActualHeight - grControl.ActualHeight;
                    grMedia.Width = stVideo.ActualWidth;
                }
            }
        }
        //Add an existing word.
        private void Lb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string data = (sender as Label).Content.ToString();
            try
            {
                data = (sender as Label).Content.ToString().Split(" 1234567890.,!?()-_<>';:\"\\/=+-/^@$%{}|&\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            }
            catch
            {
                return;
            }
            int? word = _proxy.GetWord(data);
            if (word == null)
            {
                AddWord form = new AddWord(_proxy, data, user, videoId, ServerData.Video);
                _play = true;
                btnPlay_Click(null, null);
                form.ShowDialog();
                btnPlay_Click(null, null);
            }
            else
            {
                _play = true;
                btnPlay_Click(null, null);
                if (MessageBox.Show("Do you want to add this word to your list?", "Save word", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _proxy.AddItemsWord(Convert.ToInt32(word), videoId, ServerData.Video);
                    _proxy.AddItemsWord(Convert.ToInt32(word), Convert.ToInt32(user), ServerData.User);
                }
                btnPlay_Click(null, null);
            }
           
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

                stWords.IsEnabled = _play;
            }
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
        private void chShowSubs_Checked(object sender, RoutedEventArgs e)
        {
            _sub = true;
        }
        private void chShowSubs_Unchecked(object sender, RoutedEventArgs e)
        {
            _sub = false;
        }
        #endregion

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
                case Key.Escape:
                    if (_fullScreen)
                        Window_MouseDoubleClick(null, null);
                    break;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            mainVideo.Close();
            if (dsTime.IsEnabled)
                dsTime.Stop();
            if (_time != null)
                _time.Dispose();
        }
    }
}