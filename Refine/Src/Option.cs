using System;

namespace Refine;

public class Option<T>(string @long, char @short, Func<ReadOnlySpan<char>, T> converter) : IOption {
    public string Long { get; } = @long;
    public char Short { get; } = @short;

    public Type TargetType { get; } = typeof(T);

    private readonly Func<ReadOnlySpan<char>, T> _converter = converter;

    public object? Convert(ReadOnlySpan<char> value) => _converter(value);

    public override string ToString() => $"--{Long}/-{Short}";
}