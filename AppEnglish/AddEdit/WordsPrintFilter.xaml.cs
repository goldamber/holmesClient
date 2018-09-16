using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AppEnglish.AddEdit
{
    public partial class WordsPrintFilter : Window
    {
        EngServRef.EngServiceClient _proxy;
        int item;
        int user;
        EngServRef.ServerData type;

        #region Constructors.
        //Initialization.
        public WordsPrintFilter()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes '_proxy', fills the listbox.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="itemsId">Id of item.</param>
        /// <param name="data">Items type.</param>
        public WordsPrintFilter(EngServRef.EngServiceClient tmp, int userId, int itemsId, EngServRef.ServerData data) : this()
        {
            _proxy = tmp;
            user = userId;
            item = itemsId;
            type = data;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> words = new List<int>(_proxy.GetUserItemWordsAsync(user, item, type).Result);
            FormData.WordsToPrint.Clear();
            FillWords(words);
        }
        #endregion

        //Fill 'Words' list-box with default values.
        void FillWords(List<int> tmp)
        {
            List<int> lst = new List<int>(_proxy.GetUserItemWordsAsync(user, item, type).Result);
            foreach (int item in lst)
            {
                lstWords.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Word, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }
        //Select/deselect all items.
        private void chSelect_Click(object sender, RoutedEventArgs e)
        {
            bool check = (sender as CheckBox).IsChecked == true ? true : false;
            foreach (CheckBox item in lstWords.Items)
            {
                item.IsChecked = check;
            }
        }

        #region Close form (OK, Cancel).
        //Add a new book.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox item in lstWords.Items)
            {
                if (item.IsChecked == true)
                    FormData.WordsToPrint.Add(Convert.ToInt32(item.Tag));
            }
            Close();
        }
        //Close form.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FormData.WordsToPrint.Clear();
            Close();
        }
        #endregion
    }
}