using System.Windows;
using GeneratorPP.WPF.Misc;
using GeneratorPP.WPF.Mvvm.Page1;

namespace GeneratorPP.WPF.Mvvm.MainWindow;

/// <summary>
/// The view model for the main window of the application.
/// </summary>
public partial class MainWindowViewModel : BaseViewModel, IDropFilesEnabled
{
    #region Constructors & Initialization

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindowViewModel()
    {
        GoToPage1();
    }

    #endregion Constructors & Initialization

    #region Input file selection via Drag & Drop

    /// <summary>
    /// Allows Drag & Drop of input file when on Page1.
    /// </summary>
    public void OnFilesDropped(string[] files)
    {
        if (Content is Page1ViewModel vm)
            vm.OnFilesDropped(files);
    }

    /// <summary>
    /// Allows Drag & Drop of input file when on Page1.
    /// </summary>
    public string[]? OnDragFilter(DragEventArgs e)
    {
        if (Content is Page1ViewModel vm)
            return vm.OnDragFilter(e);

        e.Effects = DragDropEffects.None;
        e.Handled = true;

        return null;
    }

    #endregion Input file selection via Drag & Drop
}
