using System;

namespace Refine;

public interface IConverter {
    Type TargetType { get; }

    object? Convert(ReadOnlySpan<char> value);
}