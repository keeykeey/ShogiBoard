using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ShogiBoard.Converters;

/// <summary>
/// Converts a USI piece string (e.g. "P" for black pawn, "p" for white pawn, "+P" for black promoted pawn, "+p" for white promoted pawn) to its image.
/// </summary>
public class PieceToBitmapConverter : IValueConverter
{
    private const string BasePath = "avares://ShogiBoard/Assets/one/piece";

    private static readonly Dictionary<string, Bitmap> Cache = [];

    private static readonly Dictionary<char, string> Names = new()
    {
        ['P'] = "pawn",
        ['L'] = "lance",
        ['N'] = "knight",
        ['S'] = "silver",
        ['G'] = "gold",
        ['B'] = "bishop",
        ['R'] = "rook",
        ['K'] = "king",
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string piece || piece.Length < 1 || piece.Length > 2) return null;
        if (piece.Length == 2 && !piece[0].Equals('+')) return null;
        if (Cache.TryGetValue(piece, out Bitmap? cached)) return cached;

        int charIndex = piece.Length - 1;
        char c = piece[charIndex];
        if (!Names.TryGetValue(char.ToUpperInvariant(c), out string? name)) return null;
        string side = char.IsUpper(c) ? "black" : "white";
        string promote = charIndex == 0 ? "" : "_prom";

        Uri uri = new($"{BasePath}/{side}{promote}_{name}.png");
        Bitmap bitmap = new(AssetLoader.Open(uri));
        Cache[piece] = bitmap;
        return bitmap;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
