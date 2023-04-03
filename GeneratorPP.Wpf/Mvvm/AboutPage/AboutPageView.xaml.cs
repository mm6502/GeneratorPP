using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;

namespace GeneratorPP.WPF.Mvvm.AboutPage;

/// <summary>
/// Interaction logic for AboutPage.xaml
/// </summary>
public partial class AboutPageView
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public AboutPageView()
    {
        InitializeComponent();

        // hook up hyperlinks handlers
        SubscribeToAllHyperlinks(AboutDocument);
    }

    #region Activate Hyperlinks

    /// <summary>
    /// Enables navigation on all hyperlinks in given document.
    /// </summary>
    private void SubscribeToAllHyperlinks(FlowDocument flowDocument)
    {
        var hyperlinks = GetVisuals(flowDocument).OfType<Hyperlink>();

        foreach (var link in hyperlinks)
            link.RequestNavigate += RequestNavigateHandler;
    }

    /// <summary>
    /// Enumerates all children of given dependency object.
    /// </summary>
    private static IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            yield return child;

            foreach (var descendants in GetVisuals(child))
                yield return descendants;
        }
    }

    /// <summary>
    /// Handles the navigation request.
    /// </summary>
    private void RequestNavigateHandler(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        var url = e.Uri.AbsoluteUri;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }

        e.Handled = true;
    }

    #endregion Activate Hyperlinks
}
