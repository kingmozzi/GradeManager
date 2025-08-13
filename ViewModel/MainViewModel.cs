using GradeManager.Model;
using GradeManager.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public GradeViewModel GradeVM { get; set; } = new GradeViewModel();
        public ClassRoomViewModel ClassVM { get; set; } = new ClassRoomViewModel();
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<ObjectControlBarViewModel> ControlBars { get; }
        public ObservableCollection<ComboBoxBarViewModel> ComboBoxBars { get; }
        public ObservableCollection<Subject> Subjects { get; set; }// = new ObservableCollection<Subject>();

        public MainViewModel()
        {
            Students = new ObservableCollection<Student>();

            GradeVM.PropertyChanged += (s, e) =>
            {
                var selectedGrade = GradeVM.SelectedGrade;
                ClassVM.ClassRooms = selectedGrade?.classRooms ?? new ObservableCollection<ClassRoom>();
            };

            ControlBars = new ObservableCollection<ObjectControlBarViewModel>()
            {
                new ObjectControlBarViewModel(),
                new ObjectControlBarViewModel(),
                new ObjectControlBarViewModel(),
                new ObjectControlBarViewModel()
            };

            ControlBars[0].DisplayText = "학년";
            ControlBars[1].DisplayText = "반";
            ControlBars[2].DisplayText = "학생";
            ControlBars[3].DisplayText = "과목";

            ComboBoxBars = new ObservableCollection<ComboBoxBarViewModel>()
            {
                new ComboBoxBarViewModel(),
                new ComboBoxBarViewModel()
            };


            Subjects = new ObservableCollection<Subject>
            {
                new Subject { Name = "국어" },
                new Subject { Name = "수학" },
                new Subject { Name = "영어" }
            };

            ControlBars[2].AddCommand = new RelayCommand(execute => AddStudent());


        }

        private void AddGrade()
        {

        }

        private void DeleteGrade()
        {

        }

        private void AddStudent()
        {
            var tempScores = new ObservableCollection<ScoreEntry>();
            foreach (var subject in Subjects)
            {
                var tempScore = new ScoreEntry();
                tempScore.Subject = subject.Name;
                tempScore.Score = 0;
                tempScores.Add(tempScore);
            }
            Students.Add(new Student
            {
                Name = "NEW STUDENT",
                StudentId = 9999,
                Scores = tempScores
            });
        }

        private void DeleteStudent()
        {

        }
    }
}
