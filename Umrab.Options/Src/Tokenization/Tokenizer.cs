using System;

namespace Umrab.Options.Tokenization;

public ref struct Tokenizer(ReadOnlySpan<string> args) {
    private readonly ReadOnlySpan<string> _args = args;

    private int _index = 0;

    private int _subIndex = 0;

    public bool EndOfOption { get; private set; } = false;

    public bool Next(out Token token) {
    start:
        if (_index >= _args.Length) {
            token = default;
            return false;
        }

        string arg = _args[_index];

        if (_subIndex > 0) {
            if (_subIndex + 1 < arg.Length && arg[_subIndex + 1] == '=') {
                token = Token.ShortOption(arg, _subIndex, _subIndex + 2, arg.Length - _subIndex - 2);
                _index += 1;
                _subIndex = 0;
                return true;
            }

            token = Token.ShortKey(arg, _subIndex);
            if (_subIndex + 1 >= arg.Length) {
                _subIndex = 0;
                _index += 1;
            } else {
                _subIndex += 1;
            }
            return true;
        }

        if (EndOfOption) {
            token = Token.Argument(arg, 0, arg.Length);
            _index++;
            return true;
        }

        if (arg == "--") {
            EndOfOption = true;
            _index++;
            goto start;
        }

        if (arg.StartsWith("--", StringComparison.Ordinal)) {
            int equalIndex = arg.IndexOf('=', 3);
            if (equalIndex != -1) {
                token = Token.LongOption(arg, 2, equalIndex - 2, equalIndex + 1, arg.Length - equalIndex - 1);
                _index++;
                return true;
            }

            token = Token.LongKey(arg, 2, arg.Length - 2);
            _index++;
            return true;
        }

        if (arg == "-") {
            token = Token.Argument(arg, 0, arg.Length);
            _index++;
            return true;
        }

        if (arg.StartsWith('-')) {
            _subIndex = 1;
            goto start;
        }

        token = Token.Argument(arg, 0, arg.Length);
        _index++;
        return true;
    }
}