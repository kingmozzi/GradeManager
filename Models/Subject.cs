using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Models
{
    public class Subject : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name { get => _name; set { if (_name != value) { _name = value; PropertyChanged?.Invoke(this, new("Name")); } } }
        public Subject() { }
        public Subject(string name) { _name = name; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public override string ToString() => Name;
    }
}
