using AppEnglish.EngServRef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish.View.Games
{
    public partial class GamePlayer : Window
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
                if (MessageBox.Show("Are you sure you want to end this game?\nYour will lose your score.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
    }
}