namespace Api.Composition.SetUp.Abstractions
{
    public interface ISetUpTask
    {
        int Order { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
