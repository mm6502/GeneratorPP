using System.Diagnostics;
using System.Windows;
using System;

namespace GeneratorPP.WPF.Misc;

/// <summary>
/// Transfers selected Drag &amp; Drop events from target <see cref="FrameworkElement"/>
/// to its view model implementing the <see cref="IDropFilesEnabled"/> interface.
/// <example>
/// <para>
/// Usage: <code>&lt;TextBox local:DropFilesBehaviorExtension.IsEnabled="True" /&gt;</code>
/// </para>
/// </example>
/// </summary>
public class DropFilesBehaviorExtension
{
    #region IsEnabled Attached Property

    /// <summary>
    /// Declaration of the attached property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(DropFilesBehaviorExtension),
        new FrameworkPropertyMetadata(default(bool), OnPropChanged)
        {
            BindsTwoWayByDefault = false,
        }
    );

    /// <summary>
    /// Setter of the attached property.
    /// </summary>
    public static void SetIsEnabled(DependencyObject element, bool value)
    {
        element.SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Getter of the attached property.
    /// </summary>
    public static bool GetIsEnabled(DependencyObject element)
    {
        return (bool)element.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Adding / removing event handlers on property change.
    /// </summary>
    private static void OnPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        switch (d)
        {
            case FrameworkElement fe:
                {
                    if ((bool)e.NewValue)
                    {
                        fe.AllowDrop = true;
                        fe.Drop += OnDrop;
                        fe.PreviewDragOver += OnPreviewDragEnter;
                        fe.PreviewDragEnter += OnPreviewDragOver;
                        fe.DragEnter += OnDragEnter;
                    }
                    else
                    {
                        fe.AllowDrop = false;
                        fe.Drop -= OnDrop;
                        fe.PreviewDragEnter -= OnPreviewDragEnter;
                        fe.PreviewDragOver -= OnPreviewDragOver;
                        fe.DragEnter -= OnDragEnter;
                    }

                    return;
                }

            case FrameworkContentElement fce:
                {
                    if ((bool)e.NewValue)
                    {
                        fce.AllowDrop = true;
                        fce.Drop += OnDrop;
                        fce.PreviewDragOver += OnPreviewDragEnter;
                        fce.PreviewDragEnter += OnPreviewDragOver;
                        fce.DragEnter += OnDragEnter;
                    }
                    else
                    {
                        fce.AllowDrop = false;
                        fce.Drop -= OnDrop;
                        fce.PreviewDragEnter -= OnPreviewDragEnter;
                        fce.PreviewDragOver -= OnPreviewDragOver;
                        fce.DragEnter -= OnDragEnter;
                    }

                    return;
                }

            default:
                throw new InvalidOperationException();
        }
    }

    #endregion IsEnabled Attached Property

    #region Misc

    /// <summary>
    /// Extracts the <see cref="FrameworkElement.DataContext"/> and casts it to <see cref="IDropFilesEnabled"/>.
    /// </summary>
    private static IDropFilesEnabled? GetTypedDataContext(object sender)
    {
        var dataContext = default(object);

        if (sender is FrameworkElement element)
            dataContext = element.DataContext;

        if (sender is FrameworkContentElement fce)
            dataContext = fce.DataContext;

        if (dataContext is IDropFilesEnabled fileDropped)
        {
            return fileDropped;
        }

        if (dataContext != null)
        {
            Trace.TraceError(
                $"Binding error, '{dataContext.GetType().Name}' doesn't implement '{nameof(IDropFilesEnabled)}'."
            );
        }

        return null;
    }

    /// <summary>
    /// Common event handler.
    /// </summary>
    private static (IDropFilesEnabled? ViewModel, string[]? Paths) CommonHandler(object sender, DragEventArgs e)
    {
        var fileDropped = GetTypedDataContext(sender);
        return (fileDropped, fileDropped?.OnDragFilter(e));
    }

    #endregion Misc

    #region Event Handlers

    /// <summary>
    /// <see cref="FrameworkElement.PreviewDragEnter"/> event handler.
    /// <remarks>Needed for some input elements such as TextBox.</remarks>
    /// </summary>
    private static void OnPreviewDragEnter(object sender, DragEventArgs e)
    {
        CommonHandler(sender, e);
    }

    /// <summary>
    /// <see cref="FrameworkElement.PreviewDragOver"/> event handler.
    /// <remarks>Needed for some input elements such as TextBox.</remarks>
    /// </summary>
    private static void OnPreviewDragOver(object sender, DragEventArgs e)
    {
        CommonHandler(sender, e);
    }

    /// <summary>
    /// <see cref="FrameworkElement.DragEnter"/> event handler.
    /// </summary>
    private static void OnDragEnter(object sender, DragEventArgs e)
    {
        CommonHandler(sender, e);
    }

    /// <summary>
    /// <see cref="FrameworkElement.Drop"/> event handler.
    /// </summary>
    private static void OnDrop(object sender, DragEventArgs e)
    {
        var (fileDropped, paths) = CommonHandler(sender, e);

        if (paths is not null)
            fileDropped?.OnFilesDropped(paths);
    }

    #endregion Event Handlers
}