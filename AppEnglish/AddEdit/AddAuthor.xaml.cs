using System;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish
{
    public partial class AddAuthor : Window
    {
        EngServRef.EngServiceClient _proxy;
        int? id = null;

        #region Constructors.
        //Initialization.
        public AddAuthor()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'.
        public AddAuthor(EngServRef.EngServiceClient tmp) : this()
        {
            _proxy = tmp;
        }
        //Initialize '_proxy' and fields.
        public AddAuthor(EngServRef.EngServiceClient tmp, int authId) : this(tmp)
        {
            id = authId;
            txtName.Text = _proxy.GetItemProperty(authId, EngServRef.ServerData.Author, EngServRef.PropertyData.Name);
            txtSurname.Text = _proxy.GetItemProperty(authId, EngServRef.ServerData.Author, EngServRef.PropertyData.Surname);
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
        //Check the name and surname of an author.
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
                ((sender as TextBox).Parent as Panel).ToolTip = "Empty strings are not allowed!";
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
        //Add an author.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_proxy.CheckAuthor(txtName.Text, txtSurname.Text))
            {
                if (id == null)
                {
                    int? authId = _proxy.AddAuthor(txtName.Text, txtSurname.Text);
                    if (authId == null)
                    {
                        MessageBox.Show("Something went wrong. This author was not added.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    FormData.Author = $"{txtSurname.Text}, {txtName.Text}";
                    FormData.AuthorsID = Convert.ToInt32(authId);
                }
                else
                {
                    _proxy.EditData(Convert.ToInt32(id), txtName.Text, EngServRef.ServerData.Author, EngServRef.PropertyData.Name);
                    _proxy.EditData(Convert.ToInt32(id), txtSurname.Text, EngServRef.ServerData.Author, EngServRef.PropertyData.Surname);
                }
                Close();
            }
            else
                MessageBox.Show("This person is already taken!", "Wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        //Close the form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}