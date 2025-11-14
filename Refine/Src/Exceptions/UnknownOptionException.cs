namespace Refine.Exceptions;

public class UnknownOptionException(string token) : RefineParseException {
    public string Token { get; } = token;

    public override string Message { get; } = $"Unknown option or flag: '{token}'";
}