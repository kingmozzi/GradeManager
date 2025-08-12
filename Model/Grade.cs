using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Model
{
    class Grade
    {
        public int GradeNumber { get; set; }
        public ObservableCollection<ClassRoom> classRooms = new ObservableCollection<ClassRoom>();
    }
}
