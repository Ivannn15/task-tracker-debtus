namespace TaskTracker.Application.Exceptions;

using System.Net;

public sealed class ConflictException : TaskTrackerException
{
    public ConflictException(string title, string detail)
        : base(title, detail, HttpStatusCode.Conflict)
    {
    }
}
