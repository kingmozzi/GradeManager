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
    class ClassRoomViewModel : ViewModelBase
    {
        public ObservableCollection<ClassRoom> ClassRooms { get; set; } = new ObservableCollection<ClassRoom>();
    
        private ClassRoom selectedClass;
        public ClassRoom SelectedClass
        {
            get => selectedClass;
            set
            {
                selectedClass = value;
                OnPropertyChanged();
            }
        }

    }
}
