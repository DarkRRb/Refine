using System;
using System.Collections.Generic;

using Refine.Tokenization;

using Commands = System.Collections.Generic.Dictionary<string, Refine.Command>.AlternateLookup<System.ReadOnlySpan<char>>;
using Options = System.Collections.Generic.Dictionary<string, Refine.IOption>.AlternateLookup<System.ReadOnlySpan<char>>;
using Flags = System.Collections.Generic.Dictionary<string, Refine.Flag>.AlternateLookup<System.ReadOnlySpan<char>>;
using Refine.Exceptions;

namespace Refine;

public class Command(string @long, char @short) : INamed {
    public string Long { get; } = @long;
    public char Short { get; } = @short;

    private readonly Dictionary<string, Command> _commands = [];
    private readonly Dictionary<char, Command> _shortCommands = [];

    private readonly Dictionary<string, IOption> _options = [];
    private readonly Dictionary<char, IOption> _shortOptions = [];

    private readonly Dictionary<string, Flag> _flags = [];
    private readonly Dictionary<char, Flag> _shortFlags = [];

    private readonly List<IArgument> _arguments = [];

    public Command Add(Command command) {
        _commands.Add(command.Long, command);
        if (command.Short == '\0') {
            _shortCommands.Add(command.Short, command);
        }

        return this;
    }
    public Command Add<T>(Option<T> option) {
        _options.Add(option.Long, option);
        if (option.Short == '\0') {
            _shortOptions.Add(option.Short, option);
        }

        return this;
    }

    public Command Add(Flag flag) {
        _flags.Add(flag.Long, flag);
        if (flag.Short == '\0') {
            _shortFlags.Add(flag.Short, flag);
        }

        return this;
    }
    public Command Add<T>(Argument<T> argument) {
        _arguments.Add(argument);

        return this;
    }

    public ParseResult Parse(ReadOnlySpan<string> args) => Parse(new Tokenizer(args));
    public ParseResult Parse(Tokenizer tokenizer) {
        Commands commands = _commands.GetAlternateLookup<ReadOnlySpan<char>>();
        Options options = _options.GetAlternateLookup<ReadOnlySpan<char>>();
        Flags flags = _flags.GetAlternateLookup<ReadOnlySpan<char>>();

        ParseResult result = new();

        IOption? wvo = null;
        int argumentIndex = 0;
        foreach (Token token in tokenizer) {
            if (wvo != null) {
                if (token.Type != TokenType.Argument) throw new MissingValueException(wvo);

                try { result.Add(wvo, wvo.Convert(token.Value)); } catch (Exception e) {
                    throw new ConvertException(wvo, token.Value.ToString(), e);
                }

                continue;
            }

            switch (token.Type) {
                case TokenType.Argument: {
                    if (commands.TryGetValue(token.Value, out Command? command)
                     || _shortCommands.TryGetValue(token.Value[0], out command)) {
                        result.Add(command, command.Parse(tokenizer));
                        return result;
                    }

                    if (argumentIndex + 1 > _arguments.Count) throw new UnexpectedArgumentException(token.Origin);

                    IArgument argument = _arguments[argumentIndex];
                    try { result.Add(argument, argument.Convert(token.Value)); } catch (Exception e) {
                        throw new ConvertException(argument, token.Value.ToString(), e);
                    }
                    argumentIndex++;

                    continue;
                }
                case TokenType.Key: {
                    if (flags.TryGetValue(token.Key, out Flag? flag)) {
                        result.Add(flag);
                        continue;
                    }

                    if (options.TryGetValue(token.Key, out IOption? option)) {
                        wvo = option;
                        continue;
                    }

                    throw new UnknownOptionException(token.Origin);
                }
                case TokenType.Option: {
                    if (options.TryGetValue(token.Key, out IOption? option)) {
                        try { result.Add(option, option.Convert(token.Value)); } catch (Exception e) {
                            throw new ConvertException(option, token.Value.ToString(), e);
                        }
                    }

                    throw new UnknownOptionException(token.Origin);
                }
                case TokenType.ShortKey: {
                    if (_shortFlags.TryGetValue(token.Key[0], out Flag? flag)) {
                        result.Add(flag);
                        continue;
                    }

                    if (_shortOptions.TryGetValue(token.Key[0], out IOption? option)) {
                        wvo = option;
                        continue;
                    }

                    throw new UnknownOptionException(token.Origin);
                }
                case TokenType.ShortOption: {
                    if (_shortOptions.TryGetValue(token.Key[0], out IOption? option)) {
                        try { result.Add(option, option.Convert(token.Value)); } catch (Exception e) {
                            throw new ConvertException(option, token.Value.ToString(), e);
                        }
                    }

                    throw new UnknownOptionException(token.Origin);
                }
            }
        }

        return result;
    }
}