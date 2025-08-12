using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Model
{
    class ClassRoom
    {
        public int ClassNumber { get; set; }
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
    }
}
