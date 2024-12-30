using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.DataAccess.Repositories
{
    public interface IRepository
    {
        public interface IRepository<TEntity> where TEntity : class, new()
        {
            void Add(TEntity entity);
            List<TEntity> GetAll();
        }
    }
}
