using System;

namespace Refine.Exceptions;

public class ConvertException(IConverter converter, string value, Exception? innerException) : RefineParseException(null, innerException) {
    public IConverter Converter { get; } = converter;

    public string Value { get; } = value;

    public override string Message { get; } = $"{converter} failed to convert '{value}' to {converter.TargetType}.";
}