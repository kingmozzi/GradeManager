using GradeManager.Infrastructure;
using GradeManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.ViewModels
{
    public class StudentsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Student> _students;
        private readonly ObservableCollection<Subject> _subjects;
        private readonly System.Action _onChanged;

        public ObservableCollection<Student> Students => _students;
        public Student? Selected { get; set; }

        public RelayCommand AddCmd { get; }
        public RelayCommand RemoveCmd { get; }

        public StudentsViewModel(ObservableCollection<Student> students, ObservableCollection<Subject> subjects, System.Action onChanged)
        {
            _students = students; _subjects = subjects; _onChanged = onChanged;
            AddCmd = new RelayCommand(Add);
            RemoveCmd = new RelayCommand(Remove, () => Selected != null);
        }

        private void Add()
        {
            var nextNo = _students.Any() ? _students.Max(s => s.Number) + 1 : 1;
            var st = new Student { Name = "새 학생", Number = nextNo, Grade = 1, Class = 1 };
            foreach (var s in _subjects) st.Scores.TryAdd(s.Name, 0);
            _students.Add(st);
            _onChanged();
        }

        private void Remove()
        {
            if (Selected == null) return;
            _students.Remove(Selected);
            _onChanged();
        }
    }
}
