using System;

namespace Refine.Exceptions;

public class RefineConfigureException : RefineException {
    public RefineConfigureException() { }

    public RefineConfigureException(string? message) : base(message) { }

    public RefineConfigureException(string? message, Exception? innerException) : base(message, innerException) { }
}