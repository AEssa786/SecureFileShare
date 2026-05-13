namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IFileShareRepository
    {
        Task<IEnumerable<Models.FileShare>> getAllAsync(string userId);
        Task shareFile(Models.File file, string sharedWithId, string sharedFromId);
    }
}
