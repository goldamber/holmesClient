using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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
        EngServRef.EngServiceClient _proxy;
        DispatcherTimer dsTime;
        Timer _time;
        List<SubTitle> _subs = new List<SubTitle>();

        bool _play = true;      //Play video.
        bool _sub = false;      //Show subs.
        bool _scroll = true;    //Scroll to subs.
        bool _justWords = false;    //Show only words(without video.).
        bool _isOver = false;       //Is a pointer over the screen.
        bool _fullScreen = false;   //Fullscreen mode.
        DateTime _start = DateTime.Now;
        //bool _prev = true;

        #region Constructors.
        public VideoPlayer()
        {
            InitializeComponent();

            dsTime = new DispatcherTimer();
            dsTime.Interval = TimeSpan.FromMilliseconds(700);
            dsTime.Tick += new EventHandler(TimerTick);
        }
        public VideoPlayer(string name, bool words, EngServRef.EngServiceClient tmp) : this()
        {
            _proxy = tmp;
            _justWords = words;
            txtName.Text = name;
        }
        #endregion
        #region Initialization.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*Thread thd = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        LoadVideo();

                        if (CheckVideoPath(tmp) && !_justWords)
                        {
                            mainVideo.Source = new Uri(!tmp.IsAbsolulute ? $"pack://siteoforigin:,,,/Videos/{tmp.Path}" : tmp.Path);
                            mainVideo.Volume = slVolume.Value;
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

                tmp = _proxy.GetVideo(_videoName);
                if (tmp == null)
                    throw new Exception("Video can not be found!");
                txtName.Text = tmp.Name;
                thd.Start();
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("There is no connection to service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }*/
        }
        private void mainVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            slDurration.Maximum = mainVideo.NaturalDuration.TimeSpan.TotalSeconds;
            dsTime.Start();
        }

        void LoadVideo()
        {
            /*try
            {
                string str;
                if (!Directory.Exists("Subtitles") || !File.Exists($@"Subtitles\{tmp.SubPath}.srt"))
                    throw new ArgumentNullException("There are no subtitles for video!");

                using (FileStream fs = new FileStream($@"Subtitles\{tmp.SubPath}.srt", FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        str = sr.ReadToEnd();
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
                                lb.MouseRightButtonDown += Lb_MouseRightButtonDown;
                                //lb.MouseEnter += Lb_MouseEnter;
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
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/
        }
        /*bool CheckVideoPath(EngServRef.Video tmp)
        {
            if (tmp.Path.StartsWith("http") || tmp.Path.StartsWith("https"))
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(tmp.Path) as HttpWebRequest;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        return (response.StatusCode == HttpStatusCode.OK);
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
                return File.Exists(!tmp.IsAbsolulute ? $"Videos/{tmp.Path}" : tmp.Path);

        }*/
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
                mainVideo.Position = TimeSpan.FromSeconds(slDurration.Value);
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
                    //_scroll = _prev;

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
        //'Add word' click.
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord("", _proxy);
            _play = true;
            btnPlay_Click(null, null);
            frm.ShowDialog();
            btnPlay_Click(null, null);
        }
        //Add an existing word.
        private void Lb_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWord frm = new AddWord((sender as Label).Content.ToString(), _proxy);
            _play = true;
            btnPlay_Click(null, null);
            frm.ShowDialog();
            btnPlay_Click(null, null);
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