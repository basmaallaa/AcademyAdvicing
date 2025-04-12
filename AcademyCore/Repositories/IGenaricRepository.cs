using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Academy.Core.Repositories
{
    public interface IGenaricRepository<T> where T : class 
    {
     
        Task<T> GetAsync(int id);
        Task AddAsync(T item);

        Task<IQueryable<T>> GetAllAsync();

        //Task Delete(int id)

        void Delete(T item);

        void Update(T item);
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
       Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllIncludingAsync(Expression<Func<T, bool>> predicate,
                    params Expression<Func<T, object>>[] includeProperties);
        //assignnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
        Task<T> GetOneIncludingAsync(
    Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<T> GetIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        //Task<List<T>> GetAllIncludingAsyncc(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllIncludingAsyncc(params Expression<Func<T, object>>[] includeProperties);
    }
}
