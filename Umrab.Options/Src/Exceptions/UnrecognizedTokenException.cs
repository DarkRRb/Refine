namespace Umrab.Options.Exceptions;

public class UnrecognizedTokenException(string token) : OptionsException($"Unrecognized {token}") {
    public string Token { get; } = token;
}