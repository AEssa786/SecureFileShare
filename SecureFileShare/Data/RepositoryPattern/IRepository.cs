namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IRepository<T> where T : class
    {

        Task<IEnumerable<T>> getAllAsync(string id);
        Task<T> getByIdAsync(int id);
        Task addAsync(T entity);
        Task deleteAsync(int id);

    }
}
