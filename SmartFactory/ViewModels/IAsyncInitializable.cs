namespace SmartFactory.ViewModels
{
    public interface IAsyncInitializable
    {
        Task InitializeAsync(CancellationToken token);
    }
}
