using AppEnglish.Classes;
using AppEnglish.EngServRef;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
                lstSubs.Items.Add(new TextBlock { Text = $"{count}\n{start.Substring(0, start.Length - 4)} --> {end.Substring(0, end.Length - 4)}\n{GeneratedSub.Name}\r\n\r\n" });
                count++;
                last = GeneratedSub.End;
                GeneratedSub.Name = null;
                btnOK.IsEnabled = true;
            }
        }
        #region Close form (OK, Cancel).
        //Add a new video.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            foreach (TextBlock item in lstSubs.Items)
            {
                str.Append(item.Text);
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