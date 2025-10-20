namespace TaskTracker.Domain.Enums;

public static class TaskStatusExtensions
{
    private static readonly IReadOnlyDictionary<TaskStatus, TaskStatus?> NextStatuses = new Dictionary<TaskStatus, TaskStatus?>
    {
        [TaskStatus.Backlog] = TaskStatus.Current,
        [TaskStatus.Current] = TaskStatus.InProgress,
        [TaskStatus.InProgress] = TaskStatus.InReview,
        [TaskStatus.InReview] = TaskStatus.Testing,
        [TaskStatus.Testing] = TaskStatus.Done,
        [TaskStatus.Done] = null,
        [TaskStatus.Cancelled] = null
    };

    public static bool IsInitial(this TaskStatus status) => status is TaskStatus.Backlog or TaskStatus.Current;

    public static bool IsFinal(this TaskStatus status) => status is TaskStatus.Done or TaskStatus.Cancelled;

    public static bool CanTransitionTo(this TaskStatus current, TaskStatus target)
    {
        if (current == target)
        {
            return true;
        }

        if (target == TaskStatus.Cancelled && current != TaskStatus.Cancelled)
        {
            return true;
        }

        if (!NextStatuses.TryGetValue(current, out var next))
        {
            return false;
        }

        return next.HasValue && next.Value == target;
    }
}
