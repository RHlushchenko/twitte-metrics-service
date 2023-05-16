namespace DataAccess.Interfaces
{
    public interface IRepository<T> : IEnumerable<T> where T : class
    {

        public T Add(T entity);

        public int Count();

        public void Clear();
    }
}