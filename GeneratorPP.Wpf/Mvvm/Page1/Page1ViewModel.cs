using System;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

using GeneratorPP.Core;
using GeneratorPP.WPF.Misc;
using Microsoft.Win32;

namespace GeneratorPP.WPF.Mvvm.Page1;

/// <summary>
/// Page1: input file selection.
/// </summary>
public partial class Page1ViewModel : BaseViewModel, IDropFilesEnabled
{
    #region Save Input Template

    /// <summary>
    /// Saves the input template to user defined location.
    /// </summary>
    [RelayCommand]
    private void SaveTemplate()
    {
        var sfd = new SaveFileDialog()
        {
            Filter           = $"Excel šablóna (*.{Constants.TemplateFileExtension})|*.{Constants.TemplateFileExtension}",
            FileName         = GetInputTemplateFileName(),
            DefaultExt       = Constants.TemplateFileExtension,
            AddExtension     = true,
            CheckPathExists  = true,
            DereferenceLinks = true,
            OverwritePrompt  = true,
            ValidateNames    = true,
        };

        if (sfd.ShowDialog() is not true)
            return;

        using var sourceStream = new MemoryStream(GetInputTemplateData());
        using var targetStream = sfd.OpenFile();
        sourceStream.CopyTo(targetStream);
    }

    /// <summary>
    /// Construct the default input template filename.
    /// </summary>
    /// <returns></returns>
    public string GetInputTemplateFileName()
    {
        return $"{Constants.InputTemplateFileName}.{Constants.TemplateFileExtension}";
    }

    /// <summary>
    /// Returns the template for input file.
    /// First tries a file beside executable. If it does not exist, returns the embedded one.
    /// </summary>
    public byte[] GetInputTemplateData()
    {
        // try file beside the application
        var file = Path.Combine(AppContext.BaseDirectory, nameof(Resources), GetInputTemplateFileName());
        if (File.Exists(file))
            File.ReadAllBytes(file);

        // otherwise use the embedded one
        return Resources.platobny_harok;
    }

    #endregion Save Input Template

    #region Source File Selection

    /// <summary>
    /// Allows user to select the input file.
    /// </summary>
    [RelayCommand]
    private void SelectInputFile()
    {
        var ofd = new OpenFileDialog
        {
            Filter           = $"Excel súbor (*.{Constants.InputFileExtension})|*.{Constants.InputFileExtension}",
            DefaultExt       = Constants.InputFileExtension,
            AddExtension     = true,
            CheckFileExists  = true,
            CheckPathExists  = true,
            DereferenceLinks = true,
            Multiselect      = false,
            ValidateNames    = true,
        };

        if (ofd.ShowDialog() is not true)
            return;

        if (!FilterCore(ofd.FileNames)) 
            return;

        if (File.Exists(ofd.FileNames[0]))
        {
            InputFilePath = ofd.FileNames[0];
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="paths"/> contains acceptable input.
    /// </summary>
    private static bool FilterCore(params string[] paths)
    {
        if (paths is not ({ Length: 1 } and [var file, ..]))
            return false;

        return string.Equals(
            Path.GetExtension(file), $".{Constants.InputFileExtension}", StringComparison.OrdinalIgnoreCase
        );
    }

    #region IDropFilesEnabled implementation

    /// <inheritdoc /> 
    public void OnFilesDropped(string[] files)
    {
        // safe, since the operation is not allowed at all, if not eligible
        InputFilePath = files[0];
    }

    /// <inheritdoc /> 
    public string[]? OnDragFilter(DragEventArgs e)
    {
        // not allowed by default,
        e.Effects = DragDropEffects.None;
        e.Handled = true;

        // but:
        // when there are file drag drop data,
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return null;

        var fileDrop = e.Data.GetData(DataFormats.FileDrop);
        if (fileDrop is not string[] paths)
            return null;

        // and it is acceptable input
        if (!FilterCore(paths)) 
            return null;

        // then allow the operation to proceed
        e.Effects = DragDropEffects.Move;

        // and return the extracted file paths
        return paths;
    }

    #endregion IDropFilesEnabled implementation

    #endregion Source File Selection

    #region Continue to the next step

    /// <summary>
    /// Command to continue (after input file selection).
    /// </summary>
    [RelayCommand(CanExecute = nameof(ContinueCanExecute))]
    private void Continue()
    {
        NextStep.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Determine whether user can continue with the process.
    /// </summary>
    private bool ContinueCanExecute()
    {
        return !string.IsNullOrWhiteSpace(InputFilePath) 
               && FilterCore(InputFilePath) 
               && File.Exists(InputFilePath);
    }

    #endregion Continue to the next step
}
