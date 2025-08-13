using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Model
{
    class Student : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public ObservableCollection<ScoreEntry> Scores { get; set; } = new();
        public double AverageScore => Scores.Count == 0 ? 0 : Scores.Average(s => s.Score);
        public event PropertyChangedEventHandler? PropertyChanged;

        public Student()
        {

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
