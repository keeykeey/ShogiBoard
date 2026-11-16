using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ShogiBoard.Converters;

/// <summary>
/// Converts a "flipped" flag to a RotateTransform: true → 180°, false → 0°.
/// Use together with RenderTransformOrigin="50%,50%" to rotate around the center.
/// </summary>
public class FlipToRotateTransformConverter : IValueConverter
{
    private static readonly RotateTransform Rotate0 = new(0);
    private static readonly RotateTransform Rotate180 = new(180);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? Rotate180 : Rotate0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
