using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppEnglish.AddEdit
{
    public partial class EditRole : Window
    {
        EngServRef.EngServiceClient _proxy;
        string role;
        int id;

        #region Constructors.
        //Initialization.
        public EditRole()
        {
            InitializeComponent();
        }
        //Initialize '_proxy'. Requires the users id.
        public EditRole(EngServRef.EngServiceClient tmp, int id) : this()
        {
            _proxy = tmp;
            this.id = id;
            int roleId = Convert.ToInt32(_proxy.GetItemProperty(id, EngServRef.ServerData.User, EngServRef.PropertyData.Role));
            role = _proxy.GetItemProperty(roleId, EngServRef.ServerData.Role, EngServRef.PropertyData.Name);
            lUsername.Content = _proxy.GetItemProperty(id, EngServRef.ServerData.User, EngServRef.PropertyData.Name);

            List<int> lst = new List<int>(_proxy.GetItems(EngServRef.ServerData.Role));
            int i = 0;
            foreach (int item in lst)
            {
                cmbRole.Items.Add(new TextBlock { Text = _proxy.GetItemProperty(item, EngServRef.ServerData.Role, EngServRef.PropertyData.Name), FontSize = 12, Foreground = Brushes.Black });
                if (item == roleId)
                    cmbRole.SelectedIndex = i;
                i++;
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
                        double len = stMain.ActualWidth - ((item as Panel).Children[0] as Label).ActualWidth - 25;
                        if (!(val is Label) && len > 10)
                            val.Width = len;
                    }
                }
            }
        }
        //Check the changes.
        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOK.IsEnabled = (sender as ComboBox).Text == role;
        }
        #endregion
        #region Close form (OK, Cancel).
        //Change.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _proxy.EditData(id, cmbRole.Text, EngServRef.ServerData.User, EngServRef.PropertyData.Role);
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