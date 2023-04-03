using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using GeneratorPP.WPF.Mvvm.AboutPage;
using GeneratorPP.WPF.Mvvm.Page1;
using GeneratorPP.WPF.Mvvm.Page2;
using GeneratorPP.WPF.Mvvm.Page3;

namespace GeneratorPP.WPF.Mvvm.MainWindow;

/// <summary>
/// Main view model commands.
/// </summary>
public partial class MainWindowViewModel
{
    /// <summary>
    /// Command to switch to AboutPage.
    /// </summary>
    [RelayCommand(CanExecute = nameof(GoToAboutPageCanExecute))]
    private void GoToAboutPage()
    {
        // stores the page to return to
        PageToReturnTo = Content;

        var page = new AboutPageViewModel();
        page.NextStep += AboutPageOnNextStep;
        Content       =  page;
    }

    /// <summary>
    /// Determines whether about page can be displayed.
    /// </summary>
    private bool GoToAboutPageCanExecute()
    {
        return Content is Page1ViewModel or Page3ViewModel;
    }

    /// <summary>
    /// Transition from AboutPage to Page1.
    /// </summary>
    private void AboutPageOnNextStep(object? sender, EventArgs e)
    {
        if (Content is not AboutPageViewModel)
            return;

        if (PageToReturnTo is not null)
        {
            Content        = PageToReturnTo;
            PageToReturnTo = null;
        }
        else
        {
            GoToPage1();
        }
    }

    /// <summary>
    /// Command to switch to Page1.
    /// </summary>
    [RelayCommand]
    private void GoToPage1()
    {
        var page1 = new Page1ViewModel();
        page1.NextStep += (_, _) => SafeHandlerAsync(Page1OnNextStepAsync);
        Content        =  page1;
    }

    /// <summary>
    /// Makes the async void handler "safe" by wrapping the task in a try/catch block.
    /// </summary>
    /// <param name="task"></param>
    private async void SafeHandlerAsync(Func<Task> task)
    {
        try
        {
            await task();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Transition from Page1 to Page2.
    /// </summary>
    private async Task Page1OnNextStepAsync()
    {
        if (Content is not Page1ViewModel page1)
            return;

        if (string.IsNullOrWhiteSpace(page1.InputFilePath))
            return;

        GoToPage2();

        if (Content is not Page2ViewModel page2)
            return;

        await page2.GenerateAsync(page1.InputFilePath);
    }

    /// <summary>
    /// Command to switch to Page2.
    /// </summary>
    [RelayCommand]
    private void GoToPage2()
    {
        var page2 = new Page2ViewModel();
        page2.NextStep += Page2OnNextStep;
        Content        =  page2;
    }

    /// <summary>
    /// Transition from Page2 to Page3.
    /// </summary>
    private void Page2OnNextStep(object? sender, EventArgs e)
    {
        if (Content is not Page2ViewModel page2)
            return;

        if (string.IsNullOrWhiteSpace(page2.OutputFilePath ))
            return;

        GoToPage3();

        if (Content is not Page3ViewModel page3)
            return;

        page3.Initialize(page2.OutputFilePath, page2.PaymentPurpose);
    }

    /// <summary>
    /// Command to switch to Page3.
    /// </summary>
    [RelayCommand]
    private void GoToPage3()
    {
        var page3 = new Page3ViewModel();
        page3.NextStep += Page3OnNextStep;
        Content = page3;
    }

    /// <summary>
    /// Transition from Page3 to Page1.
    /// </summary>
    private void Page3OnNextStep(object? sender, EventArgs e)
    {
        if (Content is not Page3ViewModel)
            return;

        GoToPage1();
    }
}
