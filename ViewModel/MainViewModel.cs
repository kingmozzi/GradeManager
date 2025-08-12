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
    class MainViewModel : ViewModelBase
    {
        public GradeViewModel GradeVM { get; set; } = new GradeViewModel();
        public ClassRoomViewModel ClassVM { get; set; } = new ClassRoomViewModel();
        public StudentViewModel StudentVM { get; set; } = new StudentViewModel();
        public ObservableCollection<ObjectControlBarViewModel> ControlBars { get; }
        public ObservableCollection<ComboBoxBarViewModel> ComboBoxBars { get; }
        public ObservableCollection<Subject> Subjects { get; set; } = new ObservableCollection<Subject>();
        

        public MainViewModel()
        {
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

            ComboBoxBars[0].DisplayText = "학년";
            ComboBoxBars[1].DisplayText = "반";

            var kor = new Subject { Name = "국어" };
            var math = new Subject { Name = "수학"};
            var eng = new Subject { Name = "영어" };
            Subjects.Add(kor);
            Subjects.Add(math);
            Subjects.Add(eng);
            StudentVM.Students.Add(new Student
            {
                Name = "홍길",
                StudentId = 1,
                Scores = new ObservableCollection<SubjectScore>
                {
                    new SubjectScore {Subject = kor, Score = 90},
                    new SubjectScore {Subject = math, Score = 80},
                    new SubjectScore {Subject = eng, Score = 90}
                }
            });
            StudentVM.Students.Add(new Student
            {
                Name = "홍아",
                StudentId = 2,
                Scores = new ObservableCollection<SubjectScore>
                {
                    new SubjectScore {Subject = kor, Score = 90},
                    new SubjectScore {Subject = math, Score = 80},
                    new SubjectScore {Subject = eng, Score = 90}
                }
            });
        }

        public void RemoveSubject(string subjectName, ObservableCollection<Student> students)
        {
         
        }
    }
}
