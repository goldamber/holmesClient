using AppEnglish.Classes;
using AppEnglish.EngServRef;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish.AddEdit
{
    public partial class GenerateSubs: Window
    {
        TimeSpan last = new TimeSpan();
        EngServiceClient _proxy;
        int videoId;
        int count = 1;

        #region Constructors.
        public GenerateSubs()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initialize 'videoId' and host.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="video">Videos id.</param>
        public GenerateSubs(EngServiceClient tmp, int video) : this()
        {
            _proxy = tmp;
            videoId = video;
        }
        #endregion

        #region Events.
        //Add new item.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddSubItem form;
            if (lstSubs.Items.Count == 0)
                form = new AddSubItem(_proxy, videoId);
            else
            {
                form = new AddSubItem(_proxy, videoId, last);
                btnOK.IsEnabled = true;
            }
            form.ShowDialog();

            if (GeneratedSub.Name != null)
            {
                string start = GeneratedSub.Start.ToString().Replace('.', ',');
                string end = GeneratedSub.End.ToString().Replace('.', ',');
                Grid grSub = new Grid();
                grSub.Children.Add(new TextBlock { Text = $"{count}\n{start.Substring(0, start.Length - 4)} --> {end.Substring(0, end.Length - 4)}\n{GeneratedSub.Name}\n\r\n" });
                grSub.MouseEnter += SubItem_MouseEnter;
                grSub.MouseLeave += SubItem_MouseLeave;

                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top, Visibility = Visibility.Collapsed };
                Button delete = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = "X", FontWeight = FontWeights.Bold, Margin = new Thickness(5), Width = 37, Foreground = Brushes.White, Height = 35, Background = Brushes.Red, ToolTip = "Remove" };
                delete.Click += Delete_Click;
                panel.Children.Add(delete);
                Button edit = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35,  Background = Brushes.Yellow, ToolTip = "Edit" };
                edit.Click += Edit_Click; ;
                panel.Children.Add(edit);

                grSub.Children.Add(panel);
                lstSubs.Items.Add(grSub);
                count++;
                last = GeneratedSub.End;
                GeneratedSub.Name = null;
                btnOK.IsEnabled = true;
            }
        }
        
        private void SubItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Grid).Background = Brushes.White;
            foreach (UIElement item in (sender as Grid).Children)
            {
                if (item is Panel)
                {
                    (item as Panel).Visibility = Visibility.Collapsed;
                    break;
                }
            }
        }
        private void SubItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Grid).Background = Brushes.Silver;
            foreach (UIElement item in (sender as Grid).Children)
            {
                if (item is Panel)
                {
                    (item as Panel).Visibility = Visibility.Visible;
                    break;
                }
            }
        }
        //Edit text.
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            TextBlock text = ((((sender as Button).Parent as Panel).Parent as Grid).Children[0] as TextBlock);
            string subData = text.Text.Split('\n')[0] + "\n" + text.Text.Split('\n')[1] + "\n";
            StringBuilder str = new StringBuilder();
            for (int i = 2; i < text.Text.Split('\n').Length; i++)
            {
                if (text.Text.Split('\n')[i] != "\r" && text.Text.Split('\n')[i] != "")
                {
                    str.Append(text.Text.Split('\n')[i]);
                }
            }

            EditSub form = new EditSub(str.ToString());
            form.ShowDialog();
            if (FormData.SubsText != null)
            {
                text.Text = subData + FormData.SubsText + "\n\r\n";
                FormData.SubsText = null;
            }
        }
        //Remove element from list.
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Grid parent = ((sender as Button).Parent as Panel).Parent as Grid;
            lstSubs.Items.Remove(parent);
            btnOK.IsEnabled = lstSubs.Items.Count != 0;

            count = 1;
            foreach (Grid item in lstSubs.Items)
            {
                foreach (UIElement val in item.Children)
                {
                    if (val is TextBlock)
                    {
                        StringBuilder str = new StringBuilder();
                        for (int i = 1; i < (val as TextBlock).Text.Split('\n').Length; i++)
                        {
                            if ((val as TextBlock).Text.Split('\n')[i] != "")
                                str.Append((val as TextBlock).Text.Split('\n')[i] + "\n");
                        }
                        (val as TextBlock).Text = count + "\n" + str;
                    }
                }
                count++;
            }
        }
        #endregion

        #region Close form (OK, Cancel).
        //Add a new video.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            foreach (Grid item in lstSubs.Items)
            {
                foreach (UIElement val in item.Children)
                {
                    if (val is TextBlock)
                        str.Append((val as TextBlock).Text);
                }
            }
            File.WriteAllText(@"Temp\File.srt", str.ToString());
            FormData.GeneratedSubsPath = $@"{Directory.GetCurrentDirectory()}\Temp\File.srt";
            FormData.EditVideos.Add(videoId);
            Close();
        }
        //Close form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}