using AppEnglish.EngServRef;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppEnglish.View.Games
{
    public partial class WordsGamePlayer : Window
    {
        #region Buttons.
        private void btnGameFlashcards_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("flashcards", ServerData.Game);
            mode = WordsGameMode.Learning;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            Label word = new Label { Style = TryFindResource("lbCard") as Style, Tag = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name) };
            StackPanel st = new StackPanel();
            AddImage(_words[wordsId], null, "WordImages", 100, st, ServerData.Word, false);
            if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)?.Length > 0)
            {
                int def = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition).Length)];
                st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(def, ServerData.Definition, PropertyData.Name) });
            }
            else if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)?.Length > 0)
            {
                int trans = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation).Length)];
                st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(trans, ServerData.Translation, PropertyData.Name) });
            }
            word.Content = st;
            word.MouseDown += FlashCard_MouseDown;
            stGameContent.Children.Add(word);
            PlayGame();
        }
        private void btnGameQuizzes_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("quizzes", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            if (_words.Count < 4)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GenerateQuiezzesCard();
            PlayGame();
        }
        private void btnGamePuzzles_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("puzzles", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            if (_words.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GeneratePuzzlesCard();
            PlayGame();
        }
        private void btnGameVisualization_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("visualization", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetWordsWithImages(_catId, _type));
            if (_words.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            _puzzle = false;
            GeneratePuzzlesCard(_puzzle);
            PlayGame();
        }
        private void btnGameMatch_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("match", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            if (_words?.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GenerateMatchCard();
            PlayGame();
        }
        private void btnFindMistake_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("match", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            if (_words?.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GenerateMistakeCard();
            PlayGame();
        }
        private void btnGameAgility_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("agility", ServerData.Game);
            mode = WordsGameMode.Challenge;
            stGameContent.Style = TryFindResource("stCard") as Style;
            _words = new List<int>(_proxy.GetItemData(_catId, _type, ServerData.Word));
            List<int> synonyms = new List<int>();
            foreach (int item in _words)
            {
                string val = _proxy.GetItemProperty(item, ServerData.Word, PropertyData.Name);
                if (_proxy.GetFItems(val, ServerData.Word, PropertyData.Synonyms) != null && _proxy.GetFItems(val, ServerData.Word, PropertyData.Synonyms).Length > 0)
                    synonyms.Add(item);
            }
            _words = new List<int>(synonyms);
            if (synonyms.Count == 0)
            {
                MessageBox.Show("There are not enough words!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            GenerateAgilityCard();
            PlayGame();
        }
        private void btnGamePronunciation_Click(object sender, RoutedEventArgs e)
        {
            _game = _proxy.GetItemsId("pronunciation", ServerData.Game);
            MessageBox.Show("You will be shown the some tongue twisters. Just repeat them in order to improve your skills.\nGuess the word and than click on card in order to see the answer.\nThere is no challange mode.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            mode = WordsGameMode.Learning;

            stGame.HorizontalAlignment = HorizontalAlignment.Center;
            stGame.VerticalAlignment = VerticalAlignment.Center;
            stGameContent.Style = TryFindResource("stCard") as Style;

            if (_proxy.GetToungeTwisters() == null)
            {
                MessageBox.Show("There are no words in this category!", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            _words = new List<int>(_proxy.GetToungeTwisters());
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            Label word = new Label { Style = TryFindResource("lbCard") as Style, Content = new TextBlock { Text = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name), Style = TryFindResource("txtCard") as Style } };
            word.MouseDown += Pronunciation_MouseDown;
            stGameContent.Children.Add(word);

            PlayGame();
        }

        //Pronunciation.
        private void Pronunciation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Random rnd = new Random();
            int wordsId = rnd.Next(0, _words.Count);
            (sender as Label).Content = new TextBlock { Text = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name), Style = TryFindResource("txtCard") as Style };
        }
        //Flashcards.
        private void FlashCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (state == CardState.Question)
            {
                string word = (sender as Label).Tag.ToString();
                (sender as Label).Content = new Label { Content = word, Style = TryFindResource("lbFormNormal") as Style };
            }
            else
            {
                Random rnd = new Random();
                int wordsId = rnd.Next(0, _words.Count);
                stGameContent.Children.Clear();
                Label word = new Label { Style = TryFindResource("lbCard") as Style, Tag = _proxy.GetItemProperty(_words[wordsId], ServerData.Word, PropertyData.Name) };
                StackPanel st = new StackPanel();
                AddImage(_words[wordsId], null, "WordImages", 100, st, ServerData.Word, false);
                if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)?.Length > 0)
                {
                    int def = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Definition).Length)];
                    st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(def, ServerData.Definition, PropertyData.Name) });
                }
                else if (_proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)?.Length > 0)
                {
                    int trans = _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation)[rnd.Next(0, _proxy.GetItemData(_words[wordsId], ServerData.Word, ServerData.Translation).Length)];
                    st.Children.Add(new TextBlock { Style = TryFindResource("txtCard") as Style, Text = _proxy.GetItemProperty(trans, ServerData.Translation, PropertyData.Name) });
                }
                word.Content = st;
                word.MouseDown += FlashCard_MouseDown;
                stGameContent.Children.Add(word);
            }
            state = state == CardState.Question ? CardState.Answer : CardState.Question;
        }
        //Quizzes.
        private void RbQuizzes_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Content.ToString() == (sender as RadioButton).Tag.ToString())
            {
                MessageBox.Show("Correct");
                AddScore();
            }
            else
            {
                MessageBox.Show("Wrong");
                SubtractScore();
            }

            GenerateQuiezzesCard();
        }
        //Puzzles.
        private void BtnSetChar_Click(object sender, RoutedEventArgs e)
        {
            bool stop = true;
            bool win = true;
            Button button = new Button();
            foreach (Button item in wrWords.Children)
            {
                if (item.Content.ToString() == "" && item.IsEnabled)
                {
                    button = item;
                    break;
                }
            }

            button.Content = (sender as Button).Content;
            if (_hints)
            {
                if (button.Content.ToString() == button.Tag.ToString() && button.IsEnabled)
                {
                    button.Foreground = Brushes.ForestGreen;
                    button.Background = Brushes.LightGreen;
                    button.BorderBrush = Brushes.ForestGreen;
                }
                else
                {
                    button.Foreground = Brushes.Red;
                    button.Background = Brushes.Pink;
                    button.BorderBrush = Brushes.Red;
                }
            }
            foreach (Button item in wrWords.Children)
            {
                if (item.Content.ToString() == "" && item.IsEnabled)
                {
                    button = item;
                    stop = false;
                    break;
                }
                if (item.Content.ToString() != item.Tag.ToString())
                    win = false;
            }
            if (stop)
            {
                MessageBox.Show(win ? "Correct" : "Wrong");
                if (win)
                    AddScore();
                else
                    SubtractScore();
                GeneratePuzzlesCard(_puzzle);
            }
        }
        private void ClearPuzzle_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Content = "";
            CheckBoxPuzzle_Click(null, null);
        }
        //Match.
        private void ContainerMatch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_wordMatch == null)
            {
                (sender as Label).IsEnabled = false;
                (sender as Label).Background = Brushes.LightSteelBlue;
                _wordMatch = (sender as Label).Tag.ToString();
            }
            else
            {
                if ((sender as Label).Tag.ToString() == _wordMatch)
                {
                    (sender as Label).IsEnabled = false;
                    (sender as Label).Background = Brushes.LightGreen;
                    (sender as Label).BorderBrush = Brushes.ForestGreen;
                    foreach (Label item in wrWords.Children)
                    {
                        if (item.Tag.ToString() == _wordMatch && item != (sender as Label))
                        {
                            item.Background = Brushes.LightGreen;
                            item.BorderBrush = Brushes.ForestGreen;
                            break;
                        }
                    }
                    AddScore();
                }
                else
                {
                    foreach (Label item in wrWords.Children)
                    {
                        if (item.Tag.ToString() == _wordMatch && item != (sender as Label))
                        {
                            item.IsEnabled = true;
                            item.Background = Brushes.White;
                            break;
                        }
                    }
                    SubtractScore();
                    MessageBox.Show($"This is not right.\n You have got {_lives} left.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                _wordMatch = null;
            }

            bool win = true;
            foreach (Label item in wrWords.Children)
            {
                if (item.IsEnabled)
                {
                    win = false;
                    break;
                }
            }
            if (win)
            {
                MessageBox.Show("You have done this game!", "Great", MessageBoxButton.OK, MessageBoxImage.Information);
                GenerateMatchCard();
            }
        }
        //Find a mistake.
        private void BtnWrong_Click(object sender, RoutedEventArgs e)
        {
            if (!valid)
                AddScore();
            else
            {
                MessageBox.Show($"You are wrong.\nThe correct word is {(sender as Button).Tag}.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                SubtractScore();
            }
            GenerateMistakeCard();
        }
        private void BtnValid_Click(object sender, RoutedEventArgs e)
        {
            if (valid)
                AddScore();
            else
            {
                MessageBox.Show("You are wrong.", "Wrong", MessageBoxButton.OK, MessageBoxImage.Error);
                SubtractScore();
            }
            GenerateMistakeCard();
        }
        //Agility.
        private void BtnFirst_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag != null)
                AddScore();
            else
            {
                MessageBox.Show("It was a wrong answer.", "Nope", MessageBoxButton.OK, MessageBoxImage.Error);
                SubtractScore();
            }
            GenerateAgilityCard();
        }

        private void btnEndGame_Click(object sender, RoutedEventArgs e)
        {
            if (mode == WordsGameMode.Challenge)
                ShowResults();
            else
                Close();
        }
        void ShowResults()
        {
            stGame.Visibility = Visibility.Collapsed;
            _level++;
            _proxy.AddScore(_score, Convert.ToInt32(_user), Convert.ToInt32(_game));
            _proxy.EditData(Convert.ToInt32(_user), _level.ToString(), ServerData.User, PropertyData.Level);
            Dictionary<int, int> lst = _proxy.GetHighScores(Convert.ToInt32(_game));
            int count = 1;
            foreach (int item in lst.Keys)
            {
                StackPanel st = new StackPanel { Orientation = Orientation.Horizontal };
                st.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, FontWeight = FontWeights.Bold, Padding = new Thickness(5), Content = count });
                AddImage(item, "Wolf.png", "Avatars", 35, st, ServerData.User, false);

                Label login = new Label { Style = TryFindResource("lbFormNormal") as Style, Margin = new Thickness(50, 5, 50, 5), Padding = new Thickness(5), Content = _proxy.GetItemProperty(Convert.ToInt32(item), ServerData.User, PropertyData.Login) };
                if (item == _user)
                    st.Background = Brushes.LightGreen;
                st.Children.Add(login);

                StackPanel panel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                panel.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, Padding = new Thickness(0), Margin = new Thickness(5), FontSize = 14, FontWeight = FontWeights.Bold, Content = "Score:" });
                panel.Children.Add(new Label { Style = TryFindResource("lbFormNormal") as Style, Margin = new Thickness(0), Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 14, Content = lst[item] });
                st.Children.Add(panel);
                lstHighScores.Items.Add(st);
                count++;
                if (count - 1 != lst.Count)
                    lstHighScores.Items.Add(new Separator { Background = Brushes.PowderBlue });
            }
            stResuts.Visibility = Visibility.Visible;
        }
        #endregion
    }
}