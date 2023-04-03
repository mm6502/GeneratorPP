using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;

using Microsoft.Win32;

using CommunityToolkit.Mvvm.Input;

using GeneratorPP.WPF.Misc;

namespace GeneratorPP.WPF.Mvvm.Page3;

/// <summary>
/// Page3: generation result.
/// </summary>
public partial class Page3ViewModel : BaseViewModel
{
    /// <summary>
    /// Initializes the page.
    /// </summary>
    public void Initialize(string? outputFilePath, string? paymentPurpose)
    {
        OutputFilePath = outputFilePath;
        PaymentPurpose = paymentPurpose;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Command to continue (start over).
    /// </summary>
    [RelayCommand]
    private void Continue()
    {
        NextStep.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Saves the generated document to user specified location.
    /// </summary>
    [RelayCommand(CanExecute = nameof(SaveOutputDocumentCanExecute))]
    private void SaveOutputDocument()
    {
        var sfd = new SaveFileDialog
        {
            Filter           = $"Excel súbor (*.{Constants.InputFileExtension})|*.{Constants.InputFileExtension}",
            FileName         = GetDefaultOutputFileName(),
            DefaultExt       = Constants.OutputFileExtension,
            AddExtension     = true,
            CheckPathExists  = true,
            DereferenceLinks = true,
            OverwritePrompt  = true,
            ValidateNames    = true,
        };

        if (sfd.ShowDialog() is not true)
            return;

        File.Move(OutputFilePath!, sfd.FileName, true);
    }

    /// <summary>
    /// Determines the availability of <see cref="SaveOutputDocumentCommand"/>.
    /// </summary>
    private bool SaveOutputDocumentCanExecute()
    {
        return !string.IsNullOrWhiteSpace(OutputFilePath);
    }

    /// <summary>
    /// Gets the default output file name.
    /// </summary>
    private string GetDefaultOutputFileName()
    {
        var candidate = Constants.OutputTemplateFileName;

        if (!string.IsNullOrWhiteSpace(PaymentPurpose))
            candidate += $" - {PaymentPurpose}";

        candidate += $" - {DateTime.Today:yyyy-MM-dd}";

        candidate += $".{Constants.OutputFileExtension}";

        return MakeValidFileName(candidate);
    }

    /// <summary>
    /// Makes the given file name a valid one.
    /// </summary>
    private static string MakeValidFileName(string filename)
    {
        var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        var invalidReStr = $@"[{invalidChars}]+";

        var reservedWords = new []
        {
            "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
            "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
            "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
        };

        var sanitisedName = Regex.Replace(filename, invalidReStr, "_");
        foreach (var reservedWord in reservedWords)
        {
            var reservedWordPattern = $"^{reservedWord}\\.";
            sanitisedName = Regex.Replace(sanitisedName, reservedWordPattern, "_.", RegexOptions.IgnoreCase);
        }

        return sanitisedName;
    }
}
