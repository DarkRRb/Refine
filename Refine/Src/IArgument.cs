using System;

namespace Refine;

internal interface IArgument : IConverter;

public class Argument<T>(Func<ReadOnlySpan<char>, T> converter) : IArgument {
    public Type TargetType { get; } = typeof(T);

    private readonly Func<ReadOnlySpan<char>, T> _converter = converter;

    public object? Convert(ReadOnlySpan<char> value) => _converter(value);

    public override string ToString() => "Argument";
}