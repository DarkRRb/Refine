using System;

namespace Umrab.Options.Tokenization;

public readonly struct Token(TokenType type, string origin, int keyStart, int keyLength, int valueStart, int valueLength) {
    private readonly int _keyStart = keyStart;
    private readonly int _keyLength = keyLength;
    private readonly int _valueStart = valueStart;
    private readonly int _valueLength = valueLength;

    public TokenType Type { get; } = type;
    public string Origin { get; } = origin;
    public ReadOnlySpan<char> Key => Origin.AsSpan(_keyStart, _keyLength);
    public ReadOnlySpan<char> Value => Origin.AsSpan(_valueStart, _valueLength);

    public static Token Argument(string origin, int start, int length) {
        return new Token(TokenType.Argument, origin, 0, 0, start, length);
    }

    public static Token LongKey(string origin, int start, int length) {
        return new Token(TokenType.LongKey, origin, start, length, 0, 0);
    }

    public static Token LongOption(string origin, int keyStart, int keyLength, int valueStart, int valueLength) {
        return new Token(TokenType.LongOption, origin, keyStart, keyLength, valueStart, valueLength);
    }

    public static Token ShortKey(string origin, int start) {
        return new Token(TokenType.ShortKey, origin, start, 1, 0, 0);
    }

    public static Token ShortOption(string origin, int keyStart, int valueStart, int valueLength) {
        return new Token(TokenType.ShortOption, origin, keyStart, 1, valueStart, valueLength);
    }

}