using GradeManager.Model;
using GradeManager.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.ViewModel
{
    class StudentViewModel : ViewModelBase
    {
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        public StudentViewModel()
        {

        }

        private Student selectedStudent;

        public Student SelectedStudent
        {
            get => selectedStudent;
            set
            {
                selectedStudent = value;
                OnPropertyChanged();
            }
        }
    }
}
