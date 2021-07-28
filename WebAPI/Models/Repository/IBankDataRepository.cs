using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Repository
{
    public interface IBankDataRepository<TEntity, TKey> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(TKey id);
        TKey Add(TEntity item);
        TKey Update(TKey id, TEntity item);
        TKey Delete(TKey id);
    }
}
