using System;
using System.Runtime.Serialization;

namespace Refine.Exceptions;

public class RefineException : Exception {
    public RefineException() { }

    public RefineException(string? message) : base(message) { }

    public RefineException(string? message, Exception? innerException) : base(message, innerException) { }
}