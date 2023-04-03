using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GeneratorPP.WPF.Mvvm.Page2;

/// <summary>
/// Page2 properties.
/// </summary>
public partial class Page2ViewModel
{
    /// <summary>
    /// Event fired when the generating of the output file has ended.
    /// </summary>
    public event EventHandler NextStep = delegate { };

    /// <summary>
    /// Current progress of the generation.
    /// </summary>
    [ObservableProperty] 
    private int currentProgress;

    /// <summary>
    /// File name and path of the generated document.
    /// </summary>
    public string? OutputFilePath { get; set; }

    /// <summary>
    /// Payment purpose extracted from the input file.
    /// </summary>
    public string? PaymentPurpose { get; set; }
}
