using AppEnglish.Classes;
using AppEnglish.EngServRef;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AppEnglish.AddEdit
{
    public partial class AddSubItem : Window
    {
        EngServiceClient _proxy;
        DispatcherTimer dsTime;
        Slider slider;
        TimeSpan? start = null;
        bool _play = true;
        int videoId;

        #region Constructors.
        public AddSubItem()
        {
            InitializeComponent();

            dsTime = new DispatcherTimer();
            dsTime.Interval = TimeSpan.FromMilliseconds(700);
            dsTime.Tick += new EventHandler(TimerTick);
            slider = slStart;
        }
        /// <summary>
        /// Initialize 'videoId' and host.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="video">Videos id.</param>
        public AddSubItem(EngServiceClient tmp, int video) : this()
        {
            _proxy = tmp;
            videoId = video;
            GeneratedSub.Name = null;
        }
        /// <summary>
        /// Set default value.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="video">Videos id.</param>
        /// <param name="pos">Posituion.</param>
        public AddSubItem(EngServiceClient tmp, int video, TimeSpan pos) : this(tmp, video)
        {
            start = pos;
        }
        #endregion
        #region Initialization.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

                        mainVideo.Source = new Uri(isAbsolute ? path : $"pack://siteoforigin:,,,/{path}");
                        mainVideo.Volume = slVolume.Value;
                        if (start != null)
                        {
                            mainVideo.Position = (TimeSpan)start;
                            slStart.Value = mainVideo.Position.TotalSeconds;
                        }
                        mainVideo.Play();
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }))
            {
                IsBackground = true
            };
            thd.Start();
        }
        private void mainVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            slStart.Maximum = mainVideo.NaturalDuration.TimeSpan.TotalSeconds;
            slEnd.Maximum = mainVideo.NaturalDuration.TimeSpan.TotalSeconds;
            dsTime.Start();
        }
        #endregion

        #region Timers.
        //Video playing.
        void TimerTick(object sender, EventArgs e)
        {
            if (_play)
            {
                slider.Value = mainVideo.Position.TotalSeconds;
                slider.ToolTip = (mainVideo.Position.Hours < 10 ? "0" : "") + $"{mainVideo.Position.Hours}:" + (mainVideo.Position.Minutes < 10 ? "0" : "") + $"{mainVideo.Position.Minutes}:" + (mainVideo.Position.Seconds < 10 ? "0" : "") + $"{mainVideo.Position.Seconds}/" + (mainVideo.NaturalDuration.TimeSpan.Hours < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Hours}:" + (mainVideo.NaturalDuration.TimeSpan.Minutes < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Minutes}:" + (mainVideo.NaturalDuration.TimeSpan.Seconds < 10 ? "0" : "") + $"{mainVideo.NaturalDuration.TimeSpan.Seconds}";
            }
        }
        #endregion
        #region Buttons.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            gbTitle.Visibility = Visibility.Collapsed;
            btnOK.IsEnabled = true;
        }
        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            if (TimeSpan.FromSeconds(slider.Value) > GeneratedSub.Start)
            {
                GeneratedSub.End = TimeSpan.FromSeconds(slEnd.Value);
                gbEnd.Visibility = Visibility.Collapsed;
                gbTitle.Visibility = Visibility.Visible;
            }
            else
                MessageBox.Show("This value is supposed to be bigger than previous one.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Hand);
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            gbEnd.Visibility = Visibility.Visible;
            GeneratedSub.Start = TimeSpan.FromSeconds(slStart.Value);
            gbStart.Visibility = Visibility.Collapsed;
            slider = slEnd;
        }
        #endregion
        #region Sliders.
        private void slDurration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mainVideo.Source != null)
                mainVideo.Position = TimeSpan.FromSeconds(slider.Value);
        }
        private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mainVideo.Volume = slVolume.Value;
            imVol.Source = new BitmapImage(new Uri((slVolume.Value == 0) ? $"pack://application:,,,/Images/NoVol.png" : "pack://application:,,,/Images/Volume.png"));
            slVolume.ToolTip = $"{Convert.ToInt32(100 * slVolume.Value)}%";
        }
        #endregion
        #region Clicks.
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
        #endregion
        #region Close form (OK, Cancel).
        //Add a new video.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            GeneratedSub.Name = txtName.Text;
            Close();
        }
        //Close form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            mainVideo.Close();
            if (dsTime.IsEnabled)
                dsTime.Stop();
        }
    }
}