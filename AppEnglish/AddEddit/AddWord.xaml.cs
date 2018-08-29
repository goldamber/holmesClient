using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AppEnglish
{
    public partial class AddWord : Window
    {
        EngServRef.EngServiceClient _proxy;

        #region Constructors.
        public AddWord()
        {
            InitializeComponent();
        }
        public AddWord(EngServRef.EngServiceClient tmp) : this()
        {
            _proxy = tmp;
        }
        public AddWord(string word, EngServRef.EngServiceClient tmp) : this(tmp)
        {
            word = word.ToLower();
            string[] _end = word.Split('\'');
            if (_end.Length == 1)
                txtWord.Text = word;
            else
                txtWord.Text = (_end[1] == "s" || _end[1] == "" || _end[1] == "ll")? _end[0] : word;
            
            txtWord_TextChanged(txtWord, null);
        }
        #endregion

        #region Visualisation.
        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in (sender as StackPanel).Children)
            {
                if (item is Panel)
                {
                    foreach (FrameworkElement val in (item as Panel).Children)
                    {
                        double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 25;
                        if (len > 10)
                            val.Width = len;
                    }
                }
                else if (item is Expander)
                {
                    foreach (var val in ((item as Expander).Content as Panel).Children)
                    {
                        if (val is Panel)
                        {
                            foreach (FrameworkElement lb in (val as Panel).Children)
                            {
                                double len = stMain.ActualWidth - ((val as Panel).Children[0] as Label).ActualWidth - 25;
                                if (len > 10)
                                    (lb as TextBox).Width = len;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Validation.
        private void txtWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*if (_proxy.GetWord(txtWord.Text) != null || (sender as TextBox).Text == "")
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                    ((sender as TextBox).Parent as Panel).ToolTip = "This word already exists!";
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
                ((sender as TextBox).Parent as Panel).ToolTip = "Input word.";
                btnOK.IsEnabled = true;
            }

            if ((sender as TextBox).Text == "")
                ((sender as TextBox).Parent as Panel).ToolTip = "Empty strings are not allowed!";*/
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
        private void Border_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            lPath.Content = files[0];
            imDrag.Source = new BitmapImage(new Uri(lPath.Content.ToString()));
            brImage.Opacity = 0.4;
        }       //Set image.
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        #region Button clicks (OK, Cancel).
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            /*EngServRef.Word tmp = new EngServRef.Word { Name = txtWord.Text, ImgPath = lPath.Content.ToString() == "..."? null: lPath.Content.ToString() };

            if (txtGerund.Text != "" && txtPast.Text != "" && txtPastTh.Text != "" && txtPlural.Text != "" && txtPresent.Text != "")
                tmp.FormID = new EngServRef.WordForm { Gerund = txtGerund.Text, PastForm = txtPast.Text, PluralForm = txtPlural.Text, PastThForm = txtPastTh.Text, PresentForm = txtPresent.Text };
            foreach (string item in txtTranlation.Text.Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (_proxy.GetTranslation(item) == null)
                    _proxy.AddTran(new EngServRef.Translation { Name = item });                
            }
            foreach (string item in txtDescription.Text.Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (_proxy.GetDescription(item) == null)
                    _proxy.AddDef(new EngServRef.Definition { Name = item });               
            }

            _proxy.AddWord(tmp);
            foreach (string item in txtTranlation.Text.Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                _proxy.AddWordTran(item, txtWord.Text);
            }
            foreach (string item in txtDescription.Text.Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                _proxy.AddWordDef(item, txtWord.Text);
            }*/

            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtDescription.Text = "";
            txtPast.Text = "";
            txtPastTh.Text = "";
            txtPlural.Text = "";
            txtTranlation.Text = "";
            Close();
        }
        #endregion
    }
}