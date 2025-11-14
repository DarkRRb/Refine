using System;

namespace Umrab.Options.Converters;

public interface IConverter {
    object? Convert(ReadOnlySpan<char> value);
}