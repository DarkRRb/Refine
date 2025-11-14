namespace Refine;

public class Flag(string @long, char @short) : INamed {
    public string Long { get; } = @long;

    public char Short { get; } = @short;
}