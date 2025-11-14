using System;

namespace Umrab.Options.Exceptions;

public class OptionsException : Exception {
    public OptionsException() { }

    public OptionsException(string? message) : base(message) { }

    public OptionsException(string? message, Exception? innerException) : base(message, innerException) { }
}