using System;

namespace GeneratorPP.WPF.Mvvm.AboutPage;

/// <summary>
/// AboutPage properties.
/// </summary>
public partial class AboutPageViewModel
{
    /// <summary>
    /// Event fired when user wants to leave the about page.
    /// </summary>
    public event EventHandler NextStep = delegate { };
}
