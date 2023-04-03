using CommunityToolkit.Mvvm.ComponentModel;

namespace GeneratorPP.WPF.Mvvm.MainWindow;

/// <summary>
/// The main view model properties.
/// </summary>
public partial class MainWindowViewModel
{
    /// <summary>
    /// Determine whether application is in debug mode.
    /// </summary>
    public static bool IsDebugMode =>
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>
    /// Current content to be displayed.
    /// </summary>
    public object? Content
    {
        get => content; 
        set
        {
            if (SetProperty(ref content, value))
            {
                GoToAboutPageCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private object? content;

    /// <summary>
    /// Stores a page when leaving to AboutPage.
    /// </summary>
    private object? PageToReturnTo { get; set; }
}
