using AppEnglish.EngServRef;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish.AddEdit
{
    public partial class AddWordsExtraData : Window
    {
        EngServiceClient _proxy;
        ServerData dataType;

        #region Constructors.
        //Initialization.
        public AddWordsExtraData()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'.
        public AddWordsExtraData(EngServiceClient tmp, ServerData type) : this()
        {
            _proxy = tmp;
            dataType = type;
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
        //Add data.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_proxy.CheckExistence(txtName.Text, dataType))
            {
                if (_proxy.AddData(txtName.Text, dataType) == null)
                {
                    MessageBox.Show("Something went wrong. This type was not addded.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            switch (dataType)
            {
                case ServerData.Example:
                    FormData.ExampleID = _proxy.GetItemsId(txtName.Text, ServerData.Example);
                    break;
                case ServerData.Translation:
                    FormData.TranslationID = _proxy.GetItemsId(txtName.Text, ServerData.Translation);
                    break;
                case ServerData.Definition:
                    FormData.DefinitionID = _proxy.GetItemsId(txtName.Text, ServerData.Definition);
                    break;
                case ServerData.Group:
                    FormData.GroupID = _proxy.GetItemsId(txtName.Text, ServerData.Group);
                    break;
                case ServerData.GrammarExample:
                    FormData.GrExampleID = _proxy.GetItemsId(txtName.Text, ServerData.GrammarExample);
                    break;
                case ServerData.GrammarException:
                    FormData.ExceptionID = _proxy.GetItemsId(txtName.Text, ServerData.GrammarException);
                    break;
                case ServerData.Rule:
                    FormData.RuleID = _proxy.GetItemsId(txtName.Text, ServerData.Rule);
                    break;
            }
            Close();
            return;
        }
        //Close the form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}