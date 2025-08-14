using GradeManager.Infrastructure;
using GradeManager.Models;
using GradeManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.ViewModels
{
    public class StatsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Student> _students;
        public double Mean { get; private set; }
        public double Max { get; private set; }
        public double Min { get; private set; }

        public StatsViewModel(ObservableCollection<Student> students) { _students = students; Refresh(); }

        public void Refresh()
        {
            (double m, double mx, double mn) = StatisticsService.Summary(_students);
            Mean = m; Max = mx; Min = mn; Raise(nameof(Mean)); Raise(nameof(Max)); Raise(nameof(Min));
        }
    }
}
