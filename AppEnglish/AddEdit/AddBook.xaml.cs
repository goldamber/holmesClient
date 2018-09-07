using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    public partial class AddBook : Window
    {
        EngServRef.EngServiceClient _proxy;
        int? mark = null;

        #region Constructors.
        //Initialization.
        public AddBook()
        {
            InitializeComponent();
        }
        //Initialize '_proxy', fill the listboxes.
        public AddBook(EngServRef.EngServiceClient tmp) : this()
        {
            _proxy = tmp;

            FillAuthors();
            FillCategories();
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
            if ((sender as TextBox).Text == "" || ((sender as TextBox) == txtName) && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Book).Result)
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
            imDrag.Source = new BitmapImage(new Uri(lPath.Content.ToString()));
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
                imDrag.Source = new BitmapImage(new Uri(lPath.Content.ToString()));
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
            ofd.Filter = "Text files (*.txt)|*.txt|PDF files (*.pdf)|*.pdf|DOCX-files (*.doc, *.docx)|*.doc;*.docx|All files (*.*)|*.*";
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
                lstAuthors.Items.Add(new CheckBox { Content = FormData.Author, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
                FormData.Author = "";
            }
        }

        //Fill 'Authors' list-box.
        void FillAuthors()
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.Author).Result);
            foreach (int item in lst)
            {
                lstAuthors.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Author, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left });
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

        #region Close form (OK, Cancel).
        //Add a new book.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int? year = null;
            if (txtYear.Text != "")
                year = Convert.ToInt32(txtYear.Text);
            int? id = _proxy.AddBook(txtName.Text, txtDesc.Text == "" ? null : txtDesc.Text, chCopy.IsChecked == true ? $"{txtName.Text}{Path.GetExtension(txtPath.Text)}": txtPath.Text, lPath.Content.ToString() == "..." ? "WolfB.png" : lPath.Content.ToString(), chCopy.IsChecked == true? false: true, mark, year, DateTime.Now);
            if (id == null)
            {
                MessageBox.Show("Something went wrong.", "Operation denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            foreach (CheckBox item in lstCategory.Items)
            {
                if (item.IsChecked == true)
                    _proxy.AddItemCategoryAsync(Convert.ToInt32(id), Convert.ToInt32(item.Tag), EngServRef.ServerData.BookCategory);
            }
            if (chCopy.IsChecked == true)
                File.Copy(txtPath.Text, $@"Books\{txtName.Text}{Path.GetExtension(txtPath.Text)}", true);
            foreach (CheckBox item in lstAuthors.Items)
            {
                //item.Content.ToString().Split(",".ToCharArray())[1].Substring(1), item.Content.ToString().Split(",".ToCharArray())[0]
                if (item.IsChecked == true)
                    _proxy.AddBookAuthorAsync(Convert.ToInt32(id), Convert.ToInt32(item.Tag));
            }
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