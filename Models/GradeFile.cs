using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Models
{
    public class GradeFile
    {
        public List<Subject> Subjects { get; set; } = new();
        public List<ClassRoom> Classes { get; set; } = new();
    }
}
