namespace Umrab.Options.Exceptions;

public class DuplicateInputException(string token, string name) : OptionsException($"Option({name}) duplicate input, from {token}") {
    public string Token { get; } = token;
    public string Name { get; } = name;
}
