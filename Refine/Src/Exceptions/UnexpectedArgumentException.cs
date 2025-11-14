namespace Refine.Exceptions;

public class UnexpectedArgumentException(string token) : RefineParseException {
    public string Token { get; } = token;

    public override string Message { get; } = $"Unexpected argument: '{token}'";
}