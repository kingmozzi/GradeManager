using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Model
{
    class Student
    {
        public string Name { get; set; }
        public int StudentId { get; set; }

        public ObservableCollection<SubjectScore> Scores { get; set; } = new ObservableCollection<SubjectScore>();
        public double AverageScore => Scores.Any() ? Scores.Average(s => s.Score) : 0;

        public int? GetScore(string subjectName)
        {
            return Scores.FirstOrDefault(s => s.Subject.Name == subjectName)?.Score;
        }


    }
}
