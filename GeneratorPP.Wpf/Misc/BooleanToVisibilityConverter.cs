using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeneratorPP.WPF.Misc;

/// <summary>
/// Converts a <see cref="bool">bool?</see> value to <see cref="Visibility"/>.
/// </summary>
[ValueConversion(typeof(bool?), typeof(Visibility))]
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>Converts source value to a value for the binding target. The data binding engine calls this method when it propagates the value from source binding to the binding target.</summary>
    /// <param name="value">The value the source binding produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">
    /// Value from enum <see cref="Visibility"/> to use for negative result.
    /// Use '!' in value to negate the result value before converting to visibility.
    /// For example:
    /// "!Hidden", "Hidden!"
    /// "!Collapsed", "Collapsed!"
    /// </param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.If the method returns <see langword="null" />, the valid <see langword="null" /> value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolResult = true;
        var negate = false;
        var visibilityResult = Visibility.Collapsed;

        if (parameter is string stringParameter)
        {
            if (stringParameter.Contains('!'))
            {
                negate = true;
            }

            if (!Enum.TryParse(stringParameter.Trim('!'), true, out Visibility tmpVisibility))
            {
                visibilityResult = tmpVisibility;
            }
        }

        if (value is bool bValue)
        {
            boolResult = bValue;
        }

        if (negate)
            boolResult = !boolResult;

        return boolResult ? Visibility.Visible : visibilityResult;
    }

    /// <summary>Converts a binding target value to the source binding value.</summary>
    /// <param name="value">The value that the binding target produces.</param>
    /// <param name="targetTypes">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A value that has been converted from the target value back to the source value.</returns>
    public virtual object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}