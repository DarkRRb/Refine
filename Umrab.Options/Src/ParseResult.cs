using System;
using System.Collections.Generic;

using Umrab.Options.Exceptions;

namespace Umrab.Options;

public class ParseResult {
    private KeyValuePair<string, ParseResult> _command = default;

    private readonly Dictionary<string, object?> _options = [];

    private readonly HashSet<string> _flags = [];

    private readonly List<object?> _arguments = [];

    internal void SetCommand(string name, ParseResult result) => _command = KeyValuePair.Create(name, result);

    internal bool AddOption(string name, object? value) => _options.TryAdd(name, value);

    internal void AddFlag(string name) => _flags.Add(name);

    internal void AddArgument(object? value) => _arguments.Add(value);

    public bool TryGetCommand(out KeyValuePair<string, ParseResult> command) {
        command = _command;
        return !_command.Equals(default);
    }

    public T GetOption<T>(string name, Func<T> @default) {
        return _options.TryGetValue(name, out object? value) ? (T)value! : @default();
    }

    public bool GetFlag(string name) => _flags.Contains(name);

    public T GetArgument<T>(int index, Func<T> @default) {
        return index < _arguments.Count ? (T)_arguments[index]! : @default();
    }
}