using GradeManager.ViewModels;
using System.Data;
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
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Threading;

namespace GradeManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow() 
    { 
        InitializeComponent();
        Loaded += async (_, __) =>
        {
            if (DataContext is MainViewModel vm)
            {
                await vm.LoadOnStartupAsync();
            }

        };
    }

    // 생성되는 각 열의 속성 제어
    private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var header = e.Column.Header?.ToString();
        if (header == "이름" || header == "번호")
            e.Column.IsReadOnly = true;

        if (header == "평균" && e.Column is DataGridTextColumn dgc)
        {
            // 평균 컬럼: 표시 전용
            if (dgc.Binding is Binding b) b.Mode = BindingMode.OneWay;
            else dgc.Binding = new Binding(header) { Mode = BindingMode.OneWay };
            e.Column.IsReadOnly = true;
        }
    }

    // 편집 시작 시점에 평균행/키열 보호
    private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (e.Row.Item is DataRowView row && row["이름"]?.ToString() == "평균")
        {
            e.Cancel = true; // 평균 행 전체 편집 금지
            return;
        }
        var header = (e.Column.Header ?? "").ToString();
        if (header == "이름" || header == "번호")
            e.Cancel = true; // 키 보호
    }

    // 선택된 행을 VM.SelectedStudent 에 매핑 (학생 삭제용)
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        var grid = (DataGrid)sender;
        if (grid.SelectedItem is not DataRowView row) { vm.SelectedStudent = null; return; }
        var name = row["이름"]?.ToString();
        var numberStr = row["번호"]?.ToString();
        if (name == null || !int.TryParse(numberStr, out var no)) { vm.SelectedStudent = null; return; }
        vm.SelectByKey(name, no);
    }

    // 점수 셀 편집 종료: 0~100 검증 → VM 반영
    private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        if (e.Row.Item is not DataRowView row) return;

        var header = e.Column.Header?.ToString() ?? "";
        if (header is "평균" or "이름" or "번호") return;

        var name = row["이름"]?.ToString() ?? "";
        if (!int.TryParse(row["번호"]?.ToString() ?? "0", out var no)) return;
        if (e.EditingElement is not TextBox tb) return;

        var original = row[header]?.ToString();   // 원래 표시값
        var newText = tb.Text;

        if (!double.TryParse(newText, out var v) || v < 0 || v > 100)
        {
            MessageBox.Show("점수는 0~100 사이여야 합니다.");
            e.Cancel = true;               // 커밋 취소
            tb.Text = original ?? "";      // 화면 즉시 원복
            tb.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            return;
        }

        // 커밋 이후에 안전 반영
        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (DataContext is MainViewModel vm2)
                vm2.ApplyCellEdit(name, no, header, newText); // 내부에서 RebuildPivot()
        }), DispatcherPriority.Background);
    }
}

