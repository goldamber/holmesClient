﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppEnglish
{
    public partial class AddBook : Window
    {
        EngServRef.EngServiceClient _proxy;
        int? mark = null;
        int? user;
        #region Edit.
        int bookId;
        string name = null;
        string desc;
        int? year;
        string path;
        bool isAbsolute;
        List<int> categories;
        List<int> authors;
        string imgPath;
        #endregion

        #region Constructors.
        //Initialization.
        public AddBook()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes '_proxy', fills the listboxes.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="userId">Id of user.</param>
        public AddBook(EngServRef.EngServiceClient tmp, int? userId) : this()
        {
            _proxy = tmp;
            user = userId;

            FillAuthors();
            FillCategories();
        }
        /// <summary>
        /// 'Edit' form. Hides rating, fills fields.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="id">Books id.</param>
        /// <param name="name">Books name.</param>
        /// <param name="description">Books description.</param>
        /// <param name="year">Year (if given).</param>
        /// <param name="path">Books path.</param>
        /// <param name="isAbsolute">Is path absolute?</param>
        /// <param name="cat">Books categories.</param>
        /// <param name="auth">Books authors.</param>
        /// <param name="img">Poster.</param>
        public AddBook(EngServRef.EngServiceClient tmp, int id, string name, string description, int? year, string path, bool isAbsolute, List<int> cat, List<int> auth, string img) : this()
        {
            _proxy = tmp;

            bookId = id;
            txtName.Text = this.name = name;
            txtDesc.Text = desc = description;
            txtPath.Text = this.path = path;
            this.year = year;
            if (year != null)
                txtYear.Text = year.ToString();
            this.isAbsolute = isAbsolute;
            chCopy.IsChecked = !this.isAbsolute;
            imgPath = img;
            if (imgPath == "WolfB.png")
                FormData.SetImage("pack://application:,,,/Images/WolfB.png", imDrag);
            else
            {
                if (File.Exists($@"Temp\BookImages\{imgPath}"))
                    FormData.SetImage($@"pack://siteoforigin:,,,/Temp\BookImages\{imgPath}", imDrag);
                else
                    MessageBox.Show("Image can not be found!", "Something went wrong", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            lPath.Content = "...";
            categories = cat;
            authors = auth;

            stRating.Visibility = Visibility.Collapsed;
            FillAuthors(auth);
            FillCategories(cat);
        }
        #endregion

        #region Visualisation, validation.
        //Change the size of the inner fields.
        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in (sender as StackPanel).Children)
            {
                if (item is StackPanel)
                {
                    foreach (FrameworkElement val in (item as Panel).Children)
                    {
                        double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 40;
                        if (val is TextBox && len > 10)
                            val.Width = len;
                    }
                }
            }
        }
        //Check the title of an book.
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "" || (((sender as TextBox) == txtName) && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Book).Result && name == null) || (((sender as TextBox) == txtName) && name != null && txtName.Text != name && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Book).Result))
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = (sender as TextBox).Text == ""? "Empty strings are not allowed!":"This name is already taken!";
                btnOK.IsEnabled = false;
            }
            else
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtNormal") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormNormal") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = "Input data.";
                btnOK.IsEnabled = true;
            }
        }
        //Forbids the input of letters.
        private void txtYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion                
        #region Drag&drop.
        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            (sender as Border).Opacity = 1;
        }
        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            (sender as Border).Opacity = 0.4;
        }
        //Sets image.
        private void Border_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            lPath.Content = files[0];
            FormData.SetImage(lPath.Content.ToString(), imDrag);
            brImage.Opacity = 0.4;
        }
        //Choose image (via OpenFileDialog).
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.tif)|*.png;*.jpg;*.jpeg;*.gif;*.tif|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                lPath.Content = openFileDialog.FileName;
                FormData.SetImage(lPath.Content.ToString(), imDrag);
            }
        }
        #endregion
        #region Rating.
        //Sets a rate.
        private void imgRating_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mark = ((sender as Image).Opacity == 1)? 0: Convert.ToInt32((sender as Image).Tag);
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = mark;
                item.Opacity = (mark != 0 && Convert.ToInt32(item.Tag) <= Convert.ToInt32((sender as Image).Tag))? 1: 0.2;
            }
        }
        //Visualisation.
        private void imgRating_MouseEnter(object sender, MouseEventArgs e)
        {
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = (sender as Image).Tag;
                item.Opacity = (Convert.ToInt32(item.Tag) <= Convert.ToInt32((sender as Image).Tag)) ? 0.5 : 0.2;
            }
        }
        //Visualisation.
        private void imgRating_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = (mark == null)? "N/A": mark.ToString();
                item.Opacity = (mark != null && Convert.ToInt32(item.Tag) <= mark) ? 1 : 0.2;
            }
        }
        #endregion

        //Choose the location of a file.
        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt, *.pdf, *.epub, *.doc, *.docx)|*.txt;*.pdf;*.epub;*.doc;*.docx|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
                txtPath.Text = ofd.FileName;
        }
        //Show a form for adding a new author.
        private void btnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AddAuthor frm = new AddAuthor(_proxy);
            frm.ShowDialog();

            if (FormData.Author != "")
            {
                lstAuthors.Items.Add(new CheckBox { Content = FormData.Author, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = FormData.AuthorsID });
                FormData.Author = "";
            }
        }

        //Fill 'Authors' list-box.
        void FillAuthors()
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.Author).Result);
            foreach (int item in lst)
            {
                lstAuthors.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Surname).Result + ", " + _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left });
            }
        }
        //Fill 'Authors' list-box with default values.
        void FillAuthors(List<int> tmp)
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.Author).Result);
            foreach (int item in lst)
            {
                lstAuthors.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Surname).Result + ", " + _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = tmp.Contains(item) });
            }
        }
        //Fill 'Categories' list-box.
        void FillCategories()
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.BookCategory).Result);
            foreach (int item in lst)
            {
                lstCategory.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.BookCategory, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left });
            }
        }
        //Fill 'Categories' list-box with default values.
        void FillCategories(List<int> tmp)
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.BookCategory).Result);
            foreach (int item in lst)
            {
                lstCategory.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.BookCategory, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = tmp.Contains(item) });
            }
        }

        #region Close form (OK, Cancel).
        //Add a new book.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int? year = null;
            if (txtYear.Text != "")
                year = Convert.ToInt32(txtYear.Text);
            int edit = 0;

            if (txtPath.Text == "")
            {
                MessageBox.Show("Choose file!", "No file chosen", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() =>
                {
                    stMain.Visibility = Visibility.Collapsed;

                    if (name == null)
                    {
                        int? id = _proxy.AddBook(txtName.Text, txtDesc.Text == "" ? null : txtDesc.Text, txtPath.Text, "WolfB.png", chCopy.IsChecked == true ? false : true, mark, user, year, DateTime.Now);
                        if (id == null)
                        {
                            MessageBox.Show("Something went wrong.", "Operation denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                            stMain.Visibility = Visibility.Visible;
                            return;
                        }
                        edit = Convert.ToInt32(id);

                        if (lPath.Content.ToString() != "...")
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), $"{edit}{Path.GetExtension(lPath.Content.ToString())}", EngServRef.FilesType.BooksImages))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                _proxy.RemoveItem(edit, EngServRef.ServerData.Book);
                                stMain.Visibility = Visibility.Visible;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(lPath.Content.ToString())}", EngServRef.ServerData.Book, EngServRef.PropertyData.Imgpath);
                        }
                        if (chCopy.IsChecked == true)
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(txtPath.Text), $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.FilesType.Books))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                _proxy.RemoveItem(edit, EngServRef.ServerData.Book);
                                stMain.Visibility = Visibility.Visible;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.ServerData.Book, EngServRef.PropertyData.Path);
                        }
                    }
                    else
                    {
                        edit = bookId;

                        if (txtPath.Text != path && chCopy.IsChecked == true)
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(txtPath.Text), $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.FilesType.Books))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.ServerData.Book, EngServRef.PropertyData.Path);
                        }
                        else if (txtPath.Text == path && txtPath.Text.Contains(":") && chCopy.IsChecked == true)
                        {
                            if (!File.Exists(txtPath.Text))
                            {
                                MessageBox.Show("This file does not exist!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                return;
                            }
                            if (!_proxy.Upload(File.ReadAllBytes(txtPath.Text), $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.FilesType.Books))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(txtPath.Text)}", EngServRef.ServerData.Book, EngServRef.PropertyData.Path);
                        }
                        else if (chCopy.IsChecked == false)
                            _proxy.EditData(edit, txtPath.Text, EngServRef.ServerData.Book, EngServRef.PropertyData.Path);
                        if (lPath.Content.ToString() != "...")
                        {
                            FormData.EditBooks.Add(edit);
                            string file = $"{edit}{Path.GetExtension(lPath.Content.ToString())}";
                            if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), file, EngServRef.FilesType.BooksImages))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                stMain.Visibility = Visibility.Visible;
                                return;
                            }
                            _proxy.EditData(edit, file, EngServRef.ServerData.Book, EngServRef.PropertyData.Imgpath);
                        }
                        _proxy.EditData(edit, chCopy.IsChecked != true ? "True" : null, EngServRef.ServerData.Book, EngServRef.PropertyData.IsAbsolute);
                        _proxy.EditData(edit, txtYear.Text == "" ? null : txtYear.Text, EngServRef.ServerData.Book, EngServRef.PropertyData.Year);
                        _proxy.RemoveFullItemData(edit, EngServRef.ServerData.Author);
                        _proxy.RemoveFullItemData(edit, EngServRef.ServerData.BookCategory);
                        if (txtName.Text != name)
                            _proxy.EditData(edit, txtName.Text, EngServRef.ServerData.Book, EngServRef.PropertyData.Name);
                        if (txtDesc.Text != desc)
                            _proxy.EditData(edit, txtDesc.Text == "" ? null : txtDesc.Text, EngServRef.ServerData.Book, EngServRef.PropertyData.Description);
                    }

                    foreach (CheckBox item in lstCategory.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.BookCategory);
                    }
                    foreach (CheckBox item in lstAuthors.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddBookAuthorAsync(edit, Convert.ToInt32(item.Tag));
                    }
                    Close();
                }));
            }));
        }
        //Close form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}