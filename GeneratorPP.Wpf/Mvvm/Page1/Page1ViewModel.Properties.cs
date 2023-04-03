using System;

namespace GeneratorPP.WPF.Mvvm.Page1;

/// <summary>
/// Page1 properties.
/// </summary>
public partial class Page1ViewModel
{
    /// <summary>
    /// Event fired when user has selected a file and wants to continue the process.
    /// </summary>
    public event EventHandler NextStep = delegate { };

    /// <summary>
    /// Input file to process.
    /// </summary>
    public string? InputFilePath
    {
        get => inputFilePath;
        set
        {
            if (SetProperty(ref inputFilePath, value))
                ContinueCommand.NotifyCanExecuteChanged();
        }
    }

    private string? inputFilePath;
}
