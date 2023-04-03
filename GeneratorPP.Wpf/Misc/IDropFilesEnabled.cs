using System.Windows;

namespace GeneratorPP.WPF.Misc;

/// <summary>
/// Interface for Drag &amp; Drop enabled view models.
/// </summary>
public interface IDropFilesEnabled
{
    /// <summary>
    /// Invoked when Drag &amp; Drop operation has ended with a drop.
    /// </summary>
    void OnFilesDropped(string[] files);

    /// <summary>
    /// Invoked on various occasions during the Drag &amp; Drop operation to determine:
    /// <list type="bullet">
    /// <item>whether the operation is allowed (by setting <see cref="DragEventArgs.Effects"/>)</item>
    /// <item>what files are being Drag &amp; Drop-ped</item>
    /// </list>
    /// </summary>
    string[]? OnDragFilter(DragEventArgs e);
}
