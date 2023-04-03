using CommunityToolkit.Mvvm.Input;

using GeneratorPP.WPF.Misc;

using System;

namespace GeneratorPP.WPF.Mvvm.AboutPage;

/// <summary>
/// AboutPage: describes the application.
/// </summary>
public partial class AboutPageViewModel : BaseViewModel
{
    /// <summary>
    /// Command to return to previous page.
    /// </summary>
    [RelayCommand]
    private void Continue()
    {
        NextStep.Invoke(this, EventArgs.Empty);
    }
}
