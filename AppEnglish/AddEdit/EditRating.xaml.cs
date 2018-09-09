using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppEnglish.AddEdit
{
    public partial class EditRating : Window
    {
        EngServRef.EngServiceClient _proxy;
        int id;
        int user;
        int count;      //Previous rating.
        int mark;       //New rating.
        EngServRef.ServerData type;

        #region Constructors.
        //Initialization.
        public EditRating()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes '_proxy' and sets default rating.
        /// </summary>
        /// <param name="tmp">Host to be initialized.</param>
        /// <param name="id">Id of item.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="stars">The number of stars.</param>
        /// <param name="data">Type of item.</param>
        public EditRating(EngServRef.EngServiceClient tmp, int id, int userId, int? stars, EngServRef.ServerData data) : this()
        {
            _proxy = tmp;

            this.id = id;
            user = userId;
            count = stars == null? 0: Convert.ToInt32(stars);
            mark = count;
            type = data;

            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = count;
                item.Opacity = (count >= Convert.ToInt32(item.Tag)) ? 1 : 0.2;
            }
        }
        #endregion

        #region Rating.
        //Sets a rate.
        private void imgRating_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mark = ((sender as Image).Opacity == 1) ? 0 : Convert.ToInt32((sender as Image).Tag);
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = mark;
                item.Opacity = (mark != 0 && Convert.ToInt32(item.Tag) <= Convert.ToInt32((sender as Image).Tag)) ? 1 : 0.2;
            }
            btnOK.IsEnabled = count != mark;
        }
        //Visualisation.
        private void imgRating_MouseEnter(object sender, MouseEventArgs e)
        {
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = (sender as Image).Tag;
                item.Opacity = (Convert.ToInt32(item.Tag) <= Convert.ToInt32((sender as Image).Tag)) ? 0.5 : 0.2;
            }
        }
        //Visualisation.
        private void imgRating_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach (Image item in wrRating.Children)
            {
                item.ToolTip = (mark == 0) ? "N/A" : mark.ToString();
                item.Opacity = (mark != 0 && Convert.ToInt32(item.Tag) <= mark) ? 1 : 0.2;
            }
        }
        #endregion
        #region Close form (OK, Cancel).
        //Change.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _proxy.EditMark(id, user, mark, type);
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