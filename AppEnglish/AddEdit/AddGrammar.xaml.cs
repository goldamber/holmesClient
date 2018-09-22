using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish.AddEdit
{
    public partial class AddGrammar : Window
    {
        EngServRef.EngServiceClient _proxy;
        int? grammarId = null;

        #region Constructors, initialization.
        //Initialization.
        public AddGrammar()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes '_proxy'.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="userId">Id of user.</param>
        public AddGrammar(EngServRef.EngServiceClient tmp) : this()
        {
            _proxy = tmp;
        }
        /// <summary>
        /// Initialize form for editting.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="grammar">Rules id.</param>
        public AddGrammar(EngServRef.EngServiceClient tmp, int grammar) : this(tmp)
        {
            grammarId = grammar;
            txtName.Text = _proxy.GetItemProperty(grammar, EngServRef.ServerData.Grammar, EngServRef.PropertyData.Name);
        }

        //Fill listboxes.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (grammarId != null)
            {
                txtDescription.Text = _proxy.GetItemProperty(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.PropertyData.Description);

                if (_proxy.GetItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.ServerData.Rule) != null)
                    FillRules(new List<int>(_proxy.GetItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.ServerData.Rule)));
                if (_proxy.GetItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.ServerData.GrammarException) != null)
                    FillExceptions(new List<int>(_proxy.GetItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.ServerData.GrammarException)));
                if (_proxy.GetItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.ServerData.GrammarExample) != null)
                    FillExamples(new List<int>(_proxy.GetItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.ServerData.GrammarExample)));
            }
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
                        if (((item as Panel).Children[0] is Label))
                        {
                            double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 40;
                            if (val is TextBox && len > 10)
                                val.Width = len;
                        }
                    }
                }
            }
        }
        //Check the title of rule.
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = _proxy.GetItemProperty(Convert.ToInt32(grammarId), EngServRef.ServerData.Grammar, EngServRef.PropertyData.Name);
            if ((sender as TextBox).Text == "" || (_proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Grammar).Result && grammarId == null) || (grammarId != null && txtName.Text != name && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Grammar).Result))
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = (sender as TextBox).Text == "" ? "Empty strings are not allowed!" : "This title is already taken!";
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

        //Fill 'Rules' list-box.
        void FillRules(List<int> lst)
        {
            foreach (int item in lst)
            {
                lstRules.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Rule, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }
        //Fill 'Exceptions' list-box.
        void FillExceptions(List<int> lst)
        {
            foreach (int item in lst)
            {
                lstExceptions.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.GrammarException, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }
        //Fill 'Examples' list-box.
        void FillExamples(List<int> lst)
        {
            foreach (int item in lst)
            {
                lstExamples.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.GrammarExample, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }
        #endregion
        #region Buttons.
        private void btnAddRule_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.Rule);
            frm.ShowDialog();
            if (FormData.RuleID != null)
            {
                int id = Convert.ToInt32(FormData.RuleID);
                lstRules.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.Rule, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.RuleID = null;
            }
        }
        private void btnAddException_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.GrammarException);
            frm.ShowDialog();
            if (FormData.ExceptionID != null)
            {
                int id = Convert.ToInt32(FormData.ExceptionID);
                lstExceptions.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.GrammarException, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.ExceptionID = null;
            }
        }
        private void btnAddExample_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.GrammarExample);
            frm.ShowDialog();
            if (FormData.GrExampleID != null)
            {
                int id = Convert.ToInt32(FormData.GrExampleID);
                lstExamples.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.GrammarExample, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.GrExampleID = null;
            }
        }
        #endregion
        #region Close form (OK, Cancel).
        //Add a new grammar.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int edit = 0;
            if (txtName.Text == "")
            {
                MessageBox.Show("You should fill all fiels marked with '*'!", "Empty strings are not allowed", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            bool ruleChecked = false;
            foreach (CheckBox item in lstRules.Items)
            {
                if (item.IsChecked == true)
                {
                    ruleChecked = true;
                    break;
                }
            }
            if (!ruleChecked)
            {
                MessageBox.Show("You should add rules.", "Choose rule", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (grammarId == null)
                    {
                        int? id = _proxy.AddGrammar(txtName.Text, txtDescription.Text == "" ? null : txtDescription.Text);
                        if (id == null)
                        {
                            MessageBox.Show("Something went wrong.", "Operation denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                            stMain.Visibility = Visibility.Visible;
                            return;
                        }
                        edit = Convert.ToInt32(id);
                    }
                    else
                    {
                        _proxy.RemoveFullItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.Rule);
                        _proxy.RemoveFullItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.GrammarExample);
                        _proxy.RemoveFullItemData(Convert.ToInt32(grammarId), EngServRef.ServerData.GrammarException);

                        _proxy.EditData(Convert.ToInt32(grammarId), txtName.Text, EngServRef.ServerData.Grammar, EngServRef.PropertyData.Name);
                        _proxy.EditData(Convert.ToInt32(grammarId), txtDescription.Text == "" ? null : txtDescription.Text, EngServRef.ServerData.Grammar, EngServRef.PropertyData.Description);
                        edit = Convert.ToInt32(grammarId);
                    }

                    foreach (CheckBox item in lstRules?.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.Rule);
                    }
                    foreach (CheckBox item in lstExceptions?.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.GrammarException);
                    }
                    foreach (CheckBox item in lstExamples?.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.GrammarExample);
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