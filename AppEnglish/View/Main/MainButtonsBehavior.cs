using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish
{
    //Clicks actions.
    public partial class MainWindow : MetroWindow
    {
        #region Video actions.
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
            _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Video);
            btnVideos_Click(null, null);
        }
        #endregion
        #region Book actions.
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
            _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Book);
            btnBooks_Click(null, null);
        }
        #endregion
        #region Word actions.
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
            _proxy.RemoveItemAsync(Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.Word);
            btnWords_Click(null, null);
        }
        //Remove a word from specific user.
        private void btnRemoveFromUser_Click(object sender, RoutedEventArgs e)
        {
            _proxy.RemoveItemWordAsync(Convert.ToInt32(lUserName.Tag), Convert.ToInt32((sender as Button).Tag), EngServRef.ServerData.User);
            btnWords_Click(null, null);
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

        //Filter the data.
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "")
                return;

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

                case "Game":
                    lst = await _proxy.GetFItemsAsync(txtSearch.Text, EngServRef.ServerData.Game, (EngServRef.PropertyData)Enum.Parse(typeof(EngServRef.PropertyData), cmbFilter.Text));
                    await Task.Run(() => LoadList(lst, DataType.Game));
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

            btn = new Button { Name = "btnVideos", Content = "Videos", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnVideos_Click;
            stActions.Children.Add(btn);

            btn = new Button { Name = "btnWords", Content = "Dictionary", Style = TryFindResource("btnNormal") as Style };
            btn.Click += btnWords_Click;
            stActions.Children.Add(btn);

            if (lRole.Content.ToString() == "admin")
            {
                btn = new Button { Name = "btnVideoCategories", Content = "Video Categories", Style = TryFindResource("btnNormal") as Style };
                stActions.Children.Add(btn);

                btn = new Button { Name = "btnBookCategories", Content = "Book Categories", Style = TryFindResource("btnNormal") as Style };
                stActions.Children.Add(btn);

                btn = new Button { Name = "btnUsers", Content = "Users", Style = TryFindResource("btnNormal") as Style };
                stActions.Children.Add(btn);
            }
        }
    }
}