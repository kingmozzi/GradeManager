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
    class GradeViewModel : ViewModelBase
    {
        public ObservableCollection<Grade> Grades { get; set; } = new ObservableCollection<Grade>();

        public GradeViewModel()
        {
            Grades.Add(new Grade
            {
                GradeNumber = 1
            });
        }


        private Grade selectedGrade;

        public Grade SelectedGrade
        {
            get => selectedGrade;
            set
            {
                selectedGrade = value;
                OnPropertyChanged();
            }
        }
    }
}
