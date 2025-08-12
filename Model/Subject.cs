using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Model
{
    public class Subject
    {
        public string Name { get; set; }
    }

    public class SubjectScore
    {
        public Subject Subject { get; set; }
        public int Score { get; set; }
    }
}

