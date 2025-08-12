using GradeManager.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GradeManager.ViewModel
{
    class ComboBoxBarViewModel : ViewModelBase
    {
        private string displayText;
        public string DisplayText
        {
            get => displayText;
            set
            {
                displayText = value;
                OnPropertyChanged();
            }
        }

        public ComboBoxBarViewModel()
        {
            DisplayText = "Temp";
        }
    }
}
