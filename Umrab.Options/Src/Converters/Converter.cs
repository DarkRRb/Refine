using System;

namespace Umrab.Options.Converters;

public class Converter<T>(Func<ReadOnlySpan<char>, T> converter) : IConverter {
    private readonly Func<ReadOnlySpan<char>, T> _converter = converter;

    public object? Convert(ReadOnlySpan<char> value) => _converter(value);
}