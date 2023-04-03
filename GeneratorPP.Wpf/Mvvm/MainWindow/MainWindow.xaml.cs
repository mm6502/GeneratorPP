namespace GeneratorPP.WPF.Mvvm.MainWindow;

/// <summary>
/// The main window of the application.
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}
