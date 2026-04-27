namespace Web.Composition.SetUpTasks.Abstractions
{
    public interface ISetUpTask
    {
        int Order { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
