using GradeManager.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GradeManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

        var vm = DataContext as MainViewModel;

        StudentDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding("Name")
        });

        StudentDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "ID",
            Binding = new Binding("StudentId")
        });

        foreach (var subject in vm.Subjects)
        {

            var binding = new Binding($"[{subject.Name}]");

            StudentDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = subject.Name,
                Binding = binding
            });

        }

        StudentDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Average",
            Binding = new Binding("AverageScore")
        });

    }
}