using System;

namespace AppEnglish
{
    /// <summary>
    /// Num - position in the panel.
    /// TimeStart - the time of the beginning of phrase.
    /// TimeEnd - the time of the end of phrase.
    /// Phrase - the content of sub.
    /// </summary>
    class SubTitle
    {
        public int Num { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Phrase { get; set; }
    }
}