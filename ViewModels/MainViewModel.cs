using GradeManager.Infrastructure;
using GradeManager.Models;
using GradeManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GradeManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGradeRepository _repo;

        // 고정 저장 경로 (디버그/릴리스 무관)
        private static readonly string StorePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "GradeManager", "grades.json");

        // 컬렉션
        public ObservableCollection<Subject> Subjects { get; } = new();
        public ObservableCollection<Student> Students { get; } = new();

        // 학년 -> {반} (빈 반 허용, 학년은 반이 없어도 유지)
        private readonly Dictionary<int, SortedSet<int>> _gradeClasses = new();

        public ObservableCollection<int> Grades { get; } = new();
        public ObservableCollection<int> Classes { get; } = new();

        private int _selectedGrade;
        public int SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                if (_selectedGrade == value) return;
                _selectedGrade = value;
                Raise();
                BuildClassListForGrade();
                RebuildPivot();
            }
        }

        private int _selectedClass;
        public int SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (_selectedClass == value) return;
                _selectedClass = value;
                Raise();
                RebuildPivot();
            }
        }

        // 학년/반 입력
        public string NewGrade { get; set; } = "1";
        public string NewClass { get; set; } = "1";

        // 검색
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { if (_searchText != value) { _searchText = value; Raise(); RebuildPivot(); } }
        }

        // 피벗 뷰
        private DataView? _pivotView;
        public DataView? PivotView
        {
            get => _pivotView;
            private set { _pivotView = value; Raise(); }
        }

        // 과목/학생 선택
        private Subject? _selectedSubject;
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set { _selectedSubject = value; Raise(); RemoveSubjectCmd?.RaiseCanExecuteChanged(); }
        }

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set { _selectedStudent = value; Raise(); RemoveStudentCmd?.RaiseCanExecuteChanged(); }
        }

        private DataRowView? _selectedRow;
        public DataRowView? SelectedRow
        {
            get => _selectedRow;
            set
            {
                _selectedRow = value;
                if (_selectedRow == null)
                {
                    SelectedStudent = null;
                    RemoveStudentCmd?.RaiseCanExecuteChanged();
                    return;
                }
                var name = _selectedRow["이름"]?.ToString();
                var noStr = _selectedRow["번호"]?.ToString();
                SelectedStudent = (name != null && int.TryParse(noStr, out var no))
                                  ? FindStudent(name, no)
                                  : null;
                RemoveStudentCmd?.RaiseCanExecuteChanged();
            }
        }

        public string PendingSubjectName { get; set; } = string.Empty;

        // 명령
        public RelayCommand AddStudentCmd { get; }
        public RelayCommand RemoveStudentCmd { get; }
        public RelayCommand AddSubjectCmd { get; }
        public RelayCommand RemoveSubjectCmd { get; }
        public RelayCommand RenameSubjectCmd { get; }
        public RelayCommand AddGradeCmd { get; }
        public RelayCommand RemoveGradeCmd { get; }
        public RelayCommand AddClassCmd { get; }
        public RelayCommand RemoveClassCmd { get; }
        public RelayCommand SaveCmd { get; }

        public MainViewModel() : this(new JsonGradeRepository()) { }

        public MainViewModel(IGradeRepository repo)
        {
            _repo = repo;

            // 1) 명령 먼저 생성 (NRE 방지)
            AddStudentCmd = new RelayCommand(AddStudent, CanAddStudent);
            RemoveStudentCmd = new RelayCommand(RemoveStudent, () => SelectedStudent != null);
            AddSubjectCmd = new RelayCommand(AddSubject);
            RemoveSubjectCmd = new RelayCommand(RemoveSubject, () => SelectedSubject != null);
            RenameSubjectCmd = new RelayCommand(RenameSubject);
            AddGradeCmd = new RelayCommand(AddGrade);
            RemoveGradeCmd = new RelayCommand(RemoveGrade);
            AddClassCmd = new RelayCommand(AddClass);
            RemoveClassCmd = new RelayCommand(RemoveClass);
            SaveCmd = new RelayCommand(async () => await SaveAsync());

            // 2) 데이터 준비
            Seed();
            RebuildIndexes();
            EnsureDefaultGradeClass(); // 1학년 1반 보장 + 콤보/선택 설정
            RebuildPivot();
        }

        // 앱 시작 시 자동 로드(버튼 없이 사용)
        public async Task LoadOnStartupAsync()
        {
            var data = await _repo.LoadAsync(StorePath);
            Subjects.Clear(); foreach (var s in data.Subjects) Subjects.Add(new Subject(s.Name));
            Students.Clear(); foreach (var cls in data.Classes) foreach (var st in cls.Students) Students.Add(st);
            RebuildIndexes();
            EnsureDefaultGradeClass();
            RebuildPivot();
        }

        // ====== 내부 로직 ======

        private void Seed()
        {
            if (Subjects.Count == 0)
            { Subjects.Add(new Subject("국어")); Subjects.Add(new Subject("수학")); Subjects.Add(new Subject("영어")); }

            if (!Students.Any())
            {
                for (int i = 1; i <= 6; i++)
                {
                    var st = new Student { Name = $"홍길-{i}", Number = i, Grade = 1, Class = 1 };
                    st.Scores["국어"] = 90; st.Scores["수학"] = 80; st.Scores["영어"] = 90;
                    Students.Add(st);
                }
            }
        }

        private void RebuildIndexes()
        {
            _gradeClasses.Clear();
            foreach (var g in Students.GroupBy(s => s.Grade))
                _gradeClasses[g.Key] = new SortedSet<int>(g.Select(x => x.Class));
        }

        private void BuildGradeList()
        {
            Grades.Clear();
            foreach (var g in _gradeClasses.Keys.OrderBy(x => x)) Grades.Add(g);
            if (!Grades.Contains(SelectedGrade) && Grades.Any()) SelectedGrade = Grades.First();
        }

        private void BuildClassListForGrade()
        {
            var prev = SelectedClass;

            Classes.Clear();
            if (_gradeClasses.TryGetValue(SelectedGrade, out var set))
                foreach (var c in set) Classes.Add(c);

            // 선택값 보정
            if (!Classes.Contains(prev))
                SelectedClass = Classes.Any() ? Classes.First() : 0;

            AddStudentCmd?.RaiseCanExecuteChanged();
            RemoveStudentCmd?.RaiseCanExecuteChanged();
            RemoveClassCmd?.RaiseCanExecuteChanged();
        }

        private void EnsureDefaultGradeClass()
        {
            // 1학년/1반 보장
            if (!_gradeClasses.ContainsKey(1))
                _gradeClasses[1] = new SortedSet<int>();
            if (!_gradeClasses[1].Contains(1))
                _gradeClasses[1].Add(1);

            BuildGradeList();
            SelectedGrade = 1;
            BuildClassListForGrade();
            if (!Classes.Contains(1)) Classes.Add(1);
            SelectedClass = 1;

            AddStudentCmd?.RaiseCanExecuteChanged();
            RemoveStudentCmd?.RaiseCanExecuteChanged();
        }

        public Student? FindStudent(string name, int number)
            => Students.FirstOrDefault(s => s.Grade == SelectedGrade && s.Class == SelectedClass &&
                                            s.Name == name && s.Number == number);

        private string NextUniqueSubjectName(string baseName)
        {
            if (!Subjects.Any(s => s.Name == baseName)) return baseName;
            int i = 2; while (Subjects.Any(s => s.Name == $"{baseName}{i}")) i++;
            return $"{baseName}{i}";
        }

        private bool CanAddStudent()
            => _gradeClasses.TryGetValue(SelectedGrade, out var set) && set.Contains(SelectedClass);

        // ====== 커맨드핸들러 ======

        private void AddStudent()
        {
            // 반이 존재해야만 추가 가능 (CanExecute 로 제한)
            var nextNo = Enumerable.Range(1, int.MaxValue)
                .First(n => !Students.Any(s => s.Grade == SelectedGrade && s.Class == SelectedClass && s.Number == n));
            var st = new Student { Name = "새 학생", Number = nextNo, Grade = SelectedGrade, Class = SelectedClass };
            foreach (var sb in Subjects) st.Scores.TryAdd(sb.Name, 0);
            Students.Add(st);
            SelectedStudent = st;
            RebuildPivot();
        }

        private void RemoveStudent()
        {
            if (SelectedStudent == null) return;
            Students.Remove(SelectedStudent);
            SelectedStudent = null;
            RebuildIndexes();
            RebuildPivot();
        }

        private void AddSubject()
        {
            var name = NextUniqueSubjectName("새과목");
            var subj = new Subject(name);
            Subjects.Add(subj);
            foreach (var st in Students) st.Scores.TryAdd(subj.Name, 0);
            SelectedSubject = subj;
            RebuildPivot();
        }

        private void RemoveSubject()
        {
            if (SelectedSubject == null) return;
            var name = SelectedSubject.Name;
            foreach (var st in Students) st.Scores.Remove(name);
            Subjects.Remove(SelectedSubject);
            SelectedSubject = null;
            RebuildPivot();
        }

        private void RenameSubject()
        {
            if (SelectedSubject == null) return;
            var oldName = SelectedSubject.Name;
            var newName = string.IsNullOrWhiteSpace(PendingSubjectName) ? oldName : PendingSubjectName.Trim();
            if (oldName == newName) return;
            if (Subjects.Any(s => s.Name == newName)) return; // 중복 방지

            foreach (var st in Students)
                if (st.Scores.TryGetValue(oldName, out var v))
                { st.Scores.Remove(oldName); st.Scores[newName] = v; }

            SelectedSubject.Name = newName; // INotifyPropertyChanged 반영
            CollectionViewSource.GetDefaultView(Subjects)?.Refresh(); // 콤보 갱신

            PendingSubjectName = string.Empty;
            RebuildPivot();
        }

        // View에서 점수 편집 완료 후 호출 (0~100 범위는 View에서 유효성 검사, 여기서는 최종 clamp)
        public void ApplyCellEdit(string studentName, int studentNumber, string columnHeader, string? newText)
        {
            var subj = Subjects.FirstOrDefault(s => s.Name == columnHeader);
            if (subj == null) return;
            if (!double.TryParse(newText, out var val)) return;
            val = Math.Max(0, Math.Min(100, val));

            var st = Students.FirstOrDefault(s =>
                s.Grade == SelectedGrade && s.Class == SelectedClass &&
                s.Name == studentName && s.Number == studentNumber);

            if (st == null) return;
            st.Scores[subj.Name] = val;
            RebuildPivot();
        }

        private int NextGradeNumber()
        {
            if (_gradeClasses.Count == 0) return 1;
            int n = 1; while (_gradeClasses.ContainsKey(n)) n++; return n;
        }

        

        private void AddGrade()
        {
            int g = NextGradeNumber();                    // ← NewGrade 무시하고 자동 증가
            if (!_gradeClasses.ContainsKey(g))
                _gradeClasses[g] = new SortedSet<int>();
            if (!Grades.Contains(g)) Grades.Add(g);
            SelectedGrade = g;                            // 콤보/그리드 갱신
            BuildClassListForGrade();
        }

        private void RemoveGrade()
        {
            var g = SelectedGrade;
            if (!_gradeClasses.ContainsKey(g)) return;

            // 해당 학년 학생 모두 제거
            var toRemove = Students.Where(s => s.Grade == g).ToList();
            foreach (var s in toRemove) Students.Remove(s);

            _gradeClasses.Remove(g);
            BuildGradeList();

            if (!Grades.Any())
            {
                EnsureDefaultGradeClass();               // 최소 1학년 1반 유지
            }
            else
            {
                SelectedGrade = Grades.First();
                BuildClassListForGrade();
                if (Classes.Any()) SelectedClass = Classes.First();
            }

            RebuildPivot();
        }

        private int NextClassNumberFor(int g)
        {
            if (!_gradeClasses.TryGetValue(g, out var set) || set.Count == 0) return 1;
            int n = 1; while (set.Contains(n)) n++; return n;
        }

        private void AddClass()
        {
            int g = SelectedGrade;                        // ← 현재 학년에 추가
            if (g <= 0) g = 1;
            if (!_gradeClasses.ContainsKey(g))
            {
                _gradeClasses[g] = new SortedSet<int>();
                if (!Grades.Contains(g)) Grades.Add(g);
            }

            int c = NextClassNumberFor(g);               // ← 자동 증가(1,2,3…)
            if (_gradeClasses[g].Add(c))
            {
                if (g == SelectedGrade && !Classes.Contains(c)) Classes.Add(c);
                SelectedGrade = g;
                SelectedClass = c;
                AddStudentCmd?.RaiseCanExecuteChanged();
            }
        }

        private void RemoveClass()
        {
            var g = SelectedGrade;
            var c = SelectedClass;

            if (!_gradeClasses.TryGetValue(g, out var set) || !set.Contains(c)) return;

            // 1) 해당 반 학생들 제거
            var toRemove = Students.Where(s => s.Grade == g && s.Class == c).ToList();
            foreach (var s in toRemove) Students.Remove(s);

            // 2) 인덱스에서 반 제거 (학년은 남김)
            set.Remove(c);

            // 3) 콤보/선택 보정 + 버튼 활성화 갱신
            BuildClassListForGrade();

            // 4) 표 재빌드
            RebuildPivot();
        }

        private async Task SaveAsync()
        {
            // _gradeClasses + 실제 Students 모두 반영하여 저장
            var pairs = new HashSet<(int g, int c)>(
                _gradeClasses.SelectMany(kv => kv.Value.Select(cls => (kv.Key, cls)))
            );

            foreach (var grp in Students.GroupBy(s => new { s.Grade, s.Class }))
                pairs.Add((grp.Key.Grade, grp.Key.Class));

            var data = new GradeFile
            {
                Subjects = Subjects.ToList(),
                Classes = pairs
                    .OrderBy(p => p.g).ThenBy(p => p.c)
                    .Select(p => new ClassRoom
                    {
                        Grade = p.g,
                        Class = p.c,
                        Students = Students
                            .Where(s => s.Grade == p.g && s.Class == p.c)
                            .OrderBy(s => s.Number)
                            .ToList()
                    })
                    .ToList()
            };

            await _repo.SaveAsync(StorePath, data);
        }

        private async Task LoadAsync()
        {
            var data = await _repo.LoadAsync(StorePath);
            Subjects.Clear(); foreach (var s in data.Subjects) Subjects.Add(new Subject(s.Name));
            Students.Clear(); foreach (var cls in data.Classes) foreach (var st in cls.Students) Students.Add(st);
            RebuildIndexes();
            BuildGradeList();
            if (Grades.Any()) SelectedGrade = Grades.First();
            BuildClassListForGrade();
            if (Classes.Any()) SelectedClass = Classes.First();
            RebuildPivot();
        }

        private void RebuildPivot()
        {
            var subjects = Subjects.Select(s => s.Name).ToList();
            var filtered = Students.Where(s => s.Grade == SelectedGrade && s.Class == SelectedClass);
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(s => s.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase));

            PivotView = StatisticsService.BuildScoresPivot(filtered, subjects, includeHeaderAverageRow: true);
        }

        // 호환용(이벤트 방식 사용할 경우): 유지해도 무해
        public void SelectByKey(string name, int number)
        {
            SelectedStudent = Students.FirstOrDefault(s =>
                s.Grade == SelectedGrade && s.Class == SelectedClass &&
                s.Name == name && s.Number == number);
            Raise(nameof(SelectedStudent));
            RemoveStudentCmd?.RaiseCanExecuteChanged();
        }
    }
}
