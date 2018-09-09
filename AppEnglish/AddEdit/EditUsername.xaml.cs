using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish.AddEdit
{
    public partial class EditUsername : Window
    {
        EngServRef.EngServiceClient _proxy;
        int id;

        #region Constructors.
        //Initialization.
        public EditUsername()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'. Requires the users id.
        public EditUsername(EngServRef.EngServiceClient tmp, int id) : this()
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
        //Check the name.
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isUser = _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.User).Result;

            foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
            {
                if (item is TextBox)
                    item.Style = TryFindResource(isUser ? "txtWrong" : "txtNormal") as Style;
                else if (item is Label)
                    item.Style = TryFindResource(isUser ? "lbFormWrong" : "lbFormNormal") as Style;
            }
            ((sender as TextBox).Parent as Panel).ToolTip = isUser ? "This username already exists!" : "Input data.";
            btnOK.IsEnabled = !isUser && txtName.Text != "";

            if ((sender as TextBox).Text == "")
                ((sender as TextBox).Parent as Panel).ToolTip = "Empty strings are not allowed!";
        }
        #endregion
        #region Close form (OK, Cancel).
        //Change.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _proxy.EditData(id, txtName.Text, EngServRef.ServerData.User, EngServRef.PropertyData.Name);
            string ext = Path.GetExtension(_proxy.GetItemProperty(id, EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath));
            _proxy.EditData(id, $"{txtName.Text}{ext}", EngServRef.ServerData.User, EngServRef.PropertyData.Imgpath);
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