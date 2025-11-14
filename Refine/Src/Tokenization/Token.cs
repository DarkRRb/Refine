using System;

namespace Refine.Tokenization;

public readonly ref struct Token(TokenType type, string origin, ReadOnlySpan<char> key, ReadOnlySpan<char> value = default) {
    public TokenType Type { get; } = type;
    public string Origin { get; } = origin;
    public ReadOnlySpan<char> Key { get; } = key;
    public ReadOnlySpan<char> Value { get; } = value;
}