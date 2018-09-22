using System;
using System.Windows;

namespace AppEnglish.View.Games
{
    public partial class TimeConverter : Window
    {
        TimeSpan value = new TimeSpan();

        public TimeConverter()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            value = DateTime.Now.TimeOfDay;
            slTime.Value = (value.Hours + value.Minutes / 100.0 + value.Seconds / 10000.0);
        }

        private void slTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string hour24 = TimeSpan.FromHours(slTime.Value).Hours < 10? $"0{TimeSpan.FromHours(slTime.Value).Hours}": TimeSpan.FromHours(slTime.Value).Hours.ToString();
            string hourAmPm = TimeSpan.FromHours(slTime.Value).Hours <= 12 ? TimeSpan.FromHours(slTime.Value).Hours.ToString() : (TimeSpan.FromHours(slTime.Value).Hours % 12).ToString();
            string minutes = TimeSpan.FromHours(slTime.Value).Minutes < 10 ? $"0{TimeSpan.FromHours(slTime.Value).Minutes}" : TimeSpan.FromHours(slTime.Value).Minutes.ToString();
            string sec = TimeSpan.FromHours(slTime.Value).Seconds < 10 ? $"0{TimeSpan.FromHours(slTime.Value).Seconds}" : TimeSpan.FromHours(slTime.Value).Seconds.ToString();
            string amPM = TimeSpan.FromHours(slTime.Value).Hours < 12? "AM":"PM";
            if ((TimeSpan.FromHours(slTime.Value).Hours == 0 || TimeSpan.FromHours(slTime.Value).Hours == 24) && TimeSpan.FromHours(slTime.Value).Minutes == 0 && TimeSpan.FromHours(slTime.Value).Seconds == 0)
                amPM = "MIDNIGHT";
            if (TimeSpan.FromHours(slTime.Value).Hours == 12 && TimeSpan.FromHours(slTime.Value).Minutes == 0 && TimeSpan.FromHours(slTime.Value).Seconds == 0)
                amPM = "NOON";

            txtAM.Text = $"{hourAmPm}:{minutes} {amPM}";
            txt24.Text = $"{hour24}:{minutes}:{sec}";
        }
    }
}