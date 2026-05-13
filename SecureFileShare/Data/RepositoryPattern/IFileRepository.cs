namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IFileRepository
    {

        Task<IEnumerable<Models.File>> getAllAsync(string id);
        Task<Models.File> getByIdAsync(int id);
        Task addAsync(Models.File entity);
        Task deleteAsync(int id);
        Task updateAsync(int id, string newName);

    }
}
