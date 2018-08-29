using System.Windows;
using System.Windows.Controls;

namespace AppEnglish
{
    public partial class AddAuthor : Window
    {
        EngServRef.EngServiceClient _proxy;

        #region Constructors.
        public AddAuthor()
        {
            InitializeComponent();
        }
        public AddAuthor(EngServRef.EngServiceClient tmp) : this()
        {
            _proxy = tmp;
        }
        #endregion

        #region Visualisation, validation.
        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in (sender as StackPanel).Children)
            {
                if (item is StackPanel)
                {
                    foreach (FrameworkElement val in (item as Panel).Children)
                    {
                        double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 25;
                        if (!(val is Label) && len > 10)
                            val.Width = len;
                    }
                }
            }
        }
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = (sender as TextBox).Text == "Empty strings are not allowed!";
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
        #endregion

        #region Close form (OK, Cancel).
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_proxy.CheckAuthor(txtName.Text, txtSurname.Text))
            {
                _proxy.AddAuthor(txtName.Text, txtSurname.Text);
                FormData.Author = $"{txtSurname.Text}, {txtName.Text}";
                Close();
            }
            else
                MessageBox.Show("This person is already taken!", "Wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}