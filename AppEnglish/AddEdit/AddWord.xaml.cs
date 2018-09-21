using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppEnglish.AddEdit
{
    public partial class AddWord : Window
    {
        EngServRef.EngServiceClient _proxy;
        EngServRef.ServerData data;
        bool editForm = false;
        int wordsId;
        string name = null;
        int? item = null;
        int user;

        #region Constructors, initialization.
        //Initialization.
        public AddWord()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes '_proxy'.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="userId">Id of user.</param>
        public AddWord(EngServRef.EngServiceClient tmp, int? userId) : this()
        {
            _proxy = tmp;
            user = Convert.ToInt32(userId);
        }
        /// <summary>
        /// Add a word for video or book.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="wordsName">Word.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="itemId">Items id.</param>
        /// <param name="type">Items type.</param>
        public AddWord(EngServRef.EngServiceClient tmp, string wordsName, int? userId, int? itemId, EngServRef.ServerData type) : this(tmp, userId)
        {
            name = wordsName;
            item = itemId;
            data = type;
        }
        /// <summary>
        /// Initialize form for editting.
        /// </summary>
        /// <param name="tmp">Host.</param>
        /// <param name="wordsName">Word.</param>
        /// <param name="userId">Id of user.</param>
        public AddWord(EngServRef.EngServiceClient tmp, int word, int? userId) : this(tmp, userId)
        {
            name = _proxy.GetItemProperty(word, EngServRef.ServerData.Word, EngServRef.PropertyData.Name);
            wordsId = word;
            editForm = true;
        }
        //Fill listboxes.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int role = Convert.ToInt32(_proxy.GetItemProperty(user, EngServRef.ServerData.User, EngServRef.PropertyData.Role));
            if (name != null)
                txtName.Text = name;
            
            if (editForm)
            {
                string plural = _proxy.GetItemProperty(wordsId, EngServRef.ServerData.Word, EngServRef.PropertyData.PluralForm);
                string past = _proxy.GetItemProperty(wordsId, EngServRef.ServerData.Word, EngServRef.PropertyData.PastForm);
                string pastTh = _proxy.GetItemProperty(wordsId, EngServRef.ServerData.Word, EngServRef.PropertyData.PastThForm);
                if (plural != null)
                {
                    grForm.Visibility = Visibility.Visible;
                    stPlural.Visibility = Visibility.Visible;
                    txtPlural.Text = plural;
                }
                if (past != null)
                {
                    grForm.Visibility = Visibility.Visible;
                    stPast.Visibility = Visibility.Visible;
                    txtPast.Text = past;
                }
                if (pastTh != null)
                {
                    grForm.Visibility = Visibility.Visible;
                    stPastParticiple.Visibility = Visibility.Visible;
                    txtPastTh.Text = pastTh;
                }
                int tran = Convert.ToInt32(_proxy.GetItemProperty(wordsId, EngServRef.ServerData.Word, EngServRef.PropertyData.Transcription));
                if (tran != 0)
                {
                    txtAmerican.Text = _proxy.GetItemProperty(tran, EngServRef.ServerData.Transcription, EngServRef.PropertyData.American);
                    txtBritish.Text = _proxy.GetItemProperty(tran, EngServRef.ServerData.Transcription, EngServRef.PropertyData.British);
                    txtCanadian.Text = _proxy.GetItemProperty(tran, EngServRef.ServerData.Transcription, EngServRef.PropertyData.Canadian);
                    txtAustralian.Text = _proxy.GetItemProperty(tran, EngServRef.ServerData.Transcription, EngServRef.PropertyData.Australian);
                }
                string imgPath = _proxy.GetItemProperty(wordsId, EngServRef.ServerData.Word, EngServRef.PropertyData.Imgpath);
                if (imgPath != null)
                {
                    if (File.Exists($@"Temp\WordImages\{imgPath}"))
                        FormData.SetImage($@"pack://siteoforigin:,,,/Temp\WordImages\{imgPath}", imDrag);
                }
                FillExamples(new List<int>(_proxy.GetItemData(wordsId, EngServRef.ServerData.Word, EngServRef.ServerData.Example)));
                FillDefinitions(new List<int>(_proxy.GetItemData(wordsId, EngServRef.ServerData.Word, EngServRef.ServerData.Definition)));
                FillTranslations(new List<int>(_proxy.GetItemData(wordsId, EngServRef.ServerData.Word, EngServRef.ServerData.Translation)));
            }
            if (_proxy.GetItemProperty(role, EngServRef.ServerData.Role, EngServRef.PropertyData.Name) == "admin")
            {
                if (!editForm)
                    FillGroups();
                else
                {
                    List<int> groups = new List<int>(_proxy.GetItemData(wordsId, EngServRef.ServerData.Word, EngServRef.ServerData.Group));
                    FillGroups(groups);
                }
                stGroups.Visibility = Visibility.Visible;
            }
            if (!editForm)
                FillCategories();
            else
            {
                List<int> categories = new List<int>(_proxy.GetItemData(wordsId, EngServRef.ServerData.Word, EngServRef.ServerData.WordCategory));
                FillCategories(categories);
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
        //Check the title of word.
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "" || (((sender as TextBox) == txtName) && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Word).Result && name == null) || (((sender as TextBox) == txtName) && name != null && txtName.Text != name && _proxy.CheckExistenceAsync(txtName.Text, EngServRef.ServerData.Word).Result))
            {
                foreach (FrameworkElement item in ((sender as TextBox).Parent as Panel).Children)
                {
                    if (item is TextBox)
                        item.Style = TryFindResource("txtWrong") as Style;
                    else if (item is Label)
                        item.Style = TryFindResource("lbFormWrong") as Style;
                }
                ((sender as TextBox).Parent as Panel).ToolTip = (sender as TextBox).Text == "" ? "Empty strings are not allowed!" : "This word is already taken!";
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
        //Fill 'Categories' list-box.
        void FillCategories()
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.WordCategory).Result);
            foreach (int item in lst)
            {
                CheckBox chItem = new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.WordCategory, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left };
                chItem.Click += ChItem_Click;
                lstCategory.Items.Add(chItem);
            }
        }
        //Fill 'Categories' list-box.
        void FillCategories(List<int> categories)
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.WordCategory).Result);
            foreach (int item in lst)
            {
                CheckBox chItem = new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.WordCategory, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = categories.Contains(item) };
                chItem.Click += ChItem_Click;
                lstCategory.Items.Add(chItem);
            }
        }
        //Fill 'Groups' list-box.
        void FillGroups()
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.Group).Result);
            foreach (int item in lst)
            {
                lstGroups.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Group, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left });
            }
        }
        //Fill 'Groups' list-box.
        void FillGroups(List<int> groups)
        {
            List<int> lst = new List<int>(_proxy.GetItemsAsync(EngServRef.ServerData.Group).Result);
            foreach (int item in lst)
            {
                lstGroups.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Group, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = groups.Contains(item) });
            }
        }
        //Fill 'Examples' list-box.
        void FillExamples(List<int> lst)
        {
            foreach (int item in lst)
            {
                lstExamples.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Example, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }
        //Fill 'Translations' list-box.
        void FillTranslations(List<int> lst)
        {
            foreach (int item in lst)
            {
                lstTranslations.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Translation, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }
        //Fill 'Definitions' list-box.
        void FillDefinitions(List<int> lst)
        {
            foreach (int item in lst)
            {
                lstDefinitions.Items.Add(new CheckBox { VerticalAlignment = VerticalAlignment.Stretch, Tag = item, Content = _proxy.GetItemPropertyAsync(item, EngServRef.ServerData.Definition, EngServRef.PropertyData.Name).Result, Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true });
            }
        }

        private void ChItem_Click(object sender, RoutedEventArgs e)
        {
            string cat = (sender as CheckBox).Content.ToString().ToLower();
            if (cat.Contains("british") || cat.Contains("american") || cat.Contains("canadian") || cat.Contains("australian"))
                return;
            if ((sender as CheckBox).IsChecked == true)
            {
                if (cat.Contains(" noun") || cat.Equals("noun"))
                {
                    grForm.Visibility = Visibility.Visible;
                    stPlural.Visibility = cat.Equals("uncountable noun") ? Visibility.Collapsed : Visibility.Visible;
                    stPast.Visibility = Visibility.Collapsed;
                    stPastParticiple.Visibility = Visibility.Collapsed;
                    //SetItemsEnabled("noun", false);
                }
                else if (cat.Contains(" verb") || cat.Equals("verb"))
                {
                    grForm.Visibility = Visibility.Visible;
                    stPlural.Visibility = Visibility.Collapsed;
                    stPast.Visibility = Visibility.Visible;
                    stPastParticiple.Visibility = Visibility.Visible;
                    //SetItemsEnabled("verb", false);
                }
               /* else if (cat.Contains(" adjective") || cat.Equals("adjective"))
                    SetItemsEnabled("adjective", false);
                else if (cat.Contains(" adverb") || cat.Equals("adverb"))
                    SetItemsEnabled("adverb", false);
                else if (cat.Contains("pronoun") || cat.Equals("pronoun"))
                    SetItemsEnabled("pronoun", false);
                else if (cat.Contains("preposition") || cat.Equals("preposition"))
                    SetItemsEnabled("preposition", false);
                else if (cat.Contains("conjunction") || cat.Equals("conjunction"))
                    SetItemsEnabled("conjunction", false);
                else if (cat.Contains("interjunction") || cat.Equals("interjunction"))
                    SetItemsEnabled("interjunction", false);
                else if (cat.Contains("idiom") || cat.Equals("idiom"))
                    SetItemsEnabled("idiom", false);*/
            }
            else
            {
                if ((cat.Contains(" noun") || cat.Equals("noun")) && !CheckItemsGroup("noun"))
                {
                   // SetItemsEnabled("noun", true);
                    stPlural.Visibility = Visibility.Collapsed;
                }
                else if ((cat.Contains(" verb") || cat.Equals("verb")) && !CheckItemsGroup("verb"))
                {
                   // SetItemsEnabled("verb", true);
                    stPast.Visibility = Visibility.Collapsed;
                    stPastParticiple.Visibility = Visibility.Collapsed;
                }
               /* else if ((cat.Contains(" adjective") || cat.Equals("adjective")) && !CheckItemsGroup("adjective"))
                    SetItemsEnabled("adjective", true);
                else if ((cat.Contains(" adverb") || cat.Equals("adverb")) && !CheckItemsGroup("adverb"))
                    SetItemsEnabled("adverb", true);
                else if ((cat.Contains(" pronoun") || cat.Equals("pronoun")) && !CheckItemsGroup("pronoun"))
                    SetItemsEnabled("pronoun", true);
                else if ((cat.Contains(" preposition") || cat.Equals("preposition")) && !CheckItemsGroup("preposition"))
                    SetItemsEnabled("preposition", true);
                else if ((cat.Contains(" conjunction") || cat.Equals("conjunction")) && !CheckItemsGroup("conjunction"))
                    SetItemsEnabled("conjunction", true);
                else if ((cat.Contains(" interjunction") || cat.Equals("interjunction")) && !CheckItemsGroup("interjunction"))
                    SetItemsEnabled("interjunction", true);
                else if ((cat.Contains(" idiom") || cat.Equals("idiom")) && !CheckItemsGroup("idiom"))
                    SetItemsEnabled("idiom", true);*/

                if (!CheckItemsGroup("noun") && !CheckItemsGroup("verb"))
                    grForm.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Checks if the items of this group are checked.
        /// </summary>
        /// <param name="comparison">Items group.</param>
        /// <returns>TRUE - there are still some items of this group that are checked.</returns>
        bool CheckItemsGroup(string comparison)
        {
            bool isPart = false;
            foreach (CheckBox item in lstCategory.Items)
            {
                if ((item.Content.ToString().ToLower().Contains(" " + comparison) || item.Content.ToString().ToLower().Equals(comparison)) && item.IsChecked == true)
                {
                    isPart = true;
                    break;
                }
            }
            return isPart;
        }
        /// <summary>
        /// Changes the words groups that could be checked.
        /// </summary>
        /// <param name="comparison">Items group.</param>
        /// <param name="isEnabled">Should it be enabled or disabled?</param>
        /*void SetItemsEnabled(string comparison,  bool isEnabled)
        {
            foreach (CheckBox item in lstCategory.Items)
            {
                if ((!item.Content.ToString().ToLower().Contains(" " + comparison) && !item.Content.ToString().ToLower().Equals(comparison)) && !item.Content.ToString().ToLower().Contains("british") && !item.Content.ToString().ToLower().Contains("american") && !item.Content.ToString().ToLower().Contains("canadian") && !item.Content.ToString().ToLower().Contains("australian"))
                    item.IsEnabled = isEnabled;
            }
        }*/
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
        //Sets image.
        private void Border_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            lPath.Content = files[0];
            FormData.SetImage(lPath.Content.ToString(), imDrag);
            brImage.Opacity = 0.4;
        }
        //Choose image (via OpenFileDialog).
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.tif)|*.png;*.jpg;*.jpeg;*.gif;*.tif|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                lPath.Content = openFileDialog.FileName;
                FormData.SetImage(lPath.Content.ToString(), imDrag);
            }
        }
        #endregion
        #region Buttons.
        //Add a new translation to the list.
        private void btnAddTranslation_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.Translation);
            frm.ShowDialog();
            if (FormData.TranslationID != null)
            {
                int id = Convert.ToInt32(FormData.TranslationID);
                lstTranslations.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.Translation, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.TranslationID = null;
            }
        }
        //Add a new definition to the list.
        private void btnAddDefinition_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.Definition);
            frm.ShowDialog();
            if (FormData.DefinitionID != null)
            {
                int id = Convert.ToInt32(FormData.DefinitionID);
                lstDefinitions.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.Definition, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.DefinitionID = null;
            }
        }
        //Add a new example to the list.
        private void btnAddExample_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.Example);
            frm.ShowDialog();
            if (FormData.ExampleID != null)
            {
                int id = Convert.ToInt32(FormData.ExampleID);
                lstExamples.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.Example, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.ExampleID = null;
            }
        }
        //Add a new group to the list.
        private void btnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddWordsExtraData frm = new AddWordsExtraData(_proxy, EngServRef.ServerData.Group);
            frm.ShowDialog();
            if (FormData.GroupID != null)
            {
                int id = Convert.ToInt32(FormData.GroupID);
                lstGroups.Items.Add(new CheckBox { Content = _proxy.GetItemProperty(id, EngServRef.ServerData.Group, EngServRef.PropertyData.Name), Style = TryFindResource("chNormal") as Style, HorizontalAlignment = HorizontalAlignment.Left, IsChecked = true, Tag = id });
                FormData.GroupID = null;
            }
        }

        private void btnBritish_Click(object sender, RoutedEventArgs e)
        {
            txtBritish.Text += (sender as Button).Content.ToString();
        }
        private void btnCanadian_Click(object sender, RoutedEventArgs e)
        {
            txtCanadian.Text += (sender as Button).Content.ToString();
        }
        private void btnAmerican_Click(object sender, RoutedEventArgs e)
        {
            txtAmerican.Text += (sender as Button).Content.ToString();
        }
        private void btnAustralian_Click(object sender, RoutedEventArgs e)
        {
            txtAustralian.Text += (sender as Button).Content.ToString();
        }
        #endregion

        #region Close form (OK, Cancel).
        //Add a new word.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int edit = 0;
            if (txtName.Text == "" || txtBritish.Text == "" || txtCanadian.Text == "" || txtAmerican.Text == "" || txtAustralian.Text == "")
            {
                MessageBox.Show("You should fill all fiels marked with '*'!", "Empty strings are not allowed", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            bool catChecked = false;
            foreach (CheckBox item in lstCategory.Items)
            {
                if (item.IsChecked == true)
                {
                    catChecked = true;
                    break;
                }
            }
            if (!catChecked)
            {
                MessageBox.Show("You should choose category.", "Choose category", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            Task.Run(new Action(() => {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!editForm)
                    {
                        int? id = _proxy.AddWord(txtName.Text, null, user);
                        if (id == null)
                        {
                            MessageBox.Show("Something went wrong.", "Operation denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                            stMain.Visibility = Visibility.Visible;
                            return;
                        }
                        edit = Convert.ToInt32(id);

                        if (lPath.Content.ToString() != "...")
                        {
                            if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), $"{edit}{Path.GetExtension(lPath.Content.ToString())}", EngServRef.FilesType.WordsImages))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                _proxy.RemoveItem(edit, EngServRef.ServerData.Word);
                                return;
                            }
                            _proxy.EditData(edit, $"{edit}{Path.GetExtension(lPath.Content.ToString())}", EngServRef.ServerData.Word, EngServRef.PropertyData.Imgpath);
                        }
                        string past = null, plural = null, pastTh = null;
                        if (stPast.Visibility == Visibility.Visible)
                            past = txtPast.Text == "" ? null : txtPast.Text;
                        if (stPastParticiple.Visibility == Visibility.Visible)
                            pastTh = txtPastTh.Text == "" ? null : txtPastTh.Text;
                        if (stPlural.Visibility == Visibility.Visible)
                            plural = txtPlural.Text == "" ? null : txtPlural.Text;
                        _proxy.AddWordsForm(edit, past, plural, pastTh);
                        _proxy.AddWordsTranscription(edit, txtBritish.Text, txtCanadian.Text, txtAmerican.Text, txtAustralian.Text);
                        if (item != null)
                            _proxy.AddItemsWord(edit, Convert.ToInt32(item), data);
                    }
                    else
                    {
                        _proxy.RemoveFullItemData(wordsId, EngServRef.ServerData.Translation);
                        _proxy.RemoveFullItemData(wordsId, EngServRef.ServerData.Definition);
                        _proxy.RemoveFullItemData(wordsId, EngServRef.ServerData.WordCategory);
                        _proxy.RemoveFullItemData(wordsId, EngServRef.ServerData.Group);
                        _proxy.RemoveFullItemData(wordsId, EngServRef.ServerData.Example);

                        _proxy.EditData(wordsId, txtName.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.Name);
                        if (lPath.Content.ToString() != "...")
                        {
                            FormData.EditWords.Add(wordsId);
                            string file = $"{wordsId}{Path.GetExtension(lPath.Content.ToString())}";
                            if (!_proxy.Upload(File.ReadAllBytes(lPath.Content.ToString()), file, EngServRef.FilesType.WordsImages))
                            {
                                MessageBox.Show("This file is too large!\nPlease choose another file.", "Unable to upload", MessageBoxButton.OK, MessageBoxImage.Stop);
                                return;
                            }
                            _proxy.EditData(wordsId, file, EngServRef.ServerData.Word, EngServRef.PropertyData.Imgpath);
                        }
                        _proxy.EditData(wordsId, txtPast.Text == ""? null: txtPast.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.PastForm);
                        _proxy.EditData(wordsId, txtPlural.Text == "" ? null : txtPlural.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.PluralForm);
                        _proxy.EditData(wordsId, txtPastTh.Text == "" ? null : txtPastTh.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.PastThForm);
                        _proxy.EditData(wordsId, txtBritish.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.British);
                        _proxy.EditData(wordsId, txtCanadian.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.Canadian);
                        _proxy.EditData(wordsId, txtAmerican.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.American);
                        _proxy.EditData(wordsId, txtAustralian.Text, EngServRef.ServerData.Word, EngServRef.PropertyData.Australian);
                        edit = wordsId;
                    }

                    foreach (CheckBox item in lstCategory.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.WordCategory);
                    }
                    foreach (CheckBox item in lstDefinitions?.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.Definition);
                    }
                    foreach (CheckBox item in lstTranslations?.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.Translation);
                    }
                    foreach (CheckBox item in lstGroups?.Items)
                    {
                        if (item.IsChecked == true)
                            _proxy.AddItemDataAsync(edit, Convert.ToInt32(item.Tag), EngServRef.ServerData.Group);
                    }
                    foreach (CheckBox item in lstExamples?.Items)
                    {
                        if (item.IsChecked == true)
                        {
                            int itemsId = Convert.ToInt32(item.Tag);
                            if (editForm && !_proxy.CheckExistence(item.Content.ToString(), EngServRef.ServerData.Example))
                                itemsId = Convert.ToInt32(_proxy.AddData(item.Content.ToString(), EngServRef.ServerData.Example));
                            _proxy.AddItemDataAsync(edit, itemsId, EngServRef.ServerData.Example);
                        }
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