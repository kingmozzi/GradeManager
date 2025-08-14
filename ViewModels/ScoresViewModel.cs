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
    public class ScoresViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Student> _students;
        private readonly ObservableCollection<Subject> _subjects;
        private readonly System.Action _onChanged;

        public ScoresViewModel(ObservableCollection<Student> students, ObservableCollection<Subject> subjects, System.Action onChanged)
        { _students = students; _subjects = subjects; _onChanged = onChanged; }

        // Cell 편집 후 호출될 공개 메서드 (View에서 EventSetter로 바인딩)
        public void OnCellEditCommitted() => _onChanged();
    }
}
