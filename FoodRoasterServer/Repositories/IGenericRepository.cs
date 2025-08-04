namespace FoodRoasterServer.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddRecord(T entity);
        Task<T?> GetSingleRecord(int id);
        Task<List<T>> GetAllRecords();
        Task<T> UpdateRecord(T entity);
        Task<int> DeleteRecord(int id);
    }
}
