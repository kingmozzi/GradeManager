using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Model
{
    class ScoreEntry : INotifyPropertyChanged
    {
        public string Subject { get; set; }

        private int _score;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Score)));
            }
        }
    }
}
