using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Refine;

public class ParseResult {
    private KeyValuePair<Command, ParseResult>? _subCommand = null;

    private readonly Dictionary<IOption, object?> _options = [];

    private readonly List<Flag> _flags = [];

    private readonly Dictionary<IArgument, object?> _arguments = [];

    internal void Add(Command command, ParseResult result) => _subCommand = KeyValuePair.Create(command, result);

    internal void Add(IOption option, object? value) => _options.Add(option, value);

    internal void Add(Flag flag) => _flags.Add(flag);

    internal void Add(IArgument argument, object? value) => _arguments.Add(argument, value);

    public bool TryGetSubCommand([NotNullWhen(true)] out Command? command, [NotNullWhen(true)] out ParseResult? result) {
        command = _subCommand?.Key;
        result = _subCommand?.Value;
        return _subCommand.HasValue;
    }

    public bool TryGet<T>(Option<T> option, [NotNullWhen(true)] out T? value) {
        if (_options.TryGetValue(option, out object? value_) &&
            value_ is T tvalue) {
            value = tvalue;
            return true;
        }
        value = default;
        return false;
        //bool result = _options.TryGetValue(option, out object? value_);
        //value = (T?)value_;
        //return result;
    }

    public bool TryGet(Flag flag) => _flags.Contains(flag);

    public bool TryGet<T>(Argument<T> argument, [NotNullWhen(true)] out T? value) {
        if (_arguments.TryGetValue(argument, out object? value_) &&
            value_ is T tvalue) {
            value = tvalue;
            return true;
        }
        value = default;
        return false;
        //bool result = _arguments.TryGetValue(argument, out object? value_);
        //value = (T?)value_;
        //return result;
    }
}