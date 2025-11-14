using System;
using System.Collections.Generic;

using Umrab.Options.Converters;
using Umrab.Options.Tokenization;

using OptionDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.KeyValuePair<string, Umrab.Options.Converters.IConverter>>.AlternateLookup<System.ReadOnlySpan<char>>;
using FlagSet = System.Collections.Generic.HashSet<string>.AlternateLookup<System.ReadOnlySpan<char>>;
using Umrab.Options.Exceptions;

namespace Umrab.Options;

public class Command {
    private readonly Dictionary<string, Command> _commands = [];
    private readonly Dictionary<char, KeyValuePair<string, Command>> _scommands = [];

    private readonly Dictionary<string, KeyValuePair<string, IConverter>> _options = [];
    private readonly Dictionary<char, KeyValuePair<string, IConverter>> _soptions = [];

    private readonly HashSet<string> _flags = [];
    private readonly Dictionary<char, string> _sflags = [];

    private readonly List<IConverter> _arguments = [];

    public Command SubCommand(string @long, Command command) {
        return !_commands.TryAdd(@long, command) ? throw new DuplicateDefinitionException(@long) : this;
    }

    public Command SubCommand(string @long, char @short, Command command) {
        return !_commands.TryAdd(@long, command)
            ? throw new DuplicateDefinitionException(@long)
            : !_scommands.TryAdd(@short, KeyValuePair.Create(@long, command))
                ? throw new DuplicateDefinitionException(@short.ToString())
                : this;
    }

    public Command Option<T>(string @long, Func<ReadOnlySpan<char>, T> converter) {
        if (_flags.Contains(@long)) throw new DuplicateDefinitionException(@long);

        if (!_options.TryAdd(@long, KeyValuePair.Create(@long, (IConverter)new Converter<T>(converter)))) {
            throw new DuplicateDefinitionException(@long);
        }

        return this;
    }

    public Command Option<T>(string @long, char @short, Func<ReadOnlySpan<char>, T> converter) {
        if (_flags.Contains(@long)) throw new DuplicateDefinitionException(@long);
        if (_sflags.ContainsKey(@short)) throw new DuplicateDefinitionException(@short.ToString());

        KeyValuePair<string, IConverter> kvp = KeyValuePair.Create(@long, (IConverter)new Converter<T>(converter));
        if (!_options.TryAdd(@long, kvp)) throw new DuplicateDefinitionException(@long);
        if (!_soptions.TryAdd(@short, kvp)) throw new DuplicateDefinitionException(@short.ToString());

        return this;
    }

    public Command Flag(string @long) {
        if (_options.ContainsKey(@long)) throw new DuplicateDefinitionException(@long);

        if (!_flags.Add(@long)) throw new DuplicateDefinitionException(@long);

        return this;
    }

    public Command Flag(string @long, char @short) {
        if (_options.ContainsKey(@long)) throw new DuplicateDefinitionException(@long);
        if (_soptions.ContainsKey(@short)) throw new DuplicateDefinitionException(@short.ToString());
        
        if (!_flags.Add(@long)) throw new DuplicateDefinitionException(@long);
        if (!_sflags.TryAdd(@short, @long)) throw new DuplicateDefinitionException(@short.ToString());

        return this;
    }

    public Command Argument<T>(Func<ReadOnlySpan<char>, T> converter) {
        _arguments.Add(new Converter<T>(converter));
        return this;
    }

    public ParseResult Parse(ReadOnlySpan<string> args) => Parse(new Tokenizer(args));
    public ParseResult Parse(Tokenizer tokenizer) {
        OptionDictionary options = _options.GetAlternateLookup<ReadOnlySpan<char>>();
        FlagSet flags = _flags.GetAlternateLookup<ReadOnlySpan<char>>();

        ParseResult result = new();

        KeyValuePair<(string Token, string Name), IConverter> wov = default;
        int argumentIndex = 0;

        while (tokenizer.Next(out Token token)) {
            if (!wov.Equals(default)) {
                if (tokenizer.EndOfOption || token.Type != TokenType.Argument) throw new MissingValueException(wov.Key.Token, wov.Key.Name);
                if (!result.AddOption(wov.Key.Name, wov.Value.Convert(token.Value))) {
                    throw new DuplicateInputException(wov.Key.Token, wov.Key.Name);
                }
                wov = default;
                continue;
            }

            switch (token.Type) {
                case TokenType.Argument: {
                    if (!tokenizer.EndOfOption) {
                        if (_commands.TryGetValue(token.Origin, out Command? command)) {
                            result.SetCommand(token.Origin, command.Parse(tokenizer));
                            return result;
                        }

                        if (token.Origin.Length == 1) {
                            if (_scommands.TryGetValue(token.Origin[0], out KeyValuePair<string, Command> kvp)) {
                                result.SetCommand(kvp.Key, kvp.Value.Parse(tokenizer));
                                return result;
                            }
                        }
                    }

                    if (argumentIndex >= _arguments.Count) throw new UnrecognizedTokenException(token.Origin);
                    result.AddArgument(_arguments[argumentIndex].Convert(token.Value));
                    argumentIndex++;
                    break;
                }
                case TokenType.LongKey: {
                    if (options.TryGetValue(token.Key, out KeyValuePair<string, IConverter> kvp)) {
                        wov = KeyValuePair.Create((token.Origin, kvp.Key), kvp.Value);
                        break;
                    }

                    if (flags.TryGetValue(token.Key, out string? flag)) {
                        result.AddFlag(flag);
                        break;
                    }

                    throw new UnrecognizedTokenException(token.Origin);
                }
                case TokenType.LongOption: {
                    if (options.TryGetValue(token.Key, out KeyValuePair<string, IConverter> kvp)) {
                        if (!result.AddOption(kvp.Key, kvp.Value.Convert(token.Value))) {
                            throw new DuplicateInputException(token.Origin, kvp.Key);
                        }
                        break;
                    }

                    throw new UnrecognizedTokenException(token.Origin);
                }
                case TokenType.ShortKey: {
                    if (_soptions.TryGetValue(token.Key[0], out KeyValuePair<string, IConverter> kvp)) {
                        wov = KeyValuePair.Create((token.Origin, kvp.Key), kvp.Value);
                        break;
                    }

                    if (_sflags.TryGetValue(token.Key[0], out string? flag)) {
                        result.AddFlag(flag);
                        break;
                    }

                    throw new UnrecognizedTokenException(token.Origin);
                }
                case TokenType.ShortOption: {
                    if (_soptions.TryGetValue(token.Key[0], out KeyValuePair<string, IConverter> kvp)) {
                        if (!result.AddOption(kvp.Key, kvp.Value.Convert(token.Value))) {
                            throw new DuplicateInputException(token.Origin, kvp.Key);
                        }
                        break;
                    }

                    throw new UnrecognizedTokenException(token.Origin);
                }
            }
        }

        if (!wov.Equals(default)) throw new MissingValueException(wov.Key.Token, wov.Key.Name);

        return result;
    }
}