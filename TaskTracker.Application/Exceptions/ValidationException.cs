namespace TaskTracker.Application.Exceptions;

using System.Net;

public sealed class ValidationException : TaskTrackerException
{
    public ValidationException(string title, string detail)
        : base(title, detail, HttpStatusCode.BadRequest)
    {
    }
}
