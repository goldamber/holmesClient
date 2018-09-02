using System.Windows;
using System.Windows.Controls;

namespace AppEnglish.AddEdit
{
    public partial class EditPassword : Window
    {
        EngServRef.EngServiceClient _proxy;
        int id;

        #region Constructors.
        //Initialization.
        public EditPassword()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'. Requires the users id.
        public EditPassword(EngServRef.EngServiceClient tmp, int id) : this()
        {
            _proxy = tmp;
            this.id = id;
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
                        double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 25;
                        if (!(val is Label) && len > 10)
                            val.Width = len;
                    }
                }
            }
        }
        //Check fields.
        private void txtName_TextChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password.Length < 6)
            {
                foreach (FrameworkElement item in ((sender as PasswordBox).Parent as Panel).Children)
                {
                    if (item is PasswordBox)
                        item.Style = TryFindResource("pswdWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                ((sender as PasswordBox).Parent as Panel).ToolTip = "Password must contain more than 6 symbols!";
                btnOK.IsEnabled = false;
            }
            else
            {
                foreach (FrameworkElement item in ((sender as PasswordBox).Parent as Panel).Children)
                {
                    if (item is PasswordBox)
                        item.Style = TryFindResource("pswdNormal") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormNormal") as Style;
                }
                ((sender as PasswordBox).Parent as Panel).ToolTip = "Input data.";
                btnOK.IsEnabled = true;
            }
        }
        #endregion
        #region Close form (OK, Cancel).
        //Edit.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (pswdNew.Password != pswdConfirm.Password)
            {
                MessageBox.Show("Passwords do not match! Try again.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _proxy.EditData(id, pswdNew.Password, EngServRef.ServerData.User, EngServRef.PropertyData.Password);
            Close();
        }
        //Close the form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}