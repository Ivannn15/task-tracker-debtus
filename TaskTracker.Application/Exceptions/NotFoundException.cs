namespace TaskTracker.Application.Exceptions;

using System.Net;

public sealed class NotFoundException : TaskTrackerException
{
    public NotFoundException(string title, string detail)
        : base(title, detail, HttpStatusCode.NotFound)
    {
    }
}
