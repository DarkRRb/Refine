namespace Umrab.Options.Exceptions;

public class MissingValueException(string token, string name) : OptionsException($"Option({name}) missing value, from {token}") {
    public string Token { get; } = token;
    public string Name { get; } = name;
}