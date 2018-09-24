using AppEnglish.AddEdit;
using AppEnglish.EngServRef;
using System;
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
        #region Clicks, keys.
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
                case Key.Escape:
                    if (_fullScreen)
                        Window_MouseDoubleClick(null, null);
                    break;
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
    }
}