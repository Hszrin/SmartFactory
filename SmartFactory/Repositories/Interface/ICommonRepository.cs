namespace SmartFactory.Repositories.Interface
{
    public interface ICommonRepository<T, TKey>
        where T : class
    {
        Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        Task DeleteAsync(TKey id);
    }
}
