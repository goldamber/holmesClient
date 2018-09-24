using AppEnglish.AddEdit;
using AppEnglish.EngServRef;
using AppEnglish.View.Games;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    //Clicks actions (user).
    public partial class MainWindow : MetroWindow
    {
        System.Windows.Forms.WebBrowser web = new System.Windows.Forms.WebBrowser();
        bool _desc = false;

        #region Users actions.
        //View user profile.
        private void lProfile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });
            Task.Run(() => {
                Dispatcher.InvokeAsync(() => {
                    WrapPanel tmp = new WrapPanel();
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Content = "Login:" });
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontSize = 14, Content = lUserName.Content });
                    Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 7 }, Width = 32, Height = 30, VerticalAlignment = VerticalAlignment.Top, Background = Brushes.Yellow, Tag = _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User), ToolTip = "Edit" };
                    btn.Click += btnEditUsername_Click;
                    tmp.Children.Add(btn);
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel();
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Content = "Role:" });
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontSize = 14, Content = lRole.Content });
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel { ToolTip = "Your level depends on the quantity of played games and the score of each game." };
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Content = "Level:" });
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontSize = 14, Content = _proxy.GetItemProperty(Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User)), ServerData.User, PropertyData.Level) });
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel();
                    btn = new Button { Style = TryFindResource("btnNormal") as Style, Content = "Change password", Margin = new Thickness(5), Tag = _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User) };
                    btn.Click += btnEditPassword_Click;
                    tmp.Children.Add(btn);
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel();
                    btn = new Button { Style = TryFindResource("btnNormal") as Style, Content = "Change avatar", Margin = new Thickness(5), Tag = _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User) };
                    btn.Click += btnEditAvatar_Click;
                    tmp.Children.Add(btn);
                    stActions.Children.Add(tmp);

                    Button ret = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Width = 50, Height = 50, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(20, 0, 20, 0) };
                    ret.Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/ArrowBack.png")), Height = 35 };
                    ret.Click += ButtonBack_Click;
                    stActions.Children.Add(ret);

                    foreach (var item in stActions.Children)
                    {
                        if (item is ProgressBar)
                        {
                            stActions.Children.Remove(item as UIElement);
                            break;
                        }
                    }
                });
            });
        }
        
        //Show a form for login editting.
        private void btnEditUsername_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            EditUsername form = new EditUsername(_proxy, id);
            form.ShowDialog();
            lUserName.Content = (_proxy.GetItemProperty(id, EngServRef.ServerData.User, EngServRef.PropertyData.Name)).ToUpper();
            (((sender as Button).Parent as Panel).Children[1] as Label).Content = lUserName.Content;
        }
        //Show a form for password editting.
        private void btnEditPassword_Click(object sender, RoutedEventArgs e)
        {
            EditPassword form = new EditPassword(_proxy, Convert.ToInt32((sender as Button).Tag));
            form.ShowDialog();
        }
        //Show a form for avatar editting.
        private void btnEditAvatar_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            EditAvatar form = new EditAvatar(_proxy, id);
            form.ShowDialog();
            SetAvatar(id, true);
        }

        //Edit rating of book or video.
        private void btnEditRating_Click(object sender, RoutedEventArgs e)
        {
            string[] keys = (sender as Button).Tag.ToString().Split(':');
            EngServRef.ServerData type = keys[0] =="Book"? EngServRef.ServerData.Book : EngServRef.ServerData.Video;
            EditRating book = new EditRating(_proxy, Convert.ToInt32(keys[1]), Convert.ToInt32(keys[2]), Convert.ToInt32(keys[3]), type);
            book.ShowDialog();

            switch (type)
            {
                case EngServRef.ServerData.Video:
                    btnVideos_Click(null, null);
                    break;
                case EngServRef.ServerData.Book:
                    btnBooks_Click(null, null);
                    break;
            }
        }
        #endregion
        #region Video actions.
        //Show a list of all videos to the user.
        private void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.Video, DataType.Video);
        }
        //Show a form for adding a new video.
        private void btnAddVideo(object sender, RoutedEventArgs e)
        {
            AddVideo form = new AddVideo(_proxy, _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            form.ShowDialog();
            btnVideos_Click(null, null);
        }
        //Show a video player.
        private void btnViewVideo_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer frm = new VideoPlayer(_proxy, Convert.ToInt32((sender as Button).Tag), _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            frm.ShowDialog();
            btnVideos_Click(null, null);
        }

        //Add subtitles.
        private void BtnAddSubs_Click(object sender, RoutedEventArgs e)
        {
            int video = Convert.ToInt32((sender as Button).Tag);
            GenerateSubs form = new GenerateSubs(_proxy, video);
            form.ShowDialog();

            if (FormData.GeneratedSubsPath != null)
            {
                if (!_proxy.Upload(File.ReadAllBytes(FormData.GeneratedSubsPath), $"{video}{Path.GetExtension(FormData.GeneratedSubsPath)}", FilesType.Subtitles))
                {
                    MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                _proxy.EditData(video, $"{video}{Path.GetExtension(FormData.GeneratedSubsPath)}", ServerData.Video, PropertyData.SubPath);
                FormData.GeneratedSubsPath = null;
                btnVideos_Click(null, null);
            }
        }
        #endregion
        #region Book actions.
        //Show a list of all books to the user.
        private void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.Book, DataType.Book);
        }
        //Show a form for adding a new book.
        private void btnAddBook(object sender, RoutedEventArgs e)
        {
            AddBook frm = new AddBook(_proxy, _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            frm.ShowDialog();
            btnBooks_Click(null, null);
        }
        //Show a book reader.
        private void btnViewBook_Click(object sender, RoutedEventArgs e)
        {
            int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            BookReader frm = new BookReader(_proxy, Convert.ToInt32((sender as Button).Tag), user);
            frm.ShowDialog();
            btnBooks_Click(null, null);
        }
        #endregion
        #region Word actions.
        //Show a list of all words to the user.
        private void btnWords_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.Word, DataType.Word);
        }
        //Show a form for adding a new word.
        private void btnAddWord(object sender, RoutedEventArgs e)
        {
            int? user = _proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User);
            AddWord form = new AddWord(_proxy, user);
            form.ShowDialog();
            btnWords_Click(null, null);
        }
        //Remove a word from the list of pecific user.
        private void btnRemoveUsersItemsWord_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            string type = (((((sender as Button).Parent as Panel).Parent as Expander).Parent as Panel).Parent as Expander).Header.ToString();
            if (MessageBox.Show("Are you sure you want to remove this word from your list?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemWord(user, id, ServerData.User);
                if (type.StartsWith("Book"))
                    btnBooks_Click(null, null);
                else if (type.StartsWith("Video"))
                    btnVideos_Click(null, null);
            }
        }

        //Generate a file to be printed.
        private void btnPrintWords_Click(object sender, RoutedEventArgs e)
        {
            string[] data = (sender as Button).Tag.ToString().Split(':');
            ServerData type = (ServerData)Enum.Parse(typeof(ServerData), data[0]);
            int parent = Convert.ToInt32(data[1]);
            int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            WordsPrintFilter form = new WordsPrintFilter(_proxy, user, parent, type);
            form.ShowDialog();
            if (FormData.WordsToPrint.Count == 0)
                return;
            try
            {
                using (StreamWriter sw = File.CreateText("Print.html"))
                {
                    sw.WriteLine($"<h2 style=\"color: #506DE9\">{_proxy.GetItemProperty(parent, type, PropertyData.Name)}</h2>");
                    sw.WriteLine("<h3 style=\"color: #6E7BB2\">Words:</h3>");

                    StringBuilder str = new StringBuilder();
                    sw.WriteLine("<ol>");
                    foreach (int item in FormData.WordsToPrint)
                    {
                        str.Append($"<li><dt><b>{_proxy.GetItemProperty(item, ServerData.Word, PropertyData.Name)}</b> - ");
                        int trans = Convert.ToInt32(_proxy.GetItemProperty(item, ServerData.Word, PropertyData.Transcription));
                        if (trans != 0)
                            str.Append($"/{_proxy.GetItemProperty(trans, ServerData.Transcription, PropertyData.British)}/ ");
                        List<int> categories = new List<int>(_proxy.GetItemData(item, ServerData.Word, ServerData.WordCategory));
                        if (categories != null && categories.Count > 0)
                        {
                            foreach (int cat in categories)
                            {
                                str.Append($"<i style=\"color: #3082c9\">{_proxy.GetItemProperty(cat, ServerData.WordCategory, PropertyData.Abbreviation)};</i> ");
                            }
                        }
                        List<int> translations = new List<int>(_proxy.GetItemData(item, ServerData.Word, ServerData.Translation));
                        if (translations != null && translations.Count > 0)
                        {
                            foreach (int tr in translations)
                            {
                                str.Append($"<i style=\"color: #8c99a4\">{_proxy.GetItemProperty(tr, ServerData.Translation, PropertyData.Name)},</i> ");
                            }
                        }
                        string plural = _proxy.GetItemProperty(item, ServerData.Word, PropertyData.PluralForm);
                        if (plural != null)
                            str.Append($"<br/><dd><i style=\"color: #5e6871\"><u>Plural:</u></i> {plural}</dd>");
                        string past = _proxy.GetItemProperty(item, ServerData.Word, PropertyData.PastForm);
                        if (past != null)
                            str.Append($"<br/><dd><i style=\"color: #5e6871\"><u>Past form:</u></i> {past}</dd>");
                        string pastTh = _proxy.GetItemProperty(item, ServerData.Word, PropertyData.PastThForm);
                        if (pastTh != null)
                            str.Append($"<br/><dd><i style=\"color: #5e6871\"><u>Past participle:</u></i> {pastTh}</dd>");
                        List<int> defintions = new List<int>(_proxy.GetItemData(item, ServerData.Word, ServerData.Definition));
                        if (defintions != null && defintions.Count > 0)
                        {
                            str.Append("<br/><i style=\"color: #5e6871\"><u>Definitions:</u></i>");
                            foreach (int def in defintions)
                            {
                                str.Append($"<dd>{_proxy.GetItemProperty(def, ServerData.Definition, PropertyData.Name)}</dd>");
                            }
                            str.Append("</dt>");
                        }
                        List<int> examples = new List<int>(_proxy.GetItemData(item, ServerData.Word, ServerData.Example));
                        if (examples != null && examples.Count > 0)
                        {
                            str.Append("<br/><i style=\"color: #5e6871\"><u>Examples:</u></i>");
                            foreach (int exp in examples)
                            {
                                str.Append($"<dd>{_proxy.GetItemProperty(exp, ServerData.Example, PropertyData.Name)}</dd>");
                            }
                            str.Append("</dt>");
                        }
                        str.Append("</dt></li><hr/>");
                        sw.WriteLine(str);
                    }
                    sw.WriteLine("</ol>");

                    MessageBox.Show("The document is ready!", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            web = new System.Windows.Forms.WebBrowser();
            web.DocumentCompleted += Print_DocumentCompleted;
            web.DocumentText = File.ReadAllText("Print.html");
        }
        //Print the document.
        private void Print_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            web.Print();
        }
        #endregion
        #region Games actions.
        //Show converter.
        private void BtnTimeConverter_Click(object sender, RoutedEventArgs e)
        {
            ///int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            TimeConverter game = new TimeConverter();
            game.ShowDialog();
        }
        //Play lyric game.
        private void PlayLyricGame_Click(object sender, RoutedEventArgs e)
        {
            int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            GamePlayer form = new GamePlayer(_proxy, Convert.ToInt32((sender as Button).Tag), user);
            form.ShowDialog();
        }
        //Play words game.
        private void PlayWordsCatGame_Click(object sender, RoutedEventArgs e)
        {
            int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            WordsGamePlayer form = new WordsGamePlayer(_proxy, user, Convert.ToInt32((sender as Button).Tag), ServerData.WordCategory);
            form.ShowDialog();
        }
        private void PlayWordsGroupGame_Click(object sender, RoutedEventArgs e)
        {
            int user = Convert.ToInt32(_proxy.GetItemsId(lUserName.Content.ToString(), ServerData.User));
            WordsGamePlayer form = new WordsGamePlayer(_proxy, user, Convert.ToInt32((sender as Button).Tag), ServerData.Group);
            form.ShowDialog();
        }
        #endregion
        #region Grammar actions.
        //Show a list of all rules to the user.
        private void btnGrammar_Click(object sender, RoutedEventArgs e)
        {
            GenerateListTemplate(ServerData.Grammar, DataType.Grammar);
        }
        //Show a form for adding a new rule.
        private void btnAddGrammar_Click(object sender, RoutedEventArgs e)
        {
            AddGrammar frm = new AddGrammar(_proxy);
            frm.ShowDialog();
            btnGrammar_Click(null, null);
        }
        #endregion

        //Filter data.
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "")
                _proxy.GetItems((ServerData)Enum.Parse(typeof(ServerData), btnSearch.Tag.ToString()));
            FormData.FilterPosition = cmbFilter.SelectedIndex;
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });
            
            await Task.Run(new Action(() => {
                Dispatcher.InvokeAsync(new Action(() => {
                    int[] lst = _proxy.GetFItems(txtSearch.Text, (ServerData)Enum.Parse(typeof(ServerData), btnSearch.Tag.ToString()), (PropertyData)Enum.Parse(typeof(PropertyData), cmbFilter.Text));
                    LoadList(lst, (DataType)Enum.Parse(typeof(DataType), btnSearch.Tag.ToString()), false);
                }));
            }));
        }
        //Sort data.
        private async void btnSort_Click(object sender, RoutedEventArgs e)
        {
            FormData.SortPosition = cmbSort.SelectedIndex;
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            await Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() => {
                    int[] lst = _proxy.GetSortedItems((ServerData)Enum.Parse(typeof(ServerData), btnSearch.Tag.ToString()), (PropertyData)Enum.Parse(typeof(PropertyData), cmbSort.Text), _desc);
                    LoadList(lst, (DataType)Enum.Parse(typeof(DataType), btnSearch.Tag.ToString()), false);

                    _desc = !_desc;
                    string pic = _desc ? "SortR" : "Sort";
                    Image tmp = new Image { Margin = new Thickness(5) };
                    FormData.SetImage($"pack://application:,,,/Images/{pic}.png", tmp);
                    (sender as Button).Content = tmp;
                }));
            }));
        }
        //Filter data via link.
        private async void ItemData_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            await Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() => {
                    int[] lst = _proxy.GetFItems((sender as TextBlock).Text, (ServerData)Enum.Parse(typeof(ServerData), btnSearch.Tag.ToString()), (PropertyData)Enum.Parse(typeof(PropertyData), (sender as TextBlock).Tag.ToString()));
                    LoadList(lst, (DataType)Enum.Parse(typeof(DataType), btnSearch.Tag.ToString()), false);
                }));
            }));
        }
        //Return to the list of actions.
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            FormData.FilterPosition = 0;
            FormData.SortPosition = 0;
            txtSearch.Text = "";
            grSearch.Visibility = Visibility.Collapsed;
            stActions.Children.Clear();

            Button btn = new Button { Name = "btnBooks", Content = "Books", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnBooks_Click;
            stActions.Children.Add(btn);
            btn = new Button { Name = "btnWords", Content = "Dictionary", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnWords_Click;
            stActions.Children.Add(btn);
            btn = new Button { Name = "btnVideos", Content = "Videos", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnVideos_Click;
            stActions.Children.Add(btn);
            btn = new Button { Name = "btnGrammar", Content = "Grammar", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnGrammar_Click;
            stActions.Children.Add(btn);
            btn = new Button { Name = "btnWordsCategoriesAct", Content = "Words Categories", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnWordsCategoriesAct_Click;
            stActions.Children.Add(btn);
            btn = new Button { Name = "btnWordsGroupsAct", Content = "Words Groups", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnWordsGroupsAct_Click;
            stActions.Children.Add(btn);
            btn = new Button { Name = "btnGames", Content = "Time Converter", Style = TryFindResource("btnNormal") as Style };
            btn.Click += BtnTimeConverter_Click;
            stActions.Children.Add(btn);

            if (lRole.Content.ToString() == "admin")
            {
                btn = new Button { Name = "btnUsersAct", Content = "Users", Style = TryFindResource("btnNormal") as Style };
                btn.Click += btnUsersAct_Click;
                stActions.Children.Add(btn);
                btn = new Button { Name = "btnAuthorsAct", Content = "Authors", Style = TryFindResource("btnNormal") as Style };
                btn.Click += btnAuthorsAct_Click;
                stActions.Children.Add(btn);
                btn = new Button { Name = "btnBooksCategoriesAct", Content = "Books Categories", Style = TryFindResource("btnNormal") as Style };
                btn.Click += btnBooksCategoriesAct_Click;
                stActions.Children.Add(btn);
                btn = new Button { Name = "btnVideoCategoriesAct", Content = "Video Categories", Style = TryFindResource("btnNormal") as Style };
                btn.Click += btnVideoCategoriesAct_Click;
                stActions.Children.Add(btn);
            }
        }
    }
}