using AppEnglish.EngServRef;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Insters extra data to an item (categories, words, ...).
        /// </summary>
        /// <param name="header">The title of expander.</param>
        /// <param name="item">Id of item to be decorated.</param>
        /// <param name="st">A panel where the data are supposed to be added.</param>
        /// <param name="data">A type of item.</param>
        /// <param name="res">A type of the inserted data.</param>
        void AddExpanderData(string header, int item, Panel st, ServerData data, ServerData res)
        {
            if (_proxy.GetItemDataAsync(item, data, res).Result == null || _proxy.GetItemDataAsync(item, data, res).Result.Length == 0)
                return;

            Expander hor = new Expander { Header = header, Background = Brushes.Azure };
            StackPanel ver = new StackPanel { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            int count = 1;
            foreach (int val in _proxy.GetItemDataAsync(item, data, res).Result)
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Background = Brushes.Azure };
                panel.MouseEnter += ExpanderItem_MouseEnter;
                panel.MouseLeave += ExpanderItem_MouseLeave;

                panel.Children.Add(new Border { CornerRadius = new CornerRadius(22, 22, 20, 20), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(2), Child = new Label { Content = count, Padding = new Thickness(2), FontSize = 9, FontWeight = FontWeights.Bold }, Padding = new Thickness(7, 5, 7, 5), Margin = new Thickness(5) });
                TextBlock label = new TextBlock { Padding = new Thickness(5), Foreground = Brushes.DarkBlue, TextDecorations = TextDecorations.Underline, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Tag = header, Text = _proxy.GetItemPropertyAsync(val, res, PropertyData.Name).Result, FontSize = 12, FontWeight = FontWeights.Normal };
                if (res == ServerData.Author)
                    label.Text = _proxy.GetItemPropertyAsync(val, res, PropertyData.Name).Result + " " + _proxy.GetItemPropertyAsync(val, res, PropertyData.Surname).Result;
                label.MouseDown += ItemData_MouseDown;
                label.MouseEnter += ItemData_MouseEnter;
                label.MouseLeave += ItemData_MouseLeave;
                panel.Children.Add(label);
                ver.Children.Add(panel);
                count++;
            }
            hor.Content = ver;
            st.Children.Add(hor);
        }
        /// <summary>
        /// Describes static one-line property.
        /// </summary>
        /// <param name="content">The data to insert.</param>
        /// <param name="item">Id of the item to which this property belongs.</param>
        /// <param name="st">The panel where to insert.</param>
        /// <param name="dataType">The type of an item.</param>
        /// <param name="property">The type of property.</param>
        void AddStaticContent(int item, Panel st, ServerData dataType, PropertyData property)
        {
            if (_proxy.GetItemPropertyAsync(item, dataType, property).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = $"{property.ToString()}:", FontSize = 14, FontWeight = FontWeights.Bold });
                hor.Children.Add(new Label { Content = _proxy.GetItemPropertyAsync(item, dataType, property).Result });
                st.Children.Add(hor);
            }
        }
        /// <summary>
        /// Inserts the data for sorting elements.
        /// </summary>
        /// <param name="item">Id of item to be decorated.</param>
        /// <param name="dataType">A type of item.</param>
        /// <param name="property">A type of the inserted data.</param>
        /// <param name="st">A panel where the data are supposed to be added.</param>
        void AddHoverableData(int item, ServerData dataType, PropertyData property, Panel st)
        {
            if (_proxy.GetItemPropertyAsync(item, dataType, property).Result != null)
            {
                StackPanel hor = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                hor.Children.Add(new Label { Content = $"{property.ToString()}:", FontSize = 14, FontWeight = FontWeights.Bold });

                TextBlock label = new TextBlock { Padding = new Thickness(5), Foreground = Brushes.DarkBlue, TextDecorations = TextDecorations.Underline, Tag = property.ToString(), Text = _proxy.GetItemPropertyAsync(item, dataType, property).Result };
                label.MouseDown += ItemData_MouseDown;
                label.MouseEnter += ItemData_MouseEnter;
                label.MouseLeave += ItemData_MouseLeave;
                hor.Children.Add(label);
                st.Children.Add(hor);
            }
        }
        /// <summary>
        /// Inserts default actions.
        /// </summary>
        /// <param name="item">Id of item.</param>
        /// <param name="st">A panel where the buttons are supposed to be inserted.</param>
        /// <param name="delete">'Delete' action.</param>
        /// <param name="edit">'Edit' action.</param>
        /// <param name="view">'View' action.</param>
        void AddButtons(int item, Panel st, RoutedEventHandler delete, RoutedEventHandler edit, RoutedEventHandler view)
        {
            StackPanel stButtons = new StackPanel { Orientation = Orientation.Horizontal };
            Button btn;
            if (delete != null && lRole.Content.ToString().Equals("admin"))
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Red, Tag = item, ToolTip = "Delete" };
                btn.Click += delete;
                stButtons.Children.Add(btn);
            }
            if (edit != null && lRole.Content.ToString().Equals("admin"))
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Background = Brushes.Yellow, Tag = item, ToolTip = "Edit" };
                btn.Click += edit;
                stButtons.Children.Add(btn);
            }
            if (view != null)
            {
                btn = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/View.png")), Height = 15 }, Margin = new Thickness(5), Width = 37, Height = 35, HorizontalAlignment = HorizontalAlignment.Left, Tag = item, ToolTip = "View" };
                btn.Click += btnViewBook_Click;
                stButtons.Children.Add(btn);
            }
            st.Children.Add(stButtons);
        }
        /// <summary>
        /// Downloads an image from server and presents it.
        /// </summary>
        /// <param name="id">Images id.</param>
        /// <param name="defaultPath">Default path.</param>
        /// <param name="tempPath">The location of temporary files.</param>
        /// <param name="parent">The panel where an image is supposed to be added.</param>
        /// <param name="type">Data type.</param>
        void AddImage(int id, string defaultPath, string tempPath, Panel parent, ServerData type, bool edit)
        {
            string img = _proxy.GetItemPropertyAsync(id, type, PropertyData.Imgpath).Result ?? defaultPath;
            if (img == null)
                return;

            if (img == defaultPath)
                parent.Children.Add(new Image { Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{defaultPath}")), Height = 110 });
            else
            {
                Task.Run(new Action(() => {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (!Directory.Exists($@"Temp\{tempPath}"))
                            Directory.CreateDirectory($@"Temp\{tempPath}");

                        FilesType filesType = FilesType.Avatar;
                        switch (type)
                        {
                            case ServerData.Video:
                                filesType = FilesType.VideoImage;
                                break;
                            case ServerData.Book:
                                filesType = FilesType.BookImage;
                                break;
                            case ServerData.User:
                                filesType = FilesType.Avatar;
                                break;
                            case ServerData.Word:
                                filesType = FilesType.WordImage;
                                break;
                        }
                        byte[] res = _proxy.Download(img, filesType);
                        if (res != null)
                        {
                            try
                            {
                                using (FileStream fs = File.OpenWrite($@"Temp\{tempPath}\{img}"))
                                {
                                    fs.Write(res, 0, res.Length);
                                    fs.Dispose();
                                }
                            }
                            catch (IOException)
                            {
                                if (edit)
                                {
                                    if (MessageBox.Show("The data have been uploaded to the server. It will be updated the next time you come.\nDo you want to restart now?", "Check next time", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                        Close();
                                }
                            }
                            parent.Children.Insert(0, new Image { Source = new BitmapImage(new Uri($@"pack://siteoforigin:,,,/Temp\{tempPath}\{img}")), Height = 110 });
                        }
                    }));
                }));
            }
        }
        /// <summary>
        /// Downloads file.
        /// </summary>
        /// <param name="id">Files id.</param>
        /// <param name="type">Path of temporary location.</param>
        /// <param name="type">Files type.</param>
        /// <param name="edit">Does file need to be edited?</param>
        void AddFile(int id, string tempPath, ServerData type, bool edit)
        {
            string path = _proxy.GetItemPropertyAsync(id, type, PropertyData.Path).Result;
            if (path == null)
                return;

            Task.Run(new Action(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!Directory.Exists($@"Temp\{tempPath}"))
                        Directory.CreateDirectory($@"Temp\{tempPath}");

                    FilesType filesType = FilesType.Avatar;
                    switch (type)
                    {
                        case ServerData.Video:
                            filesType = FilesType.Video;
                            break;
                        case ServerData.Book:
                            filesType = FilesType.Book;
                            break;
                    }
                    byte[] res = _proxy.Download(path, filesType);
                    if (res != null)
                    {
                        try
                        {
                            using (FileStream fs = File.OpenWrite($@"Temp\{tempPath}\{path}"))
                            {
                                fs.Write(res, 0, res.Length);
                                fs.Dispose();
                            }
                        }
                        catch (IOException)
                        {
                            if (edit)
                            {
                                if (MessageBox.Show("The data have been uploaded to the server. It will be updated the next time you come.\nDo you want to restart now?", "Check next time", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    Close();
                            }
                        }
                    }
                }));
            }));
        }
        /// <summary>
        /// Inserts rating.
        /// </summary>
        /// <param name="id">Id of item.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="parent">The panel, where items are supposed to be added.</param>
        /// <param name="type">Type of item.</param>
        void AddMarkingStars(int id, int? userId, Panel parent, ServerData type)
        {
            int? stars = null;
            switch (type)
            {
                case EngServRef.ServerData.Video:
                    stars = _proxy.GetMark(id, Convert.ToInt32(userId), EngServRef.ServerData.Video);
                    break;
                case EngServRef.ServerData.Book:
                    stars = _proxy.GetMark(id, Convert.ToInt32(userId), EngServRef.ServerData.Book);
                    break;
            }

            int mark = stars == null ? 0 : Convert.ToInt32(stars);
            StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal, ToolTip = mark == 0 ? "N/G" : mark.ToString() };
            stack.Children.Add(new Label { Content = $"Rating:", FontSize = 14, FontWeight = FontWeights.Bold });
            for (int i = 0; i < 5; i++)
            {
                stack.Children.Add(new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Rating.png")), Height = 40, Margin = new Thickness(2), Opacity = i < mark ? 1 : 0.2 });
            }
            Button edit = new Button { Style = TryFindResource("MetroCircleButtonStyle") as Style, Content = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/Edit.png")), Height = 7 }, Width = 32, Height = 30, VerticalAlignment = VerticalAlignment.Top, Background = Brushes.Yellow, Tag = $"{type.ToString()}:{id}:{userId}:{mark}", ToolTip = "Edit" };
            edit.Click += btnEditRating_Click;
            stack.Children.Add(edit);
            parent.Children.Add(stack);
        }

        //Customize expanders label (hover).
        private void ItemData_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = TextDecorations.Underline;
            (sender as TextBlock).Foreground = Brushes.DarkBlue;
        }
        //Customize expanders label (hover).
        private void ItemData_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = null;
            (sender as TextBlock).Foreground = Brushes.DeepSkyBlue;
        }

        //Customize expander (hover).
        private void ExpanderItem_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.Azure;
        }
        //Customize expander (hover).
        private void ExpanderItem_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Panel).Background = Brushes.LightBlue;
        }
    }
}