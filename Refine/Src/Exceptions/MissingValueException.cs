namespace Refine.Exceptions;

public class MissingValueException(IOption option) : RefineParseException {
    public IOption Option { get; } = option;

    public override string Message {get;} = $"Option '{option}' requires a value";
}