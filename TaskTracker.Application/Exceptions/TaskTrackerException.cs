namespace TaskTracker.Application.Exceptions;

using System.Net;

public abstract class TaskTrackerException : Exception
{
    protected TaskTrackerException(string title, string detail, HttpStatusCode statusCode)
        : base(detail)
    {
        Title = title;
        StatusCode = statusCode;
        Detail = detail;
    }

    public string Title { get; }

    public HttpStatusCode StatusCode { get; }

    public string Detail { get; }
}
