using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Models
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty; // 이름
        [Range(1, 9999)]
        public int Number { get; set; }                 // 번호
        [Range(1, 12)]
        public int Grade { get; set; }                  // 학년
        [Range(1, 100)]
        public int Class { get; set; }                  // 반

        // 과목별 점수 (SubjectName -> Score)
        public Dictionary<string, double> Scores { get; set; } = new();
    }
}
