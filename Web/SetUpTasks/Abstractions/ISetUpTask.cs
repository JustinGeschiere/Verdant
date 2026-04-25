namespace Web.SetUpTasks.Abstractions
{
    public interface ISetUpTask
    {
        int Order { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
