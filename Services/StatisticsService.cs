using GradeManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Services
{
    public static class StatisticsService
    {
        public static double SafeAvg(IEnumerable<double> seq) => seq.Any() ? seq.Average() : 0;

        // 동적 피벗: DataTable로 변환하여 DataGrid에 바인딩 (AutoGenerateColumns)
        public static DataView BuildScoresPivot(IEnumerable<Student> students, IEnumerable<string> subjects,
                                                bool includeHeaderAverageRow = true)
        {
            var table = new DataTable();
            table.Columns.Add("이름", typeof(string));
            table.Columns.Add("번호", typeof(int));
            foreach (var s in subjects)
                table.Columns.Add(s, typeof(double));
            table.Columns.Add("평균", typeof(double));

            // 데이터 행
            foreach (var st in students.OrderBy(s => s.Number))
            {
                var row = table.NewRow();
                row["이름"] = st.Name;
                row["번호"] = st.Number;
                foreach (var s in subjects)
                    row[s] = st.Scores.TryGetValue(s, out var v) ? v : DBNull.Value;
                var vals = subjects.Select(s => st.Scores.TryGetValue(s, out var v) ? v : double.NaN)
                                    .Where(x => !double.IsNaN(x));
                row["평균"] = SafeAvg(vals);
                table.Rows.Add(row);
            }

            // 헤더 평균 행 (옵션)
            if (includeHeaderAverageRow)
            {
                var avg = table.NewRow();
                avg["이름"] = "평균";
                avg["번호"] = DBNull.Value;
                foreach (var s in subjects)
                {
                    var seq = students.Select(st => st.Scores.TryGetValue(s, out var v) ? v : double.NaN)
                                        .Where(x => !double.IsNaN(x));
                    avg[s] = SafeAvg(seq);
                }
                avg["평균"] = SafeAvg(students.Select(st =>
                {
                    var vals = subjects.Select(sb => st.Scores.TryGetValue(sb, out var v) ? v : double.NaN)
                                        .Where(x => !double.IsNaN(x));
                    return SafeAvg(vals);
                }));
                table.Rows.InsertAt(avg, 0);
            }

            return table.DefaultView;
        }

        public static (double mean, double max, double min) Summary(IEnumerable<Student> students)
        {
            var all = students.SelectMany(s => s.Scores.Values).ToList();
            if (!all.Any()) return (0, 0, 0);
            return (all.Average(), all.Max(), all.Min());
        }

        public static string GradeOf(double score) => score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }
}

