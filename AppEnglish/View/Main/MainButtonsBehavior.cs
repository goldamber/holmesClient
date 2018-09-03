using AppEnglish.AddEdit;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    //Clicks actions.
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
                Dispatcher.Invoke(() => {
                    WrapPanel tmp = new WrapPanel();
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Content = "Login:" });
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontSize = 14, Content = lUserName.Content });
                    Button btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 7 }, Width = 32, Height = 30, VerticalAlignment = VerticalAlignment.Top, Background = Brushes.Yellow, Tag = _proxy.GetUserId(lUserName.Content.ToString()), ToolTip = "Edit" };
                    btn.Click += btnEditUsername_Click;
                    tmp.Children.Add(btn);
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel();
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Content = "Role:" });
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontSize = 14, Content = lRole.Content });
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel { ToolTip = "Your level depends on the quantity of played games and the score of each game." };
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Content = "Level:" });
                    tmp.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontSize = 14, Content = _proxy.GetItemProperty(Convert.ToInt32(_proxy.GetUserId(lUserName.Content.ToString())), EngServRef.ServerData.User, EngServRef.PropertyData.Level) });
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel();
                    btn = new Button { Style = TryFindResource("btnNormal") as Style, Content = "Change password", Margin = new Thickness(5), Tag = _proxy.GetUserId(lUserName.Content.ToString()) };
                    btn.Click += btnEditPassword_Click;
                    tmp.Children.Add(btn);
                    stActions.Children.Add(tmp);

                    tmp = new WrapPanel();
                    btn = new Button { Style = TryFindResource("btnNormal") as Style, Content = "Change avatar", Margin = new Thickness(5), Tag = _proxy.GetUserId(lUserName.Content.ToString()) };
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
        //Show a list of all users to the admin.
        private async void btnUsersAct_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.User);
            await Task.Run(() => LoadList(lst, DataType.User));
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
            string path = _proxy.GetItemPropertyAsync(id, EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath).Result ?? "Wolf.png";
            imUserAvatar.Source = new BitmapImage(new Uri(path != "Wolf.png" ? $"pack://siteoforigin:,,,/{path}" : "pack://application:,,,/Images/Wolf.png"));
        }
        //Show a form for editting the role.
        private void btnEditRole_Click(object sender, RoutedEventArgs e)
        {
            int usId = Convert.ToInt32((sender as Button).Tag);
            EditRole form = new EditRole(_proxy, usId);
            form.ShowDialog();

            if (usId == _proxy.GetUserId(lUserName.Content.ToString()))
                lRole.Content = _proxy.GetItemProperty(usId, EngServRef.ServerData.User, EngServRef.PropertyData.RolesName);
            if (lRole.Content.ToString() != "admin")
                ButtonBack_Click(null, null);
            else
                btnUsersAct_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this user?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.User);
                btnUsersAct_Click(null, null);
            }
        }
        #endregion
        #region Video actions.
        //Show a list of all videos to the user.
        private async void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Video);
            await Task.Run(() => LoadList(lst, DataType.Video));
        }
        //Show a form for adding a new video.
        private void btnAddVideo(object sender, RoutedEventArgs e)
        {
            btnVideos_Click(null, null);
        }
        //Show a video player.
        private void btnViewVideo_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer frm = new VideoPlayer((sender as Button).Tag.ToString(), false, _proxy);
            frm.ShowDialog();
        }
        //Show a form for editting the video.
        private void btnEditVideo_Click(object sender, RoutedEventArgs e)
        {
            btnVideos_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveVideo_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this video?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Video);
                btnVideos_Click(null, null);
            }
        }
        #endregion
        #region Book actions.
        //Show a list of all books to the user.
        private async void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Book);
            await Task.Run(() => LoadList(lst, DataType.Book));
        }
        //Show a form for adding a new book.
        private void btnAddBook(object sender, RoutedEventArgs e)
        {
            AddBook frm = new AddBook(_proxy);
            frm.ShowDialog();
            btnBooks_Click(null, null);
        }
        //Show a book reader.
        private void btnViewBook_Click(object sender, RoutedEventArgs e)
        {
            BookReader frm = new BookReader((sender as Button).Tag.ToString(), _proxy);
            frm.ShowDialog();
        }
        //Show a form for editting the book.
        private void btnEditBook_Click(object sender, RoutedEventArgs e)
        {
            btnBooks_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this book?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Book);
                btnBooks_Click(null, null);
            }
        }
        #endregion
        #region Word actions.
        //Show a list of all words to the user.
        private async void btnWords_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });

            int[] lst = await _proxy.GetItemsAsync(EngServRef.ServerData.Word);
            await Task.Run(() => LoadList(lst, DataType.Word));
        }
        //Show a form for adding a new word.
        private void btnAddWord(object sender, RoutedEventArgs e)
        {
            btnWords_Click(null, null);
        }
        //Show a form for editting the word.
        private void btnEditWord_Click(object sender, RoutedEventArgs e)
        {
            btnWords_Click(null, null);
        }
        //Remove item and refresh the canvas.
        private void btnRemoveWord_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this word?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Word);
                btnWords_Click(null, null);
            }
        }
        //Remove a word from specific user.
        private void btnRemoveFromUser_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this word?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _proxy.RemoveItemWordAsync(Convert.ToInt32(lUserName.Tag), Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.User);
                btnWords_Click(null, null);
            }
        }

        //Generate a file to be printed.
        private void btnPrintWords_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamWriter sw = File.CreateText("Print.html"))
                {
                    sw.WriteLine($"<h2 style=\"color: #506DE9\">{(((((e.Source as Button).Parent as Panel).Parent as Expander).Parent as Panel).Parent as Expander).Header}</h2>");
                    sw.WriteLine("<h3 style=\"color: #6E7BB2\">Words:</h3>");

                    StringBuilder str = new StringBuilder();
                    sw.WriteLine("<ol>");
                    foreach (var item in ((e.Source as Button).Parent as Panel).Children)
                    {
                        if (item is Expander)
                        {
                            str = new StringBuilder();
                            string word = (item as Expander).Header.ToString();
                            str.Append($"<li><dt><b>{word[0].ToString().ToUpper() + word.Substring(1)}</b> - ");
                            foreach (var val in ((item as Expander).Content as Panel).Children)
                            {
                                if (val is Expander)
                                {
                                    if ((val as Expander).Header.ToString().Contains("Categories"))
                                    {
                                        foreach (var i in ((val as Expander).Content as Panel).Children)
                                        {
                                            if (i is Panel)
                                                str.Append(((i as Panel).Children[1] as Label).Content + ", ");
                                        }
                                    }
                                    if ((val as Expander).Header.ToString().Contains("Translation"))
                                    {
                                        str.Append("<i style=\"color: #BBC2E0\">");
                                        foreach (var i in ((val as Expander).Content as Panel).Children)
                                        {
                                            if (i is Panel)
                                                str.Append(((i as Panel).Children[1] as Label).Content + ", ");
                                        }
                                        str.Append("</i>;");
                                    }
                                    if ((val as Expander).Header.ToString().Contains("Definition"))
                                    {
                                        foreach (var i in ((val as Expander).Content as Panel).Children)
                                        {
                                            if (i is Panel)
                                                str.Append(((i as Panel).Children[1] as Label).Content + ", ");
                                        }
                                        str.Append("</dt>");
                                    }
                                    if ((val as Expander).Header.ToString().Contains("Example"))
                                    {
                                        foreach (var i in ((val as Expander).Content as Panel).Children)
                                        {
                                            if (i is Panel)
                                                str.Append($"<dd><i>{((i as Panel).Children[1] as Label).Content }</i></dd>");
                                        }
                                    }
                                }
                            }
                            str.Append("</li>");
                            sw.WriteLine(str);
                        }
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

        //Filter data.
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "")
                _proxy.GetItems((EngServRef.ServerData)Enum.Parse(typeof(EngServRef.ServerData), btnSearch.Tag.ToString()));

            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });
            int[] lst;

            switch (btnSearch.Tag)
            {
                case "Book":
                    lst = await _proxy.GetFItemsAsync(txtSearch.Text, EngServRef.ServerData.Book, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text));
                    await Task.Run(() => LoadList(lst, DataType.Book));
                    break;

                case "Video":
                    lst = await _proxy.GetFItemsAsync(txtSearch.Text, EngServRef.ServerData.Video, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text));
                    await Task.Run(() => LoadList(lst, DataType.Video));
                    break;

                case "Word":
                    lst = await _proxy.GetFItemsAsync(txtSearch.Text, EngServRef.ServerData.Word, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text));
                    await Task.Run(() => LoadList(lst, DataType.Word));
                    break;

                case "User":
                    lst = await _proxy.GetFItemsAsync(txtSearch.Text, EngServRef.ServerData.User, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text));
                    await Task.Run(() => LoadList(lst, DataType.User));
                    break;
            }
        }
        //Sort data.
        private async void btnSort_Click(object sender, RoutedEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });
            int[] lst;

            switch (btnSort.Tag)
            {
                case "Book":
                    lst = await _proxy.GetSortedItemsAsync(EngServRef.ServerData.Book, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text), _desc);
                    await Task.Run(() => LoadList(lst, DataType.Book));
                    break;

                case "Video":
                    lst = await _proxy.GetSortedItemsAsync(EngServRef.ServerData.Video, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text), _desc);
                    await Task.Run(() => LoadList(lst, DataType.Video));
                    break;

                case "Word":
                    lst = await _proxy.GetSortedItemsAsync(EngServRef.ServerData.Word, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text), _desc);
                    await Task.Run(() => LoadList(lst, DataType.Word));
                    break;

                case "User":
                    lst = await _proxy.GetSortedItemsAsync(EngServRef.ServerData.User, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text), _desc);
                    await Task.Run(() => LoadList(lst, DataType.User));
                    break;
            }

            _desc = !_desc;
            string pic = _desc ? "SortR" : "Sort"; 
            (sender as Button).Content = new Image { Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{pic}.png")), Margin = new Thickness(5) };
        }
        //Filter data via link.
        private async void ItemData_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            stActions.Children.Clear();
            stActions.Children.Add(new ProgressBar { Template = TryFindResource("Preloader") as ControlTemplate });
            int[] lst;

            switch (btnSearch.Tag)
            {
                case "Book":
                    lst = await _proxy.GetFItemsAsync((sender as TextBlock).Text, EngServRef.ServerData.Book, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), (sender as TextBlock).Tag.ToString()));
                    await Task.Run(() => LoadList(lst, DataType.Book));
                    break;

                case "Video":
                    lst = await _proxy.GetFItemsAsync((sender as TextBlock).Text, EngServRef.ServerData.Video, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), (sender as TextBlock).Tag.ToString()));
                    await Task.Run(() => LoadList(lst, DataType.Video));
                    break;

                case "Word":
                    lst = await _proxy.GetFItemsAsync((sender as TextBlock).Text, EngServRef.ServerData.Word, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), (sender as TextBlock).Tag.ToString()));
                    await Task.Run(() => LoadList(lst, DataType.Word));
                    break;

                case "User":
                    lst = await _proxy.GetFItemsAsync((sender as TextBlock).Text, EngServRef.ServerData.User, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), (sender as TextBlock).Tag.ToString()));
                    await Task.Run(() => LoadList(lst, DataType.User));
                    break;
            }
        }
        //Return to the list of actions.
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            grSearch.Visibility = Visibility.Collapsed;
            stActions.Children.Clear();

            Button btn = new Button { Name = "btnBooks", Content = "Books", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnBooks_Click;
            stActions.Children.Add(btn);

            /*btn = new Button { Name = "btnVideos", Content = "Videos", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnVideos_Click;
            stActions.Children.Add(btn);

            btn = new Button { Name = "btnWords", Content = "Dictionary", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnWords_Click;
            stActions.Children.Add(btn);*/

            if (lRole.Content.ToString() == "admin")
            {
                btn = new Button { Name = "btnUsersAct", Content = "Users", Style = TryFindResource("btnNormal") as Style };
                btn.Click += btnUsersAct_Click;
                stActions.Children.Add(btn);
                /*btn = new Button { Name = "btnVideoCategories", Content = "Video Categories", Style = TryFindResource("btnNormal") as Style };
                stActions.Children.Add(btn);

                btn = new Button { Name = "btnBookCategories", Content = "Book Categories", Style = TryFindResource("btnNormal") as Style };
                stActions.Children.Add(btn);

                btn = new Button { Name = "btnUsers", Content = "Users", Style = TryFindResource("btnNormal") as Style };
                stActions.Children.Add(btn);*/
            }
        }
    }
}