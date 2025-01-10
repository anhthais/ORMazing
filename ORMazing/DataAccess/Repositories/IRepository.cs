namespace ORMazing.DataAccess.Repositories
{
    public interface IRepository
    {
        public interface IRepository<TEntity> where TEntity : class, new()
        {
            void Add(TEntity entity);
            List<TEntity> GetAll();
            void Update(TEntity entity);
            void Delete(TEntity entity);
            List<TEntity> GetWithCondition(string whereCondition, string groupByHaving, string havingCondition, string orderByCondition);


        }
    }
}
