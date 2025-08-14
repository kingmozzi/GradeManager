using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Models
{
    public class ClassRoom
    {
        public int Grade { get; set; }
        public int Class { get; set; }
        public List<Student> Students { get; set; } = new();
        public string Key => $"{Grade}-{Class}"; // 고유키
    }
}
