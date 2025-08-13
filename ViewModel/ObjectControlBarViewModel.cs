using GradeManager.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GradeManager.ViewModel 
{
    class ObjectControlBarViewModel : ViewModelBase
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

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ObjectControlBarViewModel()
        {
            AddCommand = new RelayCommand(_ =>OnAdd());
            DeleteCommand = new RelayCommand(_ => OnDelete());
            DisplayText = "Temp";
        }


        private void OnAdd()
        {
            MessageBox.Show("추가 버튼 클릭됨");
        }

        private void OnDelete()
        {
            MessageBox.Show("삭제 버튼 클릭됨");
        }

    }
}
