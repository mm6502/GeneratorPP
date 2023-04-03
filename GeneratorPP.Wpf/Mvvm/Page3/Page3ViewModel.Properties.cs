using System;

namespace GeneratorPP.WPF.Mvvm.Page3;

public partial class Page3ViewModel
{
    public event EventHandler NextStep = delegate { };
    public string? OutputFilePath { get; set; }
    public string? PaymentPurpose { get; set; }
}