using System;

namespace Refine.Exceptions;

public class RefineParseException : RefineException {
    public RefineParseException() {
    }

    public RefineParseException(string? message) : base(message) {
    }

    public RefineParseException(string? message, Exception? innerException) : base(message, innerException) {
    }
}