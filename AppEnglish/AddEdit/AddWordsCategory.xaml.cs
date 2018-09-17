using AppEnglish.EngServRef;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish.AddEdit
{
    public partial class AddWordsCategory : Window
    {
        EngServiceClient _proxy;
        int? id = null;

        #region Constructors.
        //Initialization.
        public AddWordsCategory()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'.
        public AddWordsCategory(EngServiceClient tmp) : this()
        {
            _proxy = tmp;
        }
        //Initialize '_proxy' and fields.
        public AddWordsCategory(EngServiceClient tmp, int catId) : this(tmp)
        {
            id = catId;
            txtName.Text = _proxy.GetItemProperty(catId, ServerData.WordCategory, PropertyData.Name);
            txtAbbr.Text = _proxy.GetItemProperty(catId, ServerData.WordCategory, PropertyData.Abbreviation);
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
            if (txtAbbr.Text == "" || txtName.Text == "")
            {
                MessageBox.Show("Empty strings are not allowed!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_proxy.CheckExistence(txtName.Text, ServerData.WordCategory))
            {
                if (id == null)
                {
                    if (_proxy.AddWordsCategory(txtName.Text, txtAbbr.Text) == null)
                    {
                        MessageBox.Show("Something went wrong. This category was not addded.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    _proxy.EditData(Convert.ToInt32(id), txtName.Text, ServerData.WordCategory, PropertyData.Name);
                    _proxy.EditData(Convert.ToInt32(id), txtAbbr.Text, ServerData.WordCategory, PropertyData.Abbreviation);
                }
                Close();
            }
            else
                MessageBox.Show("This category already exists!", "Wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        //Close the form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}