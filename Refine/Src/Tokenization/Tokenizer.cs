using System;

namespace Refine.Tokenization;

public ref struct Tokenizer(ReadOnlySpan<string> args)  /* Duck Typing: IEnumerable<Token> */  {
    private readonly ReadOnlySpan<string> _args = args;

    public readonly Tokenizer GetEnumerator() => this;

    private int _index;
    private int _subIndex;
    private bool _eoo;

    public Token Current { get; private set; }

    public bool MoveNext() {
    Start:
        if (_index >= _args.Length) return false;

        string current = _args[_index];

        if (_subIndex > 0) { // The remaining short keys or short options
            if (_subIndex + 1 < current.Length && current[_subIndex + 1] == '=') { // short options
                Current = new Token(
                    TokenType.ShortOption,
                    current,
                    current.AsSpan(_subIndex, 1),
                    current.AsSpan((_subIndex + 2)..)
                );

                _subIndex = 0;
                _index += 1;
                return true;
            }

            Current = new Token(TokenType.ShortKey, current, current.AsSpan(_subIndex, 1));

            if (_subIndex + 1 >= current.Length) {
                _subIndex = 0;
                _index += 1;
            } else {
                _subIndex += 1;
            }
            return true;
        }

        if (_eoo) {
            Current = new Token(TokenType.Argument, current, current);
            _index += 1;
            return true;
        }

        if (current == "--") {
            _eoo = true;
            _index++;
            goto Start;
        }

        if (current.StartsWith("--")) {
            int equalsIndex = current.IndexOf('=', 2);
            Current = equalsIndex != -1
                ? new Token(
                    TokenType.Option,
                    current,
                    current.AsSpan(2, equalsIndex - 2),
                    current.AsSpan(equalsIndex + 1)
                )
                : new Token(TokenType.Key, current, current.AsSpan(2));
            _index += 1;
            return true;
        }

        if (current == "-") {
            Current = new Token(TokenType.Argument, current, current);
            _index++;
            return true;
        }

        if (current.StartsWith('-')) {
            _subIndex = 1;
            goto Start;
        }

        Current = new Token(TokenType.Argument, current, current);
        _index++;
        return true;
    }
}