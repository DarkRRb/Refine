namespace Umrab.Options.Exceptions;

public class DuplicateDefinitionException(string name) : OptionsException($"Duplicate definition name: {name}") {
    public string Name { get; } = name;
}